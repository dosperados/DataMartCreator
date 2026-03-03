using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMCApp.Infrastructure;
using DMCApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMCApp.ViewModels;

public partial class WizardViewModel : ObservableObject
{
    private readonly IMetadataService _metadataService;
    private readonly ISettingsService _settingsService;

    // State
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    private int _currentStep = 1;

    // Step 1 Inputs
    [ObservableProperty]
    private ObservableCollection<string> _sourceDBs = new();

    [ObservableProperty]
    private string _selectedSourceDB = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _sourceSchemas = new();

    [ObservableProperty]
    private string _selectedSourceSchema = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TableInfo> _sourceTables = new();

    [ObservableProperty]
    private TableInfo? _selectedSourceTable;

    [ObservableProperty]
    private string _sourceDBText = string.Empty;

    partial void OnSourceDBTextChanged(string value)
    {
        if(SelectedSourceDB != value) SelectedSourceDB = value;
    }

    [ObservableProperty]
    private string _sourceSchemaText = string.Empty;

    partial void OnSourceSchemaTextChanged(string value)
    {
        if (SelectedSourceSchema != value) SelectedSourceSchema = value;
    }

    [ObservableProperty]
    private string _sourceTableText = string.Empty;

    [ObservableProperty]
    private string _destDB = "TADM";

    [ObservableProperty]
    private string _destSchema = "dbo";

    [ObservableProperty]
    private string _destTable = "dim";

    [ObservableProperty]
    private bool _addOptionIdColumn = true;

    [ObservableProperty]
    private bool _addCompanyIdColumn = true;

    // Step 2 Inputs
    [ObservableProperty]
    private ObservableCollection<ColumnDefinitionViewModel> _columns = new();

    // Commands
    public IAsyncRelayCommand NextCommand { get; }
    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand FinishCommand { get; }
    public IRelayCommand CancelCommand { get; }

    // Actions
    public Action? CloseAction { get; set; }

    public WizardViewModel(IMetadataService metadataService, ISettingsService settingsService)
    {
        _metadataService = metadataService;
        _settingsService = settingsService;

        NextCommand = new AsyncRelayCommand(OnNextAsync, () => CurrentStep < 3);
        BackCommand = new RelayCommand(OnBack, () => CurrentStep > 1);
        FinishCommand = new AsyncRelayCommand(OnFinishAsync);
        CancelCommand = new RelayCommand(() => CloseAction?.Invoke());

        LoadInitialSettings();
    }

    private void LoadInitialSettings()
    {
        var db = _settingsService.SourceDB;
        SourceDBs.Add(db);
        SelectedSourceDB = db;
        SourceDBText = db;
    }

    async partial void OnSelectedSourceDBChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var connStr = _settingsService.GetConnectionString("SourceDbExample");

        try
        {
            var schemas = await _metadataService.GetSchemasAsync(connStr);
            SourceSchemas.Clear();
            foreach (var s in schemas) SourceSchemas.Add(s);
        }
        catch { }
    }

    async partial void OnSelectedSourceSchemaChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        await LoadTablesAsync();
    }

    async partial void OnSourceTableTextChanged(string value)
    {
         if (string.IsNullOrWhiteSpace(SelectedSourceSchema)) return;
         await LoadTablesAsync();
    }

    private async Task LoadTablesAsync()
    {
        var connStr = _settingsService.GetConnectionString("SourceDbExample");
        try
        {
            var tables = await _metadataService.GetTablesAsync(connStr, SelectedSourceSchema, SourceTableText ?? "");
            SourceTables.Clear();
            foreach (var t in tables) SourceTables.Add(t);
        }
        catch {}
    }

    private async Task OnNextAsync()
    {
        if (CurrentStep == 1)
        {
            if (string.IsNullOrWhiteSpace(SourceTableText))
            {
                return;
            }

            await LoadColumnsAsync();
            CurrentStep = 2;
        }
        else if (CurrentStep == 2)
        {
            CurrentStep = 3;
        }
    }

    private void OnBack()
    {
        CurrentStep--;
    }

    private async Task LoadColumnsAsync()
    {
        var connStr = _settingsService.GetConnectionString("SourceDbExample");
        var destConnStr = _settingsService.GetConnectionString("DestDbExample");

        var rawColumns = await _metadataService.GetSourceColumnsAsync(connStr, SelectedSourceDB, SelectedSourceSchema, SourceTableText);
        var blacklist = await _metadataService.GetDefaultColumnBlackListAsync(destConnStr);
        var renameRules = await _metadataService.GetDefaultColumnNameChangeAsync(destConnStr);

        IEnumerable<ActiveOptionList> aolList = new List<ActiveOptionList>();
        if (AddOptionIdColumn)
        {
             aolList = await _metadataService.GetAolAsync(connStr, SourceTableText);
        }

        Columns.Clear();
        var destMap = new Dictionary<string, ColumnDefinitionViewModel>(StringComparer.OrdinalIgnoreCase);

        var finalColumns = new List<ColumnDefinitionViewModel>();

        foreach (var col in rawColumns)
        {
            string originalName = col.SourceColumnName;
            string destName = originalName;
            bool isRenamed = false;

            // Apply Blacklist
            if (blacklist.Contains(originalName, StringComparer.OrdinalIgnoreCase))
            {
                col.AddToDest = false;
            }

            // Is Option Field Logic
            bool isOption = aolList.Any(a => a.FieldNameDWH == originalName);
            col.IsOptionField = isOption;

            // Apply DB Rename Rules
            var rule = renameRules.FirstOrDefault(r => string.Equals(r.SourceName, originalName, StringComparison.OrdinalIgnoreCase));
            if (rule != null)
            {
                destName = rule.TargetName;
                isRenamed = true;
            }
            // Apply Prefix Stripping (Only if not option field)
            else if (destName.StartsWith("TA") && destName.Length > 2 && !isOption)
            {
                destName = destName.Substring(2);
                isRenamed = true;
            }

            col.DestColumnName = destName;

            // Conflict Resolution
            if (destMap.TryGetValue(destName, out var existing))
            {
                bool existingWasRenamed = existing.SourceColumnName != existing.DestColumnName;

                if (isRenamed && !existingWasRenamed)
                {
                    existing.AddToDest = false;
                    col.AddToDest = true; // Renamed wins
                    destMap[destName] = col;
                }
                else if (!isRenamed && existingWasRenamed)
                {
                    col.AddToDest = false; // Existing Renamed wins
                }
                else
                {
                    col.AddToDest = false; // First wins
                }
            }
            else
            {
                destMap[destName] = col;
            }

            // Smart Data Type (Settings-based)
            var mapping = _settingsService.GetDataTypeMapping(col.DataType, originalName, col.Precision);
            if (mapping != null)
            {
                if (!string.IsNullOrEmpty(mapping.TargetType)) col.DataType = mapping.TargetType;
                if (mapping.TargetPrecision.HasValue) col.Precision = mapping.TargetPrecision.Value;
                if (mapping.TargetScale.HasValue) col.Scale = mapping.TargetScale.Value;
                if (mapping.TargetMaxLength.HasValue) col.MaxLength = mapping.TargetMaxLength.Value;
            }

            // Smart Defaults Validation
            var defaults = _settingsService.GetDataTypeDefault(col.DataType);
            if (defaults != null)
            {
                if (defaults.MaxLength.HasValue) col.MaxLength = defaults.MaxLength.Value;
                if (defaults.Precision.HasValue) col.Precision = defaults.Precision.Value;
                if (defaults.Scale.HasValue) col.Scale = defaults.Scale.Value;
                if (!string.IsNullOrEmpty(defaults.Collation)) col.CollationName = defaults.Collation;
            }

            finalColumns.Add(col);

            // Virtual Row (AOL)
            if (col.IsOptionField && AddOptionIdColumn)
            {
                var aolData = aolList.FirstOrDefault(a => a.FieldNameDWH == originalName);

                var virtualRow = new ColumnDefinitionViewModel
                {
                    SourceColumnName = "OptionId",
                    DestColumnName = originalName + "OptionId",
                    ColumnSortNo = col.ColumnSortNo + 0.1f,
                    DataType = "nvarchar",
                    MaxLength = 50,
                    FieldType = "OptionId",
                    AddToDest = true,
                    RefTableName = SourceTableText + "_AOL",
                    IsNewVirtual = true,
                    RefLevelNo = aolData != null ? (float?)aolData.ColumnSortNo : null,
                    SourceKeyColumn = originalName,
                    DestKeyColumn = originalName + "OptionId",
                    IsOptionField = true
                };
                finalColumns.Add(virtualRow);
            }
        }

        foreach (var c in finalColumns)
        {
            Columns.Add(c);
        }
    }

    private async Task OnFinishAsync()
    {
        var sourceConnStr = _settingsService.GetConnectionString("SourceDbExample");
        var destConnStr = _settingsService.GetConnectionString("DestDbExample");

        var tableDef = new DMCApp.Models.Entities.TableDefine
        {
             SourceMainDBName = SelectedSourceDB,
             SourceMainSchemaName = SelectedSourceSchema,
             SourceMainTableName = SourceTableText,
             DestMainDBName = DestDB,
             DestMainSchemaName = DestSchema,
             DestMainTableName = DestTable,
             ModifiedBy = Environment.UserDomainName + "\\" + Environment.UserName
        };

        var aolList = await _metadataService.GetAolAsync(sourceConnStr, SourceTableText);

        using (var db = new DMCApp.Data.AppDbContext(destConnStr))
        {
            // Transactional Save via EF Core
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                // Ensure DBs Exist (Metadata only)
                var sourceDb = db.Dbs.FirstOrDefault(d => d.DBName == SelectedSourceDB)
                    ?? db.Dbs.Add(new DMCApp.Models.Entities.Db { DBName = SelectedSourceDB }).Entity;
                var destDb = db.Dbs.FirstOrDefault(d => d.DBName == DestDB)
                    ?? db.Dbs.Add(new DMCApp.Models.Entities.Db { DBName = DestDB }).Entity;

                await db.SaveChangesAsync();

                tableDef.SourceMainDBId = sourceDb.DBId;
                tableDef.DestMainDBId = destDb.DBId;

                // Ensure Tables Exist
                var sourceTable = db.Tables.FirstOrDefault(t => t.DBId == sourceDb.DBId && t.SchemaName == SelectedSourceSchema && t.TableName == SourceTableText)
                    ?? db.Tables.Add(new DMCApp.Models.Entities.Table { DBId = sourceDb.DBId, SchemaName = SelectedSourceSchema, TableName = SourceTableText }).Entity;
                var destTableObj = db.Tables.FirstOrDefault(t => t.DBId == destDb.DBId && t.SchemaName == DestSchema && t.TableName == DestTable)
                    ?? db.Tables.Add(new DMCApp.Models.Entities.Table { DBId = destDb.DBId, SchemaName = DestSchema, TableName = DestTable }).Entity;

                await db.SaveChangesAsync();

                tableDef.SourceMainTableId = sourceTable.TableId;
                tableDef.DestMainTableId = destTableObj.TableId;

                // 2.5 Insert Missing Columns
                // Source Columns
                foreach (var col in Columns)
                {
                    if (col.IsNewVirtual) continue; // Virtual columns handled under AOL table
                    bool srcColExists = db.Columns.Any(c => c.TableId == sourceTable.TableId && c.ColumnName == col.SourceColumnName);
                    if (!srcColExists)
                    {
                        db.Columns.Add(new DMCApp.Models.Entities.Column
                        {
                            TableId = sourceTable.TableId,
                            ColumnName = col.SourceColumnName,
                            DataType = "Unknown" // We don't have exact source schema info mapped fully, setting generic or based on mapping
                        });
                    }
                }

                // Dest Columns
                foreach (var col in Columns.Where(c => c.AddToDest))
                {
                    bool destColExists = db.Columns.Any(c => c.TableId == destTableObj.TableId && c.ColumnName == col.DestColumnName);
                    if (!destColExists)
                    {
                        db.Columns.Add(new DMCApp.Models.Entities.Column
                        {
                            TableId = destTableObj.TableId,
                            ColumnName = col.DestColumnName,
                            DataType = col.DataType,
                            MaxLength = col.MaxLength,
                            Precision = col.Precision,
                            Scale = col.Scale,
                            IsNullable = col.IsNullable ? (byte)1 : (byte)0
                        });
                    }
                }

                // Virtual AOL Table and Columns
                var optionFields = Columns.Where(c => c.IsOptionField && c.IsNewVirtual).ToList();
                if (optionFields.Any())
                {
                    var aolTableName = SourceTableText + "_AOL";
                    var aolTableObj = db.Tables.FirstOrDefault(t => t.DBId == sourceDb.DBId && t.SchemaName == SelectedSourceSchema && t.TableName == aolTableName)
                        ?? db.Tables.Add(new DMCApp.Models.Entities.Table { DBId = sourceDb.DBId, SchemaName = SelectedSourceSchema, TableName = aolTableName }).Entity;

                    await db.SaveChangesAsync();

                    foreach (var aolCol in optionFields)
                    {
                        bool aolColExists = db.Columns.Any(c => c.TableId == aolTableObj.TableId && c.ColumnName == aolCol.SourceColumnName);
                        if (!aolColExists)
                        {
                            db.Columns.Add(new DMCApp.Models.Entities.Column
                            {
                                TableId = aolTableObj.TableId,
                                ColumnName = aolCol.SourceColumnName,
                                DataType = aolCol.DataType,
                                MaxLength = aolCol.MaxLength
                            });
                        }
                    }
                }
                await db.SaveChangesAsync();

                // 3. Insert TableDefine
                db.TableDefines.Add(tableDef);
                await db.SaveChangesAsync();

                // 4. Insert ReferenceTableDefine
                // Record 1 (Main)
                db.ReferenceTableDefines.Add(new DMCApp.Models.Entities.ReferenceTableDefine
                {
                    TableDefineId = tableDef.TableDefineId,
                    RefTableName = SourceTableText,
                    RefTableId = sourceTable.TableId,
                    RefTableAliasName = "SourceMainTable",
                    RefType = "Main",
                    JoinType = "INNER JOIN",
                    Description = "SourceMainTable"
                });

                // AOL References
                if (optionFields.Any())
                {
                     db.ReferenceTableDefines.Add(new DMCApp.Models.Entities.ReferenceTableDefine
                     {
                         TableDefineId = tableDef.TableDefineId,
                         RefTableName = SourceTableText + "_AOL",
                         RefTableAliasName = "RefOption_" + SourceTableText,
                         RefType = "OptionList",
                         JoinType = "LEFT JOIN"
                     });
                }

                await db.SaveChangesAsync();

                // 5. Insert ReferenceColumnDefine
                foreach (var col in Columns.Where(c => c.AddToDest))
                {
                    db.ReferenceColumnDefines.Add(new DMCApp.Models.Entities.ReferenceColumnDefine
                    {
                        TableDefineId = tableDef.TableDefineId,
                        SourceColumnName = col.SourceColumnName,
                        DestColumnName = col.DestColumnName,
                        DataType = col.DataType,
                        MaxLength = col.MaxLength,
                        Precision = col.Precision,
                        Scale = col.Scale,
                        IsNullable = col.IsNullable ? (byte)1 : (byte)0,
                        CollationName = col.CollationName,
                        ColumnSortNo = (float)col.ColumnSortNo,
                        IsOptionField = col.IsOptionField ? (byte)1 : (byte)0,
                        FieldType = col.FieldType,
                        KeySortNo = col.KeySortNo,
                        SourceKeyColumn = col.SourceKeyColumn,
                        DestKeyColumn = col.DestKeyColumn,
                        RefTableName = col.RefTableName,
                        RefColumnName = col.IsNewVirtual ? "OptionId" : null,
                        RefLevelNo = col.RefLevelNo
                    });
                }

                // Add CompanyId Reference if enabled
                if (AddCompanyIdColumn)
                {
                    db.ReferenceColumnDefines.Add(new DMCApp.Models.Entities.ReferenceColumnDefine
                    {
                        TableDefineId = tableDef.TableDefineId,
                        SourceColumnName = "CompanyId",
                        DestColumnName = "CompanyId",
                        DataType = "nvarchar",
                        MaxLength = 50,
                        FieldType = "Main"
                    });
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        CloseAction?.Invoke();
    }
}

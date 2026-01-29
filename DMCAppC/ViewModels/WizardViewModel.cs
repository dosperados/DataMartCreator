using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMCApp.Infrastructure;
using DMCApp.Models;
using System.Collections.ObjectModel;
using System.Windows;

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
    private string _selectedSourceDB;

    [ObservableProperty]
    private ObservableCollection<string> _sourceSchemas = new();

    [ObservableProperty]
    private string _selectedSourceSchema;

    [ObservableProperty]
    private ObservableCollection<TableInfo> _sourceTables = new();

    [ObservableProperty]
    private TableInfo _selectedSourceTable;

    // Manual entries if needed, but binding to ComboBox IsEditable=True usually maps to Text property.
    // I'll stick to SelectedItem for now, assuming ComboBox can handle text entry via Text binding if needed.
    // For simplicity, I'll add string properties for the Text values.

    [ObservableProperty]
    private string _sourceDBText;
    partial void OnSourceDBTextChanged(string value) { if(SelectedSourceDB != value) SelectedSourceDB = value; }

    [ObservableProperty]
    private string _sourceSchemaText;
    async partial void OnSourceSchemaTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        // Async load tables? Maybe too frequent.
        // Logic says "On Change -> Async call GetSchemas/GetTables"
    }

    [ObservableProperty]
    private string _sourceTableText;

    [ObservableProperty]
    private string _destDB = "TADM";
    [ObservableProperty]
    private string _destSchema = "dbo";
    [ObservableProperty]
    private string _destTable = "dim";

    [ObservableProperty]
    private bool _addOptionIdColumn;
    [ObservableProperty]
    private bool _addCompanyIdColumn;

    // Step 2 Inputs
    [ObservableProperty]
    private ObservableCollection<ColumnDefinitionViewModel> _columns = new();

    // Commands
    public IAsyncRelayCommand NextCommand { get; }
    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand FinishCommand { get; }
    public IRelayCommand CancelCommand { get; }

    // Actions
    public Action CloseAction { get; set; }

    public WizardViewModel(IMetadataService metadataService, ISettingsService settingsService)
    {
        _metadataService = metadataService;
        _settingsService = settingsService;

        NextCommand = new AsyncRelayCommand(OnNextAsync, () => CurrentStep < 3);
        BackCommand = new RelayCommand(OnBack, () => CurrentStep > 1);
        FinishCommand = new AsyncRelayCommand(OnFinishAsync); // Available on Step 2? Prompt says Step 3 is Finish (Transactional Unit). Actually Step 2 is Columns, Step 3 is Finish.
        // Wait, Prompt says "Step 2: Column Definitions". "Step 3: Finish".
        // Usually Finish is available on Step 2 to go to Step 3 logic.

        LoadInitialSettings();
    }

    private void LoadInitialSettings()
    {
        // Load SourceDBs from Settings
        var db = _settingsService.SourceDB;
        SourceDBs.Add(db);
        SelectedSourceDB = db;
        SourceDBText = db;
    }

    // Step 1 Triggers
    async partial void OnSelectedSourceDBChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var connStr = _settingsService.GetConnectionString("SourceDbExample"); // Use a default or look it up
        // In reality, we might need a connection string per DB name.
        // For this task, I'll assume one connection string for TADWH.
        connStr = _settingsService.GetConnectionString("DWPROD_DestWindowsAuth").Replace("TADM", value);

        try
        {
            var schemas = await _metadataService.GetSchemasAsync(connStr);
            SourceSchemas.Clear();
            foreach (var s in schemas) SourceSchemas.Add(s);
        }
        catch { /* Handle error */ }
    }

    async partial void OnSelectedSourceSchemaChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        await LoadTablesAsync();
    }

    async partial void OnSourceTableTextChanged(string value)
    {
         // Filter tables?
         if (string.IsNullOrWhiteSpace(SelectedSourceSchema)) return;
         // Trigger search
         await LoadTablesAsync();
    }

    private async Task LoadTablesAsync()
    {
        var connStr = _settingsService.GetConnectionString("DWPROD_DestWindowsAuth").Replace("TADM", SelectedSourceDB ?? "TADWH");
        try
        {
            var tables = await _metadataService.GetTablesAsync(connStr, SelectedSourceSchema ?? "dbo", SourceTableText ?? "");
            SourceTables.Clear();
            foreach (var t in tables) SourceTables.Add(t);
        }
        catch {}
    }

    private async Task OnNextAsync()
    {
        if (CurrentStep == 1)
        {
            // Validate Step 1
            if (string.IsNullOrWhiteSpace(SourceTableText))
            {
                MessageBox.Show("Please select a Source Table.");
                return;
            }

            // Move to Step 2: Load Columns
            await LoadColumnsAsync();
            CurrentStep = 2;
        }
        else if (CurrentStep == 2)
        {
            // Move to Step 3 (Confirmation/Finish)?
            // Prompt says: "Step 3: Finish (Transactional Unit of Work). Action: On 'Finish', execute..."
            // So on Step 2, we show "Finish" button instead of Next?
            // Or Step 3 is a summary screen?
            // "Step 3: Finish ... Action: On 'Finish', execute a Transaction"
            // I'll assume Step 2 has a "Finish" button, or Next goes to Step 3 Summary.
            // Let's make "Finish" available on Step 2.
        }
    }

    private void OnBack()
    {
        CurrentStep--;
    }

    private async Task LoadColumnsAsync()
    {
        var connStr = _settingsService.GetConnectionString("DWPROD_DestWindowsAuth").Replace("TADM", SelectedSourceDB ?? "TADWH");
        // TADWH connection string: replace TADM with TADWH in the default connection settings
        var tadwhConnStr = _settingsService.GetConnectionString("DestDbWindowsAuth").Replace("TADM", "TADWH");

        var rawColumns = await _metadataService.GetSourceColumnsAsync(connStr, SelectedSourceDB, SelectedSourceSchema, SourceTableText);

        IEnumerable<ActiveOptionList> aolList = new List<ActiveOptionList>();
        if (AddOptionIdColumn)
        {
             // Use TADWH connection string for AOL lookup
             aolList = await _metadataService.GetAolAsync(tadwhConnStr, SourceTableText);
        }

        Columns.Clear();
        var destMap = new Dictionary<string, ColumnDefinitionViewModel>(StringComparer.OrdinalIgnoreCase);

        foreach (var col in rawColumns)
        {
            string originalName = col.SourceColumnName;
            string destName = originalName;
            bool isRenamed = false;

            if (destName.StartsWith("TA") && !destName.Equals("TA", StringComparison.Ordinal))
            {
                bool isOption = aolList.Any(a => a.FieldNameDWH == originalName);
                if (!isOption)
                {
                    destName = destName.Substring(2);
                    isRenamed = true;
                }
                col.IsOptionField = isOption;
            }

            // Conflict Resolution
            if (destMap.TryGetValue(destName, out var existing))
            {
                // Conflict exists.
                // existing is the one currently holding 'destName'.
                // Check if existing was renamed.
                // We assume if Source != Dest, it was renamed.
                bool existingWasRenamed = existing.SourceColumnName != existing.DestColumnName;

                if (isRenamed && !existingWasRenamed)
                {
                    // Current is Renamed (e.g. TACode -> Code). Existing is Native (Code -> Code).
                    // Rule: "keep the renamed one active".
                    // So Current wins. Existing disabled.
                    existing.AddToDest = false;
                    col.DestColumnName = destName;
                    destMap[destName] = col; // Update map to point to current
                }
                else if (!isRenamed && existingWasRenamed)
                {
                    // Current is Native. Existing is Renamed.
                    // Existing wins. Current disabled.
                    col.AddToDest = false;
                    col.DestColumnName = destName;
                    // Map stays with existing.
                }
                else
                {
                    // Both renamed or Both native (unlikely for native unless case diff).
                    // Disable current (First wins).
                    col.AddToDest = false;
                    col.DestColumnName = destName;
                }
            }
            else
            {
                col.DestColumnName = destName;
                destMap[destName] = col;
            }

            // Smart Data Type
            if (col.DataType == "decimal" && col.Precision == 38 && col.Scale == 0)
            {
                col.Precision = 18;
                col.Scale = 2;
            }
            if (col.SourceColumnName.EndsWith("Date") && col.DataType == "datetime")
            {
                col.DataType = "date";
            }

            Columns.Add(col);

            // Virtual Row
            if (col.IsOptionField && AddOptionIdColumn)
            {
                var virtualRow = new ColumnDefinitionViewModel
                {
                    SourceColumnName = "OptionId",
                    DestColumnName = col.SourceColumnName + "OptionId",
                    ColumnSortNo = col.ColumnSortNo + 0.1,
                    DataType = "nvarchar",
                    MaxLength = 50,
                    FieldType = "OptionId",
                    AddToDest = true,
                    RefTableName = SourceTableText + "_AOL",
                    IsNewVirtual = true
                };
                Columns.Add(virtualRow);
            }
        }
    }

    private async Task OnFinishAsync()
    {
        // Transactional Save
        var sourceConnStr = _settingsService.GetConnectionString("DWPROD_DestWindowsAuth").Replace("TADM", SelectedSourceDB ?? "TADWH");
        var destConnStr = _settingsService.GetConnectionString("DestDbWindowsAuth");

        var tableDef = new TableDefinition
        {
             SourceMainDBName = SelectedSourceDB,
             SourceMainSchemaName = SelectedSourceSchema,
             SourceMainTableName = SourceTableText,
             DestMainDBName = DestDB,
             DestMainSchemaName = DestSchema,
             DestMainTableName = DestTable,
             ModifiedBy = Environment.UserName
        };

        // Need AOL list again for ReferenceTableDefine
        var aolList = await _metadataService.GetAolAsync(sourceConnStr, SourceTableText);

        await _metadataService.SaveTableDefinitionAsync(tableDef, Columns, aolList, sourceConnStr, destConnStr);

        CloseAction?.Invoke();
    }
}

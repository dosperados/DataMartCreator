using CommunityToolkit.Mvvm.ComponentModel;
using DMCApp.Infrastructure;
using DMCApp.Models;
using System.Collections.ObjectModel;

namespace DMCApp.ViewModels;

public partial class TableEditorViewModel : ObservableObject
{
    private readonly IMetadataService _metadataService;
    private readonly ISettingsService _settingsService;
    private readonly TableDefinition _tableDefinition;

    [ObservableProperty]
    private string _header;

    [ObservableProperty]
    private ObservableCollection<ReferenceTableDefine> _referenceTables = new();

    [ObservableProperty]
    private ObservableCollection<ReferenceColumnDefine> _referenceColumns = new();

    // Lookup Data
    [ObservableProperty]
    private ObservableCollection<TableDefinition> _availableTables = new();

    public TableEditorViewModel(TableDefinition tableDefinition, IMetadataService metadataService, ISettingsService settingsService)
    {
        _tableDefinition = tableDefinition;
        _metadataService = metadataService;
        _settingsService = settingsService;
        Header = tableDefinition.DestMainTableName;

        LoadData();
    }

    private void LoadData()
    {
        // Load details via MetadataService
        // Stubbing load for now as I don't have GetReferenceTablesAsync implemented
        // I'll add stub data

        ReferenceTables.Add(new ReferenceTableDefine
        {
            RefTableName = _tableDefinition.SourceMainTableName,
            RefType = "Main",
            JoinType = "INNER JOIN"
        });

        // Load Lookup Data
        // Ideally fetch all tables
    }
}

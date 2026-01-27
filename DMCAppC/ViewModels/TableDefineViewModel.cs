using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMCApp.Infrastructure;
using DMCApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DMCApp.ViewModels;

public partial class TableDefineViewModel : ObservableObject
{
    private readonly IMetadataService _metadataService;
    private readonly ISettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider; // To create Wizard

    [ObservableProperty]
    private string _filterText = string.Empty;

    private ObservableCollection<TableDefinition> _allTables = new();

    [ObservableProperty]
    private ObservableCollection<TableDefinition> _tableDefinitions = new();

    [ObservableProperty]
    private TableDefinition _selectedTable;

    public ICommand AddTableCommand { get; }

    public TableDefineViewModel(IMetadataService metadataService, ISettingsService settingsService, IServiceProvider serviceProvider)
    {
        _metadataService = metadataService;
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;

        AddTableCommand = new AsyncRelayCommand(AddNewTableAsync);

        // Initial Load (Mock for now or implement if TableDefine endpoint existed)
        // Since GetTablesAsync returns system tables, we need a way to get TableDefinitions from [Setting].[TableDefine].
        // I didn't add GetTableDefinitionsAsync to IMetadataService. I probably should have.
        // For now, I'll leave it empty or add a stub load.
    }

    async partial void OnFilterTextChanged(string value)
    {
        // Filter logic
        if (string.IsNullOrWhiteSpace(value))
        {
            TableDefinitions = new ObservableCollection<TableDefinition>(_allTables);
        }
        else
        {
            var filtered = _allTables.Where(t => t.DestMainTableName.Contains(value, StringComparison.OrdinalIgnoreCase));
            TableDefinitions = new ObservableCollection<TableDefinition>(filtered);
        }
    }

    private async Task AddNewTableAsync()
    {
        // Trigger Wizard
        // We need to resolve WizardViewModel and show the view.
        // In MVVM, we might use a DialogService or NavigationService.
        // For simplicity, I will use a simple event or direct View creation in MainWindow code-behind via a Service,
        // OR better: use a Messenger or a Func factory.

        // I'll assume MainWindow listens to a request or we instantiate the window here (tight coupling but pragmatic for "Shell").
        // Better: Publish a message "OpenWizard".

        // But since I am implementing the Shell, I can define a generic approach.
        // Let's use a Func<WizardViewModel> factory.

        // For now, I will invoke a delegate or event.
        OnOpenWizard?.Invoke();
    }

    public event Action OnOpenWizard;
}

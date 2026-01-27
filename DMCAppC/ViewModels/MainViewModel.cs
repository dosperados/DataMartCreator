using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMCApp.Models;
using System.Collections.ObjectModel;

namespace DMCApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Sidebar ViewModels
    public TableDefineViewModel TableDefineVM { get; }

    // Workspace Tabs
    [ObservableProperty]
    private ObservableCollection<object> _tabs = new();

    [ObservableProperty]
    private object _selectedTab;

    private readonly IServiceProvider _serviceProvider; // To resolve EditorVM

    public MainViewModel(TableDefineViewModel tableDefineVM, IServiceProvider serviceProvider)
    {
        TableDefineVM = tableDefineVM;
        _serviceProvider = serviceProvider;
        
        // Listen to requests to open Wizard or Tabs
        TableDefineVM.OnOpenWizard += OpenWizard;
        TableDefineVM.PropertyChanged += TableDefineVM_PropertyChanged;
    }

    private void TableDefineVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TableDefineViewModel.SelectedTable))
        {
            var selected = TableDefineVM.SelectedTable;
            if (selected != null)
            {
                OpenTableEditor(selected);
            }
        }
    }

    private void OpenTableEditor(TableDefinition tableDef)
    {
        // Check if already open
        // Stub: always open new or select existing
        // Create VM manually or via factory. Since constructor needs arguments, manual is easier here or ActivatorUtilities.
        var editorVM = new TableEditorViewModel(tableDef,
            (Infrastructure.IMetadataService)_serviceProvider.GetService(typeof(Infrastructure.IMetadataService)),
            (Infrastructure.ISettingsService)_serviceProvider.GetService(typeof(Infrastructure.ISettingsService)));

        Tabs.Add(editorVM);
        SelectedTab = editorVM;
    }

    private void OpenWizard()
    {
        // In a real app, use a service. Here we rely on the View code-behind subscribing to an event
        // OR we expose an event here that the MainWindow subscribes to.
        RequestOpenWizard?.Invoke();
    }

    public event Action RequestOpenWizard;
}

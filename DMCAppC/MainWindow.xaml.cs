using System.Windows;
using DMCApp.ViewModels;
using DMCApp.Views;
using Wpf.Ui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DMCApp;

public partial class MainWindow : FluentWindow
{
    private readonly MainViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        DataContext = _viewModel;

        _viewModel.RequestOpenWizard += OnRequestOpenWizard;
    }

    private void OnRequestOpenWizard()
    {
        // Resolve WizardViewModel and View
        var wizardVM = _serviceProvider.GetRequiredService<WizardViewModel>();
        var wizardWindow = new WizardView
        {
            Owner = this,
            DataContext = wizardVM
        };

        wizardVM.CloseAction = () => wizardWindow.Close();

        wizardWindow.ShowDialog();
    }
}

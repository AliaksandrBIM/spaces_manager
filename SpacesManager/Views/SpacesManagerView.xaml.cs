using Autodesk.Revit.DB;
using Eneca.SpacesManager.ViewModels;
using Eneca.SpacesManager.ViewModels.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Eneca.SpacesManager.Views;
public partial class SpacesManagerView
{
    public SpacesManagerView(IViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    public SpacesManagerView()
    {
        InitializeComponent();
    }
    private void TopStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
    private void WindowClose_BTN_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void WindowHide_BTN_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var selectedComboBoxItem = (RevitLinkInstance)comboBox.SelectedItem;
        var spacesManagerViewModel = (SpacesManagerViewModel)DataContext;
        spacesManagerViewModel.SelectedRevitLinkInstance = selectedComboBoxItem;
        spacesManagerViewModel.UpdateDataGridContent();
    }
    private void EnecaLogo_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start($"https://enecagroup.com/");
    }
}
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ProductEditorViewModel : ViewModelBase
{
    public FamilyViewModel FamilyHost { get; } = new FamilyViewModel();
    //public ComponentUsageViewModel ComponentUsageHost { get; } = new ComponentUsageViewModel();
    public ProductWithFullComponentsViewModel ProductHost { get; } = new ProductWithFullComponentsViewModel();

    public ProductEditorViewModel()
    {
        FamilyHost.FillCollection();

        FamilyHost.OnSelectedModelChanged = () => this
            .Do(vm => this.ProductHost.FillCollection(FamilyHost.FamilyModelSelected!));

        ProductHost.OnSelectedModelChanged = () => ProductHost
            .ProductWithFullComponentsModelSelected?
            .Components
            .FillCollection(ProductHost.ProductWithFullComponentsModelSelected);
    }

    public void UpdateProductList() => ProductHost.FillCollection(FamilyHost.FamilyModelSelected!);

    [RelayCommand]
    public void OpenAddFamilyControl() => App.Host!.Services.GetRequiredService<MainWindow>()
        .ViewModel
        .DisplayPopupControl(App.Host!.Services.GetRequiredService<FamilyAddNewControl>());

    [RelayCommand]
    public void OpenEditFamilyControl(FamilyModel Family) => App.Host!.Services.GetRequiredService<MainWindow>()
        .ViewModel
        .DisplayPopupControl(App.Host!.Services.GetRequiredService<FamilyDetailsControl>()
        .Do(fd => { fd.ViewModel.SetFamily(Family); return fd; }));

    [RelayCommand]
    public void OpenAddProductControl() => App.Host!.Services.GetRequiredService<ProductAddControl>()
        .Do(control => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.DisplayPopupControl(control))
        .Do(control => control.ViewModel.RefreshFilesAndModels());

    [RelayCommand]
    public void OpenEditProductControl(ProductWithFullComponentsModel Product) => App.Host!.Services.GetRequiredService<MainWindow>()
        .ViewModel
        .DisplayPopupControl(App.Host!.Services.GetRequiredService<ProductDetailsControl>()
        .Do(pc => { pc.ViewModel.SetProduct(Product); return pc; }));

    [RelayCommand]
    public void OpenEditComponentControl(ExportComponentModel Component) => App.Host!.Services.GetRequiredService<ComponentDetails>()
        .Do(cd => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.DisplayPopupControl(cd))
        .Do(cd => cd.ViewModel.SetComponent(Component));
}

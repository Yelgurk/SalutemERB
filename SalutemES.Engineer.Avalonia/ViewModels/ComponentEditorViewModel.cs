using Avalonia.Controls.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ComponentEditorViewModel : ViewModelBase
{
    public ComponentUsageViewModel ComponentUsageHost { get; } = new ComponentUsageViewModel();

    public ProductWithComponentViewModel ProductHost { get; } = new ProductWithComponentViewModel();

    private string _searchComponentBox = "";
    public string SearchComponentBox
    {
        get => _searchComponentBox;
        set
        {
            _searchComponentBox = value;
            OnPropertyChanged(nameof(SearchComponentBox));

            if (_searchComponentBox == "")
                this.ComponentUsageHost.FillCollection();
            else
                this.ComponentUsageHost.FillCollection(DBRequests.SearchComponent, _searchComponentBox);
        }
    }

    [ObservableProperty]
    private bool _productListOpened = false;

    private bool PopupLock = false;

    [RelayCommand]
    public void AfterSelectFocusReset() => ComponentUsageHost
        .DoIf(host => !ProductListOpened)
        ?.Do(run => {
            PopupLock = true;
            ComponentUsageHost.FillCollection();
            PopupLock = false;
        });

    public ComponentEditorViewModel()
    {
        ComponentUsageHost.FillCollection();

        ComponentUsageHost.OnSelectedModelChanged = () => this
            .DoIf(state => !state.ProductListOpened, closed => this.ProductHost.FillCollection(ComponentUsageHost.ComponentUsageModelSelected!))
            ?.DoIf(vm => !PopupLock)
            ?.DoIf(unfocused => !(ComponentUsageHost.ComponentUsageModelSelected!.Base is null))
            ?.Do(_ => OpenEditComponentControl());

        ProductHost.OnSelectedModelChanged = () => ProductHost
            .ProductWithComponentsModelSelected?
            .Components
            .FillCollection(ProductHost.ProductWithComponentsModelSelected);
    }

    public void OpenEditComponentControl() => App.Host!.Services.GetRequiredService<ComponentDetails>()
        .Do(cd => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.DisplayPopupControl(cd))
        .Do(cd => cd.ViewModel.SetComponent(ComponentUsageHost.ComponentUsageModelSelected!));

    [RelayCommand]
    public void OpenAddComponentControl() => App.Host!.Services.GetRequiredService<MainWindow>()
        .ViewModel
        .DisplayPopupControl(App.Host!.Services.GetRequiredService<ComponentAddNew>());

    [RelayCommand]
    public void OpenEditProductControl(ProductWithComponentsModel Product) => App.Host!.Services.GetRequiredService<MainWindow>()
        .ViewModel
        .DisplayPopupControl(App.Host!.Services.GetRequiredService<ProductDetailsControl>()
        .Do(pc => { pc.ViewModel.SetProduct(Product); return pc; }));

    [RelayCommand]
    public void OpenEditComponentControlX2(ComponentModel Component) => App.Host!.Services.GetRequiredService<ComponentDetails>()
        .Do(cd => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.DisplayPopupControl(cd))
        .Do(cd => cd.ViewModel.SetComponent(Component));
}

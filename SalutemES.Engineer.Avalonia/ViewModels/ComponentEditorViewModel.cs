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

public partial class ComponentEditorViewModel : ViewModelBase
{
    public ComponentUsageViewModel ComponentUsageHost { get; } = new ComponentUsageViewModel();
    public ProductWithComponentViewModel ProductHost { get; } = new ProductWithComponentViewModel();

    [ObservableProperty]
    private bool _productListOpened = false;

    public ComponentEditorViewModel()
    {
        ComponentUsageHost.FillCollection();

        ComponentUsageHost.OnSelectedModelChanged = () => this
            .DoIf(state => state.ProductListOpened, closed =>
            {
                App.Host!.Services.GetRequiredService<MainWindow>()
                .ViewModel
                .DisplayPopupControl(App.Host!.Services.GetRequiredService<ComponentDetails>()
                .Do(cd => { cd.ViewModel.SetComponent(ComponentUsageHost.ComponentUsageModelSelected!); return cd; }));
            })?
            .ProductHost
            .FillCollection(ComponentUsageHost.ComponentUsageModelSelected!);

        ProductHost.OnSelectedModelChanged = () => ProductHost
            .ProductWithComponentsModelSelected?
            .Components
            .FillCollection(ProductHost.ProductWithComponentsModelSelected);
    }
}

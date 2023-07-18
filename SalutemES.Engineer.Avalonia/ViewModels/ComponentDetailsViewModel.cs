using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ComponentDetailsViewModel : ViewModelBase
{
    public ComponentViewModel ComponentHost { get; } = new ComponentViewModel();
    public ProductViewModel ProductHost { get; } = new ProductViewModel();
    public ComponentFileViewModel FilesHost { get; } = new ComponentFileViewModel();

    public ComponentDetailsViewModel() =>
        ComponentHost.OnFillCollection = () =>
        ComponentHost.ComponentModelSelected = ComponentHost.ComponentModelCollection[0] ?? new ComponentModel();

    public void SetComponent(ComponentModel Component) => FillCollection(Component.Name, Component.Code);
    public void SetComponent(ComponentUsageModel Component) => FillCollection(Component.Name, Component.Code);

    public void FillCollection(string Name, string Code)
    {
        ComponentHost.FillCollection(DBRequests.GetComponentsDetailsByNameCode, Name, Code);
        ProductHost.FillCollection(ComponentHost.ComponentModelSelected!);
        FilesHost.FillCollection(ComponentHost.ComponentModelSelected!);
    }

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();
}

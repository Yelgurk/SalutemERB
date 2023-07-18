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
    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    public ComponentViewModel ComponentHost { get; } = new ComponentViewModel();

    public ComponentModel NewComponent { get; set; } = new ComponentModel();

    public ComponentDetailsViewModel() => ComponentHost.OnFillCollection = () => ComponentHost.ComponentModelSelected = ComponentHost.ComponentModelCollection[0] ?? new ComponentModel();

    public void SetComponent(ComponentModel Component) => ComponentHost.FillCollection(DBRequests.GetComponentsDetailsByNameCode, Component.Name, Component.Code);
    public void SetComponent(ComponentUsageModel Component) => ComponentHost.FillCollection(DBRequests.GetComponentsDetailsByNameCode, Component.Name, Component.Code);
}

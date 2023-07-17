using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ComponentEditorViewModel : ViewModelBase
{
    public ComponentUsageViewModel ComponentUsageHost { get; set; }

    public ComponentEditorViewModel()
    {
        ComponentUsageHost = App.Host!.Services.GetRequiredService<ComponentUsageViewModel>();

        ComponentUsageHost.FillCollection();
    }

    [RelayCommand]
    public void UpdateList() => ComponentUsageHost.FillCollection();
}

using CommunityToolkit.Mvvm.Input;
using SalutemES.Engineer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ComponentEditorViewModel : ComponentUsageViewModel
{
    public ComponentEditorViewModel() => this.FillCollection();

    [RelayCommand]
    public void UpdateList() => this.FillCollection();
}

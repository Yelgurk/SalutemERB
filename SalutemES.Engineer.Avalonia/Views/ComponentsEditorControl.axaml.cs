using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.Extra;
using SalutemES.Engineer.Avalonia.ViewModels;
using SalutemES.Engineer.Core;
using System.Diagnostics;

namespace SalutemES.Engineer.Avalonia.Views;

public partial class ComponentsEditorControl : UserControl, IHotKeyHandler
{
    public ComponentsEditorControl()
    {
        InitializeComponent();
        DataContext = new ComponentEditorViewModel();
    }

    public ComponentEditorViewModel ViewModel => (this.DataContext as ComponentEditorViewModel)!;

    public void HotkeyWorked(KeyCombination combination)
    {
        if (combination == KeyCombination.CTRL_F)
            this.ComponentListSearch.Focus();
    }
}

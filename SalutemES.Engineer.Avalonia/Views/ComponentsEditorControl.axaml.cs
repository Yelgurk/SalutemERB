using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;
using SalutemES.Engineer.Core;

namespace SalutemES.Engineer.Avalonia.Views;

public partial class ComponentsEditorControl : UserControl
{
    public ComponentsEditorControl()
    {
        InitializeComponent();
        DataContext = new ComponentEditorViewModel();
    }

    public ComponentEditorViewModel ViewModel => (this.DataContext as ComponentEditorViewModel)!;
}

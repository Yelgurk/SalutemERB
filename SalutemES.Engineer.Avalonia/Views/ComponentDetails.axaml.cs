using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class ComponentDetails : UserControl
    {
        public ComponentDetails()
        {
            InitializeComponent();
            DataContext = new ComponentDetailsViewModel();
        }

        public ComponentDetailsViewModel ViewModel => (this.DataContext as ComponentDetailsViewModel)!;
    }
}

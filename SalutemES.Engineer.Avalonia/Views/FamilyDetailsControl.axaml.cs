using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class FamilyDetailsControl : UserControl
    {
        public FamilyDetailsControl()
        {
            InitializeComponent();
            DataContext = new FamilyDetailsViewModel();
        }

        public FamilyDetailsViewModel ViewModel => (this.DataContext as FamilyDetailsViewModel)!;
    }
}

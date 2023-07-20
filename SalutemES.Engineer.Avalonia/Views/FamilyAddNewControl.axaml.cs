using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class FamilyAddNewControl : UserControl
    {
        public FamilyAddNewControl()
        {
            InitializeComponent();
            DataContext = new FamilyAddNewViewModel();
        }

        public FamilyAddNewViewModel ViewModel => (this.DataContext as FamilyAddNewViewModel)!;
    }
}

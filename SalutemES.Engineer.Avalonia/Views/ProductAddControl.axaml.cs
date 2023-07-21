using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class ProductAddControl : UserControl
    {
        public ProductAddControl()
        {
            InitializeComponent();
            DataContext = new ProductAddViewModel();
        }

        public ProductAddViewModel ViewModel => (this.DataContext as ProductAddViewModel)!;
    }
}

using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class ProductDetailsControl : UserControl
    {
        public ProductDetailsControl()
        {
            InitializeComponent();
            DataContext = new ProductDetailsViewModel();
        }

        public ProductDetailsViewModel ViewModel => (this.DataContext as ProductDetailsViewModel)!;
    }
}

using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class ProductsEditorControl : UserControl
    {
        public ProductsEditorControl()
        {
            InitializeComponent();
            DataContext = new ProductEditorViewModel();
        }

        public ProductEditorViewModel ViewModel => (this.DataContext as ProductEditorViewModel)!;
    }
}

using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class OrderBuilderControl : UserControl
    {
        public OrderBuilderControl()
        {
            InitializeComponent();
            DataContext = new OrderBuilderViewModel();
        }

        public OrderBuilderViewModel ViewModel => (this.DataContext as OrderBuilderViewModel)!;
    }
}

using Avalonia.Controls;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class FileErrorControl : UserControl
    {
        public FileErrorControl()
        {
            InitializeComponent();
            DataContext = new FileErrorViewModel();
        }

        public FileErrorViewModel ViewModel => (this.DataContext as FileErrorViewModel)!;
    }
}

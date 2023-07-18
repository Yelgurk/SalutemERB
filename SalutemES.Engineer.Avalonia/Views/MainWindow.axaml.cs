using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using SalutemES.Engineer.Avalonia.ViewModels;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        public void SetContent<T>(T Control) => MainWindowContentPanel.Content = Control;

        public MainWindowViewModel ViewModel => (this.DataContext as MainWindowViewModel)!;
    }
}
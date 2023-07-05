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
        private object? GradientListBoxItemContent = null;

        void ListBoxBorderGradientHandler(object? sender, PointerEventArgs args)
        {
            if (sender is not null && (sender as Control)?.GetVisualParent() is ContentPresenter listBoxItem)
            {
                Point mouse = args.GetPosition(listBoxItem);
                (Resources["gradRadial"] as RadialGradientBrush)!.Center =
                (Resources["gradRadial"] as RadialGradientBrush)!.GradientOrigin =
                new RelativePoint(mouse.X / listBoxItem.Bounds.Width, mouse.Y / listBoxItem.Bounds.Height, RelativeUnit.Relative);

                GradientListBoxItemContent = listBoxItem.Content;
                listBoxItem.Content = "";
                listBoxItem.Content = GradientListBoxItemContent;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
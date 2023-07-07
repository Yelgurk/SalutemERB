using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using SalutemES.Engineer.Avalonia.ViewModels;
using SalutemES.Engineer.Avalonia.Views;

namespace SalutemES.Engineer.Avalonia
{
    public partial class App : Application
    {
        private Control? GradientListBoxItemContent = null;

        public void ListBoxItemBorderGradientHandler(object? sender, PointerEventArgs args)
        {
            if (sender is not null && (sender as Border)?.GetVisualParent() is Grid listBoxItem)
            {
                Point mouse = args.GetPosition(listBoxItem);
                RadialGradientBrush Accent = (Resources["ListItemBorderRadialGradientAccent"] as RadialGradientBrush)!;
                RadialGradientBrush Base = (Resources["ListItemBorderRadialGradientBase"] as RadialGradientBrush)!;

                Base.Center = Base.GradientOrigin
                    = Accent.Center = Accent.GradientOrigin
                    = new RelativePoint(mouse.X / listBoxItem.Bounds.Width, mouse.Y / listBoxItem.Bounds.Height, RelativeUnit.Relative);

                GradientListBoxItemContent = (sender as Border)!.Child;
                (sender as Border)!.Child = null;
                (sender as Border)!.Child = GradientListBoxItemContent;
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
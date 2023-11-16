using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using SalutemES.Engineer.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Point = Avalonia.Point;
using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Core;
using System.Diagnostics;

namespace SalutemES.Engineer.Avalonia;

public partial class App : Application
{
    /* style event handler */
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

    /* app init */

    public static IHost? Host { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices(services => {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<WindowContentService>();
                services.AddSingleton<ProductsEditorControl>();
                services.AddSingleton<ComponentsEditorControl>();
                services.AddSingleton<OrderBuilderControl>();
                services.AddSingleton<SettingsControl>();
                services.AddSingleton<ComponentDetails>();
                services.AddSingleton<ComponentAddNew>();
                services.AddSingleton<FamilyDetailsControl>();
                services.AddSingleton<FamilyAddNewControl>();
                services.AddSingleton<ProductDetailsControl>();
                services.AddSingleton<ProductAddControl>();
                services.AddSingleton<FileErrorControl>();
            })
            .Build();

        Logger.SetLoggerPath(Environment.CurrentDirectory);

        /* fast + temp solution : BEGIN */
        //string userSign = "DESKTOP-STD16R1";
        string userSign = "";// = "DESKTOP-STD16R1";

        string currentConfPath = $"{Environment.CurrentDirectory}\\config.txt";

        if (!File.Exists(currentConfPath!))
            File.Create(currentConfPath!).Close();
        else
            userSign = File.ReadAllText(currentConfPath);

        Debug.WriteLine(userSign);
        /* fast + temp solution : END */

        DataBaseApi
            .SetConnection(userSign, "DB_SE_EngineerWS")
            .DoIf(conn => conn.Do(x => { Logger.WriteLine("Попытка подключения к БД..."); return x; }).IsSuccess,
                  error => { Logger.WriteLine(error.Exception.message); Environment.Exit(0); })
            ?.Do(ok => Logger.WriteLine("Успешное подключение к БД!"));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Host!.Services
                .GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static T SetWindowContent<T>() where T : notnull => Host!.Services
        .GetRequiredService<WindowContentService>()!
        .Set<T>()!;

    public static void SetWindowContent<T>(T Control) => Host!.Services
        .GetRequiredService<MainWindow>()!
        .SetContent(Control);

    public static StreamGeometry GetIcon(string? ResourceName)
    {
        object? FoundedGeometryObject;
        if (ResourceName is not null
            && Application.Current!.TryGetResource(ResourceName, out FoundedGeometryObject)
            && FoundedGeometryObject is StreamGeometry FoundedGeometry)
            return FoundedGeometry;

        Application.Current!.TryGetResource("question_regular", out FoundedGeometryObject);
        return (FoundedGeometryObject as StreamGeometry)!;
    }
}

public interface IWindowContentService
{
    public T Set<T>() where T : notnull;
}

public class WindowContentService : IWindowContentService
{
    public T Set<T>() where T : notnull
    {
        App.Host!.Services
            .GetRequiredService<MainWindow>()!
            .SetContent(App.Host!.Services.GetRequiredService<T>()!);

        return App.Host!.Services
            .GetRequiredService<T>()!;
    }
}
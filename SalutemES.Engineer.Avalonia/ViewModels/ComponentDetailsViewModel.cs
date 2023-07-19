using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;
using System.ComponentModel;
using System.Diagnostics;


namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ComponentDetailsViewModel : ViewModelBase
{

    public ComponentViewModel ComponentHost { get; } = new ComponentViewModel();
    public ProductViewModel ProductHost { get; } = new ProductViewModel();
    public ComponentFileViewModel FilesHost { get; } = new ComponentFileViewModel();

    [ObservableProperty]
    private string _newFileLocalPath = "";

    public ComponentDetailsViewModel() =>
        ComponentHost.OnFillCollection = () =>
        {
            if (ComponentHost.ComponentModelCollection.Count > 0)
                ComponentHost.ComponentModelSelected = ComponentHost.ComponentModelCollection[0];

            ComponentHost.ComponentModelSelected!.NumericThicknessRegex = true;
            ComponentHost.ComponentModelSelected!.NumericFoldsRegex = true;
            ComponentHost.ComponentModelSelected!.NumericWeightKGRegex = true;
            ComponentHost.ComponentModelSelected!.ResetByBase();
        };

    public void SetComponent(ComponentModel Component) => FillCollection(Component.Name, Component.Code);
    public void SetComponent(ComponentUsageModel Component) => FillCollection(Component.Name, Component.Code);

    public void FillCollection(string Name, string Code)
    {
        ComponentHost.FillCollection(DBRequests.GetComponentsDetailsByNameCode, Name, Code);
        ProductHost.FillCollection(ComponentHost.ComponentModelSelected!);
        FilesHost.FillCollection(ComponentHost.ComponentModelSelected!);
    }

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    [RelayCommand]
    public void SaveInfo() => ComponentHost.ComponentModelSelected!
        .DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.EditComponent,
            onErr => Logger.WriteLine(onErr),
            c.Base!.Name!,
            c.Base!.Code!,
            c.Name,
            c.Code,
            c.Grade,
            c.Thickness,
            c.Folds,
            c.WeightKG,
            c.Note,
            c.Material
            ), error => Debug.WriteLine("Save error")
        )
        ?.Do(success => Debug.WriteLine("Save successfully"))
        .Do(c => this.SetComponent(c))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection());

    [RelayCommand]
    public void CancelInfoEdit() => ComponentHost.ComponentModelSelected!.ResetByBase();

    [RelayCommand]
    public void DeleteWithAllReference() => ComponentHost.ComponentModelSelected!
        .DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteComponent,
            onErr => Logger.WriteLine(onErr),
            c.Name,
            c.Code
            ), error => Debug.WriteLine("Delete component error")
        )?.Do(success => Debug.WriteLine("Delete component successfully"))
        .Do(c => this.ClosePopup())
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection());

    [RelayCommand]
    public void DeleteReferencedFile(ComponentFileModel SelectedFile) => ComponentHost.ComponentModelSelected!
        .DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteComponentFile,
            onErr => Logger.WriteLine(onErr),
            c.Name,
            SelectedFile.LocalFilePath
            ), error => Debug.WriteLine("Delete file error")
        )?.Do(success => Debug.WriteLine("Delete file successfully"))
        .Do(c => this.SetComponent(c))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection());

    [RelayCommand]
    [Obsolete]
    public async Task OpenFileExplorer()
    {
        string[]? paths = await new OpenFileDialog().ShowAsync(App.Host!.Services.GetRequiredService<MainWindow>());
        NewFileLocalPath = (paths?.Length ?? 0) > 0 ? paths![0] : "";
    }

    [RelayCommand]
    public void AddSelectedFileIntoRef() => NewFileLocalPath
        .DoIf(filePath => filePath.Length > 0, error => Logger.WriteLine("Can't add file path, because file not selected"))
        ?.Do(filePath => ComponentHost.ComponentModelSelected)
        ?.DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.AddComponentFile,
            onErr => Logger.WriteLine(onErr),
            c.Name,
            NewFileLocalPath
            ), error => Debug.WriteLine("Add file error")
        )?.Do(success => Debug.WriteLine("Add file successfully"))
        .Do(c => this.SetComponent(c))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection());

    [RelayCommand]
    public void ShowSelectedFileInExplorer(ComponentFileModel SelectedFile) => ExplorerProvider.OpenFolderAndSelectItem(SelectedFile.LocalFilePath);

    [RelayCommand]
    public void DeleteReferencedProduct(ProductModel Product) => ComponentHost.ComponentModelSelected!
        .DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteProductComponent,
            onErr => Logger.WriteLine(onErr),
            c.Name,
            Product.Name
            ), error => Debug.WriteLine("Delete prdouct ref error")
        )?.Do(success => Debug.WriteLine("Delete prdouct ref successfully"))
        .Do(c => this.SetComponent(c))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection());

    [RelayCommand]
    public void BackgroundProductOpen() { Debug.WriteLine("Try goto selected product"); }
}

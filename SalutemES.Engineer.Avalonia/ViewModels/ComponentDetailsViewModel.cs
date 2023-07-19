using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;
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
        ComponentHost.ComponentModelSelected = ComponentHost.ComponentModelCollection[0] ?? new ComponentModel();

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
    public void SaveInfo()
    {
        if (DataBaseApi.RequestWithBoolResponse(
            DBRequests.EditComponent,
            onErr => Logger.WriteLine(onErr),
            ComponentHost.ComponentModelSelected!.Base!.Name!,
            ComponentHost.ComponentModelSelected!.Base!.Code!,
            ComponentHost.ComponentModelSelected.Name,
            ComponentHost.ComponentModelSelected.Code,
            ComponentHost.ComponentModelSelected.Grade,
            ComponentHost.ComponentModelSelected.Thickness,
            ComponentHost.ComponentModelSelected.Folds,
            ComponentHost.ComponentModelSelected.WeightKG,
            ComponentHost.ComponentModelSelected.Note,
            ComponentHost.ComponentModelSelected.Material
            ))
            Debug.WriteLine("Save successfully");
        else
            Debug.WriteLine("Save error");
    }

    [RelayCommand]
    public void CancelInfoEdit() => ComponentHost.ComponentModelSelected!.ResetByBase();

    [RelayCommand]
    public void DeleteWithAllReference()
    {
        if (DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteComponent,
            onErr => Logger.WriteLine(onErr),
            ComponentHost.ComponentModelSelected!.Name,
            ComponentHost.ComponentModelSelected!.Code
            ))
            Debug.WriteLine("Delete component successfully");
        else
            Debug.WriteLine("Delete component error");
    }

    [RelayCommand]
    public void DeleteReferencedFile(ComponentFileModel SelectedFile)
    {
        if (DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteComponentFile,
            onErr => Logger.WriteLine(onErr),
            ComponentHost.ComponentModelSelected!.Name,
            SelectedFile.LocalFilePath
            ))
            Debug.WriteLine("Delete file successfully");
        else
            Debug.WriteLine("Delete file error");
    }

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
        .Do(filePath => {
            if (DataBaseApi.RequestWithBoolResponse(
                DBRequests.DeleteComponentFile,
                onErr => Logger.WriteLine(onErr),
                ComponentHost.ComponentModelSelected!.Name,
                filePath!
            ))
                Debug.WriteLine("Add file successfully");
            else
                Debug.WriteLine("Add file error");
        });

    [RelayCommand]
    public void ShowSelectedFileInExplorer(ComponentFileModel SelectedFile) => ExplorerProvider.OpenFolderAndSelectItem(SelectedFile.LocalFilePath);

    [RelayCommand]
    public void DeleteReferencedProduct(ProductModel Product)
    {
        if (DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteProductComponent,
            onErr => Logger.WriteLine(onErr),
            ComponentHost.ComponentModelSelected!.Name,
            Product.Name
            ))
            Debug.WriteLine("Delete file successfully");
        else
            Debug.WriteLine("Delete file error");
    }

    [RelayCommand]
    public void BackgroundProductOpen() { Debug.WriteLine("Try goto selected product"); }
}

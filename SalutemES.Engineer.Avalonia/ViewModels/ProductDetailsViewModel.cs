using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ProductDetailsViewModel : ViewModelBase
{
    public ComponentViewModel ComponentHost { get; } = new ComponentViewModel();
    public ProductViewModel ProductHost { get; } = new ProductViewModel();
    public ComponentFileViewModel FilesHost { get; } = new ComponentFileViewModel();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileInfo))]
    private string _newFileLocalPath = "";

    public string FileInfo => $"{NewFileLocalPath.Split("\\").Last()} [{NewFileLocalPath}]";

    public ProductDetailsViewModel() =>
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
            ), error => Logger.WriteLine("Save error")
        )
        ?.DoInst(success => Logger.WriteLine("Save successfully"))
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
            ), error => Logger.WriteLine("Delete component error")
        )?.Do(success => Logger.WriteLine("Delete component successfully"))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection())
        .Do(c => this.ClosePopup());

    [RelayCommand]
    public void DeleteReferencedFile(ComponentFileModel SelectedFile) => ComponentHost.ComponentModelSelected!
        .DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteComponentFile,
            onErr => Logger.WriteLine(onErr),
            c.Name,
            SelectedFile.LocalFilePath
            ), error => Logger.WriteLine("Delete file error")
        )?.DoInst(success => Logger.WriteLine("Delete file successfully"))
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
            ), error => Logger.WriteLine("Add file error")
        )?.DoInst(success => Logger.WriteLine("Add file successfully"))
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
            ), error => Logger.WriteLine("Delete prdouct ref error")
        )?.DoInst(success => Logger.WriteLine("Delete prdouct ref successfully"))
        .Do(c => this.SetComponent(c))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection());

    [RelayCommand]
    public void BackgroundProductOpen() { Logger.WriteLine("Try goto selected product"); }
}

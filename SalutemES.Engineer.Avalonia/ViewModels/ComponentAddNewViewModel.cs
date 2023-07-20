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
using SalutemES;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ComponentAddNewViewModel : ViewModelBase
{
    public ComponentModel ComponentModel { get; } = new ComponentModel();
    public ComponentFileViewModel FilesHost { get; } = new ComponentFileViewModel();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileInfo))]
    private string _newFileLocalPath = "";

    public string FileInfo => $"{NewFileLocalPath.Split("\\").Last()} [{NewFileLocalPath}]";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFree))]
    [NotifyPropertyChangedFor(nameof(IsNameUsed))]
    [NotifyPropertyChangedFor(nameof(IsCodeUsed))]
    [NotifyPropertyChangedFor(nameof(IsFullyUsed))]
    private int _componentMatch = 0;

    public bool IsFree { get => ComponentMatch == 0; }
    public bool IsNameUsed { get => ComponentMatch == 1; }
    public bool IsCodeUsed { get => ComponentMatch == 2; }
    public bool IsFullyUsed { get => ComponentMatch == 3; }

    public ComponentAddNewViewModel() => ComponentModel
        .DoInst(c => c.OnNameChangedAction = () => SearchMatchInDataBase(c))
        .DoInst(c => c.OnCodeChangedAction = () => SearchMatchInDataBase(c))
        .DoInst(c => c.NumericThicknessRegex = true)
        .DoInst(c => c.NumericFoldsRegex = true)
        .DoInst(c => c.NumericWeightKGRegex = true)
        .Do(c => c.ResetByBase());

    public void SearchMatchInDataBase(ComponentModel Component)
    {
        bool flag = ComponentMatch == 0;

        ComponentMatch = DataBaseApi.RequestWithStringResponse(
                DBRequests.CheckComponentExists,
                onErr => Logger.WriteLine(onErr),
                Component.Name,
                Component.Code
            )
            ?.Do(stringNum => Convert.ToInt32(stringNum))
            ?? 0;

        Component.DoIf(c => ComponentMatch == 3, noMatches => FilesHost.ComponentFileModelCollection.DoIf(state => !flag && flag != (ComponentMatch == 0))?.Clear())
            ?.Do(c => FilesHost.FillCollection(c));
    }

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    [RelayCommand]
    public void SaveInfo() => ComponentModel
        .DoIf(c => IsFree, error => Logger.WriteLine("Can't make copy of existed file because u on create new component user control, come on"))
        ?.DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.AddComponent,
            onErr => Logger.WriteLine(onErr),
            c.Name,
            c.Code,
            c.Grade,
            c.Thickness,
            c.Folds,
            c.WeightKG,
            c.Note,
            c.Material
            ), error => Logger.WriteLine("Add new component error")
        )
        ?.DoInst(success => Logger.WriteLine("Add new component successfully"))
        .Do(c => App.Host!.Services.GetRequiredService<ComponentsEditorControl>().ViewModel.ComponentUsageHost.FillCollection())
        .DoInst(c => FilesHost.ComponentFileModelCollection.Do(arr => arr
        .ToList()
        .ForEach(file => DataBaseApi.RequestWithBoolResponse(
            DBRequests.AddComponentFile,
            onErr => Logger.WriteLine(onErr),
            ComponentModel.Name,
            file.LocalFilePath
            ))
        ))
        .Do(c => FilesHost.ComponentFileModelCollection.Clear())
        .Do(c => c.ResetByBase())
        .Do(control => ClosePopup());
        

    [RelayCommand]
    public void DeleteReferencedFile(ComponentFileModel SelectedFile) => ComponentModel
        .DoIf(file => SelectedFile is not null, error => Logger.WriteLine("Can't delete not existed (null, empty, call how u want) file"))
        ?.DoIf(details => IsFree, error => Logger.WriteLine("Can't delete file path from already existed component while creating new one"))
        ?.Do(file => FilesHost.ComponentFileModelCollection.Remove(SelectedFile));

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
        ?.DoIf(details => IsFree, error => Logger.WriteLine("Can't add file path into already existed component while creating new one"))
        ?.DoIf(file => FilesHost.ComponentFileModelCollection.Select(x => x.LocalFilePath == file).Count() == 0, error => Logger.WriteLine("Can't add file, because u already add this one into component"))
        ?.Do(file => FilesHost.ComponentFileModelCollection.Add(new() { LocalFilePath = file }));

    [RelayCommand]
    public void ShowSelectedFileInExplorer(ComponentFileModel SelectedFile) => ExplorerProvider.OpenFolderAndSelectItem(SelectedFile.LocalFilePath);
}

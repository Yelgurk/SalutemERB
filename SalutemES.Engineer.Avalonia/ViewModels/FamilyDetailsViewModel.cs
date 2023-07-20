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

public partial class FamilyDetailsViewModel : ViewModelBase
{
    public FamilyViewModel FamilyHost { get; } = new FamilyViewModel();
    public ProductViewModel ProductHost { get; } = new ProductViewModel();

    public FamilyDetailsViewModel() =>
        FamilyHost.OnFillCollection = () =>
        {
            if (FamilyHost.FamilyModelCollection.Count > 0)
                FamilyHost.FamilyModelSelected = FamilyHost.FamilyModelCollection[0];

            FamilyHost.FamilyModelSelected!.ResetByBase();
        };

    public void SetFamily(FamilyModel Component) => FillCollection(Component.Name);

    public void FillCollection(string Name)
    {
        FamilyHost.FillCollection(DBRequests.GetFamiliesByName, Name);
        ProductHost.FillCollection(FamilyHost.FamilyModelSelected!);
    }

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    [RelayCommand]
    public void SaveInfo() => FamilyHost.FamilyModelSelected!
        .DoIf(f => DataBaseApi.RequestWithBoolResponse(
            DBRequests.EditFamily,
            onErr => Logger.WriteLine(onErr),
            f.Base!.Name!,
            f.Name
            ), error => Logger.WriteLine("Save new fimaly info error")
        )
        ?.DoInst(success => Logger.WriteLine("Save new fimaly info success"))
        .Do(f => this.SetFamily(f))
        .Do(f => App.Host!.Services.GetRequiredService<ProductsEditorControl>().ViewModel.FamilyHost.FillCollection())
        .Do(f => ClosePopup());

    [RelayCommand]
    public void CancelInfoEdit() => FamilyHost.FamilyModelSelected!.ResetByBase();

    [RelayCommand]
    public void DeleteWithAllReference() => FamilyHost.FamilyModelSelected!
        .DoIf(f => ProductHost.ProductModelCollection.Count == 0, error => Logger.WriteLine("Can't delete while category contains products(solutions)"))
        ?.DoIf(f => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteFamily,
            onErr => Logger.WriteLine(onErr),
            f.Name
            ), error => Logger.WriteLine("Delete component error")
        )?.DoInst(success => Logger.WriteLine("Delete component successfully"))
        .Do(f => App.Host!.Services.GetRequiredService<ProductsEditorControl>().ViewModel.FamilyHost.FillCollection())
        .Do(f => this.ClosePopup());
}
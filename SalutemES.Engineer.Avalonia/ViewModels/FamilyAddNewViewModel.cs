using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class FamilyAddNewViewModel :ViewModelBase
{
    public FamilyModel FamilyModel { get; } = new FamilyModel();
    public ProductViewModel ProductHost { get; } = new ProductViewModel();

    [ObservableProperty]
    private int _componentMatch = 0;

    public FamilyAddNewViewModel() => FamilyModel
        .DoInst(f => f.OnNameChangedAction = () => SearchMatchInDataBase(f))
        .Do(c => c.ResetByBase());

    public void SearchMatchInDataBase(FamilyModel Family)
    {
        int flag = ComponentMatch;

        ComponentMatch = DataBaseApi.RequestWithStringResponse(
                DBRequests.CheckFamilyExists,
                onErr => Logger.WriteLine(onErr),
                Family.Name
            )
            ?.Do(stringNum => { return Convert.ToInt32(stringNum); })
            ?? 0;

        FamilyModel.DoIf(c => ComponentMatch > 0, noMatches => ProductHost.ProductModelCollection.DoIf(state => flag > 0 && ComponentMatch == 0)?.Clear())
            ?.Do(f => ProductHost.FillCollection(f));
    }

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    [RelayCommand]
    public void SaveInfo() => FamilyModel
        .DoIf(c => ComponentMatch == 0, error => Logger.WriteLine("Can't make copy of existed file because u on create new family(category) user control, come on"))
        ?.DoIf(c => DataBaseApi.RequestWithBoolResponse(DBRequests.AddFamily,onErr => Logger.WriteLine(onErr),c.Name), error => Logger.WriteLine("Add new component error"))
        ?.DoInst(success => Logger.WriteLine("Add new component successfully"))
        .Do(c => App.Host!.Services.GetRequiredService<ProductsEditorControl>().ViewModel.FamilyHost.FillCollection())
        .Do(c => c.ResetByBase())
        .Do(control => ClosePopup());
}

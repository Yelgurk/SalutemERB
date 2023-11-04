using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ProductAddViewModel : ViewModelBase
{
    public ProductModel Product { get; } = new();
    public ExportComponentViewModel ComponentUsedInProd { get; } = new();
    public ExportComponentViewModel ComponentInDataBase { get; } = new();
    public FamilyViewModel FamilyHost { get; } = new();

    [ObservableProperty]
    private int _componentMatch = 0;

    public ProductAddViewModel() => this
        .Do(_ => Product.DoInst(p => p.OnNameChangedAction = () => SearchMatchInDataBase(Product)).Do(p => p.ResetByBase()))
        .Do(_ => ComponentUsedInProd.OnSelectedModelChanged = () =>
        {
            ComponentUsedInProd.ExportComponentModelSelected
                ?.Do(model => model.NumericCountRegex = true)
                .Do(model => model!.FilesCollection.FillCollection(ComponentUsedInProd.ExportComponentModelSelected!));
        })
        .Do(_ => ComponentInDataBase.OnSelectedModelChanged = () =>
        {
            ComponentInDataBase.ExportComponentModelSelected?
                .FilesCollection.FillCollection(ComponentInDataBase.ExportComponentModelSelected!);
        });

    public void RefreshFilesAndModels() => this
        .Do(_ => Product.ResetByBase())
        .Do(_ => FamilyHost.FillCollection())
        .Do(_ => ComponentInDataBase.FillCollection());

    public void SearchMatchInDataBase(ProductModel Product)
    {
        int flag = ComponentMatch;

        ComponentMatch = DataBaseApi.RequestWithStringResponse(
                DBRequests.CheckProductExists,
                onErr => Logger.WriteLine(onErr),
                Product.Name
            )
            ?.Do(stringNum => { return Convert.ToInt32(stringNum); })
            ?? 0;

        Product.DoIf(c => ComponentMatch > 0, noMatches => ComponentUsedInProd.ExportComponentModelCollection.DoIf(state => flag > 0 && ComponentMatch == 0)?.Clear())
            ?.Do(f => ComponentUsedInProd.FillCollection(f));
    }

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    [RelayCommand]
    public void SaveInfo() => Product
        .DoIf(p => ComponentMatch == 0)
        ?.DoIf(_ => FamilyHost.FamilyModelSelected is not null)
        ?.DoIf(p => !string.IsNullOrEmpty(p.FamilyName))
        ?.DoIf(p => DataBaseApi.RequestWithBoolResponse(
            DBRequests.AddProduct,
            onErr => Logger.WriteLine(onErr),
            p.Name,
            p.FamilyName
            ), error => Logger.WriteLine("Save new product(solution) error")
        )
        ?.DoInst(success => Logger.WriteLine("Save product(solution) success"))
        .Do(p => ComponentUsedInProd.ExportComponentModelCollection.ToList().ForEach(c => {
            c.DoIf(_ => DataBaseApi.RequestWithBoolResponse(
                DBRequests.AddProductComponent,
                onErr => Logger.WriteLine(onErr),
                p.Name,
                c.Code,
                c.Count
                ), error => Logger.WriteLine("Can't add cortage in db"))
            ?.Do(log => Logger.WriteLine($"Success add [{c.Name}] in [{p.Name}]"));
        }))
        //.Do(p => this.SetProduct(p))
        .Do(_ => App.Host!.Services.GetRequiredService<OrderBuilderControl>().ViewModel.RefreshProductInDataBase())
        .Do(_ => ClosePopup());

    [RelayCommand]
    public void EditFamily() => Product
        .DoIf(_ => FamilyHost.FamilyModelSelected is not null)
        ?.DoIf(_ => FamilyHost.FamilyModelSelected!.Name.Length > 0)
        ?.Do(p => p.FamilyName = FamilyHost.FamilyModelSelected!.Name);

    [RelayCommand]
    public void CancelInfoEdit() => Product.ResetByBase();

    [RelayCommand]
    public void ShowSelectedFileInExplorer(ComponentFileModel SelectedFile) => ExplorerProvider.OpenFolderAndSelectItem(SelectedFile.LocalFilePath);

    [RelayCommand]
    public void AddComponentReference() => Product
        .DoIf(_ => ComponentMatch == 0)
        ?.DoIf(_ => ComponentInDataBase.ExportComponentModelSelected is not null)
        ?.Do(_ => ComponentUsedInProd.ExportComponentModelCollection.Add(ComponentInDataBase.ExportComponentModelSelected!));

    [RelayCommand]
    public void DeleteComponentReference() => Product
        .DoIf(_ => ComponentMatch == 0)
        ?.DoIf(_ => ComponentUsedInProd.ExportComponentModelSelected is not null)
        ?.Do(_ => ComponentUsedInProd.ExportComponentModelCollection.Remove(ComponentUsedInProd.ExportComponentModelSelected!));

    [RelayCommand]
    public void IncrementComponentCount(ExportComponentModel Component) => Component
        .DoIf(_ => ComponentMatch == 0)
        ?.Do(_ => Convert.ToInt32(double.Parse(Component.Count, CultureInfo.InvariantCulture)) + 1)
        .Do(x => x < 0 ? 0 : x)
        .Do(x => Component.Count = x.ToString());

    [RelayCommand]
    public void DecrementComponentCount(ExportComponentModel Component) => Component
        .DoIf(_ => ComponentMatch == 0)
        ?.Do(_ => Convert.ToInt32(double.Parse(Component.Count, CultureInfo.InvariantCulture)) - 1)
        .Do(x => x < 0 ? 0 : x)
        .Do(x => Component.Count = x.ToString());
}

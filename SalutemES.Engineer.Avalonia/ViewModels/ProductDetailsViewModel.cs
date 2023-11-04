using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class ProductDetailsViewModel : ViewModelBase
{
    public ProductViewModel ProductHost { get; } = new();
    public ExportComponentViewModel ComponentUsedInProd { get; } = new();
    public ExportComponentViewModel ComponentInDataBase { get; } = new();
    public FamilyViewModel FamilyHost { get; } = new();

    public ProductDetailsViewModel() => this
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

    public void SetProduct(ProductWithFullComponentsModel Product) => this
        .Do(_ => this.FillCollection(Product.Name))
        .Do(_ => FamilyHost.FillCollection());

    public void SetProduct(ProductWithComponentsModel Product) => this
        .Do(_ => this.FillCollection(Product.Name))
        .Do(_ => FamilyHost.FillCollection());

    public void SetProduct(ProductModel Product) => this
        .Do(_ => this.FillCollection(Product.Name))
        .Do(_ => FamilyHost.FillCollection());

    public void FillCollection(string ProductName) => this
        .Do(_ => ProductHost.FillCollection(ProductName))
        .Do(_ => ProductHost.ProductModelSelected = ProductHost.ProductModelCollection.FirstOrDefault())
        .Do(_ => this.FillCollection());

    private void FillCollection() => this
        .Do(_ => ComponentUsedInProd.FillCollection(ProductHost.ProductModelSelected!))
        .Do(_ => ComponentInDataBase.FillCollection());

    [RelayCommand]
    public void ClosePopup() => App.Host!.Services.GetRequiredService<MainWindow>().ViewModel.ClosePopupControl();

    [RelayCommand]
    public void SaveInfo() => ProductHost.ProductModelSelected!
        .DoIf(p => DataBaseApi.RequestWithBoolResponse(
            DBRequests.EditProductName,
            onErr => Logger.WriteLine(onErr),
            p.Base!.Name!,
            p.Name
            ), error => Logger.WriteLine("+ Save new product(solution) name error")
        )
        ?.DoInst(success => Logger.WriteLine("Save product(solution) name success"))
        .Do(p => ComponentUsedInProd.ExportComponentModelCollection.ToList().ForEach(c => {
            c.DoIf(_ => DataBaseApi.RequestWithBoolResponse(
                DBRequests.AddProductComponent,
                onErr => Logger.WriteLine(onErr),
                p.Name,
                c.Code,
                c.Count
                ), error => Logger.WriteLine("+ Can't add cortage in db"))
            ?.Do(log => Logger.WriteLine($"Success update info for [{p.Name}] about [{c.Name}]"));
        }))
        .Do(p => this.SetProduct(p))
        .Do(_ => ClosePopup());

    [RelayCommand]
    public void EditFamily() => ProductHost.ProductModelSelected!
        .DoIf(p => DataBaseApi.RequestWithBoolResponse(
            DBRequests.EditProductFamily,
            onErr => Logger.WriteLine(onErr),
            p.Base!.Name!,
            FamilyHost.FamilyModelSelected!.Name
            ), error => Logger.WriteLine("+ Save new product(solution) family error")
        )
        ?.DoInst(success => Logger.WriteLine("Save product(solution) family success"))
        .Do(p => p.FamilyName = FamilyHost.FamilyModelSelected!.Name)
        .Do(_ => App.Host!.Services.GetRequiredService<ProductsEditorControl>().ViewModel)
        .Do(vm => vm.FamilyHost.FillCollection())
        .Do(vm => vm.UpdateProductList());

    [RelayCommand]
    public void CancelInfoEdit() => ProductHost.ProductModelSelected!.ResetByBase();

    [RelayCommand]
    public void DeleteWithAllReference() => ProductHost.ProductModelSelected!
        .DoIf(c => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteProduct,
            onErr => Logger.WriteLine(onErr),
            c.Name
            ), error => Logger.WriteLine("Delete product error")
        )?.Do(success => Logger.WriteLine("Delete product successfully"))
        .Do(_ => App.Host!.Services.GetRequiredService<ProductsEditorControl>().ViewModel)
        .Do(vm => vm.FamilyHost.FillCollection())
        .Do(vm => vm.UpdateProductList())
        .Do(_ => this.ClosePopup());

    [RelayCommand]
    public void ShowSelectedFileInExplorer(ComponentFileModel SelectedFile) => ExplorerProvider.OpenFolderAndSelectItem(SelectedFile.LocalFilePath);

    [RelayCommand]
    public void AddComponentReference() => ProductHost.ProductModelSelected!
        .DoIf(_ => ComponentInDataBase.ExportComponentModelSelected is not null)
        ?.DoIf(p => DataBaseApi.RequestWithBoolResponse(
            DBRequests.AddProductComponent,
            onErr => Logger.WriteLine(onErr),
            p.Name,
            ComponentInDataBase.ExportComponentModelSelected!.Code,
            ComponentInDataBase.ExportComponentModelSelected!.Count
            ), error => Logger.WriteLine("Add component ref to product error")
        )?.DoInst(success => Logger.WriteLine("Add component ref to product successfully"))
        .Do(p => this.SetProduct(p));

    [RelayCommand]
    public void DeleteComponentReference() => ProductHost.ProductModelSelected!
        .DoIf(_ => ComponentUsedInProd.ExportComponentModelSelected is not null)
        ?.DoIf(p => DataBaseApi.RequestWithBoolResponse(
            DBRequests.DeleteProductComponent,
            onErr => Logger.WriteLine(onErr),
            ComponentUsedInProd.ExportComponentModelSelected!.Code,
            p.Name
            ), error => Logger.WriteLine("Delete product ref to component error")
        )?.DoInst(success => Logger.WriteLine("Delete product ref to component successfully"))
        .Do(p => this.SetProduct(p));

    [RelayCommand]
    public void IncrementComponentCount(ExportComponentModel Component) => Component
        .Do(_ => Convert.ToInt32(double.Parse(Component.Count, CultureInfo.InvariantCulture)) + 1)
        .Do(x => x < 0 ? 0 : x)
        .Do(x => Component.Count = x.ToString());

    [RelayCommand]
    public void DecrementComponentCount(ExportComponentModel Component) => Component
        .Do(_ => Convert.ToInt32(double.Parse(Component.Count, CultureInfo.InvariantCulture)) - 1)
        .Do(x => x < 0 ? 0 : x)
        .Do(x => Component.Count = x.ToString());
}

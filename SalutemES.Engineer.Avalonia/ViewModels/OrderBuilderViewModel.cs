using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class OrderBuilderViewModel : ViewModelBase
{
    public FamilyViewModel FamilyHost { get; } = new FamilyViewModel();
    public ProductWithFullComponentsViewModel ProductInDataBase { get; } = new ProductWithFullComponentsViewModel();
    public ProductWithFullComponentsViewModel ProductInOrder { get; } = new ProductWithFullComponentsViewModel();

    public OrderBuilderViewModel() => this
        .Do(_ => FamilyHost.OnSelectedModelChanged = () => {
            ProductInDataBase.FillCollection(FamilyHost.FamilyModelSelected!);
        })
        .Do(_ => ProductInDataBase.OnSelectedModelChanged = () => {
            ProductInDataBase.ProductWithFullComponentsModelSelected!.NumericCountRegex = true;
            ProductInDataBase.ProductWithFullComponentsModelSelected!.Count = 1.ToString();
            ProductInDataBase.ProductWithFullComponentsModelSelected!.Components.FillCollection(ProductInDataBase.ProductWithFullComponentsModelSelected!);
        })
        .Do(_ => FamilyHost.FillCollection())
        .Do(_ => FamilyHost.FamilyModelSelected = FamilyHost.FamilyModelCollection.First());

    public void RefreshProductInDataBase() => FamilyHost.FamilyModelSelected
        ?.Do(f => ProductInDataBase.FillCollection(f!));

    [RelayCommand]
    public void ClearOrder() => ProductInOrder.ProductWithFullComponentsModelCollection.Clear();

    [RelayCommand]
    public void AddInOrder() => ProductInDataBase.ProductWithFullComponentsModelSelected
        ?.DoIf(prod => ProductInDataBase.ProductWithFullComponentsModelCollection.Contains(prod))
        .Do(x => Debug.WriteLine(x!.Name))
        ?.DoIf(prod => !ProductInOrder.ProductWithFullComponentsModelCollection.Select(x => x.Name == prod.Name).Contains(true), error => Debug.WriteLine("Prod already in order list for export"))
        ?.Do(prod => ProductInOrder.ProductWithFullComponentsModelCollection.Add(prod));

    [RelayCommand]
    public void RemoveFromOrder() => ProductInOrder.ProductWithFullComponentsModelSelected
        ?.Do(prod => ProductInOrder.ProductWithFullComponentsModelCollection.Remove(prod));

    [RelayCommand]
    public void IncrementProdCount(ProductWithFullComponentsModel Prod) => Prod
        .Do(_ => Convert.ToInt32(double.Parse(Prod.Count, CultureInfo.InvariantCulture)) + 1)
        .Do(x => x < 0 ? 0 : x)
        .Do(x => Prod.Count = x.ToString());

    [RelayCommand]
    public void DecrementProdCount(ProductWithFullComponentsModel Prod) => Prod
        .Do(_ => Convert.ToInt32(double.Parse(Prod.Count, CultureInfo.InvariantCulture)) - 1)
        .Do(x => x < 0 ? 0 : x)
        .Do(x => Prod.Count = x.ToString());

    [RelayCommand]
    public void ExportOrder() { }
}

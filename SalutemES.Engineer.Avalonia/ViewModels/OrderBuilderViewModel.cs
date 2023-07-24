using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SalutemES.Engineer.Avalonia.Views;
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Domain;
using SalutemES.Engineer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class OrderBuilderViewModel : ViewModelBase
{
    public FamilyViewModel FamilyHost { get; } = new FamilyViewModel();
    public ProductWithFullComponentsViewModel ProductInDataBase { get; } = new ProductWithFullComponentsViewModel();
    public ProductWithFullComponentsViewModel ProductInOrder { get; } = new ProductWithFullComponentsViewModel();
    public ExportComponentViewModel OrderComponents { get; } = new ExportComponentViewModel();

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
    public void ExportOrder()
    {
        string ReportTime = DateTime.Now.ToString("yyyy'_'MM'_'dd'_'HH'_'mm'_'ss");

        string ReportsContainerPath = $"{Environment.CurrentDirectory}\\Reports";
        string ReportGeneralFolderPath = $"{ReportsContainerPath}\\{ReportTime}_Салутем_Заявка";
        string ReportMergeFolderPath = $"{ReportGeneralFolderPath}\\{ReportTime}_Заявка";

        string GetProductFileSpecialName(ProductWithFullComponentsModel prod) => $"{prod.Index}_{prod.Name}";
        string GetProductReportFolder(ProductWithFullComponentsModel prod) => $"{ReportGeneralFolderPath}\\{GetProductFileSpecialName(prod)}";
        string GetProductReportFilePath(ProductWithFullComponentsModel prod, ComponentFileModel file) => $"{GetProductReportFolder(prod)}\\{file.FileName}";
        string GetReportMergeFolderPath(ComponentFileModel file) => $"{ReportMergeFolderPath}\\{file.FileName}";
        
        if (!Directory.Exists(ReportsContainerPath))
            Directory.CreateDirectory(ReportsContainerPath);

        ProductInOrder.ProductWithFullComponentsModelCollection
            .DoIf(arr => arr.Count > 0)
            ?.DoInst(_ => Directory.CreateDirectory(ReportGeneralFolderPath))
            .DoInst(_ => Directory.CreateDirectory(ReportMergeFolderPath))
            .Select(x => x)
            .ToList()
            .Do(list =>
            {
                for (int i = 0; i < list.Count; i++)
                    list[i].Index = i + 1;
            })
            .Do(list => list.ForEach(p => Directory.CreateDirectory(GetProductReportFolder(p))))
            .Do(list => list.ForEach(p =>
            {
                p.Components.FillCollection(p);
                p.Components.ExportComponentModelCollection
                    .ToList()
                    .Do(arr => arr.ForEach(comp => comp.FilesCollection.FillCollection(comp)))
                    .SelectMany(comp => comp.FilesCollection.ComponentFileModelCollection)
                    .ToList()
                    .Do(list => list.ForEach(file => File.Copy(file.LocalFilePath, GetProductReportFilePath(p, file))))
                    .Do(list => list.ForEach(file => File.Copy(file.LocalFilePath, GetReportMergeFolderPath(file))));
            }))
            .Do(list => list.ForEach(p => {
                OrderComponents.FillCollection(new List<ExportRequestTableAsArgBase>() { new ExportRequestTableAsArgBase() { product = p.Name, count = Convert.ToInt32(p.Count) } });
                ExcelOrderFileExport(ReportGeneralFolderPath, GetProductFileSpecialName(p), OrderComponents.ExportComponentModelCollection.ToList());
            }))
            .Do(list => {
                OrderComponents.FillCollection(new List<ExportRequestTableAsArgBase>(
                    list.Select(p => new ExportRequestTableAsArgBase() { product = p.Name, count = Convert.ToInt32(p.Count) }).ToList()
                    ));
                ExcelOrderFileExport(ReportMergeFolderPath, $"{ReportTime}_Заявка", OrderComponents.ExportComponentModelCollection.ToList());
            });
    }

    private void ExcelOrderFileExport(string ExportFolderPath, string ExportFileName, List<ExportComponentModel> ExcelContentSource)
    {
        string GetExportFilePath() => $"{ExportFolderPath}\\{ExportFileName}.xlsx";
    }
}

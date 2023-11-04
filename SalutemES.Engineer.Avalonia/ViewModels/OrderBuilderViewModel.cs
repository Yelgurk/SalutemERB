using ClosedXML.Excel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
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
        bool FileNotExists = false;

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
                    .Do(list => list.ForEach(file => file
                        .DoIf(exists => File.Exists(exists.LocalFilePath), not => { FileNotExists = true; Logger.WriteLine($"Отсутствует файл: {not.FileName} [{not.LocalFilePath}]"); })
                        ?.Do(_ => File.Copy(file.LocalFilePath, GetProductReportFilePath(p, file), true))
                        ))
                    .Do(list => list.ForEach(file => file
                        .DoIf(exists => File.Exists(exists.LocalFilePath), not => { FileNotExists = true; Logger.WriteLine($"Отсутствует файл: {not.FileName} [{not.LocalFilePath}]"); })
                        ?.Do(_ => File.Copy(file.LocalFilePath, GetReportMergeFolderPath(file), true))
                        ));
                /*
                    .Do(list => list.ForEach(file => { if (!File.Exists(GetProductReportFilePath(p, file))) File.Copy(file.LocalFilePath, GetProductReportFilePath(p, file)); }))
                    .Do(list => list.ForEach(file => { if (!File.Exists(GetReportMergeFolderPath(file))) File.Copy(file.LocalFilePath, GetReportMergeFolderPath(file)); }));
                */
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
            })
            .Do(_ => ExplorerProvider.OpenFolderAndSelectItem(ReportGeneralFolderPath, ""))
            .Do(_ => ClearOrder())
            .DoIf(_ => FileNotExists && Logger.FilePath is not null)
            ?.Do(_ => ExplorerProvider.OpenFolderAndSelectItem(Logger.FilePath!, "log.txt"));
    }

    private void ExcelOrderFileExport(string ExportFolderPath, string ExportFileName, List<ExportComponentModel> ExcelContentSource)
    {
        string GetExportFilePath() => $"{ExportFolderPath}\\{ExportFileName}.xlsx";

        using (var workbook = new XLWorkbook())
        {
            var w = workbook.Worksheets.Add("Заявка");

            w.Cell("B2").Value = "ООО \"Салутем\"";
            w.Cell("B2").Style
                .DoInst(s => s.Font.SetItalic())
                .DoInst(s => s.Font.SetFontSize(14));

            w.Cell("C1").Value = "Заявка";
            w.Cell("C2").Value = "в расчет";
            w.Range("C1:C2").Style
                .DoInst(s => s.Border.OutsideBorder = XLBorderStyleValues.Thin)
                .DoInst(s => s.Font.SetBold())
                .DoInst(s => s.Font.SetFontSize(18))
                .DoInst(s => s.Alignment.Horizontal = XLAlignmentHorizontalValues.Center);
            w.Cell("C2").Style.Font.SetFontSize(14);

            w.Cell("D2").Value = "№:";
            w.Range("E2:G2").Merge();

            w.Range("D2:G2").Style
                .DoInst(s => s.Font.SetBold())
                .DoInst(s => s.Alignment.Horizontal = XLAlignmentHorizontalValues.Center);

            w.Cell("A4").Value = "№ п.п.";
            w.Cell("B4").Value = "Наименование";
            w.Cell("C4").Value = "Код изделия";
            w.Cell("D4").Value = "Кол-во";
            w.Cell("E4").Value = "Марка";
            w.Cell("F4").Value =
                """
                Толщ.,
                мм
                """;
            w.Cell("G4").Value = "Гибы";
            w.Cell("H4").Value =
                """
                Масса,
                кг
                """;
            w.Cell("I4").Value =
                """
                Масса
                сумм, кг
                """;
            w.Cell("J4").Value = "Примечание";

            w.Range("A4:J4").Style
                .DoInst(s => s.Border.OutsideBorder = XLBorderStyleValues.Thin)
                .DoInst(s => s.Alignment.Horizontal = XLAlignmentHorizontalValues.Center)
                .DoInst(s => s.Alignment.Vertical = XLAlignmentVerticalValues.Center);

            int headerEndIndex = 4,
                componentsCount = ExcelContentSource.Count,
                footerAfterLine = 5,
                footerStart = headerEndIndex + componentsCount + footerAfterLine;

            for (int i = 0, cell = headerEndIndex + 1; i < componentsCount; i++)
            {
                w.Cell($"A{cell + i}").Value = i + 1;
                w.Cell($"B{cell + i}").Value = ExcelContentSource[i].Name;
                w.Cell($"C{cell + i}").Value = ExcelContentSource[i].Code;
                w.Cell($"D{cell + i}").Value = ExcelContentSource[i].Count;
                w.Cell($"E{cell + i}").Value = ExcelContentSource[i].Grade;
                w.Cell($"F{cell + i}").Value = ExcelContentSource[i].Thickness;
                w.Cell($"G{cell + i}").Value = ExcelContentSource[i].Folds;
                w.Cell($"H{cell + i}").Value = ExcelContentSource[i].WeightKG;
                w.Cell($"I{cell + i}").Value = ExcelContentSource[i].TotalKG;
                w.Cell($"J{cell + i}").Value = ExcelContentSource[i].Note;

                w.Range($"A{cell + i}:J{cell + i}").Style.Border
                    .DoInst(b => b.OutsideBorder = XLBorderStyleValues.Thin)
                    .DoInst(b => b.InsideBorder = XLBorderStyleValues.Thin);
                w.Range($"A{cell + i}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                w.Range($"D{cell + i}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                w.Range($"E{cell + i}:J{cell + i}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }

            if (footerAfterLine > 3)
                w.Range($"A{footerStart - footerAfterLine + 1}:J{footerStart - 2}").Style
                    .DoInst(s => s.Border.SetBottomBorder(XLBorderStyleValues.Dotted));

            w.Cell($"A{footerStart + 0}").Value = "Итог";
            w.Cell($"D{footerStart + 0}").Value = ExcelContentSource.Sum(comp => Convert.ToDouble(comp.Count));
            w.Cell($"I{footerStart + 0}").Value = ExcelContentSource.Sum(comp => Convert.ToDouble(comp.TotalKG));

            w.Range($"A{footerStart + 0}:J{footerStart + 0}").Style.Border
                .DoInst(b => b.TopBorder = XLBorderStyleValues.Thin)
                .DoInst(b => b.BottomBorder = XLBorderStyleValues.Thin);
            w.Range($"A{footerStart + 0}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            w.Range($"D{footerStart + 0}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            w.Range($"E{footerStart + 0}:J{footerStart + 0}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            w.Columns().AdjustToContents();

            workbook.SaveAs(GetExportFilePath());
        }
    }
}

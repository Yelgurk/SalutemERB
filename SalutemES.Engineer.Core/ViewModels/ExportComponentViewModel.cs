using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ExportComponentModel))]
public partial class ExportComponentViewModel
{
    public void FillCollection(ProductWithFullComponentsModel Product) => FillCollection(DBRequests.GetProductComponentsFullInfo, Product.Name);
    public void FillCollection(ProductModel Product) => FillCollection(DBRequests.GetProductComponentsFullInfo, Product.Name);
    public void FillCollection() => FillCollection(DBRequests.GetProductsListFullInfo);
    public ExportComponentViewModel FillCollection<T>(List<T> DataBaseArg)
    {
        ExportComponentModelCollection.Clear();
        DataBaseApi.RequestWithTableAsArg(DBProceduresWithTableArg.GetFullExportTable, DBTableTypeNames.ExportRequestTableType, DataBaseArg)
            ?.ForEach(cortage =>
            {
                ExportComponentModelCollection.Add(new ExportComponentModel(cortage));
                if (ExportComponentModelSelected!.Equals(ExportComponentModelCollection.Last()))
                    ExportComponentModelSelected = ExportComponentModelCollection.Last();
            });

        OnFillCollection?.Invoke();

        return this;
    }
}
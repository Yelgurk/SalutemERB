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
}

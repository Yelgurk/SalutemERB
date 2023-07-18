using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ComponentModel))]
public partial class ComponentViewModel
{
    public void FillCollection(ProductModel Product) => FillCollection(DBRequests.GetComponentsListByProduct, Product.Name);
    public void FillCollection(ProductWithComponentsModel Product) => FillCollection(DBRequests.GetComponentsListByProduct, Product.Name);
}

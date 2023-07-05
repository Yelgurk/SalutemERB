using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ProductModel))]
public partial class ProductViewModel
{
    public void FillCollection(ComponentModel Component) => FillCollection(DBRequests.GetProductsListByComponent, Component.Name);

    public void FillCollection(FamilyModel Family) => FillCollection(DBRequests.GetProductsListByFamily, Family.Name);
}

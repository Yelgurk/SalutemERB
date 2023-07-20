using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ProductWithFullComponentsModel))]
public partial class ProductWithFullComponentsViewModel
{
    public ProductWithFullComponentsViewModel FillCollection(FamilyModel Family) => FillCollection(DBRequests.GetProductsListByFamily, Family.Name);
}

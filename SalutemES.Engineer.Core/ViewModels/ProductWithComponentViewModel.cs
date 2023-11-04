using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ProductWithComponentsModel))]
public partial class ProductWithComponentViewModel
{
    public ProductWithComponentViewModel FillCollection(ComponentUsageModel Component) => FillCollection(DBRequests.GetProductsListByComponent, Component.Code);
}

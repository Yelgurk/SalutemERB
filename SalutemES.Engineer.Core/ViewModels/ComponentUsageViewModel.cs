using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ComponentUsageModel))]
public partial class ComponentUsageViewModel
{
    public void FillCollection() => this.FillCollection(DBRequests.GetComponentsList);
}

using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System.Collections.ObjectModel;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(FamilyModel))]
public partial class FamilyViewModel
{
    public void FillCollection() => FillCollection(DBRequests.GetFamilies);
}
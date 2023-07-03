using CommunityToolkit.Mvvm.ComponentModel;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(FamilyModel))]
public partial class FamilyViewModel
{

    public void FillCollection() => FillCollection(DBRequests.GetFamilies);
}

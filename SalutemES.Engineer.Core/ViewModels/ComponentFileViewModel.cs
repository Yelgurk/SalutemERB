using SalutemES.Engineer.Infrastructure.DataBase;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ViewModelContext(typeof(ComponentFileModel))]
public partial class ComponentFileViewModel
{
    public void FillCollection(ComponentModel Component) => FillCollection(DBRequests.GetFilesListByComponent, Component.Name);
}

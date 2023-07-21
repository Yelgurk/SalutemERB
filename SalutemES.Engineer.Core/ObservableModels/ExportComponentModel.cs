using SalutemES.Engineer.Domain;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ModelBySourceProperties(typeof(ExportExcelBase))]
public partial class ExportComponentModel
{
    public ComponentFileViewModel FilesCollection { get; } = new ComponentFileViewModel();
}
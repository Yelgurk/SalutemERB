using SalutemES.Engineer.Domain;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ModelBySourceProperties(typeof(ProductBase))]
public partial class ProductWithFullComponentsModel
{
    public int Index { get; set; } = 0;
    public ExportComponentViewModel Components { get; } = new ExportComponentViewModel();
}

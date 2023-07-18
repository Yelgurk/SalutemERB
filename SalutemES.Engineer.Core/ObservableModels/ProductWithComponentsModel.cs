using SalutemES.Engineer.Domain;
using SalutemES.Engineer.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core;

[ModelBySourceProperties(typeof(ProductBase))]
public partial class ProductWithComponentsModel
{
    public ComponentViewModel Components { get; } = new ComponentViewModel();
}

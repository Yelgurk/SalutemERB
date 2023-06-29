using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class FamilyModel : ReflectionExtension
{
    public FamilyModel() { }
    public FamilyModel(string[] DataBaseResponse) => FillModel(DataBaseResponse, this);

    public string? Name { get; set; }
    public string? Count { get; set; }
}

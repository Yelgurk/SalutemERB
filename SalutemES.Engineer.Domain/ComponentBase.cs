using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class ComponentBase : ReflectionExtension
{
    public ComponentBase() { }
    public ComponentBase(string[] DataBaseResponse) => FillModel(DataBaseResponse, this);

    public string? ProductName { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Grade { get; set; }
    public string? Thickness { get; set; }
    public string? Folds { get; set; }
    public string? WeightKG { get; set; }
    public string? Note { get; set; }
    public string? Material { get; set; }
    public string? FilesCount { get; set; }
}

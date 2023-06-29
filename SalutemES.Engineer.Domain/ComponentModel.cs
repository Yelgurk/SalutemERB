using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class ComponentModel : ReflectionExtension
{
    public ComponentModel() { }
    public ComponentModel(string[] DataBaseResponse) => FillModel(DataBaseResponse, this);

    public string? ProductName { get; set; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public string? Grade { get; init; }
    public string? Thickness { get; init; }
    public string? Folds { get; init; }
    public string? WeightKG { get; init; }
    public string? Note { get; init; }
    public string? Material { get; init; }
    public string? FilesCount { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class ComponentFileModel : ReflectionExtension
{
    public ComponentFileModel() { }
    public ComponentFileModel(string[] DataBaseResponse) => FillModel(DataBaseResponse, this);

    [SkipReflection]
    public int IdToInt => Id is null ? -1 : int.Parse(Id!);
    [SkipReflection]
    public string LocalFileName => LocalFilePath is null ? "none" : LocalFilePath!.Split('\\').Last();

    public string? Id { get; set; }
    public string? ComponentName { get; set; }
    public string? LocalFilePath { get; set; }
}

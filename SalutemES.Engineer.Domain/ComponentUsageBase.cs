using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class ComponentUsageBase : ReflectionExtension
{
    public ComponentUsageBase() { }

    public ComponentUsageBase(string[] DataBaseResponse) => FillModel(DataBaseResponse, this);

    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? FilesCount { get; set; }
    public string? ReferencesCount { get; set; }
}

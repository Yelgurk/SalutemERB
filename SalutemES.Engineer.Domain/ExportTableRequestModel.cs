using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public sealed class ExportTableRequestModel
{
    public string product { get; set; } = string.Empty;
    public int count { get; set; }
}

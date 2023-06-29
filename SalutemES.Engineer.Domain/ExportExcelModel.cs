using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class ExportExcelModel : ReflectionExtensions
{
    private static int _counter = 0;
    public void ClearCounter() => _counter = 0;

    public ExportExcelModel(string[] DataBaseResponse)
    {
        if (DataBaseResponse.Length == IgnoreSkipProperties(this).Length)
        {
            Index = Convert.ToString(++_counter);

            for (int i = 0; i < IgnoreSkipProperties(this).Length; i++)
                IgnoreSkipProperties(this)[i]
                .SetValue(this, DataBaseResponse[i]);
        }
    }

    [SkipReflection]
    public string? Index { get; private init; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public string? Count { get; init; }
    public string? Grade { get; init; }
    public string? Thickness { get; init; }
    public string? Folds { get; init; }
    public string? WeightKG { get; init; }
    public string? TotalKG { get; init; }
    public string? Note { get; init; }
    public string? Material { get; init; }
}

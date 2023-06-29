namespace SalutemES.Engineer.Domain;

public class ExportExcelModel : ReflectionExtension
{
    private static int _counter = 0;
    public static void ClearCounter() => _counter = 0;

    public ExportExcelModel(string[] DataBaseResponse)
    {
        FillModel(DataBaseResponse, this);
        Index = Convert.ToString(++_counter);
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

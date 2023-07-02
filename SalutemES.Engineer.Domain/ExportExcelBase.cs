namespace SalutemES.Engineer.Domain;

public class ExportExcelBase : ReflectionExtension
{
    private static int _counter = 0;
    public static void ClearCounter() => _counter = 0;

    public ExportExcelBase() { }
    public ExportExcelBase(string[] DataBaseResponse)
    {
        FillModel(DataBaseResponse, this);
        Index = Convert.ToString(++_counter);
    }

    [SkipReflection]
    public string? Index { get; private set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Count { get; set; }
    public string? Grade { get; set; }
    public string? Thickness { get; set; }
    public string? Folds { get; set; }
    public string? WeightKG { get; set; }
    public string? TotalKG { get; set; }
    public string? Note { get; set; }
    public string? Material { get; set; }
}

using System.Reflection;

namespace SalutemES.Engineer.Domain;

[AttributeUsage(AttributeTargets.Property)]
public class SkipReflection : Attribute { }

public class ReflectionExtension
{
    private PropertyInfo[]? _currentProperties { get; set; } = null;

    public static PropertyInfo[] IgnoreSkipPropertyInfos(Type ObservableClass) =>
        ObservableClass
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.GetCustomAttribute<SkipReflection>() == null)
        .ToArray();

    public PropertyInfo[] IgnoreSkipProperties(object ObservableClass) =>
        _currentProperties is null
        ? _currentProperties = IgnoreSkipPropertyInfos(ObservableClass.GetType())
        : _currentProperties;

    protected void FillModel<T>(string[] InputData, T Model)
    {
        if (InputData.Length == IgnoreSkipProperties(Model!).Length)
            for (int i = 0; i < IgnoreSkipProperties(Model!).Length; i++)
                IgnoreSkipProperties(Model!)[i]
                .SetValue(Model!, InputData[i]);
    }

    public override string ToString() =>
        IgnoreSkipProperties(this)
        .Select<PropertyInfo, string>(x => Convert.ToString(x.GetValue(this))!)
        .ToArray()
        .Aggregate((prev, current) => prev + $"|{current}");
}
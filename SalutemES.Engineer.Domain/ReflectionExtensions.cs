using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

[AttributeUsage(AttributeTargets.Property)]
public class SkipReflection : Attribute { }

public class ReflectionExtensions
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

    public override string ToString() =>
        IgnoreSkipProperties(this)
        .Select<PropertyInfo, string>(x => Convert.ToString(x.GetValue(this))!)
        .ToArray()
        .Aggregate((prev, current) => prev + $"|{current}");
}
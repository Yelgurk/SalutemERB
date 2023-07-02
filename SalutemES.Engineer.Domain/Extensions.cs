using System;
using System.Collections.Generic;
using System.Text;

namespace SalutemES;

public static partial class Extensions
{
    public static T? Cast<T>(this object obj) where T : class => obj as T;

    public static T? Do<T>(this T obj, Predicate<T> action) => action(obj) ? obj : default(T?);
    public static T Do<T>(this T obj, Action<T> action) { action(obj); return obj; }
    public static T2 Do<T1, T2>(this T1 obj, Func<T1, T2> action) => action(obj);
}

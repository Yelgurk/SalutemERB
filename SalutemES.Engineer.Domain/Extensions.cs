using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public static class Extensions
{
    public static T? Cast<T>(this object obj) where T : class => obj as T;

    public static T Do<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }
}

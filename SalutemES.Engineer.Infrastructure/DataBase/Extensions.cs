using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES;

public static partial class Extensions
{
    public static T? DoIf<T>(this T obj, Func<T, bool> mainAction, Action<T>? errorAction = null) where T : class =>
        mainAction(obj) ? obj : ErrorActionHandler<T>(errorAction, obj);

    private static T? ErrorActionHandler<T>(Action<T>? errorAction, T obj) where T : class
    {
        errorAction?.Invoke(obj);
        return null;
    }
}

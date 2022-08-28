using Digitalroot.Valheim.Common;
using System.Reflection;

namespace Digitalroot.Craftopia.BetterEnchanting
{
  public static class ObjectExtensions
  {
    public static object GetPropValue(this object src, string propName)
    {
      Log.Info(Main.Instance, $"[{nameof(ObjectExtensions)}.{MethodBase.GetCurrentMethod()?.Name}] propName : {propName}");
      Log.Info(Main.Instance, $"[{nameof(ObjectExtensions)}.{MethodBase.GetCurrentMethod()?.Name}] src.GetType().GetProperty(propName)?.GetValue(src, null) : {src.GetType().GetProperty(propName)?.GetValue(src, null)}");
      Log.Info(Main.Instance, $"[{nameof(ObjectExtensions)}.{MethodBase.GetCurrentMethod()?.Name}] src.GetType().GetField(propName)?.GetValue(src) : {src.GetType().GetField(propName)?.GetValue(src)}");
      return src.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(src, null) 
             ?? src.GetType().GetField(propName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(src);
    }
  }
}

using Digitalroot.Valheim.Common;
using Oc;
using Oc.Item;
using Oc.Item.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Digitalroot.Craftopia.BetterEnchanting
{
  /// <summary>
  /// Sort by EnchantRarity in desc order.
  /// </summary>
  public class CraftEnchantSheetComparer : IComparer<OcUI_CraftEnchantSheet>, ITraceableLogging
  {
    private readonly List<KeyValuePair<int, EnchantRarity>> _soEnchantmentCache;
    public CraftEnchantSheetComparer(List<KeyValuePair<int, EnchantRarity>> soEnchantmentCache)
    {
      try
      {
        Log.Trace(Main.Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        _soEnchantmentCache = soEnchantmentCache;
      }
      catch (Exception e)
      {
        Log.Error(Main.Instance, e);
      }
    }

    #region Implementation of IComparer<in OcUI_CraftEnchantSheet>

    /// <inheritdoc />
    public int Compare(OcUI_CraftEnchantSheet x, OcUI_CraftEnchantSheet y)
    {
      try
      {
        Log.Trace(Main.Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}(x?.EnchantID: {x?.EnchantID}, y?.EnchantID: {y?.EnchantID})");
        if (x?.EnchantID == y?.EnchantID) return 0; // ==
        var xr = _soEnchantmentCache.FirstOrDefault(kvp => kvp.Key == x?.EnchantID);
        var yr = _soEnchantmentCache.FirstOrDefault(kvp => kvp.Key == y?.EnchantID);
        Log.Trace(Main.Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}(xr: {xr.Value}, yr: {yr.Value})");
        Log.Trace(Main.Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}(xr: {xr.Value.ToInt()}, yr: {yr.Value.ToInt()})");
        if (xr.Value.ToInt() == yr.Value.ToInt()) return 0;
        if (xr.Value.ToInt() > yr.Value.ToInt()) return -1; // <
        return 1;                           // >
      }
      catch (Exception e)
      {
        Log.Error(Main.Instance, e);
        return 0;
      }
    }

    #endregion

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Main.Instance.Source;

    /// <inheritdoc />
    public bool EnableTrace => Main.Instance.EnableTrace;

    #endregion
  }
}

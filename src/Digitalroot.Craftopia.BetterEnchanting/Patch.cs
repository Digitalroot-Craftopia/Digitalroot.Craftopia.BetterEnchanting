using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using Oc.Item.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Digitalroot.Craftopia.BetterEnchanting
{
  [UsedImplicitly]
  public class Patch
  {
    [HarmonyPatch(typeof(Oc.Item.OcItemDataMng), nameof(Oc.Item.OcItemDataMng.CaleEnchantChance))]
    // ReSharper disable once InconsistentNaming
    public class PatchOcUI_HomeScene
    {
      [UsedImplicitly]
      [HarmonyPostfix]
      [HarmonyAfter("eradev.craftopia.UnlockEnchants")]
      // ReSharper disable once InconsistentNaming
      public static void Postfix(ref Dictionary<int, float> __result)
      {
        try
        {
          Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

          Main.Instance.OnOcItemDataMng_CaleEnchantChance(ref __result);
        }
        catch (Exception e)
        {
          Log.Error(Main.Instance, e);
        }
      }
    }

    [HarmonyPatch(typeof(OcUI_CraftEnchantList), nameof(OcUI_CraftEnchantList.Init))]
    // ReSharper disable once InconsistentNaming
    public class PatchOcUI_CraftEnchantList_Init
    {
      [UsedImplicitly]
      [HarmonyPostfix]
      // ReSharper disable once InconsistentNaming
      public static void Postfix(ref OcUI_CraftEnchantList __instance)
      {
        try
        {
          Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
          Main.Instance.OnPatchOcUI_CraftEnchantList_Init(ref __instance);
        }
        catch (Exception e)
        {
          Log.Error(Main.Instance, e);
        }
      }
    }

    // [HarmonyPatch(typeof(ocUI), nameof(OcUI_CraftEnchantList.Init))]
    // // ReSharper disable once InconsistentNaming
    // public class PatchOcUI_CraftEnchantList_Init
    // {
    //   [UsedImplicitly]
    //   [HarmonyPostfix]
    //   // ReSharper disable once InconsistentNaming
    //   public static void Postfix(ref OcUI_CraftEnchantList __instance)
    //   {
    //     try
    //     {
    //       Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
    //       Main.Instance.OnPatchOcUI_CraftEnchantList_Init(ref __instance);
    //     }
    //     catch (Exception e)
    //     {
    //       Log.Error(Main.Instance, e);
    //     }
    //   }
    // }
  }
}

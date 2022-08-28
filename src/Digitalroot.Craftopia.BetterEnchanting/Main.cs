using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using Oc;
using Oc.Item;
using Oc.Item.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Digitalroot.Craftopia.BetterEnchanting
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInDependency("eradev.craftopia.UnlockEnchants", BepInDependency.DependencyFlags.SoftDependency)]
  [BepInDependency("Lock4Enchantments", BepInDependency.DependencyFlags.SoftDependency)]
  public partial class Main : BaseUnityPlugin, ITraceableLogging
  {
    private Harmony _harmony;

    [UsedImplicitly]
    public static ConfigEntry<int> NexusId;

    private static ConfigEntry<bool> _groupByRarity;
    private static ConfigEntry<RarityOptions> _commonRarityOption;
    private static ConfigEntry<RarityOptions> _uncommonRarityOption;
    private static ConfigEntry<RarityOptions> _rareRarityOption;
    private static ConfigEntry<RarityOptions> _epicRarityOption;
    private static ConfigEntry<RarityOptions> _legendaryRarityOption;

    public static Main Instance;

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion

    public Main()
    {
      try
      {
        #if DEBUG
        EnableTrace = true;
        #else
        EnableTrace = false;
        #endif
        Instance = this;
        NexusId = Config.Bind("General", "NexusID", 117, new ConfigDescription("Nexus mod ID for updates", null, new ConfigurationManagerAttributes { Browsable = false, ReadOnly = true }));
        _groupByRarity = Config.Bind("Broken Configs", "Group By Rarity", true, new ConfigDescription("Group enchants by rarity", new AcceptableValueList<bool>(true, false), new ConfigurationManagerAttributes { Browsable = true, ReadOnly = false, Order = 0}));
        _commonRarityOption = Config.Bind("Rarity Options", "Common", RarityOptions.Show, new ConfigDescription("Rarity display option", null, new ConfigurationManagerAttributes { Browsable = true, ReadOnly = false, Order = 1 }));
        _uncommonRarityOption = Config.Bind("Rarity Options", "Uncommon", RarityOptions.Show, new ConfigDescription("Rarity display option", null, new ConfigurationManagerAttributes { Browsable = true, ReadOnly = false, Order = 2 }));
        _rareRarityOption = Config.Bind("Rarity Options", "Rare", RarityOptions.Show, new ConfigDescription("Rarity display option", null, new ConfigurationManagerAttributes { Browsable = true, ReadOnly = false, Order = 3 }));
        _epicRarityOption = Config.Bind("Rarity Options", "Epic", RarityOptions.Show, new ConfigDescription("Rarity display option", null, new ConfigurationManagerAttributes { Browsable = true, ReadOnly = false, Order = 4 }));
        _legendaryRarityOption = Config.Bind("Rarity Options", "Legendary", RarityOptions.Show, new ConfigDescription("Rarity display option", null, new ConfigurationManagerAttributes { Browsable = true, ReadOnly = false, Order = 5 }));
        Log.RegisterSource(Instance);
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        _harmony?.UnpatchSelf();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private readonly List<KeyValuePair<int, EnchantRarity>> _soEnchantmentCache = new();
    private bool _soEnchantmentCacheInit;

    // ReSharper disable once IdentifierTypo
    public void OnOcItemDataMng_CaleEnchantChance(ref Dictionary<int, float> dictionary)
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");

        if (!_soEnchantmentCacheInit)
        {
          SeedCache();
        }

        HandleRarityOption(EnchantRarity.Legendary, _legendaryRarityOption.Value, ref dictionary);
        HandleRarityOption(EnchantRarity.Epic, _epicRarityOption.Value, ref dictionary);
        HandleRarityOption(EnchantRarity.Rare, _rareRarityOption.Value, ref dictionary);
        HandleRarityOption(EnchantRarity.Uncommon, _uncommonRarityOption.Value, ref dictionary);
        HandleRarityOption(EnchantRarity.Common, _commonRarityOption.Value, ref dictionary);

      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }
    

    private void HandleRarityOption(EnchantRarity enchantRarity, RarityOptions value, ref Dictionary<int, float> dictionary)
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({enchantRarity}, {value})");
        switch (value)
        {
          case RarityOptions.Hide:
            Hide(enchantRarity, dictionary);
            break;

          case RarityOptions.ShowAll:
            ShowAll(enchantRarity, dictionary);
            break;

          case RarityOptions.Show:
          default:
            break;
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void ShowAll(EnchantRarity enchantRarity, IDictionary<int, float> dictionary)
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({enchantRarity})");
        foreach (var key in _soEnchantmentCache.Where(x => x.Value == enchantRarity).Select(y => y.Key))
        {
          if (dictionary.ContainsKey(key)) continue;
          dictionary.Add(key, 0f);
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void Hide(EnchantRarity enchantRarity, IDictionary<int, float> dictionary)
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({enchantRarity})");
        foreach (var key in _soEnchantmentCache.Where(x => x.Value == enchantRarity).Select(y => y.Key))
        {
          if (dictionary.ContainsKey(key) && dictionary[key] == 0)
          {
            dictionary.Remove(key);
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void SeedCache()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        _soEnchantmentCacheInit = true;

        if (!_soEnchantmentCache.Empty())
        {
          _soEnchantmentCache.Clear();
        }

        foreach (var rarity in (EnchantRarity[])Enum.GetValues(typeof(EnchantRarity)))
        {
          _soEnchantmentCache.AddRange(OcResidentData.EnchantDataList.all
                                                     .Where(x => x.IsEnabled && x.Rarity == rarity)
                                                     .Select(s => new KeyValuePair<int, EnchantRarity>(s.ID, rarity)));
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    public void OnPatchOcUI_CraftEnchantList_Init(ref OcUI_CraftEnchantList ocUiCraftEnchantList)
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({_groupByRarity.Value})");
        if (!_groupByRarity.Value) return;

        DumpList(ref ocUiCraftEnchantList.itemEnchantList, "PreSort");

        // ocUiCraftEnchantList.itemEnchantList.Sort(new CraftEnchantSheetComparer(_soEnchantmentCache));
        var x = ocUiCraftEnchantList.itemEnchantList
                                    .OrderByDescending(en => en.Enchantment.Source.Rarity)
                                    .ThenBy(en => en.EnchantID)
                                    .ToList();
        DumpList(ref x, "TempSort");
        ocUiCraftEnchantList.itemEnchantList.Clear();
        DumpList(ref ocUiCraftEnchantList.itemEnchantList, "PostClear");
        ocUiCraftEnchantList.itemEnchantList.AddRange(x);

        DumpList(ref ocUiCraftEnchantList.itemEnchantList, "PostSort");
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [Conditional("DEBUG")]
    private void DumpList(ref List<OcUI_CraftEnchantSheet> itemEnchantList, string name)
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        Log.Trace(Instance, $"[{GetType().Namespace}] **********************[Start ({name})]**********************");
        if (itemEnchantList.IsNullOrEmpty() || itemEnchantList.Count < 20)
        {
          Log.Trace(Instance, $"[{GetType().Namespace}] **********************[ Skip ({name})]*********************");  
          return;
        }
        for (int i = 0; i < 20; i++)
        {
          Log.Trace(Instance, $"[{GetType().Namespace}] {i}, {itemEnchantList[i].EnchantID}, {itemEnchantList[i].Enchantment.Source.Rarity} ({itemEnchantList[i].Enchantment.Source.Rarity.ToInt()})");
        }
        Log.Trace(Instance, $"[{GetType().Namespace}] **********************[ End ({name})]**********************");
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }
  }
}

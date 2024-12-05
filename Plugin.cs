using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using BetterShotgun;
using HarmonyLib;
using UnityEngine;

namespace HexiShotgunTweaks
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(BetterShotgun.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(LETHAL_CONFIG, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "dopadream.lethalcompany.HexiShotgunTweaks", PLUGIN_NAME = "HexiShotgunTweaks", PLUGIN_VERSION = "1.0.0", LETHAL_CONFIG = "ainavt.lc.lethalconfig";
        internal static new ManualLogSource Logger;
        internal static ConfigEntry<bool> configShowTracers;
        internal static ConfigEntry<string> configTracerColor;

        internal void initLethalConfig()
        {
            LethalConfig.LethalConfigManager.AddConfigItem(new LethalConfig.ConfigItems.BoolCheckBoxConfigItem(configShowTracers, false));
            LethalConfig.LethalConfigManager.AddConfigItem(new LethalConfig.ConfigItems.TextInputFieldConfigItem(configTracerColor, false));

            LethalConfig.LethalConfigManager.SkipAutoGen();
        }

        void Awake()
        {
            Logger = base.Logger;

            configShowTracers = Config.Bind("General", "Show Bullet Tracers", true,
                new ConfigDescription("Show the bullet tracers from bullets fired from the shotgun. False would make it appear more like vanilla."));

            configTracerColor = Config.Bind("General", "Bullet Tracer Color", "#F7BE17",
                new ConfigDescription("Defines the color of the tracers fired by shotguns. Default is orange."));


            if (Chainloader.PluginInfos.ContainsKey(LETHAL_CONFIG))
            {
                initLethalConfig();
            }


            new Harmony(PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }

        [HarmonyPatch]

        class HexiShotgunTweaksPatches()
        {
            [HarmonyPatch(typeof(FadeOutLine), nameof(FadeOutLine.Update))]
            [HarmonyPrefix]
            public static void FadeOutLinePreFix(FadeOutLine __instance, ref Color ___col, ref LineRenderer ___line)
            {
                if (configShowTracers.Value)
                {
                    Color newCol;
                    if (ColorUtility.TryParseHtmlString(configTracerColor.Value, out newCol))
                    {
                        ___col = newCol;
                    } else
                    {
                        ___col = new(0.969f, 0.745f, 0.09f);
                    }
                } else
                {
                    ___line.forceRenderingOff = true;
                }
            }
        }
    }
}
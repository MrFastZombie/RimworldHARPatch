using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

using System.Reflection;
using HarmonyLib;

namespace HARCheckMaskShaderPatch
{
    public class HARCheckMaskShaderPatchSettings : ModSettings
    {
        public bool enabled = true;
        public int tickLifetime = 60;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled", true);
            Scribe_Values.Look(ref tickLifetime, "tickLifetime", 60);
            base.ExposeData();
        }
    }

    public class HARCheckMaskShaderPatchMod : Mod
    {
        string intBuffer;
        public static HARCheckMaskShaderPatchSettings settings;
        public HARCheckMaskShaderPatchMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<HARCheckMaskShaderPatchSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Enabled".Translate(), ref settings.enabled, tooltip: "EnableDesc".Translate());
            listingStandard.Label("Label".Translate());
            listingStandard.SubLabel("SubLabel".Translate(), 100f);
            listingStandard.IntEntry(ref settings.tickLifetime, ref intBuffer, min: 1);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ModName".Translate();
        }
    }

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Log.Message("Starting RimworldHARPatch!");

            Harmony harmony = new Harmony("RimworldHARPatch");
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }
    
    [HarmonyPatch(typeof(AlienRace.AlienRenderTreePatches), "CheckMaskShader", new Type[] {typeof(string), typeof(Shader), typeof(bool)})]
    public static class CheckMaskShader_Patch
    {
        private static Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();
        private static Dictionary<string, int> shaderTick = new Dictionary<string, int>();
        //private static int tickLimit = LoadedModManager.GetMod<HARCheckMaskShaderPatchMod>().GetSettings<HARCheckMaskShaderPatchSettings>().tickLifetime;

        public static bool Prefix(ref Shader __result, ref string texPath, ref Shader shader, ref bool pathCheckOverride)
        {
            int tick = GenTicks.TicksGame;
            int tickLimit = LoadedModManager.GetMod<HARCheckMaskShaderPatchMod>().GetSettings<HARCheckMaskShaderPatchSettings>().tickLifetime;
            bool enabled = LoadedModManager.GetMod<HARCheckMaskShaderPatchMod>().GetSettings<HARCheckMaskShaderPatchSettings>().enabled;
            
            if(enabled == false) return true;
            
            if(shaderCache.ContainsKey(texPath))
            {
                if(shaderTick.ContainsKey(texPath))
                {
                    //Log.Message("Tick limit is " + tickLimit);
                    if(tick - shaderTick[texPath] >= tickLimit)
                    {
                        shaderTick.Remove(texPath);
                        shaderCache.Remove(texPath);
                        return true;
                    }
                }
                __result = shaderCache[texPath];
                return false;
            }
            
            return true;
        }

        static void Postfix(ref Shader __result, ref string texPath)
        {
            bool enabled = LoadedModManager.GetMod<HARCheckMaskShaderPatchMod>().GetSettings<HARCheckMaskShaderPatchSettings>().enabled;
            
            if(enabled == false) return;
            
            if(shaderCache.ContainsKey(texPath) == false)
            {
                    //Log.Message("Adding shader to cache: " + texPath);
                    shaderCache.Add(texPath, __result);
                    shaderTick.Add(texPath, GenTicks.TicksGame);
            }
        }
    }
}

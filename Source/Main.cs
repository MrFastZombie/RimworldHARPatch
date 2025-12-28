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
    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Log.Message("Starting RimworldHARPatch!");

            // *Uncomment for Harmony*
            Harmony harmony = new Harmony("RimworldHARPatch");
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }
    
    [HarmonyPatch(typeof(AlienRace.AlienRenderTreePatches), "CheckMaskShader", new Type[] {typeof(string), typeof(Shader), typeof(bool)})]
    public static class CheckMaskShader_Patch
    {
        private static Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();
        private static Dictionary<string, int> shaderTick = new Dictionary<string, int>();

        public static bool Prefix(ref Shader __result, ref string texPath, ref Shader shader, ref bool pathCheckOverride)
        {
            int tick = GenTicks.TicksGame;
            if(shaderCache.ContainsKey(texPath))
            {
                if(shaderTick.ContainsKey(texPath))
                {
                    if(tick - shaderTick[texPath] >= 60)
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
            if(shaderCache.ContainsKey(texPath) == false)
            {
                    shaderCache.Add(texPath, __result);
                    shaderTick.Add(texPath, GenTicks.TicksGame);
            }
        }
    }
}

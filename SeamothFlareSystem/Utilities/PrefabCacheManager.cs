using System.Collections;
using UnityEngine;

namespace SeamothFlareSystem.Utilities
{
    public static class PrefabCacheManager
    {
        public static GameObject? Flare;

        public static IEnumerator Initialize()
        {
            var flarePrefabTask = CraftData.GetPrefabForTechTypeAsync(TechType.Flare, false);
            yield return flarePrefabTask;

            Flare = flarePrefabTask.GetResult();
            if (Flare == null)
            {
                Plugin.Log?.LogError("Failed to load Flare prefab.");
            }
        }
    }
}
using System.Collections;
using UnityEngine;

namespace SeamothFlareSystem.Utilities
{
    public static class CustomPrefabHandler
    {
        public static IEnumerator GetPrefabForTechTypeAsync(TechType techType, System.Action<GameObject> callback)
        {
            var prefabTask = CraftData.GetPrefabForTechTypeAsync(techType, false);
            yield return prefabTask;

            var prefab = prefabTask.GetResult();
            if (prefab == null)
            {
                Plugin.Log?.LogError($"Failed to get prefab for TechType: {techType}");
            }
            else
            {
                callback?.Invoke(prefab!);
            }
        }
    }
}
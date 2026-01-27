// using HarmonyLib;
// using SeamothFlareSystem.Items;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// namespace SeamothFlareSystem.Patches
// {
//     [HarmonyPatch(typeof(Vehicle), "OnUpgradeModuleChange")]
//     public static class SeamothFlareModulePatch
//     {
//         static void Postfix(Vehicle __instance, TechType techType, int slotID, bool added)
//         {
//             // Only act on Seamoth
//             if (__instance is not SeaMoth) return;

//             // Only act for our module
//             if (techType != SeamothFlareModule.TechType) return;

//             if (added)
//             {
//                 // Find the new bay (should be added by vanilla logic)
//                 var storage = __instance.GetComponentsInChildren<SeamothStorageContainer>(true)
//                     .FirstOrDefault(s => s.storageRoot == __instance && s.gameObject.name.Contains(slotID.ToString()));
//                 if (storage != null)
//                 {
//                     storage.storageLabel = "Flares";
//                     storage.container.SetAllowedTechTypes([TechType.Flare]);
//                     storage.width = 4;
//                     storage.height = 4;
//                     Plugin.Log?.LogDebug($"Customized Seamoth flare bay storage for slot {slotID}.");
//                 }
//                 else
//                 {
//                     Plugin.Log?.LogError($"Could not find SeamothStorageContainer for flare bay slot {slotID}.");
//                 }

//                 // Add LaunchFlareBehavior if not present
//                 if (!__instance.gameObject.TryGetComponent(out Mono.LaunchFlareBehavior mono))
//                     __instance.gameObject.AddComponent<Mono.LaunchFlareBehavior>();
//             }
//             else // removed
//             {
//                 // Remove LaunchFlareBehavior if no more flare modules
//                 bool hasFlare = false;
//                 List<string> moduleSlots = [];
//                 __instance.modules.GetSlots(EquipmentType.SeamothModule, moduleSlots);
//                 foreach (var slot in moduleSlots)
//                 {
//                     if (__instance.modules.GetTechTypeInSlot(slot) == SeamothFlareModule.TechType)
//                     {
//                         hasFlare = true;
//                         break;
//                     }
//                 }
//                 if (!hasFlare && __instance.gameObject.TryGetComponent(out Mono.LaunchFlareBehavior mono))
//                     Object.Destroy(mono);
//             }
//         }
//     }
// }

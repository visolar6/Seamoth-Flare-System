using System;
using System.Collections.Generic;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using SeamothFlareSystem.Mono;
using UnityEngine;

namespace SeamothFlareSystem.Items
{
    public class SeamothFlareModule
    {
        public static string ClassID = "SeamothFlareModule";

        public static TechType TechType { get; private set; } = 0;

        public PrefabInfo Info { get; private set; }

        public int Slot = 0;

        public string BayName => $"FlareBay_{Slot}";

        public string StorageName => $"FlareBayStorage_{Slot}";

        public SeamothFlareModule()
        {
            Sprite? sprite = null;
            if (Plugin.AssetsCache != null)
            {
                if (!Plugin.AssetsCache.TryGetAsset("seamothflaremodule", out sprite))
                {
                    if (!Plugin.AssetsCache.TryGetAsset("seamothflaresystem", out sprite))
                    {
                        Plugin.AssetsCache.TryGetAsset("seamothtorpedomodule", out sprite);
                    }
                }
            }

            Info = PrefabInfo.WithTechType(classId: ClassID, displayName: null, description: null, techTypeOwner: Assembly.GetExecutingAssembly())
                .WithSizeInInventory(new(1, 1))
                .WithIcon(sprite);

            TechType = Info.TechType;
        }

        public void Patch()
        {
            List<Ingredient> recipeIngredients =
            [
                new(TechType.Titanium, 2),
                new(TechType.WiringKit, 1),
                new(TechType.Silicone, 1),
            ];
            if (Plugin.Options.RequireCrashfish)
            {
                recipeIngredients.Add(new(TechType.Crash, 1));
            }
            else
            {
                recipeIngredients.Add(new(TechType.CrashPowder, 1));
            }

            RecipeData recipe = new()
            {
                craftAmount = 1,
                Ingredients = recipeIngredients
            };

            CustomPrefab customPrefab = new(Info);

            CloneTemplate cloneTemplate = new(Info, TechType.SeamothTorpedoModule);

            customPrefab.SetGameObject(cloneTemplate);

            var scanningGadget = customPrefab
                .SetUnlock(TechType.Titanium)
                .WithPdaGroupCategoryAfter(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, TechType.SeamothTorpedoModule);

            customPrefab.SetVehicleUpgradeModule(EquipmentType.SeamothModule, QuickSlotType.Selectable)
                .WithEnergyCost(Plugin.Options.LaunchEnergyCost)
                .WithCooldown(Plugin.Options.LaunchCooldownSeconds)
                .WithOnModuleAdded(OnModuleAdded)
                .WithOnModuleRemoved(OnModuleRemoved)
                .WithOnModuleUsed(OnModuleUsed);

            customPrefab.SetRecipe(recipe)
                .WithCraftingTime(2.5f)
                .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
                .WithStepsToFabricatorTab(["SeamothModules"]);

            customPrefab.Register();
        }

        private void OnModuleAdded(Vehicle vehicle, int slotID)
        {
            Slot = slotID;
            if (!vehicle.gameObject.TryGetComponent(out LaunchFlareBehavior mono))
                vehicle.gameObject.EnsureComponent<LaunchFlareBehavior>();

            // Add visuals
            if (Plugin.AssetsCache != null)
            {
                var visuals = vehicle.gameObject.GetComponent<SeamothFlareModuleVisuals>();
                if (visuals == null)
                {
                    visuals = vehicle.gameObject.AddComponent<SeamothFlareModuleVisuals>();
                }
                visuals.AddVisual(vehicle.gameObject, slotID, Plugin.AssetsCache);
            }
            else
            {
                Plugin.Log?.LogError("Assets cache is null; cannot add Seamoth flare module visuals.");
            }

            // ErrorMessage.AddMessage(Language.main.Get("SeamothFlareModule_ModuleAddedWarning"));
        }

        private void OnModuleRemoved(Vehicle vehicle, int slotID)
        {
            if (vehicle.gameObject.TryGetComponent(out LaunchFlareBehavior mono))
                UnityEngine.Object.Destroy(mono);

            // Remove visuals
            var visuals = vehicle.gameObject.GetComponent<SeamothFlareModuleVisuals>();
            if (visuals != null)
            {
                visuals.RemoveVisual(vehicle.gameObject, slotID);
                UnityEngine.Object.Destroy(visuals);
            }
            else
            {
                Plugin.Log?.LogWarning("No Seamoth flare module visuals component found to remove.");
            }

            // ErrorMessage.AddMessage(Language.main.Get("SeamothFlareModule_ModuleRemovedWarning"));
        }

        private void OnModuleUsed(Vehicle vehicle, int slotID, float charge, float chargeScalar)
        {
            if (!vehicle.gameObject.TryGetComponent(out LaunchFlareBehavior mono))
                mono = vehicle.gameObject.EnsureComponent<LaunchFlareBehavior>();

            try
            {
                mono.LaunchFlare(vehicle, slotID);
            }
            catch (Exception e)
            {
                Plugin.Log?.LogError($"Error while launching flare: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
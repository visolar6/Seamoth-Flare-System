using System;
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

        public const float energyCost = 1f;

        public const float cooldown = 3f;

        public PrefabInfo Info { get; private set; }

        public SeamothFlareModule()
        {
            Sprite? icon = null;

            Info = PrefabInfo.WithTechType(classId: ClassID, displayName: null, description: null, techTypeOwner: Assembly.GetExecutingAssembly())
                .WithSizeInInventory(new(1, 1))
                .WithIcon(icon);

            TechType = Info.TechType;
        }

        public void Patch()
        {
            RecipeData recipe = new()
            {
                craftAmount = 1,
                Ingredients = [
                    new(TechType.TitaniumIngot, 1),
                    new(TechType.WiringKit, 1),
                    new(TechType.Silicone, 2),
                    new(TechType.Lubricant, 1)
                ]
            };

            CustomPrefab customPrefab = new(Info);

            CloneTemplate cloneTemplate = new(Info, TechType.SeamothTorpedoModule);

            customPrefab.SetGameObject(cloneTemplate);

            var scanningGadget = customPrefab
                .SetUnlock(TechType.CrashPowder)
                .WithPdaGroupCategoryAfter(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, TechType.SeamothTorpedoModule);

            customPrefab.SetVehicleUpgradeModule(EquipmentType.SeamothModule, QuickSlotType.Selectable)
            .WithEnergyCost(energyCost)
            .WithCooldown(cooldown)
            .WithOnModuleAdded((vehicle, slotID) =>
            {
                if (!vehicle.gameObject.TryGetComponent(out LaunchFlareBehavior mono))
                    vehicle.gameObject.EnsureComponent<LaunchFlareBehavior>();

                ErrorMessage.AddMessage($"Seamoth flare system installed");
            })
            .WithOnModuleRemoved((vehicle, slotID) =>
            {
                if (vehicle.gameObject.TryGetComponent(out LaunchFlareBehavior mono))
                    UnityEngine.Object.Destroy(mono);

                ErrorMessage.AddMessage($"Seamoth flare system removed");
            })
            .WithOnModuleUsed((vehicle, slotID, _, _) =>
            {
                if (!vehicle.gameObject.TryGetComponent(out LaunchFlareBehavior mono))
                    mono = vehicle.gameObject.EnsureComponent<LaunchFlareBehavior>();

                Plugin.Log?.LogInfo("Launching flare!");
                try
                {
                    mono.LaunchFlare(vehicle);
                }
                catch (Exception e)
                {
                    Plugin.Log?.LogError($"Error while launching flare: {e.Message}\n{e.StackTrace}");
                }
            });

            customPrefab.SetRecipe(recipe)
                .WithCraftingTime(2.5f)
                .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
                .WithStepsToFabricatorTab(["SeamothModules"]);

            customPrefab.Register();
        }
    }
}
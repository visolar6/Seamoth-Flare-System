using UnityEngine;
using SeamothFlareSystem.Utilities;
using SeamothFlareSystem.Items;

namespace SeamothFlareSystem.Mono
{
    /// <summary>
    /// Handles the visuals for the Seamoth flare module.
    /// <br/><br/>
    /// Slots - 0: Lower left, 1: Lower right, 2: Upper left, 3: Upper right
    /// </summary>
    public class SeamothFlareModuleVisuals : MonoBehaviour
    {
        private static readonly string[] MeshNames =
        {
            "Submersible_seaMoth_torpedo_hatch_L2_geo_0", // Lower left
            "Submersible_seaMoth_torpedo_hatch_R2_geo_0", // Lower right
            "Submersible_seaMoth_torpedo_hatch_L1_geo_0", // Upper left
            "Submersible_seaMoth_torpedo_hatch_R1_geo_0"  // Upper right
        };

        private static readonly Vector3[] SlotPositions =
        {
            new(-0.61f, -0.5f, 2.22f), // Lower left
            new(0.61f, -0.5f, 2.22f),  // Lower right
            new(-0.61f, -0.21f, 1.56f), // Upper left
            new(0.61f, -0.21f, 1.56f)   // Upper right
        };

        private static readonly Quaternion[] SlotRotations =
        {
            Quaternion.Euler(-10f, 0f, 0f),    // Lower left
            Quaternion.Euler(-10f, 0f, 0f),    // Lower right
            Quaternion.Euler(-20f, 0f, 0f),    // Upper left
            Quaternion.Euler(-20f, 0f, 0f)     // Upper right
        };

        public void AddVisual(GameObject parent, int slotID, ResourceCacheManager cache)
        {
            // Remove any existing visual for this slot first
            RemoveVisual(parent, slotID);

            Mesh? mesh = null;
            if (slotID >= 0 && slotID < MeshNames.Length)
            {
                cache.TryGetAsset(MeshNames[slotID], out mesh);
            }
            if (mesh == null)
            {
                Plugin.Log?.LogError($"Could not find mesh for slot {slotID}.");
                return;
            }

            var meshObj = new GameObject($"FlareModuleMesh_{slotID}");
            meshObj.transform.SetParent(parent.transform, false);

            var meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var meshRenderer = meshObj.AddComponent<MeshRenderer>();
            // Apply the single texture to the mesh
            cache.TryGetAsset("seamoth_torpedo_01_hatch_01", out Texture2D? texture);
            if (texture != null)
            {
                var shader = Shader.Find("MarmosetUBER") ?? Shader.Find("Standard");
                var mat = new Material(shader)
                {
                    mainTexture = texture
                };
                meshRenderer.material = mat;
            }
            else
            {
                Plugin.Log?.LogError("Could not find texture 'seamoth_torpedo_01_hatch_01' for flare module mesh.");
            }

            if (slotID >= 0 && slotID < SlotPositions.Length)
            {
                meshObj.transform.localPosition = SlotPositions[slotID];
                meshObj.transform.localRotation = SlotRotations[slotID];
            }
            else // Fallback to zero position
            {
                meshObj.transform.localPosition = Vector3.zero;
                meshObj.transform.localRotation = Quaternion.identity;
            }

            // Sync visuals with installed modules
            var vehicle = parent.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                SyncVisualsWithModules(vehicle);
            }
            else
            {
                Plugin.Log?.LogWarning("Could not find Vehicle component on parent when adding Seamoth flare module visual.");
            }
        }

        public void RemoveVisual(GameObject parent, int slotID)
        {
            var meshObj = parent.transform.Find($"FlareModuleMesh_{slotID}");
            if (meshObj != null)
            {
                Destroy(meshObj.gameObject);
            }
            else
            {
                Plugin.Log?.LogWarning($"No Seamoth flare module visual found to remove for slot {slotID}.");
            }

            // Sync visuals with installed modules
            var vehicle = parent.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                SyncVisualsWithModules(vehicle);
            }
            else
            {
                Plugin.Log?.LogWarning("Could not find Vehicle component on parent when removing Seamoth flare module visual.");
            }
        }

        public static void SyncVisualsWithModules(Vehicle vehicle)
        {
            for (int slot = 0; slot < 4; slot++)
            {
                Plugin.Log?.LogInfo($"[FlareModuleVisuals] Syncing visuals for slot {slot}.");
                TechType tt = vehicle.GetSlotBinding(slot);
                var meshObj = vehicle.gameObject.transform.Find($"FlareModuleMesh_{slot}");
                if (tt != SeamothFlareModule.TechType && meshObj != null)
                {
                    Destroy(meshObj.gameObject);
                }
            }
        }
    }
}

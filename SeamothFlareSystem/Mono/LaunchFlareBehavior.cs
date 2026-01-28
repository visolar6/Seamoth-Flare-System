using SeamothFlareSystem.Utilities;
using UnityEngine;

namespace SeamothFlareSystem.Mono;

internal class LaunchFlareBehavior : MonoBehaviour
{
    public void LaunchFlare(Vehicle vehicle, int slotID)
    {
        if (PrefabCacheManager.Flare != null)
        {
            // Find the correct mesh visual for the slot and use its position as the spawn point
            Transform flareSpawn = vehicle.transform.Find($"FlareModuleMesh_{slotID}");

            Vector3 spawnPos;
            if (flareSpawn != null)
            {
                // Subtract a small value from Y to align with the mesh
                spawnPos = flareSpawn.position + (vehicle.transform.up * -0.3f);
            }
            else
            {
                // Fallback to default position
                spawnPos = vehicle.transform.position + vehicle.transform.forward * 2f + vehicle.transform.up * 0.5f;
            }

            Quaternion spawnRot;
            if (flareSpawn != null)
            {
                spawnRot = flareSpawn.rotation;
            }
            else
            {
                spawnRot = Quaternion.LookRotation(vehicle.transform.forward, Vector3.up);
            }

            var flare = Instantiate(PrefabCacheManager.Flare);
            flare.transform.position = spawnPos;
            flare.transform.rotation = spawnRot;

            var flareComp = flare.GetComponent<Flare>();
            if (flareComp != null)
            {
                var flareType = typeof(Flare);
                // Set hasBeenThrown = true
                var hasBeenThrownField = flareType.GetField("hasBeenThrown", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                hasBeenThrownField?.SetValue(flareComp, true);

                // Set isThrowing = true (so OnDrop logic applies throw force)
                var isThrowingField = flareType.GetField("isThrowing", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                isThrowingField?.SetValue(flareComp, true);

                // Set up Rigidbody and apply force/torque as in OnDrop
                var rb = flare.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    var forwardForce = vehicle.transform.forward * Plugin.Options.LaunchForceAmount;
                    var forwardVelocity = vehicle.GetComponent<Rigidbody>().velocity + forwardForce;
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rb.velocity = forwardVelocity;
                    rb.AddForce(forwardForce);
                    rb.AddTorque(flare.transform.right * 5f); // dropTorqueAmount from vanilla
                }

                // Call SetFlareActiveState(true)
                var setFlareActiveState = flareType.GetMethod("SetFlareActiveState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (setFlareActiveState != null)
                {
                    setFlareActiveState.Invoke(flareComp, [true]);
                }
                else
                {
                    Plugin.Log?.LogError("Could not find SetFlareActiveState method on Flare component via reflection.");
                }

                // Play VFX if not already playing (fxControl.Play(1))
                var fxControlField = flareType.GetField("fxControl", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var fxIsPlayingField = flareType.GetField("fxIsPlaying", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var fxControl = fxControlField?.GetValue(flareComp);
                var fxIsPlaying = fxIsPlayingField?.GetValue(flareComp) as bool?;
                if (fxControl != null && fxIsPlayingField != null && (fxIsPlaying == null || fxIsPlaying == false))
                {
                    var playMethod = fxControl.GetType().GetMethod("Play", new[] { typeof(int) });
                    playMethod?.Invoke(fxControl, [1]);
                    fxIsPlayingField.SetValue(flareComp, true);
                }

                // Set isThrowing = false (as OnDrop does at the end)
                isThrowingField?.SetValue(flareComp, false);

                // Increase illumination of the flare
                var flareLight = flare.GetComponentInChildren<Light>();
                if (flareLight != null)
                {
                    flareLight.intensity = Plugin.Options.FlareLightIntensity;
                    flareLight.range = Plugin.Options.FlareLightRange;
                    // Optionally adjust color
                    // flareLight.color = Color.white;
                }
            }
        }
        else
        {
            Plugin.Log?.LogError("PrefabCache.Flare is null, cannot launch flare.");
        }
    }
}
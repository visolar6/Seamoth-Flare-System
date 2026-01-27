using System.Collections;
using SeamothFlareSystem.Utilities;
using UnityEngine;

namespace SeamothFlareSystem.Mono;

internal class LaunchFlareBehavior : MonoBehaviour
{
    public void LaunchFlare(Vehicle vehicle)
    {
        StartCoroutine(CustomPrefabHandler.GetPrefabForTechTypeAsync(TechType.Flare, flarePrefab =>
        {
            if (flarePrefab == null)
                return;

            float launchForceAmount = SeamothFlareSystem.Plugin.Options.LaunchForceAmount;

            var flare = Instantiate(flarePrefab);
            flare.transform.position = vehicle.transform.position + vehicle.transform.forward * 2f + vehicle.transform.up * 0.5f;
            flare.transform.rotation = Quaternion.LookRotation(vehicle.transform.forward, Vector3.up);

            var flareComp = flare.GetComponent<Flare>();
            if (flareComp != null)
            {
                var flareType = typeof(Flare);
                // Set hasBeenThrown = true
                var hasBeenThrownField = flareType.GetField("hasBeenThrown", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                if (hasBeenThrownField != null)
                    hasBeenThrownField.SetValue(flareComp, true);

                // Set isThrowing = true (so OnDrop logic applies throw force)
                var isThrowingField = flareType.GetField("isThrowing", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (isThrowingField != null)
                    isThrowingField.SetValue(flareComp, true);

                // Set up Rigidbody and apply force/torque as in OnDrop
                var rb = flare.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rb.velocity = vehicle.GetComponent<Rigidbody>().velocity + vehicle.transform.forward * launchForceAmount;
                    rb.AddForce(vehicle.transform.forward * launchForceAmount);
                    rb.AddTorque(flare.transform.right * 5f); // dropTorqueAmount from vanilla
                }

                // Call SetFlareActiveState(true)
                var setFlareActiveState = flareType.GetMethod("SetFlareActiveState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (setFlareActiveState != null)
                {
                    setFlareActiveState.Invoke(flareComp, new object[] { true });
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
                    playMethod?.Invoke(fxControl, new object[] { 1 });
                    fxIsPlayingField.SetValue(flareComp, true);
                }

                // Set isThrowing = false (as OnDrop does at the end)
                if (isThrowingField != null)
                    isThrowingField.SetValue(flareComp, false);
            }

            Plugin.Log?.LogInfo("Flare launched!");
        }));
    }
}
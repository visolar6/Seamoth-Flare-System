using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SeamothFlareSystem.Utilities
{

    public class ResourceCacheManager
    {
        public static UnityEngine.Object[]? RawResources;

        public Dictionary<string, AudioClip> CachedAudioClips { get; private set; } = [];
        public Dictionary<string, GameObject> CachedPrefabs { get; private set; } = [];
        public Dictionary<string, Material> CachedMaterials { get; private set; } = [];
        public Dictionary<string, Mesh> CachedMeshes { get; private set; } = [];
        public Dictionary<string, Sprite> CachedSprites { get; private set; } = [];
        public Dictionary<string, Texture2D> CachedTextures { get; private set; } = [];
        public Dictionary<Type, Dictionary<string, UnityEngine.Object>> CachedResources { get; private set; } = [];
        public ResourceCacheManager() { }

        public static ResourceCacheManager LoadResources(string path)
        {
            var bundle = AssetBundle.LoadFromFile(path) ?? throw new IOException($"Provided path '{path}' does not contain any assetbundle.");
            RawResources = bundle.LoadAllAssets();
            var rm = new ResourceCacheManager();

            foreach (UnityEngine.Object asset in RawResources)
            {
                var assetName = asset.name.Split('.').Last();
                if (!rm.CachedResources.ContainsKey(asset.GetType()))
                {
                    rm.CachedResources.Add(asset.GetType(), new() { { assetName, asset } });
                    Plugin.Log?.LogDebug($"Cached {assetName} ({asset.name}) in dictionary of type {asset.GetType().Name}.");
                    continue;
                }

                if (rm.CachedResources.TryGetValue(asset.GetType(), out var ResourcesOfType) && ResourcesOfType.ContainsKey(assetName))
                {
                    Plugin.Log?.LogError($"An asset of type {asset.GetType().Name} and name {assetName} already exists. Aborting this one.");
                    continue;
                }

                ResourcesOfType.Add(assetName, asset);
                Plugin.Log?.LogDebug($"Cached {assetName} ({asset.name}) in dictionary of type {asset.GetType().Name}.");
            }

            return rm;
        }

        /// <summary>
        /// Gets a resource in the AssetsCache cache.
        /// <para>You better use <see cref="TryGetAsset{T}(string, out T)"/>.</para>
        /// </summary>
        /// <typeparam name="T">Type of the item to find.</typeparam>
        /// <param name="name">Name of the asset to find.</param>
        /// <returns>A reference to the instance of the asset, or an error if nothing have been found.</returns>
        /// <exception cref="ArgumentException">If the provided type is not a valid UnityEngine.Object or is not supported.</exception>
        public T? GetAsset<T>(string name) where T : UnityEngine.Object
        {
            Plugin.Log?.LogDebug($"Getting the asset {typeof(T).Name}.{name}...");

            if (!CachedResources.TryGetValue(typeof(T), out var AssetsDict))
                throw new ArgumentException($"There is no dictionary for type '{typeof(T).Name}' does not exist in cached resources.");

            if (!AssetsDict.TryGetValue(name, out var asset))
                throw new ArgumentException($"There is no object in '{typeof(T).Name}' dictionary that matches name '{name}'.");

            if (asset is not T obj)
                throw new Exception($"For some reason, the asset '{asset.GetType().Name}.{name}' type is not matching the type {typeof(T).Name}");

            return asset as T;
        }

        /// <summary>
        /// Try to get an asset from the resources cache.
        /// </summary>
        /// <typeparam name="T">Type of the resource to find.</typeparam>
        /// <param name="name">Name of the resource to find.</param>
        /// <param name="result">Asset found. Null if not found.</param>
        /// <returns>True if the item have been found, otherwise false.</returns>
        /// <exception cref="ArgumentException">If the provided type is not a valid UnityEngine.Object or not supported.</exception>
        [Obsolete("Use <see cref=\"TryGetAsset{T}(string, out T)t\"/>.")]
        public bool TryGetAssetLegacy<T>(string name, out T result) where T : UnityEngine.Object
        {
            bool res;
            switch (true)
            {
                case true when typeof(T) == typeof(AudioClip):
                    res = CachedAudioClips.TryGetValue(name, out var audio);
                    result = (audio as T)!;
                    return res;
                case true when typeof(T) == typeof(GameObject):
                    res = CachedPrefabs.TryGetValue(name, out var gameObject);
                    result = (gameObject as T)!;
                    return res;
                case true when typeof(T) == typeof(Material):
                    res = CachedMaterials.TryGetValue(name, out var material);
                    result = (material as T)!;
                    return res;
                case true when typeof(T) == typeof(Mesh):
                    res = CachedMeshes.TryGetValue(name, out var mesh);
                    result = (mesh as T)!;
                    return res;
                case true when typeof(T) == typeof(Sprite):
                    res = CachedSprites.TryGetValue(name, out var sprite);
                    result = (sprite as T)!;
                    return res;
                case true when typeof(T) == typeof(Texture2D):
                    res = CachedTextures.TryGetValue(name, out var texture);
                    result = (texture as T)!;
                    return res;
                default:
                    throw new ArgumentException("Type of T is not a valid UnityEngine.Object, or is not supported.");
            }
        }

        /// <summary>
        /// Try to get an asset from the resources cache.
        /// </summary>
        /// <typeparam name="T">Type of the resource to find.</typeparam>
        /// <param name="name">Name of the resource to find.</param>
        /// <param name="result">Asset found. Null if not found.</param>
        /// <returns>True if the item have been found, otherwise false.</returns>
        /// <exception cref="ArgumentException">If the provided type is not a valid UnityEngine.Object or not supported.</exception>
        public bool TryGetAsset<T>(string name, out T? result) where T : UnityEngine.Object
        {
            result = null;
            if (!CachedResources.TryGetValue(typeof(T), out var AssetsDict))
            {
                Plugin.Log?.LogError($"There is no dictionary for type '{typeof(T).Name}' does not exist in cached resources.");
                return false;
            }

            if (!AssetsDict.TryGetValue(name, out var asset))
            {
                Plugin.Log?.LogError($"There is no object in '{typeof(T).Name}' dictionary that matches name '{name}'.");
                return false;
            }

            if (asset is not T obj)
            {
                Plugin.Log?.LogError($"For some reason, the asset '{asset.GetType().Name}.{name}' type is not matching the type {typeof(T).Name}");
                return false;
            }

            result = asset as T;
            return true;
        }
    }
}
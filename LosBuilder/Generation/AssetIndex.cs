using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LosBuilder.Generation
{
    internal sealed class AssetIndex
    {
        private readonly Dictionary<string, string> _lookup = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);

        public static AssetIndex Build(string assetDbRoot, System.Action<string> info)
        {
            var index = new AssetIndex();
            int count = 0;

            foreach (string filePath in Directory.EnumerateFiles(assetDbRoot, "*.nif", SearchOption.AllDirectories))
            {
                count++;
                index.AddPath(filePath);
            }

            info("Indexed " + count + " NIF assets from " + assetDbRoot);
            return index;
        }

        public bool TryResolve(string sourceName, string collisionName, out string resolvedPath)
        {
            resolvedPath = null;

            foreach (string candidate in BuildCandidates(collisionName))
            {
                if (TryResolveInternal(candidate, out resolvedPath))
                    return true;
            }

            foreach (string candidate in BuildCandidates(sourceName))
            {
                if (TryResolveInternal(candidate, out resolvedPath))
                    return true;
            }

            return false;
        }

        public static string NormalizeLookupKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim().Replace('\\', '/').ToLowerInvariant();
        }

        private void AddPath(string fullPath)
        {
            foreach (string key in BuildCandidates(fullPath))
            {
                if (key == null || _lookup.ContainsKey(key))
                    continue;

                _lookup[key] = fullPath;
            }
        }

        private bool TryResolveInternal(string candidate, out string resolvedPath)
        {
            resolvedPath = null;
            if (string.IsNullOrWhiteSpace(candidate))
                return false;

            string normalized = NormalizeLookupKey(candidate);
            return normalized != null && _lookup.TryGetValue(normalized, out resolvedPath);
        }

        private static IEnumerable<string> BuildCandidates(string value)
        {
            string normalized = NormalizeLookupKey(value);
            if (normalized == null)
                yield break;

            yield return normalized;

            string fileName = Path.GetFileName(normalized);
            if (!string.IsNullOrWhiteSpace(fileName) && !fileName.Equals(normalized, System.StringComparison.OrdinalIgnoreCase))
                yield return fileName;

            string stripped = StripAssetPrefix(fileName);
            if (!string.IsNullOrWhiteSpace(stripped) && !stripped.Equals(fileName, System.StringComparison.OrdinalIgnoreCase))
                yield return stripped;

            string collisionVariant = InsertCollisionSuffix(fileName);
            if (collisionVariant != null)
            {
                yield return collisionVariant;

                string strippedCollision = StripAssetPrefix(collisionVariant);
                if (!string.IsNullOrWhiteSpace(strippedCollision) && !strippedCollision.Equals(collisionVariant, System.StringComparison.OrdinalIgnoreCase))
                    yield return strippedCollision;
            }
        }

        private static string InsertCollisionSuffix(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !fileName.EndsWith(".nif", System.StringComparison.OrdinalIgnoreCase) || fileName.IndexOf("_collision", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return null;

            return Path.GetFileNameWithoutExtension(fileName) + "_collision.nif";
        }

        private static string StripAssetPrefix(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return fileName;

            return Regex.Replace(fileName, @"^[a-z]{2}\.\d+\.\d+\.", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}

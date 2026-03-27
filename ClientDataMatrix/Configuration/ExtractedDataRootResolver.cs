using System;
using System.Collections.Generic;
using System.IO;

namespace ClientDataMatrix.Configuration
{
    public static class ExtractedDataRootResolver
    {
        private const string DefaultExtractedRoot = @"C:\Users\Admin\Pictures\WAR_extracted";

        public static string Resolve(string explicitPath)
        {
            List<string> candidates = new List<string>();
            if (!string.IsNullOrWhiteSpace(explicitPath))
                candidates.Add(explicitPath);

            candidates.Add(DefaultExtractedRoot);
            candidates.Add(@"C:\Users\Admin\Downloads\myps");
            candidates.Add(Path.Combine("data", "WAR_extracted"));
            candidates.Add(Path.Combine("..", "WAR_extracted"));

            foreach (string candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                    continue;

                string fullPath = Path.GetFullPath(candidate);
                if (Directory.Exists(fullPath))
                    return fullPath;
            }

            throw new DirectoryNotFoundException("Unable to locate the extracted WAR client root. Pass --root explicitly or place the files at " + DefaultExtractedRoot + ".");
        }
    }
}

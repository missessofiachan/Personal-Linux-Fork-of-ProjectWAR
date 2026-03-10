using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClientDataMatrix.Services
{
    public sealed class WorkspaceCleanupReport
    {
        public WorkspaceCleanupReport(string workspaceRoot, List<string> removedPaths, List<string> failedPaths)
        {
            WorkspaceRoot = workspaceRoot;
            RemovedPaths = removedPaths ?? new List<string>();
            FailedPaths = failedPaths ?? new List<string>();
        }

        public string WorkspaceRoot { get; private set; }
        public List<string> RemovedPaths { get; private set; }
        public List<string> FailedPaths { get; private set; }
        public bool HasFailures { get { return FailedPaths.Count > 0; } }

        public string BuildSummary()
        {
            if (RemovedPaths.Count == 0 && FailedPaths.Count == 0)
                return "Workspace cleanup found no ClientDataMatrix temp artifacts under " + WorkspaceRoot + ".";

            string summary = "Workspace cleanup removed " + RemovedPaths.Count + " item(s) under " + WorkspaceRoot + ".";
            if (FailedPaths.Count > 0)
                summary += " Failed to remove " + FailedPaths.Count + " item(s).";

            return summary;
        }
    }

    public static class WorkspaceCleanupService
    {
        private static readonly string[] WorkspaceTempFiles =
        {
            "clientdatamatrix.err.txt",
            "clientdatamatrix.out.txt"
        };

        private static readonly string[] ProjectLogPaths =
        {
            Path.Combine("ClientDataMatrix", "build.log"),
            Path.Combine("ClientDataMatrix", "msbuild.log"),
            Path.Combine("ClientDataMatrix", "msbuild-validate.log")
        };

        public static WorkspaceCleanupReport CleanWorkspace(string startPath)
        {
            string workspaceRoot = ResolveWorkspaceRoot(startPath);
            List<string> removedPaths = new List<string>();
            List<string> failedPaths = new List<string>();

            foreach (string tempDirectory in Directory.GetDirectories(workspaceRoot, "tmp-data-matrix*").OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
                DeleteDirectory(tempDirectory, removedPaths, failedPaths);

            foreach (string tempFileName in WorkspaceTempFiles)
                DeleteFile(Path.Combine(workspaceRoot, tempFileName), removedPaths, failedPaths);

            foreach (string relativeProjectLogPath in ProjectLogPaths)
                DeleteFile(Path.Combine(workspaceRoot, relativeProjectLogPath), removedPaths, failedPaths);

            return new WorkspaceCleanupReport(workspaceRoot, removedPaths, failedPaths);
        }

        private static string ResolveWorkspaceRoot(string startPath)
        {
            string currentDirectory = string.IsNullOrWhiteSpace(startPath) ? Environment.CurrentDirectory : Path.GetFullPath(startPath);
            string workspaceRoot = FindWorkspaceRoot(currentDirectory);
            if (!string.IsNullOrWhiteSpace(workspaceRoot))
                return workspaceRoot;

            workspaceRoot = FindWorkspaceRoot(AppDomain.CurrentDomain.BaseDirectory);
            if (!string.IsNullOrWhiteSpace(workspaceRoot))
                return workspaceRoot;

            return currentDirectory;
        }

        private static string FindWorkspaceRoot(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(Path.GetFullPath(path));
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "ProjectWAR.sln"))
                    || Directory.Exists(Path.Combine(directory.FullName, ".git")))
                    return directory.FullName;

                directory = directory.Parent;
            }

            return null;
        }

        private static void DeleteDirectory(string path, List<string> removedPaths, List<string> failedPaths)
        {
            try
            {
                if (!Directory.Exists(path))
                    return;

                Directory.Delete(path, true);
                removedPaths.Add(path);
            }
            catch (Exception ex)
            {
                failedPaths.Add(path + " (" + ex.Message + ")");
            }
        }

        private static void DeleteFile(string path, List<string> removedPaths, List<string> failedPaths)
        {
            try
            {
                if (!File.Exists(path))
                    return;

                File.Delete(path);
                removedPaths.Add(path);
            }
            catch (Exception ex)
            {
                failedPaths.Add(path + " (" + ex.Message + ")");
            }
        }
    }
}

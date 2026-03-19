using FrameWork;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    /// <summary>
    /// Coordinates patch manifest retrieval and file downloads for the launcher content update flow.
    /// </summary>
    public class Patcher
    {
        private readonly string _address;
        private readonly Logger _logger;
        private readonly List<FileManifestItem> _neededAssets = new List<FileManifestItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Patcher"/> class.
        /// </summary>
        /// <param name="logger">The logger used for patch diagnostics.</param>
        /// <param name="address">The patch server address.</param>
        public Patcher(Logger logger, string address)
        {
            _address = address;
            _logger = logger;
        }

        /// <summary>
        /// Gets the total size of all files scheduled for download.
        /// </summary>
        public long TotalDownloadSize { get; private set; }

        /// <summary>
        /// Gets the number of bytes downloaded so far.
        /// </summary>
        public long Downloaded { get; private set; }

        /// <summary>
        /// Gets the file currently being downloaded.
        /// </summary>
        public string CurrentFile { get; private set; }

        /// <summary>
        /// Gets the patcher's current processing state.
        /// </summary>
        public State CurrentState { get; private set; } = State.RequestManifest;

        /// <summary>
        /// Downloads the patch manifest, compares local files, and fetches any outdated assets.
        /// </summary>
        /// <param name="patchDirectory">The relative directory where patched files should be written.</param>
        public async Task PatchAsync(string patchDirectory)
        {
            string temporaryFilePath = string.Empty;

            try
            {
                TotalDownloadSize = 0;
                Downloaded = 0;
                CurrentFile = string.Empty;

                lock (_neededAssets)
                    _neededAssets.Clear();

                CurrentState = State.RequestManifest;
                _logger.Info($"Requesting files to update from {_address}");

                FileManifest manifest = await HttpUtil.RequestAsync<FileManifest>(_address, "REQUEST_FILE_MANIFEST");
                CurrentState = State.ProcessManifest;

                if (manifest?.Files == null)
                {
                    _logger.Info("Invalid file update list");
                    CurrentState = State.Error;
                    return;
                }

                _logger.Info($"Processing update files. {manifest.Files.Count} files");
                BuildDownloadQueue(manifest.Files, patchDirectory);

                TotalDownloadSize = _neededAssets.Sum(asset => asset.Size);
                if (TotalDownloadSize > 0)
                    _logger.Info($"Total requested download size:{TotalDownloadSize} (Files:{_neededAssets.Count})");

                foreach (FileManifestItem file in _neededAssets.ToList())
                {
                    CurrentState = State.Downloading;
                    string destinationPath = Path.Combine(Application.StartupPath + patchDirectory, file.Name);

                    if (File.Exists(destinationPath))
                        File.Delete(destinationPath);

                    temporaryFilePath = Guid.NewGuid().ToString("N") + ".temp";
                    CurrentFile = file.Name;

                    using (FileStream fileStream = File.Create(temporaryFilePath))
                    {
                        _logger.Info($"Begin downloading file (Id:{file.Id} Name:{file.Name} Size:{file.Size})");
                        HttpUtil.RequestStream(_address, $"REQUEST_FILE?id={file.Id}", fileStream, (current, total, chunkSize) =>
                        {
                            Downloaded += chunkSize;
                        });
                        _logger.Info($"Finished downloading file (Id:{file.Id} Name:{file.Name} Size:{file.Size})");
                    }

                    int hash = CalculateAdler32Hash(temporaryFilePath);
                    if (hash != file.CRC32)
                    {
                        _logger.Error($"Invalid file received (Id:{file.Id} Name:{file.Name} Size:{file.Size} LocalHash:{hash} ServerHash:{file.CRC32})");
                        CurrentState = State.Error;
                        File.Delete(temporaryFilePath);
                        return;
                    }

                    File.Move(temporaryFilePath, destinationPath);
                    RemoveQueuedAsset(file);
                }

                TotalDownloadSize = 0;
                Downloaded = 0;
                CurrentFile = string.Empty;
                CurrentState = State.Done;
                _logger.Info("Finished downloading all files");
            }
            catch (Exception exception)
            {
                _logger.Info($"Error downloading files:{exception}");
                CurrentState = State.Error;
                if (File.Exists(temporaryFilePath))
                    File.Delete(temporaryFilePath);
            }
        }

        /// <summary>
        /// Returns the number of files still waiting to be downloaded.
        /// </summary>
        /// <returns>The remaining file count.</returns>
        public int GetRemainingFiles()
        {
            lock (_neededAssets)
                return _neededAssets.Count;
        }

        /// <summary>
        /// Compares local files against the manifest and records every file that must be downloaded.
        /// </summary>
        /// <param name="manifestFiles">The files advertised by the patch server.</param>
        /// <param name="patchDirectory">The relative output directory for patched files.</param>
        private void BuildDownloadQueue(IEnumerable<FileManifestItem> manifestFiles, string patchDirectory)
        {
            lock (_neededAssets)
            {
                foreach (FileManifestItem file in manifestFiles)
                {
                    string path = Path.Combine(Application.StartupPath + patchDirectory, file.Name);
                    if (!File.Exists(path))
                    {
                        _logger.Info($"Adding file (Id:{file.Id} Name:{file.Name})");
                        _neededAssets.Add(file);
                        continue;
                    }

                    int hash = CalculateAdler32Hash(path);
                    if (hash == file.CRC32)
                        continue;

                    _logger.Info($"File is out of date (Id:{file.Id} Name:{file.Name})");
                    _neededAssets.Add(file);
                }
            }
        }

        /// <summary>
        /// Calculates the Adler-32 checksum for a local file.
        /// </summary>
        /// <param name="filePath">The file to inspect.</param>
        /// <returns>The Adler-32 checksum.</returns>
        private static int CalculateAdler32Hash(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
                return (int)Utils.Adler32(fileStream, fileStream.Length);
        }

        /// <summary>
        /// Removes a successfully downloaded file from the pending queue.
        /// </summary>
        /// <param name="file">The file that completed download and validation.</param>
        private void RemoveQueuedAsset(FileManifestItem file)
        {
            lock (_neededAssets)
                _neededAssets.Remove(file);
        }

        /// <summary>
        /// Describes a file entry provided by the patch manifest.
        /// </summary>
        public class FileManifestItem
        {
            /// <summary>
            /// Gets or sets the patch server identifier for the file.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the relative file name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the expected Adler-32 checksum.
            /// </summary>
            public int CRC32 { get; set; }

            /// <summary>
            /// Gets or sets the file size in bytes.
            /// </summary>
            public long Size { get; set; }
        }

        /// <summary>
        /// Describes the patch manifest returned by the patch server.
        /// </summary>
        public class FileManifest
        {
            /// <summary>
            /// Gets or sets the total number of files listed in the manifest.
            /// </summary>
            public int TotalFiles { get; set; }

            /// <summary>
            /// Gets or sets the file entries included in the manifest.
            /// </summary>
            public List<FileManifestItem> Files { get; set; } = new List<FileManifestItem>();
        }

        /// <summary>
        /// Describes the patcher's progress state.
        /// </summary>
        public enum State
        {
            RequestManifest,
            ProcessManifest,
            Downloading,
            Done,
            Error,
            ServerOffline,
        }
    }
}

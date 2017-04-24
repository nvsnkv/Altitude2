using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using NV.Altitude2.Domain;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageManager : INotifyCollectionChanged
    {
        private static readonly string ExternalFolderName = "Altitude2 Data";
        private static readonly string PackagesFolderName = "Packages";

        private readonly JsonSerializer _serializer = new JsonSerializer();
        private IStorageFolder _folder;
        private int _packagesCount;

        public int PackagesCount => _packagesCount;

        public string FolderPath => _folder?.Path;

        public async Task Initialize(CancellationToken token)
        {
            var extenralFolder = (await KnownFolders.RemovableDevices.GetFoldersAsync()).FirstOrDefault();

            _folder = await GetSubfolder(await GetSubfolder(extenralFolder, ExternalFolderName), PackagesFolderName) ??
                      await GetSubfolder(ApplicationData.Current.LocalCacheFolder, PackagesFolderName, true);
            
            var files = await _folder.GetFilesAsync();
            _packagesCount = files?.Count ?? 0;
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public async Task CreatePackage(List<Measurement> data)
        {
            if (_folder == null)
            {
                throw new InvalidOperationException("PackageManager needs to be initialized before any other action!");
            }

            var filename = $"p_{Guid.NewGuid()}";

            var file = await _folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);

            using (var stream = new DeflateStream(await file.OpenStreamForWriteAsync(),
                CompressionMode.Compress))
            using (var writer = new JsonTextWriter(new StreamWriter(stream)))
            {
                _serializer.Serialize(writer, data);
                await writer.FlushAsync();
            }

            _packagesCount++;
            RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { filename }));

        }

        public Task<string> GetNextPackage(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private async Task<StorageFolder> GetSubfolder(StorageFolder folder, string name, bool throwIfFailed = false)
        {
            if (folder == null && throwIfFailed) throw new ArgumentNullException(nameof(folder));
            if (folder == null) return null;

            var subfolders = await folder.GetFoldersAsync() ?? Enumerable.Empty<StorageFolder>();
            return subfolders.FirstOrDefault(f => f.Name.Equals(name)) ?? await CreateFolder(folder, name, throwIfFailed);
        }

        private async Task<StorageFolder> CreateFolder(StorageFolder folder, string name, bool throwIfFailed)
        {
            try
            {
                return await folder.CreateFolderAsync(name, CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {
                if (throwIfFailed) throw;
            }

            return null;
        }
    }
}
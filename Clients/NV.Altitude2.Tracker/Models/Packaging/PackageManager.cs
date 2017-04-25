using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Newtonsoft.Json;
using NV.Altitude2.Domain;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal sealed class PackageManager : INotifyCollectionChanged
    {
        private static readonly string ExternalFolderName = "Altitude2 Data";
        private static readonly string PackagesFolderName = "Packages";

        private readonly JsonSerializer _serializer = new JsonSerializer();
        private IStorageFolder _folder;
        private int _packagesCount;
        private string _folderPath;

        public int PackagesCount => _packagesCount;

        public string FolderPath => _folderPath;
        public bool IsInitialized => _folder != null;

        
        public async Task Initialize(CancellationToken token)
        {
            _folder = await OpenFolderByPath() ?? await ApplicationData.Current.LocalCacheFolder.GetFolderAsync("Packages");
            token.ThrowIfCancellationRequested();

            var files = await _folder.GetFilesAsync();
            token.ThrowIfCancellationRequested();

            _packagesCount = files?.Count ?? 0;
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            RaiseInitilalized();
        }

        public void SetFolderPath(string path = null)
        {
            _folderPath = path;
        }

        public async Task ChooseExternalFolder()
        {
            var picker = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.List
            };

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                SetFolderPath(folder.Path);
                await Initialize(CancellationToken.None);
            }
        }

        public async Task CreatePackage(List<Measurement> data, CancellationToken token)
        {
            if (_folder == null)
            {
                throw new InvalidOperationException("PackageManager needs to be initialized before any other action!");
            }

            var filename = $"p_{Guid.NewGuid()}.a2p";
            token.ThrowIfCancellationRequested();

            var file = await _folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);

            using (var stream = new DeflateStream(await file.OpenStreamForWriteAsync(), CompressionMode.Compress))
            using (var writer = new JsonTextWriter(new StreamWriter(stream)))
            {
                _serializer.Serialize(writer, data);
                await writer.FlushAsync(token);
            }

            _packagesCount++;
            token.ThrowIfCancellationRequested();

            RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { filename }));

        }

        public async Task<string> GetNextPackage(CancellationToken token)
        {
            if (_folder == null)
            {
                throw new InvalidOperationException("PackageManager needs to be initialized before any other action!");
            }
            token.ThrowIfCancellationRequested();

            var files = await _folder.GetFilesAsync();
            token.ThrowIfCancellationRequested();

            return files.Select(f => f.Name).FirstOrDefault();
        }

        public async Task Clear()
        {
            if (_folder == null)
            {
                throw new InvalidOperationException("PackageManager needs to be initialized before any other action!");
            }

            var files = await _folder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event EventHandler Initilalized;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void RaiseInitilalized()
        {
            Initilalized?.Invoke(this, EventArgs.Empty);
        }

        private async Task<StorageFolder> OpenFolderByPath()
        {
            if (string.IsNullOrEmpty(_folderPath))
            {
                return null;
            }

            try
            {
                return await StorageFolder.GetFolderFromPathAsync(_folderPath);
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
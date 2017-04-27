using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using NV.Altitude2.Domain;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal sealed class PackageManager : INotifyCollectionChanged
    {
        private static readonly string _futureAccessToken = "PackagesFolder";

        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly object _moveLock = new object();

        private volatile int _packagesCount;

        public bool IsInitialized { get; private set; }

        public string FolderPath { get; private set; }

        public int PackagesCount => _packagesCount;

        public async Task Initialize()
        {
            var exists = StorageApplicationPermissions.FutureAccessList.ContainsItem(_futureAccessToken);

            IStorageFolder folder = null;
            if (exists)
            {
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_futureAccessToken);
            }

            if (folder == null)
            {
                var folderPicker = new FolderPicker()
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder,
                    FileTypeFilter = {"*"}
                };

                folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(_futureAccessToken, folder);
                    var files = await folder.GetFilesAsync();
                    _packagesCount = files?.Count ?? 0;
                }
            }

            IsInitialized = folder != null;
            FolderPath = folder?.Path ?? "Not initialized";
            RaiseReset();
        }

        public async Task DeletePackage(string package)
        {
            var targetName = $"o_{package}";
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_futureAccessToken);
            if (folder == null) throw new InvalidOperationException("Manager is not initialized");

            var target = await folder.TryGetItemAsync(targetName);
            if (target != null)
            {
                await target.DeleteAsync();
            }

            _packagesCount--;
            RaiseRemoved(package);
        }

        public async Task<string> GetNextPackage()
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_futureAccessToken);
            if (folder == null) throw new InvalidOperationException("Manager is not initialized");

            var files = await folder.GetFilesAsync();
            var file =  files?.FirstOrDefault(f => f.Name.StartsWith("p"));

            var package = file?.Name;
            
            return package;
        }

        public async Task CreatePackage(List<Measurement> data)
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_futureAccessToken);
            if (folder == null) throw new InvalidOperationException("Manager is not initialized");

            var name = $"p_{Guid.NewGuid()}.a2p";
            var file = await folder.CreateFileAsync(name);

            using (var fileStream = await file.OpenStreamForWriteAsync())
            using (var stream = new DeflateStream(fileStream, CompressionMode.Compress))
            using (var writer = new JsonTextWriter(new StreamWriter(stream)))
            {
                _serializer.Serialize(writer, data);

                await writer.FlushAsync();
                await stream.FlushAsync();
                await fileStream.FlushAsync();
            }

            _packagesCount++;
            RaiseAdded(name);
        }

        public async Task<IInputStream> OpenPackageStream(string package)
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_futureAccessToken);
            if (folder == null) throw new InvalidOperationException("Manager is not initialized");

            var file = await folder.GetFileAsync(package);
            var nextName = $"o_{package}";
            IAsyncAction rename = null;
            lock (_moveLock)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (rename != null) return null;
                rename = file.RenameAsync(nextName);
            }

            await rename;

            file = await folder.GetFileAsync(nextName);
            return await file.OpenReadAsync();
        }

        public async Task Clear()
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_futureAccessToken);
            if (folder == null) throw new InvalidOperationException("Manager is not initialized");

            var files = await folder.GetFilesAsync();
            if (files != null)
            {
                foreach (var file in files.Where(f => f.Name.StartsWith("p")))
                {
                    await file.DeleteAsync();
                    _packagesCount--;
                }
            }
            RaiseReset();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void RaiseAdded(string package)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { package }));
        }

        private void RaiseRemoved(string package)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { package }));
        }

        private void RaiseReset()
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public async Task SetFolder()
        {
            StorageApplicationPermissions.FutureAccessList.Remove(_futureAccessToken);
            await Initialize();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using NV.Altitude2.Domain;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal sealed class PackageManager : INotifyCollectionChanged
    {
        public bool IsInitialized { get; }

        public string FolderPath { get; }

        public int PackagesCount { get; set; }

        public async Task Initialize()
        {
            throw new System.NotImplementedException();
            RaiseReset();
        }

        public async Task DeletePackage(string package)
        {
            throw new System.NotImplementedException();
            RaiseRemoved(package);
        }

        public async Task<PipelineData> GetNextPackage()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreatePackage(List<Measurement> data)
        {
            throw new System.NotImplementedException();
            var name = $"p_{Guid.NewGuid()}.a2p";
            RaiseAdded(name);
        }

        public async Task<Stream> OpenPackageStream(string data)
        {
            throw new NotImplementedException();
        }

        public async Task Clear()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
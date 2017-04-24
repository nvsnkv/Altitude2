using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NV.Altitude2.Domain;

namespace NV.Altitude2.Tracker.Models.Packaging
{
    internal class PackageManager
    {
        public Task Initialize(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task CreatePackage(List<Measurement> data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNextPackage(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using NV.Altitude2.Tracker.Models.Packaging;
using NV.Altitude2.Tracker.Models.Pipeline;

namespace NV.Altitude2.Tracker.Models.Transfer
{
    internal class TransferService:InOutPipelineService<string, string, GenericState>
    {
        private static readonly Uri Ping = new Uri("/api/measurements/count", UriKind.Relative);
        private static readonly Uri Packages = new Uri("/api/measurements", UriKind.Relative);

        private readonly PackageManager _manager;

        private volatile bool _isStarting;
        private volatile bool _isStopping;

        private Uri _apiUrl;
        private HttpClient _client;

        public TransferService(PackageManager manager)
        {
            _manager = manager;
        }

        public string DeviceId { get; set; }

        public string ApiUrl
        {
            get => _apiUrl?.ToString();
            set
            {
                if (State != GenericState.Disabled)
                {
                   RaiseErrorOccured(new InvalidOperationException("Unable to change ApiUrl while service is running!")); 
                }

                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                {
                    RaiseErrorOccured(new ArgumentException("Invalid URI given", nameof(value)));
                }

                _apiUrl = new Uri(value, UriKind.Absolute);
            }
        }

        public async Task<bool> CheckApiAvailable()
        {
            if (_apiUrl == null) return false;

            var client = _client ?? new HttpClient()
            {
                BaseAddress = _apiUrl
            };
            try
            {
                var response = await client.GetAsync(Ping);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                RaiseErrorOccured(e);
                return false;
            }
        }

        protected override Task<bool> DoInitialize()
        {
            return Task.FromResult(true);
        }

        protected override async Task DoStart()
        {
            if (_apiUrl == null) throw new InvalidOperationException("ApiUrl should not be empty!");
            if (_isStarting) throw new InvalidOperationException("Attempt to parallel start");
            _isStarting = true;

            _client = new HttpClient
            {
                BaseAddress = _apiUrl
            };

            if (!await CheckApiAvailable())
            {
                await DoStop();
            }
            else
            {
                State = GenericState.Enabled;
            }

            _isStarting = false;
        }

        protected override Task DoStop()
        {
            if (_isStopping) throw new InvalidOperationException("Attempt to parallel stop"); ;
            _isStopping = true;

            _client?.CancelPendingRequests();
            _client?.Dispose();
            _client = null;

            State = GenericState.Disabled;
            _isStopping = false;

            return Task.CompletedTask;
        }

        protected override async Task Handle(string data)
        {
            var package = await _manager.OpenPackageStream(data);
            if (package == null) return;
            
            using (package)
            using (var readStream = package.AsStreamForRead())
            using (var content = new StreamContent(readStream))
            {
                var request = new HttpRequestMessage(HttpMethod.Put, Packages)
                {
                    Content = content
                };

                if (!string.IsNullOrEmpty(DeviceId))
                {
                    request.Headers.Add("X-DEVICE-ID", DeviceId);
                }

                var response = await _client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                if (response.Headers.TryGetValues("X-DEVICE-ID", out IEnumerable<string> values))
                {
                    DeviceId = values.First();
                }
            }

           
            await SendNext(data);
        }
    }
}
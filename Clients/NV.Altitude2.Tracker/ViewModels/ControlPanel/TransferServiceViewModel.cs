﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using NV.Altitude2.Tracker.Models.Settings;
using NV.Altitude2.Tracker.Models.Transfer;

namespace NV.Altitude2.Tracker.ViewModels.ControlPanel
{
    internal class TransferServiceViewModel : ViewModelBase
    {
        private static readonly Brush GreenBrush = new SolidColorBrush(Colors.LightGreen);
        private static readonly Brush RedBrush = new SolidColorBrush(Colors.OrangeRed);

        private readonly TransferService _service;
        private readonly TransferServiceSettings _settings;
        private string _connectionStatus;
        private Brush _statusColor;

        public TransferServiceViewModel(TransferService service, IServiceTogglerViewModel toggler, TransferServiceSettings settings, CoreDispatcher dispatcher) : base(dispatcher)
        {
            _service = service;
            TransferService = toggler;
            _settings = settings;

            if (Uri.IsWellFormedUriString(_settings.ApiUrl, UriKind.Absolute))
            {
                _service.ApiUrl = _settings.ApiUrl;
            }
            else
            {
                _settings.ApiUrl = null;
            }

            CheckConnection = new CheckConnectionCommand(this);
        }

        public IServiceTogglerViewModel TransferService { get; }

        public string Url
        {
            get => _service.ApiUrl;
            set
            {
                if (value == _service.ApiUrl) return;
                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                {
                    _service.ApiUrl = _settings.ApiUrl = null;
                }
                else
                {
                    _service.ApiUrl = _settings.ApiUrl = value;
                }

                RaisePropertyChanged();
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            private set
            {
                if (value == _connectionStatus) return;
                _connectionStatus = value;
                RaisePropertyChanged();
            }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            private set
            {
                if (Equals(value, _statusColor)) return;
                _statusColor = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CheckConnection { get; }

        private class CheckConnectionCommand : ICommand
        {
            private readonly TransferServiceViewModel _viewModel;

            public CheckConnectionCommand(TransferServiceViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                var _ = ExecuteAsync();
            }

            private async Task ExecuteAsync()
            {
                var result = await _viewModel._service.CheckApiAvailable();
                if (result)
                {
                    _viewModel.ConnectionStatus = $"{DateTime.Now:dd.MM.yyyy hh:mm:ss}: Connection succeed.";
                    _viewModel.StatusColor = GreenBrush;
                }
                else
                {
                    _viewModel.ConnectionStatus = $"{DateTime.Now:dd.MM.yyyy hh:mm:ss}: Connection failed.";
                    _viewModel.StatusColor = RedBrush;
                }
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
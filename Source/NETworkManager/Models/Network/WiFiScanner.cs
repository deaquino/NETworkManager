﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;

namespace NETworkManager.Models.Network
{
    public class WiFiScanner
    {
        #region Variables
        private WiFiAdapter _wiFiAdapter;
        public WiFiAdapter WiFiAdapter
        {
            get { return _wiFiAdapter; }
            set
            {
                if (value == _wiFiAdapter)
                    return;

                _wiFiAdapter = value;
            }
        }

        private DeviceInformationCollection _wiFiAdapters;
        public DeviceInformationCollection WiFiAdapters
        {
            get { return _wiFiAdapters; }
            set
            {
                if (value == _wiFiAdapters)
                    return;

                _wiFiAdapters = value;
            }
        }
        #endregion

        #region Events
        public event EventHandler<WiFiScannerWiFiNetworkFoundArgs> WiFiNetworkFound;

        protected virtual void OnWiFiNetworkFound(WiFiScannerWiFiNetworkFoundArgs e)
        {
            WiFiNetworkFound?.Invoke(this, e);
        }

        public event EventHandler Complete;

        protected virtual void OnComplete()
        {
            Complete?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Constructor
        public WiFiScanner()
        {
            Initialize();
        }
        #endregion


        #region Methods
        private async void Initialize()
        {
            WiFiAccessStatus wiFiAccessStatus = await WiFiAdapter.RequestAccessAsync();

            if (wiFiAccessStatus != WiFiAccessStatus.Allowed)
                throw new WiFiScannerAccessDeniedException();
        }

        public async void FindAdapters()
        {
            WiFiAdapters = await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
        }

        public async void SetAdpater(string id = null)
        {
            if (WiFiAdapters.Count == 0)
                throw new Exception("No adapter available!");

            WiFiAdapter = await WiFiAdapter.FromIdAsync(id == null ? WiFiAdapters.First().Id : WiFiAdapters.First(x => x.Id == id).Id);
        }

        public async Task Scan()
        {
            if (WiFiAdapter == null)
                throw new Exception("No adapter selected");

            // Do a scan
            await WiFiAdapter.ScanAsync();

            // Process the result
            foreach(WiFiAvailableNetwork network in WiFiAdapter.NetworkReport.AvailableNetworks)
            {
                OnWiFiNetworkFound(new WiFiScannerWiFiNetworkFoundArgs(network.Bssid, network.Ssid, network.SignalBars, network.ChannelCenterFrequencyInKilohertz, network.NetworkKind, network.PhyKind));
            }

            OnComplete();
        }
        #endregion
    }
}

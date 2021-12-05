using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Diagnostics;

namespace Robotic_Arm
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //scans for bt devices
        async void OnButtonClicked(object sender, EventArgs args)
        {
            await label.RelRotateTo(360, 2000);
            //OINK
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;
            var state = ble.State;
            Debug.WriteLine("STATE: " + state);


            adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
            await adapter.StartScanningForDevicesAsync();



            
            state = ble.State;
            Debug.WriteLine("STATE: " + state);
        }
    }
}

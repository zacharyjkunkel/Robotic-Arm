using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


        BTController btcon;



        public MainPage(BTController _btcon)
        {
            InitializeComponent();

            btcon = _btcon;
        }

        
        //scans for bt devices
        async void BTscan(object sender, EventArgs args)
        {
            if (btcon.adapter.IsScanning)
            {
                return;
            }


            btcon.adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
            btcon.adapter.ScanTimeout = 3000;

            btcon.deviceList.Clear();
            btcon.adapter.DeviceDisconnected += (s, e) =>
            {
                //btcon.deviceList.Remove(e.Device);
                Debug.WriteLine("REMOVE list: " + e.Device);
            };

            btcon.adapter.DeviceDiscovered += (s, e) =>
            {
                //  Debug.WriteLine(e.Device);
                if (e.Device.ToString() != "")
                {
                    btcon.deviceList.Add(e.Device);
                }
                Debug.WriteLine("Device list: " + e.Device); 
                Debug.WriteLine("Device NAME: " + e.Device.Name);

            };

            btcon.adapter.DeviceAdvertised += (s, e) =>
            {
                Debug.WriteLine("Device advertised: " + e.Device);
            };  

            //add already known devices
            Debug.WriteLine("SIZE OF TISWEHTNOBH: " + btcon.adapter.GetSystemConnectedOrPairedDevices().Count()); ;
            foreach (IDevice newDev in btcon.adapter.GetSystemConnectedOrPairedDevices())
            {
                Debug.WriteLine("NEWDEVICE " + newDev);
                btcon.deviceList.Add(newDev);
            }

            //scan for devices
            if (!btcon.adapter.IsScanning)
            {
                await btcon.adapter.StartScanningForDevicesAsync();
            }


            await label.RelRotateTo(360, 3000);

            App.Current.MainPage = new NavigationPage(new DeviceSelection(btcon));
            //Navigation.PushAsync(new DeviceSelection(DeviceList));

        }

        //checks if bluetooth is on
        async void CheckStatus(object sender, EventArgs args)
        {
            var state = btcon.ble.State;
            await this.DisplayAlert("title", state.ToString(), "yes thanks");

        }

        //setup bt characteristics
        async void BTservices(object sender, EventArgs args)
        {
            btcon.services = await btcon.btDev.GetServicesAsync();
            btcon.service = await btcon.btDev.GetServiceAsync(btcon.btDev.Id);
            //characteristics = await services[0].GetCharacteristicsAsync();
            btcon.characteristics = await btcon.service.GetCharacteristicsAsync();
            //characteristic = characteristics[0];
            btcon.characteristic = await btcon.service.GetCharacteristicAsync(btcon.btDev.Id); //Guid.Parse("guidd")

        }

        //sends over test data
        async void SendData(object sender, EventArgs args)
        {
            byte[] data = { 0x01, 0x00 };
            await btcon.characteristic.WriteAsync(data);

        }

    }
}

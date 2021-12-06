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
                btcon.deviceList.Add(e.Device);
                Debug.WriteLine("Device list: " + e.Device);
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

        async void CheckStatus(object sender, EventArgs args)
        {
            var state = btcon.ble.State;
            await this.DisplayAlert("title", state.ToString(), "yes thanks");

        }

        async void BTConnect(object sender, EventArgs args)
        {
            try
            {
                if (btcon.btDev != null)
                {
                    await btcon.adapter.ConnectToDeviceAsync(btcon.btDev);
                }
                else
                {
                    await DisplayAlert("no device selected", "e", "e");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("cannot connect to device", "e","e");
            }

        }

        async void BTConnectKnown(object sender, EventArgs args)
        {
            try
            {
                await btcon.adapter.ConnectToKnownDeviceAsync(new Guid("guidd"));
            }
            catch (Exception ex)
            {
                await DisplayAlert("cannot connect to device", "e", "e");
            }

        }

        async void BTservices(object sender, EventArgs args)
        {
            btcon.services = await btcon.btDev.GetServicesAsync();
            btcon.service = await btcon.btDev.GetServiceAsync(btcon.btDev.Id);
            //characteristics = await services[0].GetCharacteristicsAsync();
            btcon.characteristics = await btcon.service.GetCharacteristicsAsync();
            //characteristic = characteristics[0];
            btcon.characteristic = await btcon.service.GetCharacteristicAsync(Guid.Parse("guidd")); //Btdev.Id

        }













        /*void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DeviceList.Add(e.Device);
        }
        void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine("Device already connected");
        }

        void Adapter_DeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            //DeviceDisconnectedEvent?.Invoke(sender,e);
            Debug.WriteLine("Device already disconnected");
        }

        void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            adapter.StopScanningForDevicesAsync();
            Debug.WriteLine("Timeout", "Bluetooth scan timeout elapsed");
        }*/


        /*
        var state = ble.State;
        Debug.WriteLine("STATE: " + state);


        adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
        adapter.DeviceConnected += Adapter_DeviceConnected;
        adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
        adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;


        adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
        await adapter.StartScanningForDevicesAsync();




        state = ble.State;
        Debug.WriteLine("STATE: " + state);
        Debug.WriteLine("connected devices?? " + adapter.ConnectedDevices);







        var services = await Btdev.GetServicesAsync();
        var characteristics = await services[0].GetCharacteristicsAsync();
        var characteristic = characteristics[0];

        Debug.WriteLine(await characteristic.ReadAsync());


        */
    }
}

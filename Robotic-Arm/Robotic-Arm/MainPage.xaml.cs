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
using Plugin.BLE.Abstractions.Exceptions;
using System.Diagnostics;


namespace Robotic_Arm
{
    public partial class MainPage : ContentPage
    {


        BTController btcon;
        ICharacteristic chara;



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
            btcon.adapter.ScanTimeout = 1000;

            btcon.deviceList.Clear();
            btcon.adapter.DeviceDisconnected += (s, e) =>
            {
                //btcon.deviceList.Remove(e.Device);
                Debug.WriteLine("REMOVE list: " + e.Device);
            };

            btcon.adapter.DeviceDiscovered += (s, e) =>
            {
                //  Debug.WriteLine(e.Device);
                if (e.Device.Name != "")
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

            foreach (IDevice newDev in btcon.adapter.ConnectedDevices)
            {
                Debug.WriteLine("NEWDEVICE " + newDev);
                btcon.deviceList.Add(newDev);
            }

            //scan for devices
            if (!btcon.adapter.IsScanning)
            {
                await btcon.adapter.StartScanningForDevicesAsync();
            }


            //await label.RelRotateTo(360, 3000);

            App.Current.MainPage = new NavigationPage(new DeviceSelection(btcon));
            //Navigation.PushAsync(new DeviceSelection(DeviceList));

        }

        //checks if bluetooth is on
        //var state = btcon.ble.State;
        //await this.DisplayAlert("title", state.ToString(), "yes thanks");


        //connect to device faster
        async void ConnectToESP32(object sender, EventArgs args)
        {

            await btcon.adapter.ConnectToKnownDeviceAsync(new Guid("00000000-0000-0000-0000-58bf25177936"));

            //set up services and characteristics
            btcon.services = await btcon.adapter.ConnectedDevices[0].GetServicesAsync();
            btcon.service = await btcon.adapter.ConnectedDevices[0].GetServiceAsync(btcon.services[2].Id);
            //IService servi = await btcon.btDev.GetServiceAsync(btcon.btDev.Id);
            //characteristics = await services[0].GetCharacteristicsAsync();
            btcon.characteristics = await btcon.service.GetCharacteristicsAsync();
            //characteristic = characteristics[0];
            btcon.characteristic = await btcon.service.GetCharacteristicAsync(btcon.characteristics[0].Id); //Guid.Parse("guidd")  btcon.btDev.Id

            btcon.characteristicTX = btcon.characteristics[1];
            btcon.characteristicRX = btcon.characteristics[0];
        }



        //constantly read input
        async void ReadData(object sender, EventArgs args)
        {
            byte[] test = await btcon.characteristics[0].ReadAsync();
            byte[] prevTest = test;
            //Debug.WriteLine("Received: " + Encoding.Default.GetString(test));

            btcon.characteristicRX.ValueUpdated += (s, e) =>
            {
                Debug.WriteLine("Received: " + Encoding.Default.GetString(e.Characteristic.Value));
            };
            await btcon.characteristicRX.StartUpdatesAsync();

            /*while (true)
            {
                try
                {
                    test = await btcon.characteristics[0].ReadAsync();
                    if (test != prevTest)
                    {
                        Debug.WriteLine("Received: " + Encoding.Default.GetString(test));
                    }
                    await Task.Delay(1000);
                }
                catch (CharacteristicReadException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }*/
        }

        private void CharacteristicRX_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }


        //sends over test data
        async void SendData(object sender, EventArgs args)
        {
            //byte[] data = { 0x01 };
            byte[] data = Encoding.ASCII.GetBytes("OINK");

            try
            {
                await btcon.characteristicTX.WriteAsync(data);
                //await chara.WriteAsync(data);

                Debug.WriteLine("Sent: " + "OINK");
            } 
            catch(CharacteristicReadException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            while (true)
            {
                try
                {
                    await btcon.characteristicTX.WriteAsync(data);
                    //await chara.WriteAsync(data);
                    
                    Debug.WriteLine("Sent: " + "OINK");
                }
                catch (CharacteristicReadException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                await Task.Delay(4100);
            }

        }



    }
}

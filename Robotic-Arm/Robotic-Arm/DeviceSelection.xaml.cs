using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System.Diagnostics;



namespace Robotic_Arm
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceSelection : ContentPage
    {

        BTController btcon;

        public ObservableCollection<IDevice> deviceList { get; set; }



        public DeviceSelection(BTController _btcon) //
        {
            btcon = _btcon;
            //this.BindingContext = btcon.deviceList;
            BindingContext = btcon;

            //deviceList = btcon.deviceList;
            //lvdev.ItemsSource = btcon.deviceList;

            InitializeComponent();
        }



        //when a device is selected, connect to it
        async void SelectedDevice(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            Debug.WriteLine("YOU PICKED: " + e.Item.ToString());
            await btcon.adapter.StopScanningForDevicesAsync();
            try
            {
                btcon.btDev = e.Item as IDevice;
                var device = e.Item as IDevice;
                if (btcon.adapter.ConnectedDevices.Count == 0)
                {
                    await btcon.adapter.ConnectToDeviceAsync(device);
                    await Navigation.PopAsync();
                    await DisplayAlert("Bluetooth", "Connected to: " + device, "THANK YOU");
                }
                else
                {
                    await btcon.adapter.DisconnectDeviceAsync(device);
                }
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", "Could not connect to :" + ex.DeviceId, "OK");
            }

            //set up services and characteristics
            btcon.services = await btcon.btDev.GetServicesAsync();
            btcon.service = await btcon.btDev.GetServiceAsync(btcon.services[2].Id);
            //IService servi = await btcon.btDev.GetServiceAsync(btcon.btDev.Id);
            //characteristics = await services[0].GetCharacteristicsAsync();
            btcon.characteristics = await btcon.service.GetCharacteristicsAsync();
            //characteristic = characteristics[0];
            btcon.characteristic = await btcon.service.GetCharacteristicAsync(btcon.characteristics[0].Id); //Guid.Parse("guidd")  btcon.btDev.Id

            btcon.characteristicTX = btcon.characteristics[1];
            btcon.characteristicRX = btcon.characteristics[0];


            Debug.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEEE");
            Debug.WriteLine(btcon.btDev.Id.ToString());

            App.Current.MainPage = new MainPage(btcon);
        }




        //supposed to prevent app from closing when pressing the hardware back button, doesnt seem to work
        protected override bool OnBackButtonPressed()
        {
            //Navigation.PopAsync();
            App.Current.MainPage = new MainPage(btcon);
            return base.OnBackButtonPressed();
            //return true;
        }
    }
}
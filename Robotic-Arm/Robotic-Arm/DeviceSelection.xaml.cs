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


            App.Current.MainPage = new MainPage(btcon);
        }





        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage = new MainPage(btcon);
            //return base.OnBackButtonPressed();
            return false;
        }
    }
}
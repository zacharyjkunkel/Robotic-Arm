using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Robotic_Arm
{
    public partial class App : Application
    {
        BTController btcon;
        public App()
        {
            InitializeComponent();
            btcon = new BTController();
            MainPage = new MainPage(btcon);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}

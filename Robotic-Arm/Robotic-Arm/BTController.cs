using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Diagnostics;
using System.ComponentModel;

namespace Robotic_Arm
{
    public class BTController
    {
        public IDevice btDev { get; set; }
        public ObservableCollection<IDevice> deviceList { get; set; }


        public IBluetoothLE ble;
        public IAdapter adapter;

        public IReadOnlyList<IService> services;
        public IService service;

        public IReadOnlyList<ICharacteristic> characteristics;
        public ICharacteristic characteristic;

        public ICharacteristic characteristicRX;
        public ICharacteristic characteristicTX;

        public BTController()
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
        }

 

    }
}

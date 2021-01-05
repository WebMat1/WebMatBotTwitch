//using InTheHand.Net.Bluetooth;
//using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMatBot.LedArt
{
    public class Test
    {
        public static async Task Start()
        {
            try
            {

                //BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
                //using (BluetoothClient client = new BluetoothClient())
                //{
                //    BluetoothDeviceInfo[] devices = client.DiscoverDevices();

                //    var PixelDevice = devices.FirstOrDefault(q => q.DeviceName.Contains("PIXEL"));
                //    var installedService = PixelDevice.InstalledServices[0];
                //    var ep = new InTheHand.Net.BluetoothEndPoint(PixelDevice.DeviceAddress, installedService);
                //    client.Connect(ep);
                //    var bluetoothStream = client.GetStream();

                //    if (client.Connected && bluetoothStream != null)
                //    {
                //        bluetoothStream.WriteByte(0x1F);
                //        await bluetoothStream.WriteAsync(Encoding.ASCII.GetBytes("Teste Teste Teste Teste"));
                //        await bluetoothStream.FlushAsync();
                //        bluetoothStream.Close();
                //    }

                //    #region Teste
                //    //String authenticated;
                //    //String classOfDevice;
                //    //String connected;
                //    //String deviceAddress;
                //    //String deviceName;
                //    //String installedServices;
                //    //String lastSeen;
                //    //String lastUsed;
                //    //String remembered;
                //    //String rssi;


                //    //foreach (BluetoothDeviceInfo device in devices)
                //    //{
                //    //    authenticated = device.Authenticated.ToString();
                //    //    classOfDevice = device.ClassOfDevice.ToString();
                //    //    connected = device.Connected.ToString();
                //    //    deviceAddress = device.DeviceAddress.ToString();
                //    //    deviceName = device.DeviceName.ToString();
                //    //    installedServices = device.InstalledServices.ToString();
                //    //    lastSeen = device.LastSeen.ToString();
                //    //    lastUsed = device.LastUsed.ToString();
                //    //    remembered = device.Remembered.ToString();
                //    //    rssi = device.Rssi.ToString();
                //    //    string[] row = new string[] { authenticated, classOfDevice, connected, deviceAddress, deviceName, installedServices, lastSeen, lastUsed, remembered, rssi };
                //    //    Console.WriteLine(string.Join(',',row));
                //    //}

                //    #endregion

                //}                //BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
                //using (BluetoothClient client = new BluetoothClient())
                //{
                //    BluetoothDeviceInfo[] devices = client.DiscoverDevices();

                //    var PixelDevice = devices.FirstOrDefault(q => q.DeviceName.Contains("PIXEL"));
                //    var installedService = PixelDevice.InstalledServices[0];
                //    var ep = new InTheHand.Net.BluetoothEndPoint(PixelDevice.DeviceAddress, installedService);
                //    client.Connect(ep);
                //    var bluetoothStream = client.GetStream();

                //    if (client.Connected && bluetoothStream != null)
                //    {
                //        bluetoothStream.WriteByte(0x1F);
                //        await bluetoothStream.WriteAsync(Encoding.ASCII.GetBytes("Teste Teste Teste Teste"));
                //        await bluetoothStream.FlushAsync();
                //        bluetoothStream.Close();
                //    }

                //    #region Teste
                //    //String authenticated;
                //    //String classOfDevice;
                //    //String connected;
                //    //String deviceAddress;
                //    //String deviceName;
                //    //String installedServices;
                //    //String lastSeen;
                //    //String lastUsed;
                //    //String remembered;
                //    //String rssi;


                //    //foreach (BluetoothDeviceInfo device in devices)
                //    //{
                //    //    authenticated = device.Authenticated.ToString();
                //    //    classOfDevice = device.ClassOfDevice.ToString();
                //    //    connected = device.Connected.ToString();
                //    //    deviceAddress = device.DeviceAddress.ToString();
                //    //    deviceName = device.DeviceName.ToString();
                //    //    installedServices = device.InstalledServices.ToString();
                //    //    lastSeen = device.LastSeen.ToString();
                //    //    lastUsed = device.LastUsed.ToString();
                //    //    remembered = device.Remembered.ToString();
                //    //    rssi = device.Rssi.ToString();
                //    //    string[] row = new string[] { authenticated, classOfDevice, connected, deviceAddress, deviceName, installedServices, lastSeen, lastUsed, remembered, rssi };
                //    //    Console.WriteLine(string.Join(',',row));
                //    //}

                //    #endregion

                //}

            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

    }
}

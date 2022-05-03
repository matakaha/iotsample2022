using Microsoft.Azure.Devices.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SimulateX509Device
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an X.509 certificate object.
            var cert = new X509Certificate2(@"{full path to pfx certificate.pfx}", "{your certificate password}");

            // Create an authentication object using your X.509 certificate. 
            var auth = new DeviceAuthenticationWithX509Certificate("{your-device-id}", cert);

            // Create the device client.
            var deviceClient = DeviceClient.Create("{your-IoT-Hub-name}.azure-devices.net", auth, TransportType.Mqtt);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes("This is a d2c message using X.509."));
            deviceClient.SendEventAsync(eventMessage);
         }
    }
}
using Microsoft.Azure.Devices;
using System.Text;
namespace service1
{

    class Program
    {
        static string connectionString = "{Your IoT Hub connection string here}";
        static string targetDevice = "{deviceId}";
        private static async Task Main(string[] args)
        {
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(targetDevice, commandMessage);
        }
    }
}
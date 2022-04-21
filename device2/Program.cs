using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text;

namespace device2
{
    class Program
    {

        private static string s_connectionString = "{Your device connection string here}";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device.");

            // Connect to the IoT hub using the MQTT protocol
            DeviceClient s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);


            // Set up a condition to quit the sample
            Console.WriteLine("Press control-C to exit.");
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            int i = 0;
            while (!cts.IsCancellationRequested)
            {
                string messageBody = "counter : " + i;
                using var message = new Message(Encoding.ASCII.GetBytes(messageBody));

                // Send the telemetry message
                await s_deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending message: {messageBody}");

                ReceiveC2dAsync(s_deviceClient);

                Twin twin = await s_deviceClient.GetTwinAsync();
                Console.WriteLine($"\t{twin.ToJson()}");
                ReportConnectivity(s_deviceClient);

                await Task.Delay(1000);
                i++;
            }

            await s_deviceClient.CloseAsync();
            s_deviceClient.Dispose();
            Console.WriteLine("Device simulator finished.");
        }
        private static async void ReceiveC2dAsync(DeviceClient s_deviceClient)
        {
            Message receivedMessage = await s_deviceClient.ReceiveAsync();
            if (receivedMessage != null) 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                foreach (KeyValuePair<string, string> prop in receivedMessage.Properties)
                {
                    Console.WriteLine($"\tProperty: key={prop.Key}, value={prop.Value}");
                }
                Console.ResetColor();

                await s_deviceClient.CompleteAsync(receivedMessage);

            }
        }
        public static async void ReportConnectivity(DeviceClient s_deviceClient)
        {
            try
            {
                Console.WriteLine("Sending connectivity data as reported property");

                TwinCollection reportedProperties, connectivity;
                reportedProperties = new TwinCollection();
                connectivity = new TwinCollection();
                connectivity["type"] = "cellular";
                reportedProperties["connectivity"] = connectivity;
                await s_deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }
    }
}
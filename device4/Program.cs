using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport;

namespace device4
{
    class Program
    {
        private static string s_connectionString = "{Your device connection string here}";
        private static async Task Main(string[] args)
        {
            // アップロードするファイルを作成する
            string filePath = "./device4" + Guid.NewGuid().ToString() + ".txt";
            File.WriteAllText(filePath, "Hello, World!");
            using var fileStreamSource = new FileStream(filePath, FileMode.Open);
            var fileName = Path.GetFileName(fileStreamSource.Name);

            // アップロード先のSASURIを取得
            DeviceClient s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);

            var fileUploadSasUriRequest = new FileUploadSasUriRequest
            {
                BlobName = fileName
            };
            FileUploadSasUriResponse sasUri = await s_deviceClient.GetFileUploadSasUriAsync(fileUploadSasUriRequest);

            // SASURIを使ってBlob Storageにファイルアップロード
            Uri uploadUri = sasUri.GetBlobUri();
            var blockBlobClient = new BlockBlobClient(uploadUri);
            await blockBlobClient.UploadAsync(fileStreamSource, new BlobUploadOptions());

            // アップロード完了通知
            var successfulFileUploadCompletionNotification = new FileUploadCompletionNotification
            {
                CorrelationId = sasUri.CorrelationId,
                IsSuccess = true,
                StatusCode = 200,
                StatusDescription = "Success"
            };
            await s_deviceClient.CompleteFileUploadAsync(successfulFileUploadCompletionNotification);

            await s_deviceClient.CloseAsync();
            s_deviceClient.Dispose();
        }
    }
}
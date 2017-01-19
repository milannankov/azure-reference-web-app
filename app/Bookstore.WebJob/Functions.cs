using ImageResizer;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;

namespace Bookstore.WebJob
{
    public class Functions
    {
        public static void ProcessNewListingImage([QueueTrigger("%ImageProcessQueueName%")] string messageText, CloudStorageAccount cloudStorageAccount)
        {
            var message = JsonConvert.DeserializeObject<ImageProcessMessage>(messageText);

            ResizeOriginalImage(cloudStorageAccount, message);
            UpdateBookListingImageId(cloudStorageAccount, message);
        }

        private static void ResizeOriginalImage(CloudStorageAccount cloudStorageAccount, ImageProcessMessage message)
        {
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            byte[] originalImageBytes = GetImageBytes(cloudStorageAccount, message.ImageId);

            var thumbnailViewBytes = ResizeImageForThumbnailView(originalImageBytes);
            var fullViewBytes = ResizeImageForFullView(originalImageBytes);

            UploadFinalImage(blobClient, message.ImageId + "_thumb.jpg", thumbnailViewBytes);
            UploadFinalImage(blobClient, message.ImageId + ".jpg", fullViewBytes);

            DeleteOriginalImage(cloudStorageAccount, message.ImageId);
        }

        private static void UploadFinalImage(CloudBlobClient blobClient, string imageName, byte[] imageBytes)
        {
            var imagesContainerName = ConfigurationManager.AppSettings["ImagesContainerName"];
            var imagesContainer = blobClient.GetContainerReference(imagesContainerName);

            var container = imagesContainer.GetBlockBlobReference(imageName);
            container.UploadFromByteArray(imageBytes, 0, imageBytes.Length);
        }

        private static byte[] ResizeImageForThumbnailView(byte[] originalImageBytes)
        {
            var thumbnailViewSettings = new ResizeSettings();
            thumbnailViewSettings.Format = "jpg";
            thumbnailViewSettings.MaxHeight = 500;
            thumbnailViewSettings.MaxWidth = 300;
            thumbnailViewSettings.Quality = 80;

            return ResizeImage(originalImageBytes, thumbnailViewSettings);
        }

        private static byte[] ResizeImageForFullView(byte[] originalImageBytes)
        {
            var fullViewSettings = new ResizeSettings();
            fullViewSettings.Format = "jpg";
            fullViewSettings.MaxHeight = 800;
            fullViewSettings.MaxWidth = 1200;
            fullViewSettings.Quality = 80;

            return ResizeImage(originalImageBytes, fullViewSettings);
        }

        private static byte[] ResizeImage(byte[] originalImageBytes, ResizeSettings settings)
        {
            var resizedImageStream = new MemoryStream();
            ImageBuilder.Current.Build(originalImageBytes, resizedImageStream, settings, false);

            return resizedImageStream.ToArray();
        }

        private static byte[] GetImageBytes(CloudStorageAccount cloudStorageAccount, string imageId)
        {
            var incomingContainerName = ConfigurationManager.AppSettings["IncomingContainerName"];

            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var incomingContainer = blobClient.GetContainerReference(incomingContainerName);
            var image = incomingContainer.GetBlobReference(imageId);
            image.FetchAttributes();

            var imageBytes = new byte[image.Properties.Length];
            image.DownloadToByteArray(imageBytes, 0);

            return imageBytes;
        }

        private static void DeleteOriginalImage(CloudStorageAccount cloudStorageAccount, string imageId)
        {
            var incomingContainerName = ConfigurationManager.AppSettings["IncomingContainerName"];

            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var incomingContainer = blobClient.GetContainerReference(incomingContainerName);
            var image = incomingContainer.GetBlobReference(imageId);

            image.Delete();
        }

        private static void UpdateBookListingImageId(CloudStorageAccount cloudStorageAccount, ImageProcessMessage message)
        {
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("listings");
            var op = TableOperation.Retrieve("books", message.ListingId);
            var r = table.Execute(op).Result as DynamicTableEntity;

            r.Properties.Add("ImageId", new EntityProperty(message.ImageId));
            table.Execute(TableOperation.Replace(r));
        }
    }
}

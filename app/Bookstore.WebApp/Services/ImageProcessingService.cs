using Bookstore.WebApp.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace Bookstore.WebApp.Services
{
    public class ImageProcessingService
    {
        private HttpServerUtilityBase server;
        private CloudStorageAccount storage;

        public ImageProcessingService(HttpServerUtilityBase server)
        {
            this.server = server;

            var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            this.storage = CloudStorageAccount.Parse(connectionString);
        }

        public void ProcessImage(Stream imageStream, BookListing listing)
        {
            var imageName = Guid.NewGuid().ToString();

            this.UploadImageToStorage(imageStream, imageName);
            this.SubmitImageForProcessing(listing.Id, imageName);
        }

        private void UploadImageToStorage(Stream imageStream, string imageName)
        {
            var blobClient = storage.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("incoming");
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            
            var blob = container.GetBlockBlobReference(imageName);
            blob.UploadFromStream(imageStream);
        }

        private void SubmitImageForProcessing(string bookListingId, string imageName)
        {
            var processQueueName = ConfigurationManager.AppSettings["ImageProcessQueueName"];
            var queueClient = this.storage.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(processQueueName);

            var queueMessage = new { ListingId = bookListingId, ImageId = imageName };
            var queueText = JsonConvert.SerializeObject(queueMessage);
            var message = new CloudQueueMessage(queueText);

            queue.AddMessage(message);
        }
    }
}
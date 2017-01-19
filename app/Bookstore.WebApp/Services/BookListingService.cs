using Bookstore.WebApp.Entities;
using Bookstore.WebApp.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Bookstore.WebApp.Services
{
    public class BookListingService
    {
        private HttpServerUtilityBase server;

        public BookListingService(HttpServerUtilityBase server)
        {
            this.server = server;
        }

        public IEnumerable<BookViewModel> GetBookListingViews()
        {
            var containerUrl = this.GetImagesContainerUrl();

            var views = this.GetBookListings()
                .OrderByDescending(l => l.PublishedOn)
                .Select(l => this.BookViewModelFromEntity(l, containerUrl));

            return views;
        }

        private string GetImagesContainerUrl()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            var imageContainerName = ConfigurationManager.AppSettings["ImagesContainerName"];
            var storage = CloudStorageAccount.Parse(connectionString);
            var blobClient = storage.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(imageContainerName);

            return container.Uri.AbsoluteUri + "/";
        }

        private BookViewModel BookViewModelFromEntity(BookListing entity, string imageContainerUrl)
        {
            var viewModel = new BookViewModel()
            {
                BookDescription = entity.Description,
                BookId = entity.Id,
                BookTitle = entity.Title,
                Price = entity.Price,
                ImageFullUrl = GetImageUrlForListing(entity, imageContainerUrl, ".jpg"),
                ImageThumbnailUrl = GetImageUrlForListing(entity, imageContainerUrl, "_thumb.jpg")
            };

            return viewModel;
        }

        private static string GetImageUrlForListing(BookListing listing, string containerUrl, string suffix)
        {
            if(listing.ImageId == null)
            {
                return "/Content/images/no-image.png";
            }

            return containerUrl + listing.ImageId + suffix;
        }

        private IEnumerable<BookListing> GetBookListings()
        {
            var table = this.GetListingsTable();
            var query = new TableQuery<BookListingTableEntity>();
            var listings = new List<BookListing>();

            foreach (BookListingTableEntity entity in table.ExecuteQuery(query))
            {
                var bookListing = new BookListing(entity.RowKey, entity.Title, entity.Description, entity.Price, entity.PublishedOn);
                bookListing.SetImage(entity.ImageId);

                listings.Add(bookListing);
            }

            return listings;
        }

        private CloudTable GetListingsTable()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            var listingsTableName = ConfigurationManager.AppSettings["ListingsTableName"];

            var storage = CloudStorageAccount.Parse(connectionString);
            var tableClient = storage.CreateCloudTableClient();
            var table = tableClient.GetTableReference(listingsTableName);

            return table;
        }

        public void SaveNewListing(BookListing newListing)
        {
            var tableEntity = BookListingTableEntity.FromEntity(newListing);

            StoreBookListingEntity(tableEntity);
        }

        private static void StoreBookListingEntity(BookListingTableEntity tableEntity)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            var listingsTableName = ConfigurationManager.AppSettings["ListingsTableName"];

            var storage = CloudStorageAccount.Parse(connectionString);
            var tableClient = storage.CreateCloudTableClient();
            var table = tableClient.GetTableReference(listingsTableName);

            var insertOperation = TableOperation.Insert(tableEntity);
            table.Execute(insertOperation);
        }
    }
}
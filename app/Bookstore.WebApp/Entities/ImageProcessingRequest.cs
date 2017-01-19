using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.WebApp.Entities
{
    public class ImageProcessingRequest
    {
        public ImageProcessingRequest(string listingId, string imageName)
        {
            this.BookListingId = listingId;
            this.ImageName = imageName;
        }

        public string BookListingId
        {
            get;
            private set;
        }

        public string ImageName
        {
            get;
            private set;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.WebApp.Entities
{
    public class BookListing
    {
        public BookListing(string id, string title, string description, double price, DateTimeOffset publishedOn)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new NullReferenceException("id");
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new NullReferenceException("title");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new NullReferenceException("description");
            }

            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PublishedOn = publishedOn;
        }

        public static BookListing New(string title, string description, double price)
        {
            var newId = Guid.NewGuid().ToString();
            var listing = new BookListing(newId, title, description, price, DateTimeOffset.Now);

            return listing;
        }

        public string Id
        {
            get;
            set;
        }

        public string ImageId
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public double Price
        {
            get;
            private set;
        }

        public DateTimeOffset PublishedOn
        {
            get;
            private set;
        }

        public void SetImage(string image)
        {
            this.ImageId = image;
        }
    }
}
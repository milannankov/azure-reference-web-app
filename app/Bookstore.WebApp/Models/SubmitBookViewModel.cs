using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookstore.WebApp.Models
{
    public class SubmitBookViewModel
    {
        [Required]
        [Display(Name = "Book title")]
        [DataType(DataType.Text)]
        [StringLength(150)]
        public string BookTitle
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Book description")]
        [DataType(DataType.Text)]
        [StringLength(1500)]
        public string BookDescription
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Range(1, 100)]
        public double Price
        {
            get;
            set;
        }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase BookImage
        {
            get;
            set;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.WebJob
{
    public class ImageProcessMessage
    {
        public string ListingId
        {
            get;
            set;
        }

        public string ImageId
        {
            get;
            set;
        }
    }
}

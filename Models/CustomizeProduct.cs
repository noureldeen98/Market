using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;



namespace Emarket.Models
{
    [MetadataType(typeof(Product_metadata))]
    //add new methods or new properties
    public partial class Product
    {
    }
    //edit properties
    public class Product_metadata
    {
        public int id { get; set; }
        [Display( Name = "ProductName")]
        public string name { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public Nullable<double> price { get; set; }
      
        public string image { get; set; }
        public string description { get; set; }
        public Nullable<int> category_id { get; set; }

        public virtual Category Category { get; set; }
    }
}





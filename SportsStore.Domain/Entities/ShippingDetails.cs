using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class ShippingDetails
    {

        [Required(ErrorMessage ="Please Enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Please Enter the first Address line")]
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3{ get; set; }

        [Required(ErrorMessage ="Please enter a city name")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter a state name")]
        public string State { get; set; }
        public string zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public string Country { get; set; }
        public bool GiftWrap { get; set; }


    }
}

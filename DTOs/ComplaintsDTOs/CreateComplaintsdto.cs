using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Uber.Uber.Application
{
    public class CreateComplaintsdto
    {
        public string Massege {  get; set; }

        [Required(ErrorMessage = " Please  Enter Your Email Address " )]
        [Display(Name = " Customer Email ")]
        [DataType(DataType.EmailAddress)]
        public string CustomerEmail {  get; set; }

        [Required(ErrorMessage = " Please  Enter Your  Driver Email Address ")]
        [Display(Name = " Driver Email ")]
        [DataType(DataType.EmailAddress)]
        public string DriverEmail {  get; set; }
        [Required(ErrorMessage ="  Enter Trip ID  ")]
        [Display(Name = " Trip Id ")]
        public int TripId {  get; set; }



    }
}

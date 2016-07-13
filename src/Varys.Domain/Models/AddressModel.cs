using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Varys.Domain.Models
{
    public class AddressModel
    {
        [Required(ErrorMessage = "Key is Required")]
        public string Key { get; set; }
        [Required(ErrorMessage = "Environment is Required")]
        public int? Environment { get; set; }
        [Required(ErrorMessage = "Application Name is Required")]
        public string AppName { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
       
    }

    public class AddressGetResponseModel
    {
        public AddressModel Address { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    public class AddressSearchResponseModel
    {
        public List<AddressModel> Addresses { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    public class AddressSaveResponseModel
    {
        public string Key { get; set; }
        public Boolean Status { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}

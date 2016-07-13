using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varys.Domain.Models;

namespace Varys.Elastic
{
    public interface IAddressService
    {
        AddressGetResponseModel Get(string key);
        AddressSearchResponseModel Search(int Environment, string AppName);
        AddressSaveResponseModel Save(AddressModel model);
        AddressSaveResponseModel Update(AddressModel model, bool overrideData);
    }
}

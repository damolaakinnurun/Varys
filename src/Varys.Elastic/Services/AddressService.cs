using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varys.Domain.Models;
using Varys.Elastic.Entities;

namespace Varys.Elastic.Services
{
    public class AddressService : EsClient, IAddressService
    {
        public AddressGetResponseModel Get (string key)
        {
            var result = new AddressGetResponseModel()
            {
                ErrorMessages = new List<string>()
            };

            var sd = new SearchDescriptor<EsAddress>();
            var filter = Filter<EsAddress>.Term(a => a.Id, key.Trim());
            sd.Filter(filter);
            ISearchResponse<EsAddress> sr = Client.Search<EsAddress>(sd);
            var res = sr.Documents.FirstOrDefault();
            if (res == null)
            {
                result.ErrorMessages.Add(string.Format("Address record with Key {0} does not exist", key));
                return result;
            }
            //map back to addressmodel
            var addM = new AddressModel()
            {
                Key = res.Id,
                AppName = res.AppName,
                Environment = res.Environment,
                Address = res.Address
            };
            result.Address = addM;
            return result;
        }
        public AddressSearchResponseModel Search(int environment, string appName)
        {
            var result = new AddressSearchResponseModel()
            {
                Addresses = new List<AddressModel>(),
                ErrorMessages = new List<string>()
            };

            try
            {
                var allAddresses = EsClient.Client.Search<EsAddress>(a => a
                    .AllTypes()
                    .MatchAll()
                   ).Documents.AsEnumerable();
                if (environment > 0) { allAddresses = allAddresses.Where(a => a.Environment == environment); }
                if (!string.IsNullOrEmpty(appName)) { allAddresses = allAddresses.Where(a => a.AppName == appName.Trim()); }

                foreach (var add in allAddresses.ToList())
                {
                    var addM = new AddressModel()
                    {
                        Key = add.Id,
                        Address = add.Address,
                        AppName = add.AppName,
                        Environment = add.Environment
                    };

                    result.Addresses.Add(addM);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.ToString());
                return result;
            }
            
        }
        public AddressSaveResponseModel Save(AddressModel model)
        {
            var result = new AddressSaveResponseModel()
            {
                ErrorMessages = new List<string>()
            };
            try
            {
                if (VerifyIndex(model.Key))
                {
                    result.ErrorMessages.Add("Key already exists!");
                    return result;
                }
                
                //map addressmodel to address
                var add = new EsAddress()
                {
                    Id = model.Key.Trim(),
                    Environment = model.Environment.Value,
                    Address = model.Address.Trim(),
                    AppName = model.AppName.Trim()
                };
                
                var indx = EsClient.Client.Index(add, i => i.Refresh()).Id;
                result.Key = indx;
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.ToString());
                return result;
            }

        }

        public AddressSaveResponseModel Update(AddressModel model, bool overrideData)
        {
            var result = new AddressSaveResponseModel()
            {
                ErrorMessages = new List<string>()
            };
            try
            {
                if (!VerifyIndex(model.Key))
                {
                    result.ErrorMessages.Add("Key does not exist!");
                    return result;
                }

                if(!overrideData)
                {
                    result.ErrorMessages.Add("Address cannot be updated!");
                    return result;
                }

                //map addressmodel to address
                var add = new EsAddress()
                {
                    Id = model.Key.Trim(),
                    Environment = model.Environment.Value,
                    Address = model.Address.Trim(),
                    AppName = model.AppName.Trim()
                };

                EsClient.Client.Index(add, i => i.Refresh());
                result.Key = model.Key;
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.ToString());
                return result;
            }
        }

        private bool VerifyIndex(string id)
        {
            var sd = new SearchDescriptor<EsAddress>();
            var filter = Filter<EsAddress>.Term(a => a.Id, id.Trim());
            sd.Filter(filter);
            ISearchResponse<EsAddress> sr = Client.Search<EsAddress>(sd);
            var res = sr.Documents.FirstOrDefault();
            if (res == null)
            {
                return false;
            }
            return true;
        }

 
    }
}

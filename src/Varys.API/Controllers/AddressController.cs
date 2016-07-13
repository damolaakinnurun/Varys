using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Varys.API.Infrastructure;
using Varys.Domain.Models;
using Varys.Elastic;

namespace Varys.API.Controllers
{
    public class AddressController : ApiController
    {
        public IAddressService AddressService
        {
            get { return EngineContext.Resolve<IAddressService>(); }
        }
        
        [HttpPost]
        public IHttpActionResult Save(AddressModel model)
        {
            var result = new AddressSaveResponseModel()
            {
                ErrorMessages = new List<string>()
            };

            if (ModelState.IsValid)
            {
                result = AddressService.Save(model);
                if (result.ErrorMessages.Any())
                {
                    return Content(HttpStatusCode.BadRequest, result);
                }
                return Ok(result);
            }
            IList<string> allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
            var errors = allErrors.Where(err => !string.IsNullOrEmpty(err)).ToList();
            if (errors.Count == 0)
            {
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.Exception.Message)).ToList();
            }
            result.ErrorMessages.AddRange(errors);
            return Content(HttpStatusCode.BadRequest, result);
        }

        [HttpPost]
        public IHttpActionResult Update(AddressModel model, bool overrideData = false )
        {
            var result = new AddressSaveResponseModel()
            {
                ErrorMessages = new List<string>()
            };
            if (ModelState.IsValid)
            {
                result = AddressService.Update(model, overrideData);
                if (result.ErrorMessages.Any())
                {
                    return Content(HttpStatusCode.BadRequest, result);
                }
                return Ok(result);
            }
            IList<string> allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
            var errors = allErrors.Where(err => !string.IsNullOrEmpty(err)).ToList();
            if (errors.Count == 0)
            {
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.Exception.Message)).ToList();
            }
            result.ErrorMessages.AddRange(errors);
            return Content(HttpStatusCode.BadRequest, result);
        }

        [HttpGet]
        public IHttpActionResult Get (string key = "")
        {
            var result = new AddressGetResponseModel()
            {
                ErrorMessages = new List<string>()
            };

            if (string.IsNullOrEmpty(key))
            {
                result.ErrorMessages.Add("Invalid Key");
                return Content(HttpStatusCode.BadRequest, result);
            }

            result = AddressService.Get(key);
            if (result.ErrorMessages.Any())
            {
                return Content(HttpStatusCode.BadRequest, result);
            }
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Search (string appName = "", int? environment = -1)
        {
            var result = new AddressSearchResponseModel()
            {
                ErrorMessages = new List<string>()
            };

            if (environment == null) { environment = -1; }
            if (string.IsNullOrEmpty(appName)){appName = string.Empty;}

            result = AddressService.Search(environment.Value, appName);
            if (result.ErrorMessages.Any())
            {
                return Content(HttpStatusCode.BadRequest, result);
            }
            return Ok(result);
        }
    }
}

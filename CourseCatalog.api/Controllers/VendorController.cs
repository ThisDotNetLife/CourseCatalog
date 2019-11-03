using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Entities;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace CourseCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase {
        private readonly IVendorRepository _vendorRepository;

        public VendorController(IVendorRepository vendorRepository) {
            _vendorRepository = vendorRepository;
        }

        [HttpGet()]
        public IActionResult Get() {
            try {
                string vendors = _vendorRepository.Get();
                string jsonFormatted = JValue.Parse(vendors).ToString(Formatting.Indented);
                return StatusCode(200, jsonFormatted);
            }
            catch (Exception ex) {
                dynamic response = new ExpandoObject();
                response.ErrorMsg = ex.Message;
                string errMsg = JsonConvert.SerializeObject(response, Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        [HttpPut()]
        public IActionResult Update(Vendor vendor) {
            try {
                _vendorRepository.Update(vendor);
                return Ok();
            }
            catch (Exception ex) {
                dynamic response = new ExpandoObject();
                response.ErrorMsg = ex.Message;
                string errMsg = JsonConvert.SerializeObject(response, Formatting.Indented);
                return BadRequest(errMsg);
            }
        }
    }
}
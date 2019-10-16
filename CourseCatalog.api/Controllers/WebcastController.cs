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

namespace CourseCatalog.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebcastController : ControllerBase {

        private readonly IWebcastRepository _dataRepository;

        public WebcastController(IWebcastRepository dataRepository) {
            _dataRepository = dataRepository;
        }

        // GET: api/Webcasts/GetAll
        [HttpGet()]
        public List<DataLayer.Entities.Webcast> GetAll() {
            List<DataLayer.Entities.Webcast> webcasts = _dataRepository.GetAll();

            return webcasts;
        }

        [HttpPost()]
        [Route("Save")]
        public IActionResult Save(DataLayer.Entities.Webcast webcast) {
            try {
                if (!ModelState.IsValid) {
                    return BadRequest(ModelState);
                }
                string payload = _dataRepository.Save(webcast).ToString();
                string jsonFormatted = JValue.Parse(payload).ToString(Formatting.Indented);

                return StatusCode(201, jsonFormatted);
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
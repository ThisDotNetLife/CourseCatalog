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
    public class TagController : ControllerBase { 
        private readonly ITagRepository _tagRepository;

        public TagController(ITagRepository tagRepository) {
            _tagRepository = tagRepository;
        }

        [HttpGet()]
        public IActionResult Get() {
            try {
                string vendors = _tagRepository.Get();
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
        public IActionResult Update(Tag tag) {
            try {
                _tagRepository.Update(tag);
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
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
    public class AuthorController : ControllerBase {
        private readonly IAuthorRepository _authorRepository;

        public AuthorController(IAuthorRepository authorRepository) {
            _authorRepository = authorRepository;
        }

        [HttpGet()]
        public IActionResult Get() {
            try {
                string authors = _authorRepository.Get();
                string jsonFormatted = JValue.Parse(authors).ToString(Formatting.Indented);
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
        public IActionResult Update(Author author) {
            try {
                _authorRepository.Update(author);
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
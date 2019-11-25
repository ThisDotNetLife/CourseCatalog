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

        [HttpGet()]
        [Route("ValidateFilesOnDisk")]
        public IActionResult ValidateFilesOnDisk(string driveLetter="") {
            try {
                return StatusCode(200, _dataRepository.ValidateFilesOnDisk(driveLetter));
            }
            catch (Exception ex) {
                dynamic response = new ExpandoObject();
                response.ErrorMsg = ex.Message;
                string errMsg = JsonConvert.SerializeObject(response, Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        [HttpGet()]
        [Route("FoldersOnDisk")]
        public IActionResult TopicsOnDisk() {
            try {
                string topics = _dataRepository.FoldersOnDisk();
                string jsonFormatted = JValue.Parse(topics).ToString(Formatting.Indented);
                return StatusCode(200, jsonFormatted);
            }
            catch (Exception ex) {
                dynamic response = new ExpandoObject();
                response.ErrorMsg = ex.Message;
                string errMsg = JsonConvert.SerializeObject(response, Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        [HttpGet()]
        [Route("Search")]
        public IActionResult Search(CourseCatalog.DAL.DTO.SearchCriteria searchCriteria) {
            try {
                string vendors = _dataRepository.Search(searchCriteria);
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

        [HttpDelete()]
        [Route("Delete")]
        public IActionResult Delete(int ID) {
            try {
                _dataRepository.Delete(ID);
                return StatusCode(200);
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
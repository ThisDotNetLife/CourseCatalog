using CourseCatalog.api.Services;
using DataLayerServices;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalog.api.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IWebcastRepository _dataRepository;

        public IndexController(IWebcastRepository dataRepository) {
            _dataRepository = dataRepository;
        }

        //public IActionResult Index() {
        //    string conn = _dataRepository.GetConnectionString();
        //    return Created(nameof(DataLayer.DTO.GET_Index), new { ConnectionString = conn });
        //}
    }
}
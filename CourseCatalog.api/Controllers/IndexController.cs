using CourseCatalog.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalog.api.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public IndexController(IDataRepository dataRepository) {
            _dataRepository = dataRepository;
        }

        public IActionResult Index() {
            string conn = _dataRepository.GetConnectionString();
            return Created(nameof(DTO.GET_Index), new { ConnectionString = conn });
        }
    }
}
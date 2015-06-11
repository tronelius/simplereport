using System.Web.Http;

namespace WorkerWebApi.Controllers
{
    public class TestController : ApiController
    {
        private readonly ILogger _logger;

        public TestController(ILogger logger)
        {
            _logger = logger;
        }

        [Route("api/test")]
        [HttpGet]
        public IHttpActionResult Test(string message)
        {
            _logger.Info("Hello, webapi called.");
            return Json(new { Msg = "Hello " + message });
        }
    }
}

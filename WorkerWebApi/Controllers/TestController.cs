﻿using System.ServiceModel;
using System.Web.Http;
using Worker.Common.Common;
using Worker.Common.Model;
using Worker.Common.Repository;

namespace WorkerWebApi.Controllers
{
    public class TestController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IScheduleRepository _scheduleRepository;

        public TestController(ILogger logger, IScheduleRepository scheduleRepository)
        {
            _logger = logger;
            _scheduleRepository = scheduleRepository;
        }

        [Route("api/test")]
        [HttpGet]
        public IHttpActionResult Test(string message)
        {
            _logger.Info("Hello, webapi called.");
            return Json(new { Msg = "Hello " + message });
        }

        [Route("api/create")]
        [HttpGet]
        public IHttpActionResult Create()
        {
            _logger.Info("Creating schedule");
            var schedule = new Schedule()
                           {
                               Name = "TestSchedule",
                               Cron = "0 0 0 0 0 0"

                           };
            var id = _scheduleRepository.Insert(schedule);
            return Json(new { Msg = "Created: " + id});
        }
    }
}

﻿using System;
using System.Web.Http;
using Newtonsoft.Json;
using Worker.Common.Common;
using Worker.Common.Model;
using Worker.Common.Repository;

namespace WorkerWebApi.Controllers
{
    [RoutePrefix("api/schedule")]
    public class ScheduleController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleController(ILogger logger, IScheduleRepository scheduleRepository)
        {
            _logger = logger;
            _scheduleRepository = scheduleRepository;
        }
        
        [Route("save")]
        [HttpPost]
        public IHttpActionResult Save(Schedule schedule)
        {
            _logger.Info("Creating schedule: " + JsonConvert.SerializeObject(schedule));
            if (schedule.Id == 0)
            {
                var id = _scheduleRepository.Insert(schedule);
                return Json(new { Id = id });
            }
            else
            {
                _scheduleRepository.Update(schedule);
                return Json(new { Id = schedule.Id });
            }
        }

        [Route("delete")]
        [HttpPost]
        public IHttpActionResult Delete([FromBody]int id)
        {
            _logger.Info("Deleting schedule: " + id);

            _scheduleRepository.Delete(id);
            var result = _scheduleRepository.List();
            return Json(result);
        }

        [Route("all")]
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                _logger.Info("Getting all schedules");
                var result = _scheduleRepository.List();
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.Error("Schedulecontroller.All", e);
                throw;
            }
        }
    }
}
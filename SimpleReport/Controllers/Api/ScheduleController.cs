using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleReport.Helpers;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;
using SimpleReport.Model.Subscriptions;

namespace SimpleReport.Controllers.Api
{

    public class ScheduleController : BaseApiController
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleController(IStorage reportStorage, IScheduleRepository scheduleRepository,  ILogger logger) : base(reportStorage, logger)
        {
            _scheduleRepository = scheduleRepository;
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> All()
        {
            try
            {
                var result = _scheduleRepository.List();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in All", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Save(Schedule schedule)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var id = _scheduleRepository.Insert(schedule);
                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Save", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        [Route("delete")]
        public async Task<IHttpActionResult> Delete([FromBody]int id)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                if (_scheduleRepository.IsInUse(id))
                {
                    return Ok(new { error = "The schedule is in use in subsciptions and cant be removed." });
                }

                _scheduleRepository.Delete(id);
                var result = _scheduleRepository.List();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Delete", ex);
                return InternalServerError();
            }
        }
    }
}
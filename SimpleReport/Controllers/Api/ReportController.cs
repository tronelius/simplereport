using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;

namespace SimpleReport.Controllers.Api
{

    public class ReportController : BaseApiController
    {
        public ReportController(IStorage reportStorage, ILogger logger): base(reportStorage, logger)
        {
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult IdToNameMappings()
        {
            try
            {
                if (!_subscriptionAccess.IsAllowedToSeeSubscriptions(User))
                {
                    _adminAccess.IsAllowedForMe(User);
                }
                
                var result = _reportStorage.GetAllReportInfos().Select(x => new { x.Id, x.Name });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in All", ex);
                return InternalServerError();
            }
        }
    }
}
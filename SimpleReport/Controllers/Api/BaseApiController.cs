using System;
using System.Web.Http;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;

namespace SimpleReport.Controllers.Api
{
    public class BaseApiController : ApiController
    {

        protected readonly IStorage _reportStorage;
        protected readonly ILogger _logger;
        protected Access _adminAccess;

        public BaseApiController(IStorage reportstorage, ILogger logger)
        {
            _reportStorage = reportstorage;
            _logger = logger;
            _adminAccess = _reportStorage.GetSettings().AdminAccessChecker();

        }

        protected void HandleNewEntity(IEntity entity)
        {
            if (entity.Id == null || entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
        }
    }
}
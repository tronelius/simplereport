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

        public BaseApiController(IStorage reportstorage, ILogger logger)
        {
            _reportStorage = reportstorage;
            _logger = logger;
        }

        protected void CheckForAdminAccess()
        {
            var adminaccess = _reportStorage.GetSettings().AdminAccess;
            if (!string.IsNullOrWhiteSpace(adminaccess) && !User.IsInRole(adminaccess))
                throw new Exception("Action not allowed");
        }

        protected void HandleNewEntity(IEntity entity)
        {
            if (entity.Id == null || entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
        }
    }
}
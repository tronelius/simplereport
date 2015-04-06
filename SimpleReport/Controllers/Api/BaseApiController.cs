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
            if (!User.IsInRole(_reportStorage.GetSettings().AdminAccess))
                throw new Exception("Action not allowed");
        }

        protected void HandleNewEntity(IEntity entity)
        {
            if (entity.Id == null || entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
        }
    }
}
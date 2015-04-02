using System;
using System.Web.Http;
using SimpleReport.Model;
using SimpleReport.Model.Storage;

namespace SimpleReport.Controllers.Api
{
    public class BaseApiController : ApiController
    {

        protected readonly IStorage _reportStorage;
        public BaseApiController(IStorage reportstorage)
        {
            _reportStorage = reportstorage;
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
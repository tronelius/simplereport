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
        protected Access _subscriptionAccess;

        public BaseApiController(IStorage reportstorage, ILogger logger)
        {
            _reportStorage = reportstorage;
            _logger = logger;
            _adminAccess = _reportStorage.GetSettings().AdminAccessChecker();
            _subscriptionAccess = _reportStorage.GetSettings().SubscriptionSettingAccessChecker();

        }

    }
}
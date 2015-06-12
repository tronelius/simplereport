using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace WorkerWebApi.Auth
{
    public class BasicAuthMiddleware : OwinMiddleware
    {
        private readonly string _username;
        private readonly string _password;

        public BasicAuthMiddleware(OwinMiddleware next, string username, string password) : base(next)
        {
            _username = username;
            _password = password;
        }

        public override Task Invoke(IOwinContext context)
        {
            var header = context.Request.Headers.Get("Authorization");

            if (!String.IsNullOrWhiteSpace(header))
            {
                var authHeader = System.Net.Http.Headers
                    .AuthenticationHeaderValue.Parse(header);

                if ("Basic".Equals(authHeader.Scheme,
                    StringComparison.OrdinalIgnoreCase))
                {
                    string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                    var parts = parameter.Split(':');

                    string userName = parts[0];
                    string password = parts[1];

                    if (userName == _username && password == _password)
                    {
                        Claim[] claims = new Claim[0];
                        var identity = new ClaimsIdentity(claims, "Basic");

                        context.Request.User = new ClaimsPrincipal(identity);
                        return Next.Invoke(context);
                    }
                }
            }

            context.Response.StatusCode = 401;
            return Task.FromResult<object>(null);
        }
    }
}

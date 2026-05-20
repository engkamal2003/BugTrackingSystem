using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BugTrackingSystem.Filters
{
    public class JwtAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null || authHeader.Scheme != "Bearer")
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                    {
                        Success = false,
                        Message = "Authorization token is missing."
                    });

                return;
            }

            var token = authHeader.Parameter;

            try
            {
                var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
                var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
                var audience = ConfigurationManager.AppSettings["JwtAudience"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var handler = new JwtSecurityTokenHandler();

                var principal = handler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken
                );

                Thread.CurrentPrincipal = principal;

                if (actionContext.RequestContext != null)
                {
                    actionContext.RequestContext.Principal = principal;
                }
            }
            catch
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                    {
                        Success = false,
                        Message = "Invalid or expired token."
                    });
            }
        }
    }
}
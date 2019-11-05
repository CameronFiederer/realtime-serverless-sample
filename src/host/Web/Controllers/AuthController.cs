using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAmazonCognitoIdentity cognitoClient;
        private readonly IAmazonIoT iotClient;
        private readonly IConfiguration config;

        public AmazonAuthenticationController(IAmazonCognitoIdentity cognitoClient, IAmazonIoT iotClient, IConfiguration config)
        {
            this.cognitoClient = cognitoClient;
            this.iotClient = iotClient;
            this.config = config;
        }

        [Route("auth/")]
        public Credentials Get()
        {
            CustomPrincipal currentUser = (CustomPrincipal)User;

            GetOpenIdTokenForDeveloperIdentityResponse identity = cognitoClient.GetOpenIdTokenForDeveloperIdentity(new GetOpenIdTokenForDeveloperIdentityRequest
            {
                IdentityPoolId = config.AwsIdentityPoolId(),
                Logins =
                {
                    { "sps-identity", currentUser.Id }
                }
            });

            iotClient.AttachPrincipalPolicy(config.EventsIotPolicyName(), identity.IdentityId);

            GetCredentialsForIdentityResponse credentials = cognitoClient.GetCredentialsForIdentity(new GetCredentialsForIdentityRequest
            {
                IdentityId = identity.IdentityId,
                Logins =
                {
                    { "cognito-identity.amazonaws.com", identity.Token }
                }
            });

            return credentials.Credentials;
        }
    }
}
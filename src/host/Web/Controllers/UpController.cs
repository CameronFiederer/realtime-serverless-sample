using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UpController : Controller
    {
        [Route("up/")]
        public string Get()
        {
            return "Happy";
        }
    }
}
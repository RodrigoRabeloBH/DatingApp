using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class HealthCheckController : BaseController
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "API started at: " + DateTime.Now.ToString();
        }
    }
}
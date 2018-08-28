using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IcgSoftware.Threema.CoreWebApp.Models;
using IcgSoftware.Threema.CoreMsgApi;
using Microsoft.Extensions.Configuration;

namespace IcgSoftware.Threema.CoreWebApp.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult Index()
        {
            string myThreemaId = this._configuration["Threema:ThreemaId"];
            string mySecret = this._configuration["Threema:Secret"];
            APIConnector apiConnector = new APIConnector(myThreemaId, mySecret, null);
            CreditsViewModel creditsViewModel;
            try
            {
                creditsViewModel = new CreditsViewModel() { Credits = apiConnector.LookupCredits() };
            }
            catch (Exception ex)
            {
                creditsViewModel = new CreditsViewModel() { ErrorMessage = ex.Message };
            }
            return View(creditsViewModel);
        }

    }
}

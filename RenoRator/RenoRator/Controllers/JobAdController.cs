using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RenoRator.Models;

namespace RenoRator.Controllers
{
    public class JobAdController : Controller
    {
        renoRatorDBEntities _db;
        //
        // GET: /JobAd/

        public ActionResult Index()
        {
            //_db = new renoRatorDBEntities();
            //var priceRanges = _db.PriceRanges1.ToList();
            //ViewData["priceranges"] = priceRanges;
            return View();
        }

        public ActionResult Post()
        {

            return View();
        }

        public ActionResult Ads()
        {
            return View();
        }

    }
}

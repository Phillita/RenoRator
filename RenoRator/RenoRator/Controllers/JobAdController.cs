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
            return View();
        }

        public ActionResult Post()
        {
            _db = new renoRatorDBEntities();
            var priceRanges = from range in _db.PriceRanges1.ToList()
                              select new { priceRangeID = range.priceRangeID, range = range.min + " - " + range.max };
            SelectList priceranges = new SelectList(priceRanges.ToArray(), "priceRangeID", "range");
            ViewBag.priceranges = priceranges;

            var citiesList = _db.cities.ToList();

            SelectList cities = new SelectList(citiesList.ToArray(), "cityID", "city1");
            ViewBag.cities = cities;

            return View();
        }

        public ActionResult Ads()
        {
            _db = new renoRatorDBEntities();
            var th = (from JobAds1 in _db.JobAds1 select JobAds1).ToList();

            return View(th);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Post(FormCollection form)
        {
            _db = new renoRatorDBEntities();
            var newJobAd = new JobAd();
            newJobAd.address = new Address();
            try
            {
                newJobAd.address.addressLine1 = form["address.addressLine1"];
                newJobAd.address.addressLine2 = form["address.addressLine2"];
                newJobAd.address.postalCode = form["address.postalCode"];
                newJobAd.address.cityID = Convert.ToInt32(form["address.cityID"]);
                newJobAd.address.province = "ON";
                newJobAd.address.country = "Canada";

                newJobAd.userID = 4;
                newJobAd.active = true;
                newJobAd.priceRangeID = Convert.ToInt32(form["priceRangeID"]);
                newJobAd.tags = form["tags"].Replace(",", "||");
                newJobAd.description = form["description"];
                newJobAd.targetEndDate = Convert.ToDateTime(form["targetEndDate"]);
            }
            catch (Exception ex) { }


            // Validate
            if (String.IsNullOrEmpty(newJobAd.address.addressLine1))
                ModelState.AddModelError("address_addressLine1", "First name is required!");
            if (newJobAd.address.cityID < 1)
                ModelState.AddModelError("lname", "Last name is required!");
            if (newJobAd.priceRangeID < 1)
                ModelState.AddModelError("password", "Password is required!");



            // If valid, save movie to database
            if (ModelState.IsValid)
            {
                _db.AddToJobAds1(newJobAd);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Otherwise, reshow form
            return View(newJobAd);

        }

    }
}

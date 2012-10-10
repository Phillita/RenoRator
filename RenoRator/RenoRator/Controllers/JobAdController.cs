using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RenoRator.Models;
using RenoRatorLibrary;

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

        public void populateDropdowns()
        {
            _db = new renoRatorDBEntities();
            var priceRanges = from range in _db.PriceRanges.ToList()
                              select new { priceRangeID = range.priceRangeID, range = range.min + " - " + range.max };
            SelectList priceranges = new SelectList(priceRanges.ToArray(), "priceRangeID", "range");
            ViewBag.priceranges = priceranges;

            var citiesList = _db.Cities.ToList();
            SelectList cities = new SelectList(citiesList.ToArray(), "cityID", "city1");
            ViewBag.cities = cities;

            var provinceList = _db.Provinces.ToList();
            SelectList provinces = new SelectList(provinceList.ToArray(), "provinceID", "province1");
            ViewBag.provinces = provinces;
        }

        public ActionResult Post()
        {
            if (Session["userID"] == null)
                return RedirectToAction("Login", "User", new { redirectPage = "Post", redirectController = "JobAd" });

            populateDropdowns();        

            return View();
        }

        public ActionResult Ads()
        {
            //if (Session["userID"] == null)
                //return RedirectToAction("Login", "User", new { redirectPage = "Post", redirectController = "JobAd" });

            _db = new renoRatorDBEntities();
            var ads = (from JobAds1 in _db.JobAds select JobAds1).ToList();

            Dictionary<string, int> tags = new Dictionary<string, int>();
            foreach( var ad in ads ){
                string[] allTags = ad.tags.Split('|');
                foreach (string tag in allTags)
                {
                    if (tag != "")
                    {
                        if (!tags.ContainsKey(tag))
                        {
                            tags[tag] = 1;
                        }
                        else
                        {
                            tags[tag]++;
                        }
                    }
                }
            }
                
            ViewBag.tags = tags;

            return View(ads);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Post(FormCollection form)
        {
            if (Session["userID"] == null)
                return RedirectToAction("Login", "User", new { redirectPage = "Post", redirectController = "JobAd" });

            _db = new renoRatorDBEntities();
            var newJobAd = new JobAd();
            newJobAd.address = new Address();

            TryUpdateModel(newJobAd, new string[] { "address.addressLine1", "address.addressLine2", "address.postalCode", "address.cityID" }, form.ToValueProvider());
            List<string> requiredFields = new List<string>(){"title","address.addressLine1","address.cityID","priceRangeID","description", "targetEndDate"};
            // check for null fields
            foreach(string field in requiredFields) 
            {
                if (String.IsNullOrEmpty(form[field].Trim()))
                        ModelState.AddModelError(field, "Field is required!");
            }

            // validate other fields
            if(!ValidateFunctions.validPostalCode(form["address.postalCode"]))
                ModelState.AddModelError("address.postalCode","Postal code is invalid!");
            if(!ValidateFunctions.validDateFormat(form["targetEndDate"]))
                ModelState.AddModelError("targetEndDate","Date format is invalid!");
            
            try
            {
                newJobAd.address.addressLine1 = form["address.addressLine1"];
                newJobAd.address.addressLine2 = form["address.addressLine2"];
                newJobAd.address.postalCode = form["address.postalCode"];
                newJobAd.address.cityID = Convert.ToInt32(form["address.cityID"]);
                newJobAd.address.country = "Canada";

                newJobAd.userID = (int)Session["userID"];
                newJobAd.active = true;
                newJobAd.priceRangeID = Convert.ToInt32(form["priceRangeID"]);
                newJobAd.tags = form["tags"].Replace(",", "||");
                newJobAd.description = form["description"];
                newJobAd.targetEndDate = Convert.ToDateTime(form["targetEndDate"]);
                newJobAd.title = form["title"];
            }
            catch{ }

            

            if(ModelState.IsValid) {
                _db.AddToJobAds(newJobAd);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Otherwise, reshow form
            TryUpdateModel(newJobAd);
            populateDropdowns();
            return View(newJobAd);

        }

    }
}

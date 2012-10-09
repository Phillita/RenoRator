using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RenoRator.Models;
using System.Security.Cryptography;

namespace RenoRator.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        renoRatorDBEntities _db;

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(FormCollection form)
        {
            _db = new renoRatorDBEntities();
            var newUser = new User();

            //temp
            newUser.userTypeID = 1;

            // Deserialize (Include white list!)
            TryUpdateModel(newUser, new string[] { "fname", "lname", "email", "password" }, form.ToValueProvider());

            // Validate
            if (String.IsNullOrEmpty(newUser.fname))
                ModelState.AddModelError("fname", "First name is required!");
            if (String.IsNullOrEmpty(newUser.lname))
                ModelState.AddModelError("lname", "Last name is required!");
            if (String.IsNullOrEmpty(newUser.email))
                ModelState.AddModelError("email", "Email is required!");
            if (String.IsNullOrEmpty(newUser.password))
                ModelState.AddModelError("password", "Password is required!");

            // salt and hash the password
            string salt = CreateSalt(8);
            newUser.salt = salt;
            newUser.password = CreateHash(newUser.password + salt);

            // If valid, save movie to database
            if (ModelState.IsValid)
            {
                _db.AddToUsers1(newUser);
                _db.SaveChanges();
                return RedirectToAction("Home");
            }

            // Otherwise, reshow form
            return View(newUser);

        }

        public ActionResult Login()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(FormCollection form, string redirectPage, string redirectController)
        {
            int id = tryLogin(form["email"].ToString(), form["password"].ToString());
            if (id > 0)
            {
                HttpContext.Session["userID"] = id;
                if (!string.IsNullOrEmpty(redirectPage) && !String.IsNullOrEmpty(redirectController))
                    return RedirectToAction(redirectPage, redirectController);
                return RedirectToAction("Home", "User");
            }

            // Otherwise, reshow form
            return View();

        }

        public ActionResult Logout()
        {
            return View();
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Logout(FormCollection form)
        {

            if (HttpContext.Session["userID"] != null)
                HttpContext.Session["userID"] = null;
            return RedirectToAction("Home");
        }

        private static int tryLogin(string email, string password) {
            renoRatorDBEntities _db = new renoRatorDBEntities();
            var user = _db.Users1.Where(u => u.email == email).FirstOrDefault();
            if (user != null && user.password == CreateHash(password + user.salt))
                return user.userID;
            return -1;
        }

        private static string CreateSalt(int size)
        {
            // Generate a cryptographic random number using the cryptographic 
            // service provider
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        private static string CreateHash(string passwordToHash)
        {
            // Create a new instance of the hash crypto service provider.
            HashAlgorithm hashAlg = new SHA256CryptoServiceProvider();
            // Convert the data to hash to an array of Bytes.
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(passwordToHash);
            // Compute the Hash. This returns an array of Bytes.
            byte[] bytHash = hashAlg.ComputeHash(bytValue);
            // Optionally, represent the hash value as a base64-encoded string, 
            // For example, if you need to display the value or transmit it over a network.
            string base64 = Convert.ToBase64String(bytHash);

            return base64;
        }

    }
}


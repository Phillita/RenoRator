using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RenoRator.Models;
using System.Security.Cryptography;
using RenoRatorLibrary;

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
            var newUser = new RegisterModel();

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
            if (newUser.password != form["passwordConfirm"])
                ModelState.AddModelError("passwordConfirm", "Passwords don't match!");
            if (newUser.email != form["emailConfirm"])
                ModelState.AddModelError("emailConfirm", "Email addresses don't match!");

            

            // If valid, save movie to database
            if (ModelState.IsValid)
            {
                newUser.Save();
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
            if (user != null && user.password == PasswordFunctions.CreateHash(password, user.salt))
                return user.userID;
            return -1;
        }
    }
}


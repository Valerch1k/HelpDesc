using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HelpDesc.Models;

namespace HelpDesc.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LogViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else //если авторизиовались , то переходим к списку заявок 
                    {
                        return RedirectToAction("Index", "Request");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный пароль или логин");
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            SelectList departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Departments = departments;
            return View();
        }


        [HttpPost]
        public ActionResult Registration(User user)
        {

            if (ModelState.IsValid && ValidateRegistr(user.Login))
            {
                string query = string.Format("insert into Users ([Name],[LastName], [Login],[Password],[Email],[PhoneNumber],[Position],[DepartmentId]) "
                                               +" Values(N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}', N'{6}', {7})",
                                                    user.Name, user.LastName, user.Login, user.Password, user.Email, user.PhoneNumber, user.Position , user.DepartmentId);
                int numberOfRowInserted = db.Database.ExecuteSqlCommand(query);
                if (numberOfRowInserted > 0)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    return RedirectToAction("Registration", "Account");
                }
               
            }
            SelectList departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Departments = departments;
            return View(user);
        }

        // проверяем , повторяется  логин или нет 
        private bool ValidateRegistr(string login)
        {
            bool isValid = false;
            try
            {
                User user = (from u in db.Users
                             where u.Login == login 
                             select u).FirstOrDefault();

                if (user == null)
                {
                    isValid = true;
                }
            }
            catch
            {

                isValid = false;
            }
            return isValid;
        }

        private bool ValidateUser(string login, string password)
        {
            bool isValid = false;

            using (HelpdeskContext _db = new HelpdeskContext())
            {
                try
                {
                    User user = (from u in _db.Users
                                 where u.Login == login && u.Password == password
                                 select u).FirstOrDefault();

                    if (user != null)
                    {
                        isValid = true;
                    }
                }
                catch
                {
                    isValid = false;
                }
            }
            return isValid;
        }
    }
}

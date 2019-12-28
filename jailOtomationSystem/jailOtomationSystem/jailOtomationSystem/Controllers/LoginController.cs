using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using jailOtomationSystem.Models;

namespace jailOtomationSystem.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        JailEntities db = new JailEntities();
        // GET: Login
        public ActionResult Index(string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Index(AdministratorAccount account, string ReturnUrl = null)
        {

            AdministratorAccount personel = db.AdministratorAccounts.FirstOrDefault(x => x.username ==account.username && x.password == account.password);
            if (personel == null)
            {
                ViewBag.Message = "yanlis Email veya Şifre !";
                return View();
            }
            else
            {
                //if (ReturnUrl == null || ReturnUrl == string.Empty)
                //    ReturnUrl = Server.UrlDecode("/");
                //else
                //    ReturnUrl = Server.UrlDecode(ReturnUrl);

                FormsAuthentication.SetAuthCookie(personel.administratorID.ToString(), false);
                return Redirect("~/admin/Index");
            }
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/Login");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using T4MVCHostMvcApp.Misc;

namespace T4MVCHostMvcApp.Controllers {
    [HandleError]
    public partial class HomeController : Controller {
        public virtual ActionResult Index() {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        const string firstConst = "The About";
        const string secondConst = "Action";
        [ActionName(firstConst + " " + secondConst)]
        public virtual ActionResult About() {
            return View();
        }

        [ActionName("New-Name for Blah")]
        public virtual ActionResult Blah(string name, int age) {
            return View();
        }

        public virtual void SomeVoidAction() {
        }

        public virtual ViewResult SomeViewResultAction() {
            return new ViewResult();
        }

        public virtual JsonResult SomeJsonResultAction() {
            return null;
        }

        public virtual FileContentResult SomeFileContentResultAction() {
            return null;
        }

        public virtual FileStreamResult SomeFileStreamResultAction() {
            return null;
        }

        public virtual FileResult SomeFileResultAction() {
            return null;
        }

        public virtual MyCustomResult SomeCustomResultAction() {
            return null;
        }

        public virtual ActionResult ActionWithArrayParam(string[] someStrings) {
            return new EmptyResult();
        }
    }
}

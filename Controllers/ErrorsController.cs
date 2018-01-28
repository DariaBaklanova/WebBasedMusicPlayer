using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    // Inspiration: http://stackoverflow.com/a/9026907 

    public sealed class ErrorsController : Controller
    {
        public ActionResult NotFound()
        {
            ViewBag.StatusCode = Response.StatusCode.ToString();

            return View();
        }

        public ActionResult ServerError()
        {
            ViewBag.StatusCode = Response.StatusCode.ToString();

            // The view will show the error message 
            // if running on localhost (for you, the programmer)

            // Otherwise, you must hover your mouse over the
            // "Technical information" error message

            ViewBag.Prompt = "Error message:";

            // Gather the details..
            if (HttpContext.Error == null)
            {
                ViewBag.Error = "(unavailable)";
            }
            else
            {
                ViewBag.Error = HttpContext.Error.Message.Replace("\r\n", "<br>");

                // Look deeper...
                if (HttpContext.Error.InnerException == null)
                {
                    ViewBag.ErrorDetail = "";
                }
                else
                {
                    ViewBag.ErrorDetail = 
                        HttpContext.Error.InnerException.Message.Replace("\r\n", "<br>");
                }
            }

            return View();
        }
    }
}

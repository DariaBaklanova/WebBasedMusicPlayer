using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    public class AudioController : Controller
    {
        Manager m = new Manager();
        // GET: Audio
        public ActionResult Index()
        {
            return View("index", "home");
        }

        [Route("clip/{id}")]
        public ActionResult Details(int? id)
        {
            // Attempt to get the matching object
            var o = m.TrackAudioGetById(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(o);
            }
        }
    }
}

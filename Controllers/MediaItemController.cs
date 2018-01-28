using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    public class MediaItemController : Controller
    {
        Manager m = new Manager();
        // GET: MediaItem
        public ActionResult Index()
        {
            return View("index", "home");
        }

        [Route("media/{stringId}")]
        public ActionResult Details(string stringId = "")
        {
            // Attempt to get the matching object
            var o = m.ArtistAudioGetById(stringId);

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(o);
            }
        }

        [Route("media/{stringId}/download")]
        public ActionResult DetailsDownload(string stringId = "")
        {
            // Attempt to get the matching object
            var o = m.ArtistAudioGetById(stringId);

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                string extension;
                RegistryKey key;
                object value;
                key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + o.ContentType, false);

                value = (key == null) ? null : key.GetValue("Extension", null);

                extension = (value == null) ? string.Empty : value.ToString();


                var cd = new System.Net.Mime.ContentDisposition
                {

                    FileName = $"media-{stringId}{extension}",

                    Inline = false
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(o.Content, o.ContentType);
            }
        }
    }
}

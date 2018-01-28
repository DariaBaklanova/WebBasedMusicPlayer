using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        // Reference to the manager object
        Manager m = new Manager();

        // GET: Albums
        public ActionResult Index()
        {
            return View(m.AlbumGetAll());
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            // Attempt to get the matching object
            var o = m.AlbumGetByIdWithDetail(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Pass the object to the view
                return View(o);
            }
        }

        // Add new album method is actually located in the Artists controller

        // ############################################################
        // Add track for this album

        // GET: Albums/5/AddTrack
        [Route("albums/{id}/addtrack")]
        [Authorize(Roles = "Clerk")]
        public ActionResult AddTrack(int? id)
        {
            // Attempt to get the associated object
            var a = m.ArtistGetByIdWithDetail(id.GetValueOrDefault());

            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Prepare the form
                var form = new TrackAddForm();

                // Attention 16 - Prepare the data for the add track form

                // Album name and identifier
                form.AlbumName = a.Name;
                form.AlbumId = a.Id;

                // Genre list
                form.GenreList = new SelectList(m.GenreGetAll(), dataValueField: "Name", dataTextField: "Name");

                // Attention 17 - Study the view code too

                return View(form);
            }
        }

        // POST: Albums/5/AddTrack
        [Route("albums/{id}/addtrack")]
        [Authorize(Roles = "Clerk")]
        [HttpPost]
        public ActionResult AddTrack(int? id, TrackAdd newItem)
        {
            // Validate the input
            if (!ModelState.IsValid & id.GetValueOrDefault() == newItem.AlbumId)
            {
                return RedirectToAction("details", "albums", new { id = id });
            }

            // Process the input
            var addedItem = m.TrackAdd(newItem);

            if (addedItem == null)
            {
                return RedirectToAction("details", "albums", new { id = id });
            }
            else
            {
                // Attention 19 - Must redirect to the tracks controller
                return RedirectToAction("details", "tracks", new { id = addedItem.Id });
            }
        }

    }
}

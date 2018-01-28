using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    [Authorize]
    public class ArtistsController : Controller
    {
        // Reference to the manager object
        Manager m = new Manager();

        // GET: Artists
        public ActionResult Index()
        {
            return View(m.ArtistGetAll());
        }

        // GET: Artists/Details/5
        public ActionResult Details(int? id)
        {
            // Attempt to get the matching object
            var o = m.ArtistGetByIdWithDetail(id.GetValueOrDefault());

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

        // GET: Properties/DetailsWithAudioInfo/5
        public ActionResult DetailsWithMediaInfo(int? id)
        {
            // Attempt to get the matching object
            var o = m.ArtistGetByIdWithAudioInfo(id.GetValueOrDefault());

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

        // GET: Artists/Create
        [Authorize(Roles = "Executive")]
        public ActionResult Create()
        {
            // Prepare the form
            var form = new ArtistAddForm();

            form.GenreList = new SelectList(m.GenreGetAll(), dataValueField: "Name", dataTextField: "Name");

            return View(form);
        }

        // POST: Artists/Create
        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "Executive")]
        public ActionResult Create(ArtistAdd newItem)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }

            // Process the input
            var addedItem = m.ArtistAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", new { id = addedItem.Id });
            }
        }

        // ############################################################
        // For this artist, add a new album

        // GET: Artists/5/AddAlbum
        [Route("artists/{id}/addalbum")]
        [Authorize(Roles = "Coordinator")]
        public ActionResult AddAlbum(int? id)
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
                var form = new AlbumAddForm();

                // Artist name
                form.ArtistName = a.Name;

                // Attention 12 - Prepare all the select item lists

                // Genre list
                form.GenreList = new SelectList(m.GenreGetAll(), dataValueField: "Name", dataTextField: "Name");

                // Collection of one int identifier
                // TODO fix this
                //var selectedValues = new List<int> { a.Id };

                // Artist list
                form.ArtistList = new MultiSelectList(
                    items: m.ArtistGetAll(),
                    dataValueField: "Id",
                    dataTextField: "Name",
                    selectedValues: new List<int> { a.Id });

                // Track list
                form.TrackList = new MultiSelectList(
                    items: m.TrackGetAllByArtistId(a.Id),
                    dataValueField: "Id",
                    dataTextField: "Name");

                // Attention 13 - Study the view code too

                return View(form);
            }
        }

        // POST: Artists/5/AddAlbum
        [Route("artists/{id}/addalbum")]
        [Authorize(Roles = "Coordinator")]
        [HttpPost]
        public ActionResult AddAlbum(int? id, AlbumAdd newItem)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return RedirectToAction("details", "artists", new { id = id });
            }

            // Verify that the passed-in identifier is in the new item's
            // collection of identifiers
            if (!newItem.ArtistIds.Contains(id.GetValueOrDefault()))
            {
                return RedirectToAction("details", "artists", new { id = id });
            }

            // Process the input
            var addedItem = m.AlbumAdd(newItem);

            if (addedItem == null)
            {
                return RedirectToAction("details", "artists", new { id = id });
            }
            else
            {
                // Attention 15 - Must redirect to the albums controller
                return RedirectToAction("details", "albums", new { id = addedItem.Id });
            }
        }

        // ############################################################
        //For this artist, add a media item
        // GET: Media Item
        [Route("media/{Id}/addmediaitem")]
        [Authorize(Roles = "Coordinator")]
        public ActionResult AddMediaItem(int? id)
        {
            var a = m.ArtistGetByIdWithDetail(id.GetValueOrDefault());

            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Prepare the form
                var form = new MediaItemAddForm();

                form.ArtistId = a.Id;
                form.ArtistInfo = a.Name;

                return View(form);
            }
        }



        [HttpPost]
        [Route("media/{id}/addmediaitem")]
        public ActionResult AddMediaItem(int? id, MediaItemAdd newItem)
        {
            if (!ModelState.IsValid && id.GetValueOrDefault() == newItem.ArtistId)
            {
                return View(newItem);
            }

            // Process the input
            var addedItem = m.ArtistMediaAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", new { id = addedItem.Id });
            }
        }
    }
}

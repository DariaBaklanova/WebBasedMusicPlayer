using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    [Authorize]
    public class TracksController : Controller
    {
        // Reference to the manager object
        Manager m = new Manager();

        // GET: Tracks
        public ActionResult Index()
        {
            return View(m.TrackGetAll());
        }

       // [Route("clip/{id}")]
        public ActionResult Details(int? id)
        {
            var o = m.TrackGetByIdWithDetail(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(o);
            }
        }

        // GET: Vehicles/Create
        public ActionResult Create()
        {
            var form = new TrackAddForm();

            return View(form);

 
        }

        // POST: Vehicles/Create
        [HttpPost]
        public ActionResult Create(TrackAdd newItem)
        {
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }

            // Process the input
            var addedItem = m.TrackAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", new { id = addedItem.Id });
            }
        }


        // Add new track method is actually located in the Artists controller

        // GET: Tracks/Edit/5
        [Authorize(Roles = "Coordinator")]
        public ActionResult Edit(int? id)
        {
            // Attention 21 - Track edit; study the view code too

            // Attempt to fetch the matching object
            var o = m.TrackGetByIdWithDetail(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Create and configure an "edit form"

                var editForm = m.mapper.Map<TrackEditComposers>(o);

                return View(editForm);
            }
        }

        // POST: Tracks/Edit/5
        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        public ActionResult Edit(int? id, TrackEditComposers newItem)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                // Our "version 1" approach is to display the "edit form" again
                return RedirectToAction("edit", new { id = newItem.Id });
            }

            if (id.GetValueOrDefault() != newItem.Id)
            {
                // This appears to be data tampering, so redirect the user away
                return RedirectToAction("index");
            }

            // Attempt to do the update
            var editedItem = m.TrackEdit(newItem);

            if (editedItem == null)
            {
                // There was a problem updating the object
                // Our "version 1" approach is to display the "edit form" again
                return RedirectToAction("edit", new { id = newItem.Id });
            }
            else
            {
                // Show the details view, which will have the updated data
                return RedirectToAction("details", new { id = newItem.Id });
            }
        }

        // GET: Tracks/Delete/5
        [Authorize(Roles = "Coordinator")]
        public ActionResult Delete(int? id)
        {
            // Attention 26 - Track delete; study the view code too

            var itemToDelete = m.TrackGetByIdWithDetail(id.GetValueOrDefault());

            if (itemToDelete == null)
            {
                // Don't leak info about the delete attempt
                // Simply redirect
                return RedirectToAction("index");
            }
            else
            {
                return View(itemToDelete);
            }
        }

        // POST: Tracks/Delete/5
        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        public ActionResult Delete(int? id, FormCollection collection)
        {
            var result = m.TrackDelete(id.GetValueOrDefault());

            // "result" will be true or false
            // We probably won't do much with the result, because 
            // we don't want to leak info about the delete attempt

            // In the end, we should just redirect to the list view
            return RedirectToAction("index");
        }

    }

}

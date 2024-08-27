using full_Project.Models.Domain;
using full_Project.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace full_Project.Controllers
{
    [Authorize]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // GET: Genre/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Genre/Add
        [HttpPost]
        [ValidateAntiForgeryToken]  // Add AntiForgeryToken to prevent CSRF attacks
        public IActionResult Add(Genre model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = _genreService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added successfully";
                return RedirectToAction(nameof(GenreList));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }

        // GET: Genre/Edit/{id}
        public IActionResult Edit(int id)
        {
            var data = _genreService.GetById(id);
            if (data == null)
            {
                TempData["msg"] = "Genre not found";
                return RedirectToAction(nameof(GenreList));
            }

            return View(data);
        }

        // POST: Genre/Update
        [HttpPost]
        [ValidateAntiForgeryToken]  // Add AntiForgeryToken to prevent CSRF attacks
        public IActionResult Update(Genre model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = _genreService.Update(model);
            if (result)
            {
                TempData["msg"] = "Updated successfully";
                return RedirectToAction(nameof(GenreList));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }

        // GET: Genre/GenreList
        public IActionResult GenreList()
        {
            var data = _genreService.List().ToList();
            return View(data);
        }

        // POST: Genre/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["msg"] = "Invalid genre ID";
                return RedirectToAction(nameof(GenreList));
            }

            var result = _genreService.Delete(id);
            if (result)
            {
                TempData["msg"] = "Deleted successfully";
            }
            else
            {
                TempData["msg"] = "Error on server side";
            }
            return RedirectToAction(nameof(GenreList));
        }
    }
}

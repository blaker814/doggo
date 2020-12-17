using DogGo.Models;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class DogsController : Controller
    {
        private readonly IDogRepository _dogRepo;

        // ASP.NET will give us an instance of our Dog Repository. This is called "Dependency Injection"
        public DogsController(IDogRepository dogRepository)
        {
            _dogRepo = dogRepository;
        }

        // GET: DogsController
        [Authorize]
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);
            return View(dogs);
        }

        // GET: DogController/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // GET: DogsController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
            try
            {
                dog.OwnerId = GetCurrentUserId();

                _dogRepo.AddDog(dog);

                return RedirectToAction(nameof(Index), "Owners");
            }
            catch
            {
                return View(dog);
            }
        }

        // GET: DogsController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            if (dog == null || dog.OwnerId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: DogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            try
            {
                dog.OwnerId = GetCurrentUserId();

                _dogRepo.UpdateDog(dog);

                return RedirectToAction(nameof(Index), "Owners");
            }
            catch
            {
                return View(dog);
            }
        }

        // GET: DogsController/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            if (dog == null || dog.OwnerId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: DogsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            try
            {
                _dogRepo.DeleteDog(id);

                return RedirectToAction(nameof(Index), "Owners");
            }
            catch
            {
                return View(dog);
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}

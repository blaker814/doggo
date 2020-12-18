using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class WalksController : DoggoControllerBase
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalkRepository _walkRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly IDogRepository _dogRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalksController(IWalkerRepository walkerRepository, IWalkRepository walkRepository, IOwnerRepository ownerRepository, IDogRepository dogRepository)
        {
            _walkerRepo = walkerRepository;
            _walkRepo = walkRepository;
            _ownerRepo = ownerRepository;
            _dogRepo = dogRepository;
        }

        // GET: Walks
        public ActionResult Index()
        {
            return View();
        }

        // GET: Walks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Walks/Create
        public ActionResult Create(int walkerId)
        {
            Walker walker = _walkerRepo.GetWalkerById(walkerId);
            RequestWalkViewModel viewModel = new RequestWalkViewModel()
            {
                Walk = new Walk()
                {
                    Date = DateTime.Now,
                    WalkerId = walker.Id,
                    WalkStatusId = 1
                },
                Walker = walker,
                Dogs = _dogRepo.GetDogsByOwnerId(GetCurrentUserId())
            };

            return View(viewModel);
        }

        // POST: Walks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RequestWalkViewModel viewModel)
        {
            try
            {
                _walkRepo.AddWalk(viewModel.Walk);

                return RedirectToAction(nameof(Index), "Owners");
            }
            catch
            {
                RequestWalkViewModel sameView = viewModel;
                viewModel.Dogs = _dogRepo.GetDogsByOwnerId(GetCurrentUserId());

                return View(sameView);
            }
        }

        // GET: Walks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Walks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Walks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Walks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

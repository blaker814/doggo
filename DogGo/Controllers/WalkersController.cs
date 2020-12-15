using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalkRepository _walkRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly IDogRepository _dogRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository, IWalkRepository walkRepository, IOwnerRepository ownerRepository, IDogRepository dogRepository)
        {
            _walkerRepo = walkerRepository;
            _walkRepo = walkRepository;
            _ownerRepo = ownerRepository;
            _dogRepo = dogRepository;
        }

        // GET: Walkers
        public ActionResult Index()
        {
            if (GetCurrentUserId() != -1)
            {
                int neighborhoodId = _ownerRepo.GetOwnerById(GetCurrentUserId()).NeighborhoodId;
                List<Walker> walkersInNeighborhood = _walkerRepo.GetWalkersInNeighborhood(neighborhoodId);

                return View(walkersInNeighborhood);
            }
            else
            {
                List<Walker> walkers = _walkerRepo.GetAllWalkers();

                return View(walkers);
            }

        }

        // GET: Walkers/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);

            if (walker == null)
            {
                return NotFound();
            }

            List<Walk> walks = _walkRepo.GetWalksByWalkerId(walker.Id);
            List<Owner> owners = _ownerRepo.GetAllOwners();
            List<Owner> ownersWithDogs = new List<Owner>();
            foreach (Owner owner in owners)
            {
                ownersWithDogs.Add(new Owner
                {
                    Name = owner.Name,
                    Dogs = _dogRepo.GetDogsByOwnerId(owner.Id)
                });
            };

            int secs = walks.Sum(walk => walk.Duration);
            TimeSpan time = TimeSpan.FromSeconds(secs);

            WalkerProfileViewModel vm = new WalkerProfileViewModel
            {
                Walker = walker,
                Walks = walks,
                Owners = ownersWithDogs,
                WalkTime = time.ToString(@"hh\:mm")
            };

            return View(vm);
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Walker walker)
        {
            try
            {
                _walkerRepo.AddWalker(walker);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);

            if (walker == null)
            {
                return NotFound();
            }

            return View(walker);
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Walker walker)
        {
            try
            {
                _walkerRepo.UpdateWalker(walker);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);

            return View(walker);
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Walker walker)
        {
            try
            {
                _walkerRepo.DeleteWalker(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(walker);
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return id != null ? int.Parse(id) : -1;
        }
    }
}

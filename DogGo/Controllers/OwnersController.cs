﻿using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public class OwnersController : DoggoControllerBase
    {
        private IOwnerRepository _ownerRepo;
        private IDogRepository _dogRepo;
        private IWalkerRepository _walkerRepo;
        private INeighborhoodRepository _neighborhoodRepo;
        private IWalkRepository _walkRepo;

        public OwnersController(IOwnerRepository ownerRepo, IDogRepository dogRepo, IWalkerRepository walkerRepo, INeighborhoodRepository neighborhoodRepo, IWalkRepository walkRepo)
        {
            _ownerRepo = ownerRepo;
            _dogRepo = dogRepo;
            _walkerRepo = walkerRepo;
            _neighborhoodRepo = neighborhoodRepo;
            _walkRepo = walkRepo;
        }

        [Authorize]
        // GET: OwnersController
        public ActionResult Index()
        {
            int currentUserId = GetCurrentUserId();

            return RedirectToAction("Details", new { id = currentUserId });
        }

        // GET: OwnersController/Details/5
        public ActionResult Details(int id)
        { 
            Owner owner = _ownerRepo.GetOwnerById(id);
            
            if (owner == null)
            {
                return NotFound();
            }

            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(owner.Id);
            List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);
            List<Walk> walks = _walkRepo.GetUpcomingWalksByOwnerId(owner.Id);

            ProfileViewModel vm = new ProfileViewModel()
            {
                Owner = owner,
                Dogs = dogs,
                Walkers = walkers,
                UpcomingWalks = walks
            };

            return View(vm);           
        }

        // GET: OwnersController/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();

            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };

            return View(vm);
        }

        // POST: OwnersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OwnerFormViewModel ownerFormModel)
        {
            try
            {
                _ownerRepo.AddOwner(ownerFormModel.Owner);

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, ownerFormModel.Owner.Id.ToString()),
                    new Claim(ClaimTypes.Email, ownerFormModel.Owner.Email),
                    new Claim(ClaimTypes.Role, "DogOwner"),
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction(nameof(Details), new { id = ownerFormModel.Owner.Id });
            }
            catch
            {
                ownerFormModel.ErrorMessage = "Whoops! Something went wrong while saving this owner";
                ownerFormModel.Neighborhoods = _neighborhoodRepo.GetAll();

                return View(ownerFormModel);
            }
        }

        // GET: OwnersController/Edit/5
        public ActionResult Edit(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);

            if (owner == null)
            {
                return NotFound();
            }

            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();

            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = owner,
                Neighborhoods = neighborhoods
            };

            return View(vm);
        }

        // POST: OwnersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, OwnerFormViewModel ownerFormModel)
        {
            try
            {
                _ownerRepo.UpdateOwner(ownerFormModel.Owner);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ownerFormModel.Neighborhoods = _neighborhoodRepo.GetAll();
                return View(ownerFormModel);
            }
        }

        // GET: OwnersController/Delete/5
        public ActionResult Delete(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);

            return View(owner);
        }

        // POST: OwnersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Owner owner)
        {
            try
            {
                _ownerRepo.DeleteOwner(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(owner);
            }

        }
    }
}

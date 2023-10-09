using log4net;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetCoreMVCApp.Models.Constants;
using DotNetCoreMVCApp.Models.Repository;
using DotNetCoreMVCApp.Models.Web;
using DotNetCoreMVCApp.Repository.Implementation;
using DotNetCoreMVCApp.Service.Abstraction;
using DotNetCoreMVCApp.Web.Models;

namespace DotNetCoreMVCApp.Web.Controllers
{
    public class CountryController : BaseController
    {
        private readonly ILog _logger;
        private readonly ICountryService _countryService;
        private const string EntityName = "Country";
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
            _logger = LogManager.GetLogger(typeof(CountryController));
        }

        public async Task<IActionResult> Index()
        {
            var countries = await _countryService.GetAllAsync();
            return View(countries);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryViewModel country)
        {
            if (ModelState.IsValid)
            {
                var errorStateModel = await _countryService.ValidateAsync(country);
                if (errorStateModel.IsValid)
                {
                    await _countryService.CreateAsync(country, GetCurrentUserId());
                    TempData["NotificationMessage"] = $"{Messages.NotificationSuccess}#{string.Format(Messages.CreateSuccess, EntityName)}";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in errorStateModel.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Value);
                    }
                }
            }
            TempData["NotificationMessage"] = $"{Messages.NotificationError}#{Messages.ValidationError}"; ;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var country = await _countryService.GetByIdAsync(id.Value);
            if (country == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CountryViewModel country)
        {
            if (id != country.Id)
            {
                return RedirectToAction("Error", "Home");
            }

            if (ModelState.IsValid)
            {
                var errorStateModel = await _countryService.ValidateAsync(country);
                if (errorStateModel.IsValid)
                {
                    await _countryService.UpdateAsync(country, GetCurrentUserId());
                    TempData["NotificationMessage"] = $"{Messages.NotificationSuccess}#{string.Format(Messages.UpdateSuccess, EntityName)}";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in errorStateModel.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Value);
                    }
                }
            }
            TempData["NotificationMessage"] = $"{Messages.NotificationError}#{Messages.ValidationError}";
            return View(country);
        }

        [HttpPost]
        public async Task Delete(int id)
        {
            await _countryService.DeleteAsync(id, GetCurrentUserId());
            TempData["NotificationMessage"] = $"{Messages.NotificationSuccess}#{string.Format(Messages.DeleteSuccess, EntityName)}";
        }
    }
}
/*
Copyright © 2020 Annice Strömberg – Annice.se

This script is MIT (Massachusetts Institute of Technology) licensed, which means that
permission is granted, free of charge, to any person obtaining a copy of this software
and associated documentation files to deal in the software without restriction. This
includes, without limitation, the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the software, and to permit persons to whom the software
is furnished to do so subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the software.
*/
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VisitorStatistics.Models;
using Microsoft.AspNetCore.Http;

namespace VisitorStatistics.Controllers
{
    /// <summary>
    /// The home controller is the interaction layer between the view and the model to handle the public
    /// pages of this application.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly PasswordHasher<VsAdmin> hasher = new PasswordHasher<VsAdmin>();
        private readonly VisitorStatisticsContext _db;

        public HomeController(VisitorStatisticsContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get the public application start page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index() => View();

        /// <summary>
        /// Render the login page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserID") != null)
                return RedirectToAction("Settings", "Admin");
            else
                return View();
        }

        /// <summary>
        /// Handle the post request when the admin attempts to login.
        /// </summary>
        /// <param name="input">Login credentials.</param>
        /// <returns>Valid or invalid login.</returns>
        [HttpPost]
        public IActionResult Login(IFormCollection input)
        {
            string errorFeedback = "Invalid login!";

            try
            {
                VsAdmin existingUser = _db.VsAdmins.Where(a => a.Email.Equals(input["email"])).Select(a => a).First();

                if (ValidLogin(existingUser, input["password"]))
                {
                    HttpContext.Session.SetString("UserID", existingUser.Id.ToString());
                    TempData["UserID"] = HttpContext.Session.GetString("UserID");

                    // Get IP address for logged in admin user to be used later for admin visit storage:
                    HttpContext.Session.SetString("AdminIP", HttpContext.Connection.RemoteIpAddress.ToString());

                    return RedirectToAction("Settings", "Admin");
                }
                else
                    ViewBag.Error = errorFeedback;
            }
            catch(Exception e)
            {
                ViewBag.Error = errorFeedback;
                Console.Write(e.Message);
            }
            return View(input);
        }

        /// <summary>
        /// Utilize the ASP.NET Core Identity function to verify a user password.
        /// </summary>
        /// <param name="item">VsAdmins object.</param>
        /// <param name="passwordInput">User password input in plain text.</param>
        /// <returns>Valid or invalid login.</returns>
        public bool ValidLogin(VsAdmin item, string passwordInput)
        {
            var result = hasher.VerifyHashedPassword(item, item.Password, passwordInput);

            if (result == PasswordVerificationResult.Success)
                return true;

            return false;
        }

    } // End controller.
} // End namespace.

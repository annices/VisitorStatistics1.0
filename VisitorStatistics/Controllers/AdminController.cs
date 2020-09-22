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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReflectionIT.Mvc.Paging;
using VisitorStatistics.Models;
using VisitorStatistics.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace VisitorStatistics.Controllers
{
    /// <summary>
    /// The admin controller is the interaction layer between the view and the model to handle the
    /// admin settings, application settings and visit settings.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly PasswordHasher<VsAdmin> hasher = new PasswordHasher<VsAdmin>();
        private readonly IWritableOptions<AppSettings> _appSettings;
        private readonly VisitorStatisticsContext _db;
        private readonly IConfiguration _config;

        /// <summary>
        /// Inject neccessary dependencies.
        /// </summary>
        /// <param name="db"></param>
        public AdminController(VisitorStatisticsContext db, IWritableOptions<AppSettings> appSettings, IConfiguration config)
        {
            _db = db;
            _appSettings = appSettings;
            _config = config;
        }

        /// <summary>
        /// Prepare the admin start page with the data we want to use once the page is loaded.
        /// <returns>The admin start page.</returns>
        [HttpGet]
        public IActionResult Settings()
        {
            // Check valid login:
            if (HttpContext.Session.GetString("UserID") != null)
            {
                try
                {
                    // Catch server side validation messages if any:
                    if (TempData["errors"] != null)
                        ViewBag.ErrorMessages = TempData["errors"];

                    VsApplication app = _db.VsApplications.Select(a => a).First();
                    VsAdmin admin = _db.VsAdmins.Select(a => a).First();
                    List<string> urls = _db.VsAppUrls.Select(u => u.RegisteredUrl).ToList();
                    List<string> ignoredIPs = _db.VsVisitors.Where(i => i.IsIgnored.Equals(true)).Select(i => i.Ipaddress).ToList();
                    int deletionDays = Convert.ToInt32(_config["IpDeletionDays:Days"]);

                    Config settings = new Config
                    {
                        AdminID = admin.Id,
                        Firstname = admin.Firstname,
                        Lastname = admin.Lastname,
                        Email = admin.Email,
                        AppID = app.Id,
                        ApplicationName = app.Name,
                        UrlList = urls,
                        IPList = ignoredIPs,
                        DaysBeforeDeletion = deletionDays < 1 ? 1 : deletionDays
                    };

                    return View(settings);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Get all visits that are currently not ignored and return them to
        /// the view with applied pagination function.
        /// </summary>
        /// <param name="sortOrder">Sort by date.</param>
        /// <param name="page">Pagination page number.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult VisitLogs(string sortOrder, int page = 1)
        {
            // Check valid login:
            if (HttpContext.Session.GetString("UserID") != null)
            {
                // Prepare view variable to sort table columns by visit time:
                TempData["VisitTime"] = sortOrder == "VisitTime" ? "visit_time_desc" : "VisitTime";

                try
                {
                    // Create a left join query of related entities to get wanted values for the visit log rows:
                    var visitList = from visitor in _db.VsVisitors
                                    join visit in _db.VsVisits on visitor.Id equals visit.VisitorId into visits
                                    from visitsResult in visits.DefaultIfEmpty()
                                    join adminVisit in _db.VsAdminVisits on visitsResult.Id equals adminVisit.VisitId into adminVisits
                                    from adminVisitResult in adminVisits.DefaultIfEmpty()
                                    join admin in _db.VsAdmins on adminVisitResult.AdminId equals admin.Id into admins
                                    from adminResult in admins.DefaultIfEmpty()
                                    where visitor.IsIgnored != true
                                    select new VisitorStats()
                                    {
                                        VisitID = visitsResult.Id,
                                        IPAddress = visitor.Ipaddress,
                                        VisitTime = visitsResult.VisitTime,
                                        VisitURL = visitsResult.VisitUrl,
                                        RefererURL = visitsResult.RefererUrl,
                                        AdminID = adminVisitResult.AdminId,
                                        AdminFirstname = adminResult.Firstname,
                                        AdminEmail = adminResult.Email
                                    };

                    visitList = sortOrder switch
                    {
                        "visit_time_desc" => visitList.OrderByDescending(v => v.VisitTime),
                        "VisitTime" => visitList.OrderBy(v => v.VisitTime),
                        _ => visitList.OrderByDescending(v => v.VisitTime)
                    };

                    var model = PagingList.Create(visitList, Convert.ToInt32(_config["Paging:ItemsPerPage"]), page);
                    model.Action = nameof(VisitLogs);

                    return View(model);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Get details about a specific visit, such as IP, visit time, host, agent, etc.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Visit details.</returns>
        [HttpGet]
        public IActionResult VisitDetails(int? id)
        {
            // Check valid login:
            if (HttpContext.Session.GetString("UserID") != null)
            {
                try
                {
                    VisitorStats visit = _db.VsVisitors
                                         .Join(_db.VsVisits,
                                         v1 => v1.Id,
                                         v2 => v2.VisitorId,
                                         (v1, v2) => new VisitorStats()
                                         {
                                             VisitID = v2.Id,
                                             IPAddress = v1.Ipaddress,
                                             VisitTime = v2.VisitTime,
                                             VisitURL = v2.VisitUrl,
                                             RefererURL = v2.RefererUrl,
                                             HostName = v1.HostName,
                                             Agent = v1.Agent
                                         })
                                         .Where(v => v.VisitID.Equals(id))
                                         .FirstOrDefault();

                    return View(visit);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }

                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Receive posted data from view passed via Ajax request to update the admin settings.
        /// </summary>
        /// <param name="input">Admin settings.</param>
        /// <returns>Json result on success to enable the use of Ajax, otherwise a redirection to admin start.</returns>
        [HttpPost]
        public IActionResult AdminSettings(Config input)
        {
            if (ModelState.IsValid && HttpContext.Session.GetString("UserID") != null)
            {
                try
                {
                    VsAdmin user = _db.VsAdmins.Find(input.AdminID);
                    string userPassword = !string.IsNullOrEmpty(input.Password) ? hasher.HashPassword(user, input.Password) : user.Password;

                    user.Id = input.AdminID;
                    user.Firstname = input.Firstname;
                    user.Lastname = input.Lastname;
                    user.Email = input.Email;
                    user.Password = userPassword;

                    _db.VsAdmins.Update(user);
                    _db.SaveChanges();

                    // Return Json to support Ajax submits:
                    return new JsonResult(input);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }

            // Get server side feedback:
            TempData["errors"] = ErrorFeedback();
            return RedirectToAction(nameof(Settings));
        }

        /// <summary>
        /// Receive posted data from view passed via Ajax request to update the application settings.
        /// </summary>
        /// <param name="input">Application settings.</param>
        /// <returns>Json result on success to enable the use of Ajax, otherwise a redirection to admin start.</returns>
        [HttpPost]
        public IActionResult AppSettings(Config input)
        {
            if (ModelState.IsValid && HttpContext.Session.GetString("UserID") != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(input.ApplicationURL))
                    {
                        string[] inputUrls = input.ApplicationURL.Split(",");

                        // Easy way to ensure DB URLs always reflect input URLs is to first clear old ones:
                        if (_db.VsAppUrls.Any())
                            _db.VsAppUrls.RemoveRange(_db.VsAppUrls);

                        // Then insert the new ones based on posted input:
                        foreach (string link in inputUrls)
                        {
                            VsAppUrl item = new VsAppUrl()
                            {
                                AppId = input.AppID,
                                RegisteredUrl = link
                            };

                            _db.VsAppUrls.Add(item);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (_db.VsAppUrls.Any())
                            _db.VsAppUrls.RemoveRange(_db.VsAppUrls);

                        _db.SaveChanges();
                    }

                    VsApplication app = new VsApplication
                    {
                        Id = input.AppID,
                        Name = input.ApplicationName
                    };

                    _db.VsApplications.Update(app);
                    _db.SaveChanges();

                    return new JsonResult(input);
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                }
            }

            TempData["errors"] = ErrorFeedback();
            return RedirectToAction(nameof(Settings));
        }

        /// <summary>
        /// Receive posted data from view passed via Ajax request to update the visit settings.
        /// </summary>
        /// <param name="input">Visit settings.</param>
        /// <returns>Json result on success to enable the use of Ajax, otherwise a redirection to admin start.</returns>
        [HttpPost]
        public IActionResult VisitSettings(Config input)
        {
            // Write input deletion days to appsettings.json file:
            _appSettings.Update(options => { options.Days = input.DaysBeforeDeletion.ToString(); });

            if (ModelState.IsValid && HttpContext.Session.GetString("UserID") != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(input.IPAddress))
                    {
                        string[] inputIPs = input.IPAddress.Split(",");

                        // Save visit settings to DB, IP by IP...
                        foreach (string ip in inputIPs)
                        {
                            VsVisitor item = new VsVisitor();

                            // If the IP already exists, we'll do an update. Otherwise a new insert:
                            if (_db.VsVisitors.Where(i => i.Ipaddress.Equals(ip)).Any())
                            {
                                item = _db.VsVisitors.Where(i => i.Ipaddress.Equals(ip)).Select(i => i).FirstOrDefault();
                                item.Ipaddress = ip;
                                item.IsIgnored = true;
                                _db.VsVisitors.Update(item);
                                _db.SaveChanges();
                            }
                            else
                            {
                                // Calculate the deletion date to be set for new IPs:
                                DateTime deletionDate = DateTime.Now.AddDays(input.DaysBeforeDeletion);

                                item = new VsVisitor()
                                {
                                    AppId = input.AppID,
                                    Ipaddress = ip,
                                    IsIgnored = true,
                                    DeleteDate = deletionDate
                                };
                                _db.VsVisitors.Add(item);
                                _db.SaveChanges();
                            }
                        }

                        // Ensure that remaining IPs stored in DB (not included in the post request) are not ignored:
                        string[] theRestTrue = _db.VsVisitors.Where(i => i.IsIgnored.Equals(true)).Select(i => i.Ipaddress).ToArray();
                        string[] theRest = theRestTrue.Select(i => i).Except(inputIPs).ToArray();

                        if (theRest.Any())
                        {
                            foreach (string ip in theRest)
                            {
                                VsVisitor item = _db.VsVisitors.Where(i => i.Ipaddress.Equals(ip)).Select(i => i).FirstOrDefault();
                                item.IsIgnored = false;

                                _db.VsVisitors.Update(item);
                                _db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        // If no IPs were posted, it means we want to ensure ALL IPs stored in DB is not ignored:
                        string[] theRestTrue = _db.VsVisitors.Where(i => i.IsIgnored.Equals(true)).Select(i => i.Ipaddress).ToArray();

                        if (theRestTrue.Any())
                        {
                            foreach (string ip in theRestTrue)
                            {
                                VsVisitor item = _db.VsVisitors.Where(i => i.Ipaddress.Equals(ip)).Select(i => i).FirstOrDefault();
                                item.IsIgnored = false;

                                _db.VsVisitors.Update(item);
                                _db.SaveChanges();
                            }
                        }
                    }
                    return new JsonResult(input);
                }
                catch (Exception e)
                {
                    String innerMessage = (e.InnerException != null) ? e.InnerException.Message : "";
                    Console.Write(innerMessage);
                }
            }

            TempData["errors"] = ErrorFeedback();
            return RedirectToAction(nameof(Settings));
        }

        /// <summary>
        /// Calculate and return an overview of the visitor statistics to the end user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Statistics()
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                try
                {
                    // Prepare range dates for visits this week calculation:
                    DateTime dateThisMonday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime dateThisSunday = Convert.ToDateTime(dateThisMonday).AddDays(6);

                    int visitsToday = _db.VsVisits.Where(v => v.VisitTime.Date == DateTime.Now.Date).Count();
                    int visitsThisWeek = _db.VsVisits.Where(v => v.VisitTime >= dateThisMonday && v.VisitTime <= dateThisSunday).Count();
                    int visitsThisMonth = _db.VsVisits.Where(v => v.VisitTime.Month == DateTime.Now.Month).Count();
                    int visitsThisYear = _db.VsVisits.Where(v => v.VisitTime.Year.Equals(DateTime.Now.Year)).Count();
                    int totalVisits = _db.VsVisits.Select(v => v.VisitTime).Count();

                    Statistics item = new Statistics()
                    {
                        VisitsToday = visitsToday,
                        VisitsThisWeek = visitsThisWeek,
                        VisitsThisMonth = visitsThisMonth,
                        VisitsThisYear = visitsThisYear,
                        TotalVisits = totalVisits
                    };

                    return View(item);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Clear all set sessions to logout the admin.
        /// </summary>
        /// <returns>Logged out admin user.</returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Remove("UserID");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Collect potential model state errors returned in a string list to work with TempData.
        /// This can be used to handle the server side validation in case JavaScript would be disabled.
        /// </summary>
        /// <returns>Model state error messages.</returns>
        public List<string> ErrorFeedback()
        {
            string messages = string.Join(";", ModelState.Values
                              .SelectMany(x => x.Errors)
                              .Select(x => x.ErrorMessage));

            List<string> messageList = messages.Split(";").ToList();

            return messageList;
        }

    } // End controller.
} // End namespace.

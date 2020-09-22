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
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using VisitorStatistics.Models;
using VisitorStatistics.Services.Abstraction;

namespace VisitorStatistics.Services
{
    /// <summary>
    /// This class implements the IVisitorHandler interface to work as a service class to interact with the
    /// middleware and DB, as well as to interact with the timer service and DB regarding the page visitors.
    /// </summary>
    public class VisitorHandler : IVisitorHandler
    {
        private readonly VisitorStatisticsContext _db;
        private readonly IHttpContextAccessor _context;

        public VisitorHandler(VisitorStatisticsContext db, IHttpContextAccessor context)
        {
            _db = db;
            _context = context;
        }

        /// <summary>
        /// Method to check if there is any registered application URLs in DB. Since there's only one
        /// app stored for this script version, we can fetch all URLs without filtering by app.
        /// </summary>
        /// <returns></returns>
        public bool IsRegisteredUrl()
        {
            bool isRegisteredUrl = false;

            try
            {
                isRegisteredUrl = _db.VsAppUrls.Any() ? true : false;
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }

            return isRegisteredUrl;
        }

        /// <summary>
        /// Get the registered application URLs.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAppUrls(out int appID)
        {
            List<string> urls = new List<string>();
            int id = 0;

            try
            {
                id = _db.VsApplications.Select(a => a.Id).First(); // Select first since we know we only have one app.
                urls = _db.VsAppUrls.Select(u => u.RegisteredUrl).ToList();
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }

            appID = id;
            return urls;
        }

        /// <summary>
        /// Save the visitor statistics passed from the middleware to the DB.
        /// </summary>
        /// <param name="input"></param>
        public void SaveVisitorStats(VisitorStats input)
        {
            try
            {
                VsVisitor visitor = new VsVisitor()
                {
                    Ipaddress = input.IPAddress,
                    HostName = input.HostName,
                    Agent = input.Agent,
                    DeleteDate = input.DeletionDate,
                    AppId = input.AppID
                };

                // Check if visitor (IP) is to be inserted or updated to DB:
                if (!_db.VsVisitors.Where(i => i.Ipaddress.Equals(input.IPAddress)).Any())
                {
                    _db.VsVisitors.Add(visitor);
                    _db.SaveChanges();
                }
                else
                {
                    visitor = _db.VsVisitors.Where(v => v.Ipaddress.Equals(input.IPAddress)).Select(v => v).FirstOrDefault();
                    visitor.HostName = input.HostName;
                    visitor.Agent = input.Agent;

                    _db.VsVisitors.Update(visitor);
                    _db.SaveChanges();
                }

                // Now ensure the visitor ID (to be used as reference key for visit entity) exists in DB:
                int visitorID = _db.VsVisitors.Where(i => i.Ipaddress.Equals(input.IPAddress)).Select(v => v.Id).First();

                if(visitorID > 0)
                {
                    VsVisit visit = new VsVisit
                    {
                        VisitorId = visitorID,
                        RefererUrl = input.RefererURL,
                        VisitUrl = input.VisitURL,
                        VisitTime = input.VisitTime
                    };

                    _db.VsVisits.Add(visit);
                    _db.SaveChanges();

                    // Handle storage of admin visit:
                    if(_context.HttpContext.Session.GetString("AdminIP") != null)
                    {
                        if(_context.HttpContext.Session.GetString("AdminIP").Equals(input.IPAddress))
                        {
                            VsAdminVisit adminVisit = new VsAdminVisit()
                            {
                                // If admin IP session is set, then so is the user ID session (via HomeController > Login):
                                AdminId = Convert.ToInt32(_context.HttpContext.Session.GetString("UserID")),
                                VisitId = visit.Id
                            };

                            _db.VsAdminVisits.Add(adminVisit);
                            _db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        /// <summary>
        /// This method is called by the DeleteVisitorsTimer to delete all visitors and the
        /// related data from DB based on their deletion dates.
        /// </summary>
        public void DeleteVisitors()
        {
            DateTime currentDate = DateTime.Now;

            try
            {
                bool isDateMatch = _db.VsVisitors.Any(v => v.DeleteDate == currentDate || v.DeleteDate < currentDate);

                if (isDateMatch)
                {
                    _db.VsVisitors.RemoveRange(_db.VsVisitors.Where(v => v.DeleteDate == currentDate || v.DeleteDate < currentDate));
                    _db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
        }

    } // End class.
} // End namespace.

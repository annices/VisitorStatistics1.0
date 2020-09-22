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
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using VisitorStatistics.Models;
using System.Threading.Tasks;
using VisitorStatistics.Services.Abstraction;
using Microsoft.Extensions.Configuration;

namespace VisitorStatistics.Middleware
{
    /// <summary>
    /// This class handles the middleware to register visitor statistics based on registered application URLs.
    /// </summary>
    public class VisitorStatisticsServer
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;        

        /// <summary>
        /// Inject necessary dependencies.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="environment"></param>
        /// <param name="config"></param>
        public VisitorStatisticsServer(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        /// <summary>
        /// This method is called on every new HTTP request to check whether the requested URL matches
        /// a registered application URL. If so, visitor statistics will be saved for this URL.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, [FromServices] IVisitorHandler handler)
        {
            string requestedWith = context.Request.Headers["X-Requested-With"];
            string ajaxRequest = "XMLHttpRequest";

            if (handler.IsRegisteredUrl() && requestedWith != ajaxRequest)
            {
                string deletionDays = Convert.ToInt32(_config["IpDeletionDays:Days"]) < 1 ? "1" : _config["IpDeletionDays:Days"];
                DateTime deletionDate = DateTime.Now.AddDays(Convert.ToDouble(deletionDays));

                string visitorIP = context.Connection.RemoteIpAddress.ToString();
                var hostEntry = Dns.GetHostEntry(visitorIP);
                string hostName = hostEntry.HostName;

                foreach (string url in handler.GetAppUrls(out int appID))
                {
                    if (context.Request.GetDisplayUrl().Equals(url))
                    {
                        VisitorStats item = new VisitorStats
                        {
                            VisitTime = DateTime.Now,
                            VisitURL = context.Request.GetDisplayUrl(),
                            RefererURL = context.Request.Headers["Referer"].ToString(),
                            IPAddress = visitorIP,
                            HostName = hostName,
                            Agent = context.Request.Headers["User-Agent"].ToString(),
                            DeletionDate = deletionDate,
                            AppID = appID
                        };

                        handler.SaveVisitorStats(item);
                    }
                }
            }

            // Call the next middleware delegate in the pipeline:
            await _next.Invoke(context);
        }

    } // End class.
} // End namespace.

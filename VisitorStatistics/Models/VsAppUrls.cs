using System;
using System.Collections.Generic;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This model class reflects the DB table "VS_AppUrls". Changes made in this class
    /// must reflect the corresponding DB table and VisitorStatisticsContext class!
    /// </summary>
    public partial class VsAppUrl
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public string RegisteredUrl { get; set; }

        public virtual VsApplication App { get; set; }
    }
}

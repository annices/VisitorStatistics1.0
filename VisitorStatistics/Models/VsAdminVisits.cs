using System;
using System.Collections.Generic;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This model class reflects the DB table "VS_AdminVisits". Changes made in this class
    /// must reflect the corresponding DB table and VisitorStatisticsContext class!
    /// </summary>
    public partial class VsAdminVisit
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public int VisitId { get; set; }

        public virtual VsAdmin Admin { get; set; }
        public virtual VsVisit Visit { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This model class reflects the DB table "VS_Visits". Changes made in this class
    /// must reflect the corresponding DB table and VisitorStatisticsContext class!
    /// </summary>
    public partial class VsVisit
    {
        public VsVisit()
        {
            VsAdminVisits = new HashSet<VsAdminVisit>();
        }

        public int Id { get; set; }
        public int VisitorId { get; set; }
        public string RefererUrl { get; set; }
        public string VisitUrl { get; set; }
        public DateTime VisitTime { get; set; }

        public virtual VsVisitor Visitor { get; set; }
        public virtual ICollection<VsAdminVisit> VsAdminVisits { get; set; }
    }
}

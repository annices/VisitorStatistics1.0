using System;
using System.Collections.Generic;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This model class reflects the DB table "VS_Visitors". Changes made in this class
    /// must reflect the corresponding DB table and VisitorStatisticsContext class!
    /// </summary>
    public partial class VsVisitor
    {
        public VsVisitor()
        {
            VsVisits = new HashSet<VsVisit>();
        }

        public int Id { get; set; }
        public string Ipaddress { get; set; }
        public string HostName { get; set; }
        public string Agent { get; set; }
        public bool IsIgnored { get; set; }
        public DateTime DeleteDate { get; set; }
        public int AppId { get; set; }

        public virtual VsApplication App { get; set; }
        public virtual ICollection<VsVisit> VsVisits { get; set; }
    }
}

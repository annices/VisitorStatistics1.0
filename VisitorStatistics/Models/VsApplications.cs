using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This model class reflects the DB table "VS_Applications". Changes made in this class
    /// must reflect the corresponding DB table and VisitorStatisticsContext class!
    /// </summary>
    public partial class VsApplication
    {
        public VsApplication()
        {
            VsAppUrls = new HashSet<VsAppUrl>();
            VsVisitors = new HashSet<VsVisitor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<VsAppUrl> VsAppUrls { get; set; }
        public virtual ICollection<VsVisitor> VsVisitors { get; set; }
    }
}

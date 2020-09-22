using System.Collections.Generic;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This model class reflects the DB table "VS_Admins". Changes made in this class
    /// must reflect the corresponding DB table and VisitorStatisticsContext class!
    /// </summary>
    public partial class VsAdmin
    {
        public VsAdmin()
        {
            VsAdminVisits = new HashSet<VsAdminVisit>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<VsAdminVisit> VsAdminVisits { get; set; }
    }
}

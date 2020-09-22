using System;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This entity class is used to pass combined model properties between the middleware, view and the service layer.
    /// </summary>
    public class VisitorStats
    {
        public int VisitID { get; set; }
        public DateTime VisitTime { get; set; }
        public string VisitURL { get; set; }
        public string RefererURL { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set; }
        public string Agent { get; set; }
        public DateTime DeletionDate { get; set; }
        public int AppID { get; set; }
        public int AdminID { get; set; }
        public string AdminFirstname { get; set; }
        public string AdminEmail { get; set; }
    }
}

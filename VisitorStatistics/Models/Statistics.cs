
namespace VisitorStatistics.Models
{
    /// <summary>
    /// This entity class is used to pass a statistic overview of the visits to the view layer.
    /// </summary>
    public class Statistics
    {
        public int VisitsToday { get; set; }
        public int VisitsThisWeek { get; set; }
        public int VisitsThisMonth { get; set; }
        public int VisitsThisYear { get; set; }
        public int TotalVisits { get; set; }
    }
}

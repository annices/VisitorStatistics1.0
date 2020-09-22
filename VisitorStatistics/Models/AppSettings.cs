
namespace VisitorStatistics.Models
{
    /// <summary>
    /// This entity class specifies the appsettings.json property we want to be able to update dynamically
    /// from the application page section "VisitSettings". It is used with the IWriteableOptions interface
    /// and WriteableOptions class.
    /// </summary>
    public class AppSettings
    {
        public string Days { get; set; }

        public AppSettings() 
        {
            Days = "1"; // Default value.
        }
    }
}

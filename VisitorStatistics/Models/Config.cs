using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorStatistics.Models
{
    /// <summary>
    /// This entity class is used to pass combined model properties from one view to the DB.
    /// </summary>
    public class Config
    {
        // Admin settings:
        public int AdminID { get; set; }
        [Display(Name = "First name:")]
        public string Firstname { get; set; }
        [Display(Name = "Last name:")]
        public string Lastname { get; set; }
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please type a valid email.")]
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Display(Name = "Password:")]
        public string Password { get; set; }

        // Application URL settings:
        public int AppID { get; set; }
        [Display(Name = "Application name:")]
        public string ApplicationName { get; set; }
        [Display(Name = "Application URL:")]
        public string ApplicationURL { get; set; }
        public List<string> UrlList { get; set; }

        // Visitor settings:
        [Display(Name = "IP to ignore:")]
        public string IPAddress { get; set; }
        [Display(Name = "Days before deletion:")]
        public int DaysBeforeDeletion { get; set; }
        public List<string> IPList { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Mantle.Web.Models;

public class InstallationModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Admin Email")]
    public string AdminEmail { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Admin Password")]
    public string AdminPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("AdminPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "Enter Connection String")]
    public bool EnterConnectionString { get; set; }

    [Display(Name = "Connection String")]
    public string ConnectionString { get; set; }

    [Display(Name = "Database Server")]
    public string DatabaseServer { get; set; }

    [Display(Name = "Database Name")]
    public string DatabaseName { get; set; }

    [Display(Name = "Use Windows Authentication")]
    public bool UseWindowsAuthentication { get; set; }

    [Display(Name = "Database Username")]
    public string DatabaseUsername { get; set; }

    [Display(Name = "Database Password")]
    public string DatabasePassword { get; set; }

    [Display(Name = "Create Sample Data")]
    public bool CreateSampleData { get; set; }
}
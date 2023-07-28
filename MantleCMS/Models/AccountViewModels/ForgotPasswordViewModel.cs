using System.ComponentModel.DataAnnotations;

namespace MantleCMS.Models.AccountViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
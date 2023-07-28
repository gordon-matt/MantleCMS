using System.ComponentModel.DataAnnotations;

namespace MantleCMS.Models.ManageViewModels;

public class AddPhoneNumberViewModel
{
    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
}
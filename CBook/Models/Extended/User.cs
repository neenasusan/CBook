using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CBook.Models
{
  [MetadataType(typeof(UserMetadata))]
  public partial class User
  {
    public string ConfirmPassword { get; set; }
  }

  public class UserMetadata
  {
    [Display(Name = "First name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "First Name Required")]
    public string FirstName { get; set; }

    [Display(Name = "Last name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name Required")]
    public string LastName { get; set; }

    [Display(Name = "Email ID")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID Required")]
    [DataType(DataType.EmailAddress)]
    public string EmailID { get; set; }

    [Display(Name = "Date Of Birth")]
    [DataType(DataType.Date)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/DD/YYYY}")]
    public Nullable<System.DateTime> DOB { get; set; }


    [Display(Name = "Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
    public string Password { get; set; }

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Confirm password and password do not match")]
    public string ConfirmPassword { get; set; }

  }
}

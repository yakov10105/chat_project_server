using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Dtos
{
    public class UserLoginDto
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User Name field is required.")]
        [StringLength(15)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        [Display(Name = "Password")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Tasks.Models.ViewModles
{
    public class RegisterVeiwModle
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Miss Matching")]
        public string ConfirmPassword { get; set; }

        public string Phone { get; set; }
    }
}

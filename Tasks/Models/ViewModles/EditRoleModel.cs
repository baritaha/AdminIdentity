using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tasks.Models.ViewModles
{
    public class EditRoleModel
    {
        public EditRoleModel()
        {
            usersNames= new List<string>();
        }
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        public List<string> usersNames  { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Room Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The room name cannot be empty.")]
        [StringLength(20, ErrorMessage = "Max length of name : 20 chars.")]
        public string RoomName { get; set; }

        public bool IsGameOn { get; set; } = false;

        public virtual ICollection<User> Members { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}

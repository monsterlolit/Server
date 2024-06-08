using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class Roles
    {
        [Key]
        public int IdRoles { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}

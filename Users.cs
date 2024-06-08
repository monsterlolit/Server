using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public virtual Roles Roles { get; set; }
        public int IdRoles { get; set; }
    }
}

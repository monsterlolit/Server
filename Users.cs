using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

    }
}

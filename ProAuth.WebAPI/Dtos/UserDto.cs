using System.Collections.Generic;

namespace ProAuth.WebAPI.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Departamento { get; set; }
        public string ImagemUrlUser { get; set; }
        public string Role { get; set; }
        public string SecurityStamp { get; set; }
        public Guid ExternalId { get; set; } = Guid.NewGuid();

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProAuth.WebAPI.Models.Identity
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Departamento { get; set; }
        public string ImagemUrlUser { get; set; }
        public DateTime DataUltimoLogin { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public bool Status { get; set; } = true;
        public Guid ExternalId { get; set; } = Guid.NewGuid();
    }
}
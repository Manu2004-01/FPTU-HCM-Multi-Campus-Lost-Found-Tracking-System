using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{

    public class User 
    {
     
        public Guid UserId { get; set; }
        
        public string Fullname { get; set; }
     
        public string Email { get; set; }
    
        public string Phone { get; set; }
       
        public int RoleId { get; set; }
        
        public int? CampusId { get; set; }
        
        // Navigation properties
        public UserRoleLookup Role { get; set; }
        public Campus Campus { get; set; }
     
        public DateTime CreateAt { get; set; }
       
        public DateTime UpdateAt { get; set; }
        public string PasswordHash { get; set; }
    }
}

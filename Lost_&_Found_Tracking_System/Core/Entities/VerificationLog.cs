using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{

    public class VerificationLog 
    {
        
        public int VerificationId { get; set; }

       
        public int ItemId { get; set; }

        
        public int? ClaimId { get; set; }

        
        public Guid VerifiedByUserId { get; set; }

        public string VerificationType { get; set; }

   
        public string Notes { get; set; }

     
        public DateTime CreatedAt { get; set; }
    }
}

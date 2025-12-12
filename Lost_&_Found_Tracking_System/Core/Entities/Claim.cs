using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{

    public class Claim 
    {

        public int ClaimId { get; set; }


        public int ItemId { get; set; }


        public Guid StudentId { get; set; }


        public string Description { get; set; }


        public string EvidenceImageUrl { get; set; }

        public int StatusId { get; set; }

 
        public DateTime CreatedAt { get; set; }


        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public Item Item { get; set; }
        public User Student { get; set; }
        public ClaimStatusLookup Status { get; set; }
    }
}

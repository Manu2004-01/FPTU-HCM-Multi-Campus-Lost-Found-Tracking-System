using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{

    public class Item 
    {

        public int ItemId { get; set; }


        public string Title { get; set; }


        public string Description { get; set; }


        public string ImageUrl { get; set; }

        public int ItemTypeId { get; set; }

        public int StatusId { get; set; }

        public int? CategoryId { get; set; }


        public Guid? LostByStudentId { get; set; }

        public Guid? FoundByUserId { get; set; }


        public int CampusId { get; set; }


        public DateTime CreatedAt { get; set; }

 
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ItemTypeLookup ItemType { get; set; }
        public ItemStatusLookup Status { get; set; }
        public ItemCategory Category { get; set; }
        public User LostByStudent { get; set; }
        public User FoundByUser { get; set; }
        public Campus Campus { get; set; }
    }
}

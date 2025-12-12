using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{

    public class ItemCategory 
    {

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}

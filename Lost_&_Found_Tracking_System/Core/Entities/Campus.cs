using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{

    public class Campus 
    {

        public int CampusId { get; set; }

        public string CampusName { get; set; }

        public string CampusAddress { get; set; }

        public string CampusPhone { get; set; }

        public DateTime CreateAt { get; set; }
    }
}

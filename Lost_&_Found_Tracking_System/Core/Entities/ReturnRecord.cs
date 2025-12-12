using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{


    public class ReturnRecord 
    {

        public int ReturnId { get; set; }


        public int ItemId { get; set; }


        public Guid StaffId { get; set; }


        public Guid StudentId { get; set; }


        public DateTime ReturnTime { get; set; }

 
        public string SignatureUrl { get; set; }
    }
}

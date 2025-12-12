using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Core.Entities
{
    public class Appointment 
    {

        public int AppointmentId { get; set; }

        public int ItemId { get; set; }

        public Guid? StaffId { get; set; }

        public Guid StudentId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public int StatusId { get; set; }
        
        // Navigation properties
        public Item Item { get; set; }
        public User Staff { get; set; }
        public User Student { get; set; }
        public AppointmentStatusLookup Status { get; set; }
    }
}

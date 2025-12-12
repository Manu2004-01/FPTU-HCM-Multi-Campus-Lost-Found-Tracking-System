using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ProfileDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int CampusId { get; set; }
        public string CampusName { get; set; }
        public string Phone { get; set; }
    }
}

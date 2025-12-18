using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CampusDTO
    {
        public int CampusId { get; set; }
        public string? CampusName { get; set; }
        public string? CampusAddress { get; set; }
        public string? CampusPhone { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class CreateCampusDTO
    {
        public string CampusName { get; set; } = null!;
        public string? CampusAddress { get; set; }
        public string? CampusPhone { get; set; }
    }

    public class UpdateCampusDTO
    {
        public string? CampusName { get; set; }
        public string? CampusAddress { get; set; }
        public string? CampusPhone { get; set; }
    }
}

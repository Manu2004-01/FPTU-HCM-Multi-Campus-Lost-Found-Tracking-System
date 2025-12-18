using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ItemTypeName { get; set; }
        public string StatusName { get; set; }
        public string CategoryName { get; set; }
        public string CampusName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

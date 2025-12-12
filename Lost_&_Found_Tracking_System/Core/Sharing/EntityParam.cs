using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing
{
    public class EntityParam
    {
        //Sorting
        public string Sorting { get; set; }
        //Filter
        public int? ItemTypeId { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public int? CampusId { get; set; }
        public Guid? UserId { get; set; }
    }
}

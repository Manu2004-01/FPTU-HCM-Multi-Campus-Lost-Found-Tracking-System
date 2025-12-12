using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class StudentItemDTO
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
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateItemDTO 
    {
        private int _statusId = 1;
        private int? _categoryId;

        public Guid? LostByStudentId { get; set; }
        public Guid? FoundByUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
        public int ItemTypeId { get; set; } // 1 = Lost, 2 = Found
        public int StatusId 
        { 
            get => _statusId; 
            set 
            {
                _statusId = value == 0 ? 1 : value;
            }
        }
        public int? CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = (value.HasValue && value.Value == 0) ? null : value;
            }
        }
        public int CampusId { get; set; }
    }

    public class UpdateItemDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int? CampusId { get; set; }
    }
}

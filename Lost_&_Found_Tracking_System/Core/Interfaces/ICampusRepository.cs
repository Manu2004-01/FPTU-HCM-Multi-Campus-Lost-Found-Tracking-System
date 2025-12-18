using Core.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICampusRepository
    {
        Task<Campus> CreateCampusAsync(CreateCampusDTO dto);
        Task<Campus?> UpdateCampusAsync(int campusId, UpdateCampusDTO dto);
        Task<List<CampusDTO>> GetAllAsync();
    }
}

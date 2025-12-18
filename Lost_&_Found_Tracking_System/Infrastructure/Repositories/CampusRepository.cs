using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CampusRepository : ICampusRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public CampusRepository(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CampusDTO>> GetAllAsync()
        {
            return await _context.Campus
                .AsNoTracking()
                .OrderBy(c => c.CampusName)
                .ProjectTo<CampusDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<Campus> CreateCampusAsync(CreateCampusDTO dto)
        {
            var name = dto.CampusName.Trim();

            var exists = await _context.Campus
                .AnyAsync(c => c.CampusName.ToLower() == name.ToLower());

            if (exists)
                throw new InvalidOperationException("CampusName đã tồn tại");

            var campus = _mapper.Map<Campus>(dto);

            await _context.Campus.AddAsync(campus);
            await _context.SaveChangesAsync();

            return campus;
        }

        public async Task<Campus?> UpdateCampusAsync(int campusId, UpdateCampusDTO dto)
        {
            var campus = await _context.Campus
                .FirstOrDefaultAsync(c => c.CampusId == campusId);

            if (campus == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.CampusName))
            {
                var newName = dto.CampusName.Trim();
                var nameExists = await _context.Campus.AnyAsync(c =>
                    c.CampusId != campusId &&
                    c.CampusName.ToLower() == newName.ToLower());

                if (nameExists)
                    throw new InvalidOperationException("CampusName đã tồn tại");
            }

            _mapper.Map(dto, campus);

            await _context.SaveChangesAsync();
            return campus;
        }
    }
}

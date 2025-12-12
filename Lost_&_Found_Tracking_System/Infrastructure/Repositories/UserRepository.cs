using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDBContext context, IFileProvider fileProvider, IMapper mapper) : base(context)
        {
            _context = context;
            _fileProvider = fileProvider;
            _mapper = mapper;
        }

        public async Task<ProfileDTO> GetProfileAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Campus)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                
                if (user == null) return null;
                
                // Map manually to ensure all properties are set
                var profile = new ProfileDTO
                {
                    FullName = user.Fullname,
                    Email = user.Email,
                    Phone = user.Phone,
                    CampusId = user.CampusId ?? 0,
                    CampusName = user.Campus?.CampusName ?? string.Empty
                };
                
                return profile;
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"GetProfileAsync error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}

using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Sharing;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IMapper _mapper;

        public ItemRepository(ApplicationDBContext context, IFileProvider fileProvider, IMapper mapper) : base(context)
        {
            _context = context;
            _fileProvider = fileProvider;
            _mapper = mapper;
        }

        public async Task<bool> CreateItemAsync(CreateItemDTO itemDTO)
        {
            var item = _mapper.Map<Item>(itemDTO);

            // Đảm bảo logic đúng dựa trên ItemTypeId
            if (itemDTO.ItemTypeId == 1) // Lost Item
            {
                item.LostByStudentId = itemDTO.LostByStudentId;
                item.FoundByUserId = null;
            }
            else if (itemDTO.ItemTypeId == 2) // Found Item
            {
                item.FoundByUserId = itemDTO.FoundByUserId;
                item.LostByStudentId = null;
            }

            // Validate CampusId tồn tại
            if (itemDTO.CampusId > 0)
            {
                var campusExists = await _context.Campus.AnyAsync(c => c.CampusId == itemDTO.CampusId);
                if (!campusExists)
                {
                    return false; // CampusId không tồn tại
                }
            }

            if (itemDTO.ImageUrl != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(itemDTO.ImageUrl.FileName)}";

                var relativePath = Path.Combine("images", "lostitems", fileName);

                var fileInfo = _fileProvider.GetFileInfo(relativePath);

                var physicalPath = fileInfo.PhysicalPath;

                var directory = Path.GetDirectoryName(physicalPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await itemDTO.ImageUrl.CopyToAsync(stream);
                }

                item.ImageUrl = $"/images/lostitems/{fileName}";
            }
            else
            {
                item.ImageUrl = null;
            }

            // Set timestamps
            var now = DateTime.UtcNow;
            item.CreatedAt = now;
            item.UpdatedAt = now;

            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            var item = _context.Items.Find(itemId);
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                var pic_info = _fileProvider.GetFileInfo(item.ImageUrl);
                var root_path = pic_info.PhysicalPath;
                System.IO.File.Delete($"{root_path}");

                //Delete Db
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<StudentItemDTO>> GetAllItemsAsync(EntityParam entityParam)
        {
            var query = _context.Items
                .AsNoTracking()
                .Include(i => i.ItemType)
                .Include(i => i.Status)
                .Include(i => i.Category)
                .Include(i => i.Campus)
                .AsQueryable();

            if (!string.IsNullOrEmpty(entityParam.Sorting))
            {
                query = entityParam.Sorting switch
                {
                    "updateAt_asc" => query.OrderBy(i => i.UpdatedAt),
                    _ => query.OrderByDescending(i => i.CreatedAt),
                };
            }
            else
            {
                query = query.OrderByDescending(i => i.CreatedAt);
            }

            if (entityParam.ItemTypeId.HasValue)
            {
                query = query.Where(i => i.ItemTypeId == entityParam.ItemTypeId.Value);
            }

            if (entityParam.StatusId.HasValue)
            {
                query = query.Where(i => i.StatusId == entityParam.StatusId.Value);
            }

            if (entityParam.CategoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == entityParam.CategoryId.Value);
            }

            if (entityParam.CampusId.HasValue)
            {
                query = query.Where(i => i.CampusId == entityParam.CampusId.Value);
            }

            // Filter theo userId: chỉ lấy items của user đang đăng nhập
            if (entityParam.UserId.HasValue)
            {
                if (entityParam.ItemTypeId.HasValue)
                {
                    // Nếu có ItemTypeId, filter theo loại tương ứng
                    if (entityParam.ItemTypeId.Value == 1) // Lost Item
                    {
                        query = query.Where(i => i.LostByStudentId == entityParam.UserId.Value);
                    }
                    else if (entityParam.ItemTypeId.Value == 2) // Found Item
                    {
                        query = query.Where(i => i.FoundByUserId == entityParam.UserId.Value);
                    }
                }
                else
                {
                    // Nếu không có ItemTypeId, lấy cả lost và found items của user
                    query = query.Where(i => i.LostByStudentId == entityParam.UserId.Value || 
                                            i.FoundByUserId == entityParam.UserId.Value);
                }
            }

            var list = await query.ToListAsync();
            return _mapper.Map<IEnumerable<StudentItemDTO>>(list);
        }

        public async Task<bool> UpdateItemAsync(int itemId, UpdateItemDTO itemDTO)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return false;

            // Chỉ update Title nếu có giá trị (không null và không empty)
            if (!string.IsNullOrWhiteSpace(itemDTO.Title))
            {
                item.Title = itemDTO.Title.Trim();
            }

            // Chỉ update Description nếu có giá trị (không null và không empty)
            if (!string.IsNullOrWhiteSpace(itemDTO.Description))
            {
                item.Description = itemDTO.Description.Trim();
            }

            // Update CategoryId nếu có giá trị (null hoặc > 0)
            if (itemDTO.CategoryId.HasValue)
            {
                item.CategoryId = itemDTO.CategoryId.Value == 0 ? null : itemDTO.CategoryId.Value;
            }
            
            // Chỉ update CampusId nếu giá trị hợp lệ (> 0 và tồn tại trong database)
            if (itemDTO.CampusId.HasValue && itemDTO.CampusId.Value > 0)
            {
                var campusExists = await _context.Campus.AnyAsync(c => c.CampusId == itemDTO.CampusId.Value);
                if (campusExists)
                {
                    item.CampusId = itemDTO.CampusId.Value;
                }
            }

            // Xử lý file upload nếu có
            if (itemDTO.ImageUrl != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(itemDTO.ImageUrl.FileName)}";
                var relativePath = Path.Combine("images", "lostitems", fileName);
                var fileInfo = _fileProvider.GetFileInfo(relativePath);
                var physicalPath = fileInfo.PhysicalPath;
                var directory = Path.GetDirectoryName(physicalPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await itemDTO.ImageUrl.CopyToAsync(stream);
                }

                item.ImageUrl = $"/images/lostitems/{fileName}";
            }

            // Luôn cập nhật UpdatedAt khi có thay đổi
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

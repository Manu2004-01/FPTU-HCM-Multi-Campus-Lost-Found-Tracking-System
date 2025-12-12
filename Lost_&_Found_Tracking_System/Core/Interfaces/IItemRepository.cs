using Core.DTOs;
using Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<StudentItemDTO>> GetAllItemsAsync(EntityParam entityParam);
        Task<bool> CreateItemAsync(CreateItemDTO itemDTO);
        Task<bool> UpdateItemAsync(int itemId, UpdateItemDTO itemDTO);
        Task<bool> DeleteItemAsync(int itemId);
    }
}

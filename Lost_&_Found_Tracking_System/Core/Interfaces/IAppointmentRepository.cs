using Core.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<CreateAppointmentResult> CreateReturnAppointmentAsync(Guid staffId, int itemId, Guid studentId, DateOnly date, TimeSpan time);
    }
}

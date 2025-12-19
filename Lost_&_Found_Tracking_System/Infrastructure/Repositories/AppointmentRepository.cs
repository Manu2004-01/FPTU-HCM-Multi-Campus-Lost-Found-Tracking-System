using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDBContext _context;

        public AppointmentRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CreateAppointmentResult> CreateReturnAppointmentAsync(Guid staffId, int itemId, Guid studentId, DateOnly date, TimeSpan time)
        {
            var itemExists = await _context.Items.AnyAsync(i => i.ItemId == itemId);
            if (!itemExists)
                return new CreateAppointmentResult { Ok = false, ErrorMessage = "Không tìm thấy item" };

            var studentExists = await _context.Users.AnyAsync(u => u.UserId == studentId);
            if (!studentExists)
                return new CreateAppointmentResult { Ok = false, ErrorMessage = "Không tìm thấy student" };

            var appt = new Appointment
            {
                ItemId = itemId,
                StaffId = staffId,
                StudentId = studentId,
                Date = date,
                Time = TimeOnly.FromTimeSpan(time),

                StatusId = 1 // scheduled
            };

            try
            {
                await _context.Appointments.AddAsync(appt);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                throw new Exception("DB error: " + msg, ex);
            }

            return new CreateAppointmentResult
            {
                Ok = true,
                AppointmentId = appt.AppointmentId
            };
        }
    }
}

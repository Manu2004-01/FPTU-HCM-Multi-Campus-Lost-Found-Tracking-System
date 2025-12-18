using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IItemRepository ItemRepository { get; }
        public IClaimRepository ClaimRepository { get; }
        public IAppointmentRepository AppointmentRepository { get; }
    }
}

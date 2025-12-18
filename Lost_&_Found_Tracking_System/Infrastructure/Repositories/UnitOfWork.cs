using AutoMapper;
using Core.Interfaces;
using Infrastructure.DBContext;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _dbContext;

        private readonly IFileProvider _fileProvider;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        //public ITokenService TokenServices { get; }
        public IUserRepository UserRepository { get; }

        public IItemRepository ItemRepository { get; }

        public IClaimRepository ClaimRepository { get; }

        public IAppointmentRepository AppointmentRepository { get; }

        public IReportRepository ReportRepository { get; }


        public UnitOfWork(ApplicationDBContext dbContext, IFileProvider fileProvider, IMapper mapper, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _fileProvider = fileProvider;
            _mapper = mapper;
            UserRepository = new UserRepository(_dbContext, _fileProvider, _mapper);
            ItemRepository = new ItemRepository(_dbContext, _fileProvider, _mapper);
            ClaimRepository = new ClaimRepository(_dbContext, _fileProvider, _mapper);
            AppointmentRepository = new AppointmentRepository(_dbContext);
            ReportRepository = new ReportRepository(_dbContext);
        }
    }
}

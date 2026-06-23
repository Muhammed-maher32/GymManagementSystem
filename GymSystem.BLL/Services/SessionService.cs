using AutoMapper;
using GymSystem.BLL.Contracts;
using GymSystem.BLL.ViewModels;
using GymSystem.DAL.Contracts;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionViewModel?>> GetAllSessionsAsync(CancellationToken ct)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = await sessionRepo.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (sessions == null) return null;

            //var mappedSessions = sessions.Select(s => new SessionViewModel
            //{
            //    Id = s.Id,
            //    Capacity = s.Capacity,
            //    CategoryName = s.Category.Name,
            //    TrainerName = s.Trainer.Name,
            //    Description = s.Description,
            //    EndDate = s.EndDate,
            //    StartDate = s.CreatedAt,
            //});
            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            // N + 1 !!!!!!!!!!!!!!
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id, ct);
            }

            return mappedSessions;
        }
    }
}

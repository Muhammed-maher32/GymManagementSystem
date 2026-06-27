using AutoMapper;
using GymSystem.BLL.Services.Common;
using GymSystem.BLL.Services.Contracts;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Contracts;
using GymSystem.DAL.Models;
using GymSystem.DAL.Models.Enums;
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

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after start date.");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start date must be in the future.");

            var trainerRepo = _unitOfWork.GetRepository<Trainer>();

            var trainer = await trainerRepo.GetByIdAsync(model.TrainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer not found.");

            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(model.CategoryId, ct);

            if (category is null)
                return Result.NotFound("Category not found.");

            var isValidSpecialty = Enum.TryParse<Speciality>(category.Name, true, out var categorySpecialty);

            if (!isValidSpecialty || trainer.Speciality != categorySpecialty)
                return Result.Validation("Cannot create this session for this trainer.");

            var session = _mapper.Map<Session>(model);

            var sessionRepo = _unitOfWork.GetRepository<Session>();
            sessionRepo.Add(session);

            var affectedRows = await _unitOfWork.SaveChangesAsync(ct);

            return affectedRows > 0 ? Result.Ok() : Result.Fail("Failed to create session.");
        }

        public async Task<IEnumerable<SessionViewModel?>> GetAllSessionsAsync(CancellationToken ct)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = await sessionRepo.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (sessions == null) return null!;
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

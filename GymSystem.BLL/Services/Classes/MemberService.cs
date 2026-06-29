using AutoMapper;
using GymSystem.BLL.Services.Common;
using GymSystem.BLL.Services.Contracts;
using GymSystem.BLL.ViewModels.MemberViewModels;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct)
        {
            var _memberRepo = _unitOfWork.GetRepository<Member>();

            var members = await _memberRepo.GetAllAsync(ct);

            var memberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return Result<IEnumerable<MemberViewModel>>.Ok(memberViewModels);
        }

        public async Task<Result<MemberDetailsViewModel?>> GetMemberDetailsByIdAsync(int id, CancellationToken ct)
        {
            var _memberRepo = _unitOfWork.GetRepository<Member>();
            var member = await _memberRepo.GetByIdAsync(id, ct);

            if (member is null) return Result<MemberDetailsViewModel>.NotFound("Member not found.")!;

            var memberDetailsVM = _mapper.Map<MemberDetailsViewModel>(member);

            //active membership
            var _membershipRepo = _unitOfWork.GetRepository<Membership>();

            var activeMembership = await _membershipRepo.FirstOrDefaultAsync(
                x => x.MemberId == member.Id && x.EndDate > DateTime.Now, ct);

            if (activeMembership is not null)
            {
                //PlAN name
                var _planRepo = _unitOfWork.GetRepository<Plan>();

                var plan = await _planRepo.GetByIdAsync(activeMembership.PlanId, ct);
                memberDetailsVM.PlanName = plan?.Name;
                //assign membershipStartDate & membershipEndDate
                memberDetailsVM.MembershipStartDate = activeMembership.StartDate.ToString();
                memberDetailsVM.MembershipEndDate = activeMembership.EndDate.ToString();
            }
            return Result<MemberDetailsViewModel>.Ok(memberDetailsVM)!;
        }

        public async Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int memberId, CancellationToken ct)
        {
            var _healthRecordRepo = _unitOfWork.GetRepository<HealthRecord>();

            var healthRecord = await _healthRecordRepo.FirstOrDefaultAsync(x => x.MemberId == memberId, ct);

            if (healthRecord is null) return Result<HealthRecordViewModel>.NotFound("Health record not found")!;
            var vm = _mapper.Map<HealthRecordViewModel>(healthRecord);
            return Result<HealthRecordViewModel>.Ok(vm)!;
        }

        public async Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var _memberRepo = _unitOfWork.GetRepository<Member>();

            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if (member is null) return Result<MemberToUpdateViewModel>.NotFound("Member not found.")!;
            var vm = new MemberToUpdateViewModel
            {
                Name = member.Name,
                Phone = member.Phone,
                Email = member.Email,
                Photo = member.Photo,
                Street = member.Address.Street,
                City = member.Address.City,
                BuildingNumber = member.Address.BuildingNumber
            };
            return Result<MemberToUpdateViewModel>.Ok(vm)!;
        }
        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            var _memberRepo = _unitOfWork.GetRepository<Member>();
            //Validate Email Doesn't Exist
            var emailExists = await _memberRepo.AnyAsync(m => m.Email == model.Email, ct);
            //Validate PHone Doesn't Exist
            var phoneExists = await _memberRepo.AnyAsync(m => m.Phone == model.Phone, ct);

            if (emailExists) return Result.Fail("Creation Failed.Email already in use.");
            if (phoneExists) return Result.Fail("Creation Failed.Phone already in use.");

            var member = _mapper.Map<Member>(model);

            _memberRepo.Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Try again.");

        }

        public async Task<Result> RemoveMemberAsync(int id, CancellationToken ct)
        {
            var _memberRepo = _unitOfWork.GetRepository<Member>();

            var member = await _memberRepo.GetByIdAsync(id, ct);
            if (member is null) return Result.NotFound();

            var _bookingRepo = _unitOfWork.GetRepository<Booking>();

            var hasFutureBookings = await _bookingRepo.AnyAsync(x => x.MemberId == id && x.BookingDate > DateTime.Now, ct);
            if (hasFutureBookings) return Result.Fail("Cannot remove member with future bookings.");

            _memberRepo.Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Try again.");
        }

        public async Task<Result> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var _memberRepo = _unitOfWork.GetRepository<Member>();

            var member = await _memberRepo.GetByIdAsync(id, ct);
            if (member is null) return Result.Fail("Member not found");

            if (await _memberRepo.AnyAsync(m => m.Email == model.Email && m.Id != id, ct))
                return Result.Fail("Email is already used.");

            if (await _memberRepo.AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
                return Result.Fail("Phone is already used.");

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.Street = model.Street;
            member.Address.City = model.City;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;

            _memberRepo.Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Error,Try again.");
        }
    }
}

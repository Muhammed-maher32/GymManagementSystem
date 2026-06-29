using AutoMapper;
using AutoMapper.Execution;
using GymSystem.BLL.ViewModels.MemberViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapMember();
            MapSession();


        }

        private void MapMember()
        {
            CreateMap<DAL.Models.Member, MemberDetailsViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(m =>
                $"{m.Address.BuildingNumber}-{m.Address.Street}-{m.Address.City}"));

            CreateMap<DAL.Models.Member, MemberViewModel>().ReverseMap();

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<CreateMemberViewModel, DAL.Models.Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(s => new Address
                {
                    City = s.City,
                    Street = s.Street,
                    BuildingNumber = s.BuildingNumber
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(s => s.HealthRecordViewModel));

            CreateMap<MemberToUpdateViewModel, DAL.Models.Member>()
                .ForMember(d => d.Address, o => o.MapFrom(s => new Address
                {
                    City = s.City,
                    Street = s.Street,
                    BuildingNumber = s.BuildingNumber
                }))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());
        }

        private void MapSession()
        {
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(s => s.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(c => c.Category.Name));

            CreateMap<CreateSessionViewModel, Session>()
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(s => s.StartDate)
                );

            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();

        }

    }
}

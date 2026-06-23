using AutoMapper;
using AutoMapper.Execution;
using GymSystem.BLL.ViewModels;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MembersMappings();
            
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(s => s.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(c => c.Category.Name));
        }


        private void MembersMappings()
        {
            CreateMap<DAL.Models.Member, MemberDetailsViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(m =>
                $"{m.Address.BuildingNumber}-{m.Address.Street}-{m.Address.City}"));

            CreateMap<DAL.Models.Member, MemberViewModel>().ReverseMap();

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

        }

    }
}

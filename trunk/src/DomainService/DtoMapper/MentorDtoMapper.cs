using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.DataTransferObject;
using Consumentor.ShopGun.Gateway;

namespace Consumentor.ShopGun.DomainService.DtoMapper
{
    public class MentorDtoMapper : IMapper<Mentor, MentorDto>
    {
        public MentorDto Map(Mentor source)
        {
            return new MentorDto
                       {
                           Description = source.Description,
                           Id = source.Id,
                           MentorName = source.MentorName,
                           Url = source.Url
                       };
        }

        public Mentor Map(MentorDto source)
        {
            return new Mentor
                       {
                           Description = source.Description,
                           Id = source.Id,
                           MentorName = source.MentorName,
                           Url = source.Url
                       };
        }
    }
}

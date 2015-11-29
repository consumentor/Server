using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IUserRegistrationApplicationService
    {
        User PersistNewUser(User user);
    }
}
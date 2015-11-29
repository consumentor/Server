using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class UserRegistrationApplicationService : IUserRegistrationApplicationService
    {
        private readonly IRepository<User> _userRepository;
        public UserRegistrationApplicationService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public ILogger Log { get; set; }

        public User PersistNewUser(User user)
        {
            _userRepository.Add(user);
            _userRepository.Persist();
            
            Log.Debug("Adding and Persisting new user");

            return user;
        }
    }
}
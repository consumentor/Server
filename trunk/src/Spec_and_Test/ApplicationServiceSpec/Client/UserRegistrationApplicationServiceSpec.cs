using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using Rhino.Mocks;

namespace ApplicationServiceSpec.Client
{
    [TestFixture]
    public class UserRegistrationApplicationServiceSpec
    {
        [TestFixture]
        public class WhenSigningUpAsNewUser : SpecBase
        {
            private IUserRegistrationApplicationService _userRegistrationApplicationService;
            private IRepository<User> _userRepositoryMock;

            protected override void Before_each_spec()
            {
                _userRepositoryMock = CreateDependency<IRepository<User>>();
                _userRegistrationApplicationService = new UserRegistrationApplicationService(_userRepositoryMock) 
                { Log = new ConsoleLogger() };
            }

            [Test]
            public void ShouldPersistANewUser()
            {
                _userRepositoryMock.Expect(x => x.Add(new User())).IgnoreArguments().Repeat.Once();
                _userRepositoryMock.Expect(x => x.Persist()).Repeat.Once();

                _userRegistrationApplicationService.PersistNewUser(new User());

                _userRepositoryMock.VerifyAllExpectations();
            }
        }


    }
}

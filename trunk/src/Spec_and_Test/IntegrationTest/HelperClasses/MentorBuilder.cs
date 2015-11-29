using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class MentorBuilder
    {
        public static Mentor BuildMentor()
        {
            return new Mentor {MentorName = "Consumentor"};
        }
    }
}

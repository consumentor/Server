using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class SemaphoreBuilder
    {
        public static Semaphore BuildGreenSemaphore()
        {
            return new Semaphore {ColorName = "green", Value = 1};
        }

        public static Semaphore BuildYellowSemaphore()
        {
            return new Semaphore {ColorName = "yellow", Value = 0};
        }

        public static Semaphore BuildRedSemaphore()
        {
            return new Semaphore {ColorName = "red", Value = -1};
        }
    }
}

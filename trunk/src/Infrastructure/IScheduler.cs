namespace Consumentor.ShopGun
{
    public interface IScheduler
    {
        void StartScheduler();
        void StopScheduler();
        void OnScheduledEvent();
    }
}
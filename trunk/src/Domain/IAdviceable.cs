namespace Consumentor.ShopGun.Domain
{
    public interface IAdviceable<TAdviceType> where TAdviceType : AdviceBase
    {
        void AddAdvice(TAdviceType advice);
        void RemoveAdvice(TAdviceType adviceToRemove);
    }
}
using System;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class AdviceBuilder
    {

        public static TAdviceType BuildAdvice<TAdviceType>() where TAdviceType : AdviceBase, new()
        {
            return BuildAdvice<TAdviceType>("Vatten");
        }

        public static TAdviceType BuildAdvice<TAdviceType>(string adviceText) where TAdviceType : AdviceBase, new()
        {
            return new TAdviceType
                       {
                           Advice = adviceText ?? "Vatten",
                Introduction = "Some introduction...",
                KeyWords = "Some keywords...",
                Label = "The label",
                Mentor = MentorBuilder.BuildMentor(),
                Semaphore = SemaphoreBuilder.BuildGreenSemaphore(),
                Published = true,
                PublishDate = DateTime.Now
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class CertificationMarkBuilder
    {
        public static CertificationMark BuildCertificationMark()
        {
            return BuildCertificationMark(MentorBuilder.BuildMentor());
        }

        public static CertificationMark BuildCertificationMark(Mentor mentor)
        {
            var certificationMark = new CertificationMark
            {
                CertificationMarkImageUrlLarge = "",
                CertificationMarkImageUrlMedium = "",
                CertificationMarkImageUrlSmall = "",
                Gs1Code = "",
                CertificationName = "Krav",
                Certifier = mentor
            };
            return certificationMark;
        }
    }
}

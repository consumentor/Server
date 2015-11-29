using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consumentor.ShopGun.DomainService
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AddStatisticsAttribute : Attribute
    {
        private Type _type;

        public AddStatisticsAttribute(Type type)
        {
            _type = type;
        }
    }

}

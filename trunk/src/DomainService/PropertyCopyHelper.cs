using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Consumentor.ShopGun.DomainService
{
    public static class PropertyCopyHelper
    {
        public static void CopyStringProperties<TEntity>(this TEntity obj, TEntity other) where TEntity : class
        {
            CopyProperties(other, obj, typeof(string));
            //PropertyInfo[] properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //foreach (PropertyInfo p in properties)
            //{
            //    // Only work with strings
            //    if (p.PropertyType != typeof(string))
            //    {
            //        continue;
            //    }

            //    // If not writable then cannot null it; if not readable then cannot check it's value
            //    if (!p.CanWrite || !p.CanRead)
            //    {
            //        continue;
            //    }

            //    MethodInfo mget = p.GetGetMethod(false);
            //    MethodInfo mset = p.GetSetMethod(false);

            //    // Get and set methods have to be public
            //    if (mget == null)
            //    {
            //        continue;
            //    }
            //    if (mset == null)
            //    {
            //        continue;
            //    }

            //    p.SetValue(obj, p.GetValue(other, null), null);
            //}  
        }

        public static void CopyIntProperties<TEntity>(this TEntity obj, TEntity other, bool copyNullables) where TEntity : class
        {
            CopyProperties(other, obj, typeof(int));
            if (copyNullables)
            {
                CopyProperties(other, obj, typeof(int?));
            }
        }

        public static void CopyDateTimeProperties<TEntity>(this TEntity obj, TEntity other) where TEntity : class
        {
            CopyProperties(other, obj, typeof(DateTime));
        }

        private static void CopyProperties<TEntity>(TEntity from, TEntity to, Type type)
        {
            PropertyInfo[] properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                // Only work with strings
                if (p.PropertyType != type)
                {
                    continue;
                }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null)
                {
                    continue;
                }
                if (mset == null)
                {
                    continue;
                }

                p.SetValue(to, p.GetValue(from, null), null);
            }             
        }
    }
}

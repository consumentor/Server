using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Consumentor.ShopGun.DomainService
{
    public static class JsonHelper
    {
        public static string ToJson<TEntityType>(this TEntityType obj) where TEntityType : class
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            var result = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return result;
        }

        public static TEntityType ToEntity<TEntityType>(this string json) where TEntityType : class
        {
            var obj = Activator.CreateInstance<TEntityType>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (TEntityType)serializer.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return obj;
        }
    }
}

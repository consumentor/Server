using System;
using System.Text;
using System.Security.Cryptography;

namespace Consumentor.ShopGun
{
    /// <summary>
    /// Använd denna klass för att generera er MD5-sträng
    /// Lägg till denna fil i ert projekt.
    /// 
    /// Exempel:
    ///     
    ///     Md5Helper md5h = new Md5Helper(); // skapar en instans av hjälpklassen
    ///     String md5 = md5h.CalculateMD5Hash("Md5-Strängen","Md5-Nyckeln"); //skapar den hashade Md5strängen
    ///     
    /// 
    /// </summary>
    public class Md5Helper
    {
        public Md5Helper()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string CalculateMd5Hash(string md5String, string key)
        {
            string md5Hash;
            string completeMd5String = md5String + key;

            ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

            Byte[] result = md5Provider.ComputeHash(encoding.GetBytes(completeMd5String));
            md5Hash = ToHexString(result);

            return md5Hash;
        }

        private static string ToHexString(Byte[] input)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                builder.Append(input[i].ToString("x2"));
            }

            return builder.ToString();
        }

    }
}
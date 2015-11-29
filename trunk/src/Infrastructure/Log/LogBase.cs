using Castle.Core.Logging;

namespace Consumentor.ShopGun.Log
{
    public class LogBase
    {
        public ILogger Log { get; set; }

        // Displays the bitpattern as a string
        protected string GetLongBinaryString(long number)
        {
            ulong un = (ulong) number;
            var b = new char[64];
            int pos = 63;
            int i = 0;

            while (i < 64)
            {
                if ((un & ((ulong) 1 << i)) != 0)
                {
                    b[pos] = '1';
                }
                else
                {
                    b[pos] = '0';
                }
                pos--;
                i++;
            }
            return new string(b);
        }
    }
}
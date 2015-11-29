namespace Consumentor.ShopGun.Domain.Extensions
{
    public static class BarcodeHelper
    {
        public static bool IsGtin(this string obj)
        {
            if (obj.Length == 8)
            {
                obj = obj.PadLeft(13, '0');
            }

            long tmp;
            if (long.TryParse(obj, out tmp))
            {
                switch (obj.Length)
                {
                    case 12:
                        return CheckGtin12(obj);
                    case 13:
                        return CheckGtin13(obj);
                    case 14:
                        return CheckGtin14(obj);
                    default:
                        return false;
                }
            }
            return false;
        }

        private static bool CheckGtin12(string gtin)
        {
            int[] digits = ToIntArray(gtin);
            int checkValue = (((digits[0] + digits[2] + digits[4] + digits[6]
                 + digits[8] + digits[10]) * 3) + (digits[1] + digits[3] + digits[5]
                 + digits[7] + digits[9])) % 10;

            if (checkValue != 0)
            {
                checkValue = 10 - checkValue;
            }

            return checkValue == digits[11];
        }

        private static bool CheckGtin13(string gtin)
        {
            int[] digits = ToIntArray(gtin);
            int checkValue = ((digits[1] + digits[3] + digits[5] + digits[7]
                 + digits[9] + digits[11]) * 3 + (digits[0] + digits[2]
                 + digits[4] + digits[6] + digits[8] + digits[10])) % 10;

            if (checkValue != 0)
            {
                checkValue = 10 - checkValue;
            }

            return checkValue == digits[12];
        }

        private static bool CheckGtin14(string gtin)
        {
            int[] digits = ToIntArray(gtin);
            int checkValue = (((digits[0] + digits[2] + digits[4] + digits[6]
            + digits[8] + digits[10] + digits[12]) * 3)
            + (digits[1] + digits[3] + digits[5] + digits[7] + digits[9]
            + digits[11])) % 10;

            if (checkValue != 0)
            {
                checkValue = 10 - checkValue;
            }

            return checkValue == digits[13];
        }

        private static int[] ToIntArray(string value)
        {
            var digits = new int[value.Length];

            for (var i = 0; i < digits.Length; i++)
            {
                digits[i] = int.Parse(value.Substring(i, 1));
            }

            return digits;
        }
    }
}

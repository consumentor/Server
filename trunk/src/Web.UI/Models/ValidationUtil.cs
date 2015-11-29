using System;

namespace Consumentor.Shopgun.Web.UI.Models
{
    internal static class ValidationUtil
    {
        private const string StringRequiredErrorMessage = "Value cannot be null or empty.";

        public static void ValidateRequiredStringValue(string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException(StringRequiredErrorMessage, parameterName);
            }
        }
    }
}
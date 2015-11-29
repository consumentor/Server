using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Consumentor.ShopGun.Services
{
    /// <summary>
    /// This code was found at: http://www.pluralsight.com/community/blogs/keith/archive/2005/10/17/15632.aspx
    /// It gives the specified user enough access rights for the provided urlPrefix. 
    /// I.e. makes it possible to set up a webservice on the specified url without running with administrative rights.
    /// Created as a workaround for the problem decsribed here: http://blogs.msdn.com/paulwh/archive/2007/05/04/addressaccessdeniedexception-http-could-not-register-url-http-8080.aspx
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1053")]
    public class ServiceConfigurationManager
    {
        private const int HttpInitializeConfig = 2;
        private const int HttpServiceConfigUrlAclInfo = 2;

        [SuppressMessage("Microsoft.Usage", "CA2219")]
        [SuppressMessage("Microsoft.Design", "CA1054")]
        public static void ModifyReservation(string urlPrefix, string accountName, bool removeReservation)
        {
            string sddl = CreateSddl(accountName);
            HttpServiceConfigUrlAclSet configInfo;
            configInfo.Key.UrlPrefix = urlPrefix;
            configInfo.Param.Sddl = sddl;
            HttpApiVersion httpApiVersion =
                new HttpApiVersion(1, 0);
            int errorCode = HttpInitialize(httpApiVersion, HttpInitializeConfig, IntPtr.Zero);
            if (0 != errorCode)
                throw GetException("HttpInitialize", errorCode);
            try
            {
                // do our best to delete any existing ACL
                errorCode = HttpDeleteServiceConfigurationAcl(IntPtr.Zero, HttpServiceConfigUrlAclInfo, ref configInfo, Marshal.SizeOf(typeof(HttpServiceConfigUrlAclSet)), IntPtr.Zero);
                if (removeReservation)
                {
                    if (0 != errorCode)
                        throw GetException("HttpDeleteServiceConfigurationAcl", errorCode);
                    return;
                }
                errorCode = HttpSetServiceConfigurationAcl(IntPtr.Zero, HttpServiceConfigUrlAclInfo, ref configInfo, Marshal.SizeOf(typeof(HttpServiceConfigUrlAclSet)), IntPtr.Zero);
                if (0 != errorCode)
                    throw GetException("HttpSetServiceConfigurationAcl", errorCode);
            }
            finally
            {
                errorCode = HttpTerminate(HttpInitializeConfig, IntPtr.Zero);
                if (0 != errorCode)
                    throw GetException("HttpTerminate", errorCode);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        [SuppressMessage("Microsoft.Usage", "CA2201")]
        private static Exception GetException(string fcn, int errorCode)
        {
            Exception x = new Exception(string.Format(CultureInfo.CurrentCulture, "{0} failed: {1}", fcn, GetWin32ErrorMessage(errorCode)));
            return x;
        }

        private static string CreateSddl(string account)
        {
            string sid = new NTAccount(account).Translate(typeof(SecurityIdentifier)).ToString();
            // DACL that Allows Generic eXecute for the user specified by account
            // see help for HttpServiceConfigUrlAclParam for details on what this means
            return string.Format(CultureInfo.CurrentCulture, "D:(A;;GX;;;{0})", sid);
        }

        private static string GetWin32ErrorMessage(int errorCode)
        {
            int hr = HResultFromWin32(errorCode);
            Exception x = Marshal.GetExceptionForHR(hr);
            return x.Message;
        }

        private static int HResultFromWin32(int errorCode)
        {
            if (errorCode <= 0) return errorCode;
            return (int)((0x0000FFFFU & ((uint)errorCode)) | (7U << 16) | 0x80000000U);
        }

        [SuppressMessage("Microsoft.Design", "CA1060")]
        [DllImport("httpapi.dll", ExactSpelling = true, EntryPoint = "HttpSetServiceConfiguration")]
        private static extern int HttpSetServiceConfigurationAcl(IntPtr mustBeZero, int configID, [In] ref HttpServiceConfigUrlAclSet configInfo, int configInfoLength, IntPtr mustBeZero2);

        [SuppressMessage("Microsoft.Design", "CA1060")]
        [DllImport("httpapi.dll", ExactSpelling = true, EntryPoint = "HttpDeleteServiceConfiguration")]
        private static extern int HttpDeleteServiceConfigurationAcl(IntPtr mustBeZero, int configID, [In] ref HttpServiceConfigUrlAclSet configInfo, int configInfoLength, IntPtr mustBeZero2);

        [SuppressMessage("Microsoft.Design", "CA1060")]
        [DllImport("httpapi.dll")]
        private static extern int HttpInitialize(HttpApiVersion version, int flags, IntPtr mustBeZero);

        [SuppressMessage("Microsoft.Design", "CA1060")]
        [DllImport("httpapi.dll")]
        private static extern int HttpTerminate(int flags, IntPtr mustBeZero);

        [StructLayout(LayoutKind.Sequential)]
        internal struct HttpSericeConfigUrlAclKey
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UrlPrefix;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HttpServiceConfigUrlAclParam
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Sddl;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HttpServiceConfigUrlAclSet
        {
            public HttpSericeConfigUrlAclKey Key;
            public HttpServiceConfigUrlAclParam Param;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HttpApiVersion
        {
            private short Major;
            private short Minor;

            public HttpApiVersion(short maj, short min)
            {
                Major = maj;
                Minor = min;
            }
        }
    }
}
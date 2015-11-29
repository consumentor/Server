using System.Globalization;
using System.IO;
using System.Text;
using Castle.Core.Logging;

namespace Consumentor.ShopGun.Log
{
    /// <summary>
    /// Wrapper for Castle Windsor's ILogger
    /// Register it in the container with
    /// <code>
    /// _container
    ///     .RegisterComponent()
    ///         .AsTransient
    ///         .AsService(typeof(TextWriter))
    ///         .OfType(typeof(DataContextLog));
    /// </code>
    /// and it will hook in to the DataContext Log property.
    /// </summary>
    public class DataContextLog : TextWriter
    {
        public DataContextLog()
            : base(CultureInfo.CurrentCulture)
        {
        }

        public ILogger Log { get; set; }

        public override Encoding Encoding
        {
            get { return new UnicodeEncoding(false, false); }
        }

        public override void Write(string value)
        {
            if (Log != null)
            {
                Log.Debug(value);    
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }

    }
}
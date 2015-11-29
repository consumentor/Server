namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Web;
    using System.Web.UI;
    using Microsoft.Web.Resources;

    public class MvcSerializer {

        private const SerializationMode _defaultMode = SerializationMode.Plaintext;

        private static readonly Dictionary<SerializationMode, Func<IStateFormatter>> _registeredFormatterFactories =
                new Dictionary<SerializationMode, Func<IStateFormatter>>() {
                    { SerializationMode.Plaintext, () => new ObjectStateFormatter() },
                    { SerializationMode.Encrypted, LazilyGetFormatterGenerator(true /* encrypt */, false /* sign */) },
                    { SerializationMode.Signed, LazilyGetFormatterGenerator(false /* encrypt */, true /* sign */) },
                    { SerializationMode.EncryptedAndSigned, LazilyGetFormatterGenerator(true /* encrypt */, true /* sign */) }
                };

        private static SerializationException CreateSerializationException(Exception innerException) {
            return new SerializationException(MvcResources.MvcSerializer_DeserializationFailed, innerException);
        }

        public object Deserialize(string serializedValue) {
            return Deserialize(serializedValue, _defaultMode);
        }

        public virtual object Deserialize(string serializedValue, SerializationMode mode) {
            if (String.IsNullOrEmpty(serializedValue)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "serializedValue");
            }

            IStateFormatter formatter = GetFormatter(mode);
            try {
                object deserializedValue = formatter.Deserialize(serializedValue);
                return deserializedValue;
            }
            catch (Exception ex) {
                throw CreateSerializationException(ex);
            }
        }

        private static IStateFormatter GetFormatter(SerializationMode mode) {
            Func<IStateFormatter> formatterFactory;
            if (!_registeredFormatterFactories.TryGetValue(mode, out formatterFactory)) {
                throw new ArgumentOutOfRangeException("mode", MvcResources.MvcSerializer_InvalidSerializationMode);
            }

            return formatterFactory();
        }

        private static Func<IStateFormatter> LazilyGetFormatterGenerator(bool encrypt, bool sign) {
            Lazy<Func<IStateFormatter>> generatorFactory = new Lazy<Func<IStateFormatter>>(
                () => TokenPersister.CreateFormatterGenerator(encrypt, sign)
            );

            return () => {
                Func<IStateFormatter> generator = generatorFactory.Eval();
                return generator();
            };
        }

        public string Serialize(object state) {
            return Serialize(state, _defaultMode);
        }

        public virtual string Serialize(object state, SerializationMode mode) {
            IStateFormatter formatter = GetFormatter(mode);
            string serializedValue = formatter.Serialize(state);
            return serializedValue;
        }

        // This type is very difficult to unit-test because Page.ProcessRequest() requires mocking
        // much of the hosting environment. For now, we can perform functional tests of this feature.
        private sealed class TokenPersister : PageStatePersister {
            private TokenPersister(Page page)
                : base(page) {
            }

            public static Func<IStateFormatter> CreateFormatterGenerator(bool encrypt, bool sign) {
                // This code instantiates a page and tricks it into thinking that it's servicing
                // a postback scenario with encrypted ViewState, which is required to make the
                // StateFormatter properly decrypt data. Specifically, this code sets the
                // internal Page.ContainsEncryptedViewState flag.
                TextWriter writer = TextWriter.Null;
                HttpResponse response = new HttpResponse(writer);
                HttpRequest request = new HttpRequest("DummyFile.aspx", HttpContext.Current.Request.Url.ToString(), "__EVENTTARGET=true" + ((encrypt) ? "&__VIEWSTATEENCRYPTED=true" : null));
                HttpContext context = new HttpContext(request, response);

                Page page = new Page() {
                    EnableViewStateMac = sign,
                    ViewStateEncryptionMode = (encrypt) ? ViewStateEncryptionMode.Always : ViewStateEncryptionMode.Never
                };
                page.ProcessRequest(context);

                return () => new TokenPersister(page).StateFormatter;
            }

            public override void Load() {
                throw new NotImplementedException();
            }

            public override void Save() {
                throw new NotImplementedException();
            }
        }

    }
}

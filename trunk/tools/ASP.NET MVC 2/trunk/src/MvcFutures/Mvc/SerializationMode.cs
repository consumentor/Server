namespace Microsoft.Web.Mvc {
    using System;

    [Flags]
    public enum SerializationMode {

        Plaintext = 0,

        Encrypted = 1 << 0,
        Signed = 1 << 1,
        EncryptedAndSigned = Encrypted | Signed

    }
}

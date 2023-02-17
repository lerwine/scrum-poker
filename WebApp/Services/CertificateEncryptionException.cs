namespace ScrumPoker.WebApp.Services;

[Serializable]
public class CertificateEncryptionException : Exception
{
    public CertificateEncryptionException() { }
    public CertificateEncryptionException(string message) : base(message) { }
    public CertificateEncryptionException(string message, Exception inner) : base(message, inner) { }
    protected CertificateEncryptionException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
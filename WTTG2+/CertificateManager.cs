using UnityEngine.Networking;

public static class CertificateManager
{
	public class IgnoreCertificateHandler : CertificateHandler
	{
		protected override bool ValidateCertificate(byte[] certificateData)
		{
			return true;
		}
	}
}

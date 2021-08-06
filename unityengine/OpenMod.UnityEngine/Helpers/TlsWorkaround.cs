using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace OpenMod.UnityEngine.Helpers
{
    // http://answers.unity.com/answers/1089592/view.html
    internal static class TlsWorkaround
    {
        public static void Install()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationWorkaroundCallback;
        }

        public static void Uninstall()
        {
            ServicePointManager.ServerCertificateValidationCallback = null;
        }

        private static bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            foreach (var chainStatus in chain.ChainStatus)
            {
                if (chainStatus.Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }


                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

                if (!chain.Build((X509Certificate2)certificate))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
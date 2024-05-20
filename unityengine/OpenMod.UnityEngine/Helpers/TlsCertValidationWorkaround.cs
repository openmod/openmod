using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace OpenMod.UnityEngine.Helpers
{
    /// <summary>
    /// Fixes <c>TlsException: Invalid certificate received from server</c>.
    /// </summary>
    internal sealed class TlsCertValidationWorkaround : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ServicePointManager.ServerCertificateValidationCallback += CertificateValidationWorkaroundCallback;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ServicePointManager.ServerCertificateValidationCallback -= CertificateValidationWorkaroundCallback;

            return Task.CompletedTask;
        }

        // Should be an instance method for event unsubscription to work correctly.
        private bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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

                if (!chain.Build((X509Certificate2) certificate))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
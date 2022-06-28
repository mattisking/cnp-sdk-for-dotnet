using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cnp.Sdk.Interfaces
{
    public interface ICommunications
    {
        event EventHandler HttpAction;

        string HttpPost(string xmlRequest);

        Task<string> HttpPostAsync(string xmlRequest, CancellationToken cancellationToken);

        void NeuterXml(ref string inputXml);

        void NeuterUserCredentials(ref string inputXml);

        void FtpDropOff(string fileDirectory, string fileName);

        void FtpPoll(string fileName, int timeout);

        void FtpPickUp(string destinationFilePath, string fileName);

        bool ValidateServerCertificate(object sender, X509Certificate certificate,
             X509Chain chain, SslPolicyErrors sslPolicyErrors);
    }
}

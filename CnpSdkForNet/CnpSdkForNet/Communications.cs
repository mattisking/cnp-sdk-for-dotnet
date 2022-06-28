using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;
using Cnp.Sdk.Interfaces;
using Cnp.Sdk.Core;
using Cnp.Sdk.Configuration;
using Microsoft.Extensions.Logging;

namespace Cnp.Sdk
{
    /// <summary>
    /// Communications handles outbound communications with the API
    /// There is a component of this class that deals with HTTP requests (using HttpClient) and one that deals with SFTP
    /// </summary>
    public class Communications : ICommunications
    {
        public event EventHandler HttpAction;

        /// <summary>
        /// Client for communicating with the APIs through HTTP
        ///   _client is static so it will only be created once, as recommended in the documentation
        /// </summary>
        private static HttpClient _client;

        /// <summary>
        /// The configuration dictionary containing logging, proxy, and other various properties
        /// </summary>
        private readonly CnpOnlineConfig _config;

        // Use a logger extension to allow any kind of logging
        private readonly ILogger<Communications> _logger;

        /// <summary>
        /// The main constructor, which initializes the config and HttpClient
        /// </summary>
        /// <param name="config"></param>
        public Communications(HttpClient httpClient, ILogger<Communications> logger, CnpOnlineConfig config = null)
        {
            _config = config ?? new ConfigManager().getConfig();
            _client = httpClient;
            _logger = logger;
        }

        private void OnHttpAction(RequestType requestType, string xmlPayload)
        {
            if (HttpAction == null) return;
 
            NeuterXml(ref xmlPayload);
            NeuterUserCredentials(ref xmlPayload);

            HttpAction(this, new HttpActionEventArgs(requestType, xmlPayload));
        }

        public bool ValidateServerCertificate(
             object sender,
             X509Certificate certificate,
             X509Chain chain,
             SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            _logger.LogDebug($"Certificate error: {sslPolicyErrors}");

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        /// <summary>
        /// Obfuscates account information in the XML, only if the config value specifies to do so
        /// </summary>
        /// <param name="inputXml">the XML to obfuscate</param>
        public void NeuterXml(ref string inputXml)
        {
            if (!_config.NeuterAccountNums) return;
            
            const string pattern1 = "(?i)<number>.*?</number>";
            const string pattern2 = "(?i)<accNum>.*?</accNum>";
            const string pattern3 = "(?i)<track>.*?</track>";
            const string pattern4 = "(?i)<accountNumber>.*?</accountNumber>";

            var rgx1 = new Regex(pattern1);
            var rgx2 = new Regex(pattern2);
            var rgx3 = new Regex(pattern3);
            var rgx4 = new Regex(pattern4);
            inputXml = rgx1.Replace(inputXml, "<number>xxxxxxxxxxxxxxxx</number>");
            inputXml = rgx2.Replace(inputXml, "<accNum>xxxxxxxxxx</accNum>");
            inputXml = rgx3.Replace(inputXml, "<track>xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx</track>");
            inputXml = rgx4.Replace(inputXml, "<accountNumber>xxxxxxxxxxxxxxxx</accountNumber>");
        }

        /// <summary>
        /// Obfuscates user credentials in the XML, only if the config value specifies to do so
        /// </summary>
        /// <param name="inputXml">the XML to obfuscate</param>
        public void NeuterUserCredentials(ref string inputXml)
        {
            if (!_config.NeuterUserCredentials) return;

            const string pattern1 = "(?i)<user>.*?</user>";
            const string pattern2 = "(?i)<password>.*?</password>";

            var rgx1 = new Regex(pattern1);
            var rgx2 = new Regex(pattern2);
            inputXml = rgx1.Replace(inputXml, "<user>xxxxxx</user>");
            inputXml = rgx2.Replace(inputXml, "<password>xxxxxxxx</password>");
        }

        /// <summary>
        /// Sends a POST request with the given XML to the API, asynchronously
        /// Prefer the use of this method over HttpPost
        /// </summary>
        /// <param name="xmlRequest">The XML to send to the API</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The XML response on success, null otherwise</returns>
        public async Task<string> HttpPostAsync(string xmlRequest, CancellationToken cancellationToken)
        {
            // Log any data to the appropriate places, only if we need to
            _logger.LogDebug(xmlRequest);

            // Now that we have gotten the values for logging from the config, we need to actually send the request
            try
            {
                OnHttpAction(RequestType.Request, xmlRequest);
                var xmlContent = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");

                if (String.IsNullOrEmpty(_config.Apikey) == false)
                {
                    xmlContent.Headers.Add("apikey", _config.Apikey);
                }

                var response = await _client.PostAsync(_config.Url, xmlContent, cancellationToken);
                var xmlResponse = await response.Content.ReadAsStringAsync();
                OnHttpAction(RequestType.Response, xmlResponse);

                _logger.LogDebug(xmlResponse);

                return xmlResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Sends a POST request synchronously to the API. Prefer the async variant of this method when possible.
        /// Eventually, this method and all other sync-based methods should be deprecated to match C# style
        /// This is only kept now for backwards compatibility
        /// </summary>
        /// <param name="xmlRequest">The XML to send to the API</param>
        /// <returns>The XML response as a string on success, or null otherwise</returns>
        public virtual string HttpPost(string xmlRequest)
        {
            var source = new CancellationTokenSource();
            var asyncTask = Task.Run(() => HttpPostAsync(xmlRequest, source.Token), source.Token);
            asyncTask.Wait(source.Token);
            return asyncTask.Result;
        }

        public virtual void FtpDropOff(string fileDirectory, string fileName)
        {
            SftpClient sftpClient;

            var filePath = Path.Combine(fileDirectory, fileName);

            _logger.LogDebug($"Sftp Url: {_config.SftpUrl}");
            _logger.LogDebug($"Username: {_config.SftpUsername}");
            //_logger.LogDebug($"Password: {_config.SftpPassword}");

            sftpClient = new SftpClient(_config.SftpUrl, _config.SftpUsername, _config.SftpPassword);

            try
            {
                sftpClient.Connect();
            }
            catch (SshConnectionException e)
            {
                throw new CnpOnlineException("Error occured while establishing an SFTP connection", e);
            }
            catch (SshAuthenticationException e)
            {
                throw new CnpOnlineException("Error occured while attempting to establish an SFTP connection", e);
            }

            try {
                _logger.LogInformation($"Dropping off local file {filePath} to inbound/{fileName}.prg");

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                sftpClient.UploadFile(fileStream, "inbound/" + fileName + ".prg");
                fileStream.Close();

                _logger.LogInformation($"File copied - renaming from inbound/ {fileName}.prg to inbound/ {fileName}.asc"); 

                sftpClient.RenameFile($"inbound/{fileName}.prg", $"inbound/{fileName}.asc");
            }
            catch (SshConnectionException e) {
                throw new CnpOnlineException("Error occured while attempting to upload and save the file to SFTP", e);
            }
            catch (SshException e) {
                throw new CnpOnlineException("Error occured while attempting to upload and save the file to SFTP", e);
            }
            finally {
                sftpClient.Disconnect();
            }
        }

        public virtual void FtpPoll(string fileName, int timeout)
        {
            fileName = fileName + ".asc";
            _logger.LogDebug($"Polling for outbound result file.  Timeout set to {timeout}ms. File to wait for is {fileName}");

            SftpClient sftpClient;

            sftpClient = new SftpClient(_config.SftpUrl, _config.SftpUsername, _config.SftpPassword);

            try
            {

                sftpClient.Connect();

            }
            catch (SshConnectionException e)
            {
                throw new CnpOnlineException("Error occured while establishing an SFTP connection", e);
            }
            catch (SshAuthenticationException e)
            {
                throw new CnpOnlineException("Error occured while attempting to establish an SFTP connection", e);
            }

            SftpFileAttributes sftpAttrs = null;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            do
            {
                _logger.LogDebug($"Elapsed time is {stopWatch.Elapsed.TotalMilliseconds}");

                try
                {
                    sftpAttrs = sftpClient.Get("outbound/" + fileName).Attributes;
                    _logger.LogDebug($"Attrs of file are: {getSftpFileAttributes(sftpAttrs)}");
                }
                catch (SshConnectionException e)
                {
                    _logger.LogDebug(e.Message);

                    System.Threading.Thread.Sleep(30000);
                }
                catch (SftpPathNotFoundException e)
                {
                    _logger.LogDebug(e.Message);

                    System.Threading.Thread.Sleep(30000);
                }
            } while (sftpAttrs == null && stopWatch.Elapsed.TotalMilliseconds <= timeout);

            // Close the connections.
            sftpClient.Disconnect();
        }

        public virtual void FtpPickUp(string destinationFilePath, string fileName)
        {
            SftpClient sftpClient;

            sftpClient = new SftpClient(_config.SftpUrl, _config.SftpUsername, _config.SftpPassword);

            try
            {
                sftpClient.Connect();
            }
            catch (SshConnectionException e)
            {
                throw new CnpOnlineException("Error occured while attempting to establish an SFTP connection", e);
            }

            try {
                _logger.LogDebug($"Picking up remote file outbound/{fileName}.asc");
                _logger.LogDebug($"Putting it at {destinationFilePath}");

                FileStream downloadStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite);
                sftpClient.DownloadFile("outbound/" + fileName + ".asc", downloadStream);
                downloadStream.Close();

                _logger.LogDebug($"Removing remote file output/{fileName}.asc");

                sftpClient.Delete("outbound/" + fileName + ".asc");
            }
            catch (SshConnectionException e) {
                throw new CnpOnlineException("Error occured while attempting to retrieve and save the file from SFTP",
                    e);
            }
            catch (SftpPathNotFoundException e) {
                throw new CnpOnlineException("Error occured while attempting to locate desired SFTP file path", e);
            }
            finally {
                sftpClient.Disconnect();
            }
        }

        public enum RequestType
        {
            Request, Response
        }

        public class HttpActionEventArgs : EventArgs
        {
            public RequestType RequestType { get; set; }
            public string XmlPayload;

            public HttpActionEventArgs(RequestType requestType, string xmlPayload)
            {
                RequestType = requestType;
                XmlPayload = xmlPayload;
            }
        }

        private string getSftpFileAttributes(SftpFileAttributes sftpAttrs)
        {
            var permissions = sftpAttrs.GetBytes().ToString();

            return $@"Permissions: {permissions} 
                | UserID: {sftpAttrs.UserId} 
                | GroupID: {sftpAttrs.GroupId} 
                | Size: {sftpAttrs.Size} 
                | LastEdited: {sftpAttrs.LastWriteTime.ToString()}";
        }

        public struct SshConnectionInfo
        {
            public string Host;
            public string User;
            public string Pass;
            public string IdentityFile;
        }
    }
}

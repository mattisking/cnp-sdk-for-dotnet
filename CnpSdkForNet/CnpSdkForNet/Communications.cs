﻿using System;
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
        private readonly Dictionary<string, string> _config;

        /// <summary>
        /// The main constructor, which initializes the config and HttpClient
        /// </summary>
        /// <param name="config"></param>
        public Communications(HttpClient httpClient, Dictionary<string, string> config = null)
        {
            _config = config ?? new ConfigManager().getConfig();
            _client = httpClient;
        }

        ///// <summary>
        ///// A no-arg constructor that simply calls the main constructor, primarily used for mocking in tests
        /////   This constructor serves no other purpose than to keep the tests passing
        ///// </summary>
        //public Communications() : this(null) { }

        private void OnHttpAction(RequestType requestType, string xmlPayload)
        {
            if (HttpAction == null) return;
 
            NeuterXml(ref xmlPayload);
            NeuterUserCredentials(ref xmlPayload);

            HttpAction(this, new HttpActionEventArgs(requestType, xmlPayload));
        }

        //public static bool ValidateServerCertificate(
        //     object sender,
        //     X509Certificate certificate,
        //     X509Chain chain,
        //     SslPolicyErrors sslPolicyErrors)
        //{
        //    if (sslPolicyErrors == SslPolicyErrors.None)
        //        return true;

        //    Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

        //    // Do not allow this client to communicate with unauthenticated servers.
        //    return false;
        //}

        /// <summary>
        /// Obfuscates account information in the XML, only if the config value specifies to do so
        /// </summary>
        /// <param name="inputXml">the XML to obfuscate</param>
        public void NeuterXml(ref string inputXml)
        {
            var neuterAccountNumbers = 
                _config.ContainsKey("neuterAccountNums") && "true".Equals(_config["neuterAccountNums"]);
            if (!neuterAccountNumbers) return;
            
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
            var neuterUserCredentials =
                _config.ContainsKey("neuterUserCredentials") && "true".Equals(_config["neuterUserCredentials"]);
            if (!neuterUserCredentials) return;

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
            var printXml = _config.ContainsKey("printxml") && "true".Equals(_config["printxml"]);

            // Log any data to the appropriate places, only if we need to
            if (printXml)
            {
                Console.WriteLine(xmlRequest);
            }

            // Now that we have gotten the values for logging from the config, we need to actually send the request
            try
            {
                OnHttpAction(RequestType.Request, xmlRequest);
                var xmlContent = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");

                if (RequireApiKey())
                {
                    xmlContent.Headers.Add("apikey", _config["apikey"]);
                }

                var response = await _client.PostAsync(_config["url"], xmlContent, cancellationToken);
                var xmlResponse = await response.Content.ReadAsStringAsync();
                OnHttpAction(RequestType.Response, xmlResponse);

                if (printXml)
                {
                    Console.WriteLine(xmlResponse);
                }

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

        /// <summary>
        /// Determines if an apikey is needed based on the object's configuration
        /// </summary>
        /// <returns>Whether or not an apikey should be used</returns>
        public bool RequireApiKey()
        {
            return IsValidConfigValueSet("apiKey");
        }

        /// <summary>
        /// Determines whether the specified parameter is properly set in the configuration
        /// </summary>
        /// <param name="propertyName">The property to check for in the config</param>
        /// <returns>Whether or not propertyName is properly set in _config</returns>
        public bool IsValidConfigValueSet(string propertyName)
        {
            return _config.ContainsKey(propertyName) && !string.IsNullOrEmpty(_config[propertyName]);
        }

        public virtual void FtpDropOff(string fileDirectory, string fileName)
        {
            SftpClient sftpClient;

            var url = _config["sftpUrl"];
            var username = _config["sftpUsername"];
            var password = _config["sftpPassword"];
            var filePath = Path.Combine(fileDirectory, fileName);

            var printxml = _config["printxml"] == "true";
            if (printxml)
            {
                Console.WriteLine("Sftp Url: " + url);
                Console.WriteLine("Username: " + username);
                // Console.WriteLine("Password: " + password);
            }

            sftpClient = new SftpClient(url, username, password);

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
                if (printxml) {
                    Console.WriteLine("Dropping off local file " + filePath + " to inbound/" + fileName + ".prg");
                }

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                sftpClient.UploadFile(fileStream, "inbound/" + fileName + ".prg");
                fileStream.Close();
                if (printxml) {
                    Console.WriteLine("File copied - renaming from inbound/" + fileName + ".prg to inbound/" +
                                      fileName + ".asc");
                }

                sftpClient.RenameFile("inbound/" + fileName + ".prg", "inbound/" + fileName + ".asc");
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
            var printxml = _config["printxml"] == "true";
            if (printxml)
            {
                Console.WriteLine("Polling for outbound result file.  Timeout set to " + timeout + "ms. File to wait for is " + fileName);
            }

            SftpClient sftpClient;

            var url = _config["sftpUrl"];
            var username = _config["sftpUsername"];
            var password = _config["sftpPassword"];

            sftpClient = new SftpClient(url, username, password);

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
                if (printxml)
                {
                    Console.WriteLine("Elapsed time is " + stopWatch.Elapsed.TotalMilliseconds);
                }
                try
                {
                    sftpAttrs = sftpClient.Get("outbound/" + fileName).Attributes;
                    if (printxml)
                    {
                        Console.WriteLine("Attrs of file are: " + getSftpFileAttributes(sftpAttrs));
                    }
                }
                catch (SshConnectionException e)
                {
                    if (printxml)
                    {
                        Console.WriteLine(e.Message);
                    }
                    System.Threading.Thread.Sleep(30000);
                }
                catch (SftpPathNotFoundException e)
                {
                    if (printxml)
                    {
                        Console.WriteLine(e.Message);
                    }
                    System.Threading.Thread.Sleep(30000);
                }
            } while (sftpAttrs == null && stopWatch.Elapsed.TotalMilliseconds <= timeout);

            // Close the connections.
            sftpClient.Disconnect();
        }

        public virtual void FtpPickUp(string destinationFilePath, string fileName)
        {
            SftpClient sftpClient;

            var printxml = _config["printxml"] == "true";
            var url = _config["sftpUrl"];
            var username = _config["sftpUsername"];
            var password = _config["sftpPassword"];

            sftpClient = new SftpClient(url, username, password);

            try
            {
                sftpClient.Connect();
            }
            catch (SshConnectionException e)
            {
                throw new CnpOnlineException("Error occured while attempting to establish an SFTP connection", e);
            }

            try {
                if (printxml) {
                    Console.WriteLine("Picking up remote file outbound/" + fileName + ".asc");
                    Console.WriteLine("Putting it at " + destinationFilePath);
                }

                FileStream downloadStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite);
                sftpClient.DownloadFile("outbound/" + fileName + ".asc", downloadStream);
                downloadStream.Close();
                if (printxml) {
                    Console.WriteLine("Removing remote file output/" + fileName + ".asc");
                }

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

        private String getSftpFileAttributes(SftpFileAttributes sftpAttrs)
        {
            String permissions = sftpAttrs.GetBytes().ToString();
            return "Permissions: " + permissions
                                   + " | UserID: " + sftpAttrs.UserId
                                   + " | GroupID: " + sftpAttrs.GroupId
                                   + " | Size: " + sftpAttrs.Size
                                   + " | LastEdited: " + sftpAttrs.LastWriteTime.ToString();
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

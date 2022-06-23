using Cnp.Sdk.Core;
using Cnp.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Cnp.Sdk
{
    // Represent cnpRequest, which contains multiple batches.
    public class cnpRequest
    {
        private readonly ICommunications _communications;

        private authentication _authentication;
        private Dictionary<string, string> _config;
        private cnpXmlSerializer _cnpXmlSerializer;
        private int _numOfCnpBatchRequest = 0;
        private int _numOfRFRRequest = 0;
        public string _finalFilePath = null;
        private string _batchFilePath = null;
        private string _requestDirectory;
        private string _responseDirectory;
        private cnpTime _cnpTime;
        private cnpFile _cnpFile;

        /**
         * Construct a Cnp online using the configuration specified in CnpSdkForNet.dll.config
         */
        public cnpRequest(ICommunications communications)
        {
            _config = new Dictionary<string, string>();
            ConfigManager configManager = new ConfigManager();
            _config = configManager.getConfig();
            _communications = communications;

            // Retrieve all the settings.
            //_config["url"] = Properties.Settings.Default.url;
            //_config["reportGroup"] = Properties.Settings.Default.reportGroup;
            //_config["username"] = Properties.Settings.Default.username;
            //_config["printxml"] = Properties.Settings.Default.printxml;
            //_config["timeout"] = Properties.Settings.Default.timeout;
            //_config["proxyHost"] = Properties.Settings.Default.proxyHost;
            //_config["merchantId"] = Properties.Settings.Default.merchantId;
            //_config["password"] = Properties.Settings.Default.password;
            //_config["proxyPort"] = Properties.Settings.Default.proxyPort;
            //_config["sftpUrl"] =  Properties.Settings.Default.sftpUrl;
            //_config["sftpUsername"] = Properties.Settings.Default.sftpUsername;
            //_config["sftpPassword"] = Properties.Settings.Default.sftpPassword;
            //_config["onlineBatchUrl"] = Properties.Settings.Default.onlineBatchUrl;
            //_config["onlineBatchPort"] = Properties.Settings.Default.onlineBatchPort;
            //_config["requestDirectory"] = Properties.Settings.Default.requestDirectory;
            //_config["responseDirectory"] = Properties.Settings.Default.responseDirectory;
            //_config["useEncryption"] = Properties.Settings.Default.useEncryption;
            //_config["vantivPublicKeyId"] = Properties.Settings.Default.vantivPublicKeyId;
            //_config["pgpPassphrase"] = Properties.Settings.Default.pgpPassphrase;

            initializeRequest();
        }

        /**
         * Construct a CnpOnline specifying the configuration in code.  This should be used by integration that have another way
         * to specify their configuration settings or where different configurations are needed for different instances of CnpOnline.
         * 
         * Properties that *must* be set are:
         * url (eg https://payments.cnp.com/vap/communicator/online)
         * reportGroup (eg "Default Report Group")
         * username
         * merchantId
         * password
         * timeout (in seconds)
         * Optional properties are:
         * proxyHost
         * proxyPort
         * printxml (possible values "true" and "false" - defaults to false)
         * sftpUrl
         * sftpUsername
         * sftpPassword
         * onlineBatchUrl
         * onlineBatchPort
         * requestDirectory
         * responseDirectory
         */
        public cnpRequest(ICommunications communications, Dictionary<string, string> config)
        {
            _communications = communications;
            _config = config;
            initializeRequest();
        }

        // 
        private void initializeRequest()
        {
            _authentication = new authentication();
            _authentication.user = _config["username"];
            _authentication.password = _config["password"];

            _requestDirectory = Path.Combine(_config["requestDirectory"],"Requests") + Path.DirectorySeparatorChar;
            _responseDirectory = Path.Combine(_config["responseDirectory"],"Responses") + Path.DirectorySeparatorChar;

            _cnpXmlSerializer = new cnpXmlSerializer();
            _cnpTime = new cnpTime();
            _cnpFile = new cnpFile();
        }

        public authentication getAuthenication()
        {
            return _authentication;
        }

        public string getRequestDirectory()
        {
            return _requestDirectory;
        }

        public string getResponseDirectory()
        {
            return _responseDirectory;
        }

        public ICommunications getCommunication()
        {
            return _communications;
        }

        public void setCnpXmlSerializer(cnpXmlSerializer cnpXmlSerializer)
        {
            _cnpXmlSerializer = cnpXmlSerializer;
        }

        public cnpXmlSerializer getCnpXmlSerializer()
        {
            return _cnpXmlSerializer;
        }

        public void setCnpTime(cnpTime cnpTime)
        {
            _cnpTime = cnpTime;
        }

        public cnpTime getCnpTime()
        {
            return _cnpTime;
        }

        public void setCnpFile(cnpFile cnpFile)
        {
            _cnpFile = cnpFile;
        }

        public cnpFile getCnpFile()
        {
            return _cnpFile;
        }

        // Add a single batch to batch request.
        public void addBatch(batchRequest cnpBatchRequest)
        {
            if (_numOfRFRRequest != 0)
            {
                throw new CnpOnlineException("Can not add a batch request to a batch with an RFRrequest!");
            }
            // Fill in report group attribute for cnpRequest xml element.
            fillInReportGroup(cnpBatchRequest);
            // Add batchRequest xml element into cnpRequest xml element.
            _batchFilePath = SerializeBatchRequestToFile(cnpBatchRequest, _batchFilePath);
            _numOfCnpBatchRequest++;
        }

        public void addRFRRequest(RFRRequest rfrRequest)
        {
            if (_numOfCnpBatchRequest != 0)
            {
                throw new CnpOnlineException("Can not add an RFRRequest to a batch with requests!");
            }
            else if (_numOfRFRRequest >= 1)
            {
                throw new CnpOnlineException("Can not add more than one RFRRequest to a batch!");
            }

            _batchFilePath = SerializeRFRRequestToFile(rfrRequest, _batchFilePath);
            _numOfRFRRequest++;
        }

        //public cnpResponse sendToCnpWithStream()
        //{
        //    var requestFilePath = this.Serialize();
        //    var batchName = Path.GetFileName(requestFilePath);

        //    var responseFilePath = communication.SocketStream(requestFilePath, responseDirectory, config);

        //    var cnpResponse = (cnpResponse)cnpXmlSerializer.DeserializeObjectFromFile(responseFilePath);
        //    return cnpResponse;
        //}

        public string sendToCnp()
        {
            var useEncryption = _config.ContainsKey("useEncryption")? _config["useEncryption"] : "false";
            var vantivPublicKeyId = _config.ContainsKey("vantivPublicKeyId")? _config["vantivPublicKeyId"] : "";
            
            var requestFilePath = this.Serialize();
            var batchRequestDir = _requestDirectory;
            var finalRequestFilePath = requestFilePath;
            if ("true".Equals(useEncryption))
            {
                batchRequestDir = Path.Combine(_requestDirectory, "encrypted");
                Console.WriteLine(batchRequestDir);
                finalRequestFilePath =
                    Path.Combine(batchRequestDir, Path.GetFileName(requestFilePath) + ".encrypted");
                _cnpFile.createDirectory(finalRequestFilePath);
                PgpHelper.EncryptFile(requestFilePath, finalRequestFilePath, vantivPublicKeyId);
            }

            _communications.FtpDropOff(batchRequestDir, Path.GetFileName(finalRequestFilePath));
            
            return Path.GetFileName(finalRequestFilePath);
        }


        public void blockAndWaitForResponse(string fileName, int timeOut)
        {
            _communications.FtpPoll(fileName, timeOut);
        }

        public cnpResponse receiveFromCnp(string batchFileName)
        {
            var useEncryption = _config.ContainsKey("useEncryption")? _config["useEncryption"] : "false";
            var pgpPassphrase = _config.ContainsKey("pgpPassphrase")? _config["pgpPassphrase"] : "";

            _cnpFile.createDirectory(_responseDirectory);
            
            var responseFilePath = Path.Combine(_responseDirectory, batchFileName);
            var batchResponseDir = _responseDirectory;
            var finalResponseFilePath = responseFilePath;

            if ("true".Equals(useEncryption))
            {
                batchResponseDir = Path.Combine(_responseDirectory, "encrypted");
                finalResponseFilePath =
                    Path.Combine(batchResponseDir, batchFileName);
                _cnpFile.createDirectory(finalResponseFilePath);
            }
            _communications.FtpPickUp(finalResponseFilePath, batchFileName);

            if ("true".Equals(useEncryption))
            {
                responseFilePath = responseFilePath.Replace(".encrypted", "");
                PgpHelper.DecryptFile(finalResponseFilePath, responseFilePath, pgpPassphrase);
            }

            var cnpResponse = _cnpXmlSerializer.DeserializeObjectFromFile(responseFilePath);
                        
            return cnpResponse;
        }

        // Serialize the batch into temp xml file, and return the path to it.
        public string SerializeBatchRequestToFile(batchRequest cnpBatchRequest, string filePath)
        {
            // Create cnpRequest xml file if not exist.
            // Otherwise, the xml file created, thus storing some batch requests.
            filePath = _cnpFile.createRandomFile(_requestDirectory, Path.GetFileName(filePath), "_temp_cnpRequest.xml", _cnpTime);
            // Serializing the batchRequest creates an xml for that batch request and returns the path to it.
            var tempFilePath = cnpBatchRequest.Serialize();
            // Append the batch request xml just created to the accummulating cnpRequest xml file.
            _cnpFile.AppendFileToFile(filePath, tempFilePath);
            // Return the path to temp xml file.
            return filePath;
        }

        public string SerializeRFRRequestToFile(RFRRequest rfrRequest, string filePath)
        {
            filePath = _cnpFile.createRandomFile(_requestDirectory, Path.GetFileName(filePath), "_temp_cnpRequest.xml", _cnpTime);
            var tempFilePath = rfrRequest.Serialize();

            _cnpFile.AppendFileToFile(filePath, tempFilePath);

            return filePath;
        }

        // Convert all batch objects into xml and place them in cnpRequest, then build the Session file.
        public string Serialize()
        {
            var xmlHeader = "<?xml version='1.0' encoding='utf-8'?>\r\n<cnpRequest version=\"" + CnpVersion.CurrentCNPXMLVersion + "\"" +
             " xmlns=\"http://www.vantivcnp.com/schema\" " +
             "numBatchRequests=\"" + _numOfCnpBatchRequest + "\">";

            var xmlFooter = "\r\n</cnpRequest>";

            // Create the Session file.
            _finalFilePath = _cnpFile.createRandomFile(_requestDirectory, Path.GetFileName(_finalFilePath), ".xml", _cnpTime);
            var filePath = _finalFilePath;

            // Add the header into the Session file.
            _cnpFile.AppendLineToFile(_finalFilePath, xmlHeader);
            // Add authentication.
            _cnpFile.AppendLineToFile(_finalFilePath, _authentication.Serialize());

            // batchFilePath is not null when some batch is added into the batch request.
            if (_batchFilePath != null)
            {
                _cnpFile.AppendFileToFile(_finalFilePath, _batchFilePath);
            }
            else
            {
                throw new CnpOnlineException("No batch was added to the CnpBatch!");
            }

            // Add the footer into Session file
            _cnpFile.AppendLineToFile(_finalFilePath, xmlFooter);

            _finalFilePath = null;

            return filePath;
        }

        private void fillInReportGroup(batchRequest cnpBatchRequest)
        {
            if (cnpBatchRequest.reportGroup == null)
            {
                cnpBatchRequest.reportGroup = _config["reportGroup"];
            }
        }

    }

    public class cnpFile
    {
        // Create a file with name and timestamp if not exists.
        public virtual string createRandomFile(string fileDirectory, string fileName, string fileExtension, cnpTime cnpTime)
        {
            string filePath = null;
            if (string.IsNullOrEmpty(fileName))
            {
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                fileName = cnpTime.getCurrentTime("MM-dd-yyyy_HH-mm-ss-ffff_") + RandomGen.NextString(8);
                filePath = fileDirectory + fileName + fileExtension;

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                }
            }
            else
            {
                filePath = fileDirectory + fileName;
            }

            return filePath;
        }

        public virtual string AppendLineToFile(string filePath, string lineToAppend)
        {
            using (var fs = new FileStream(filePath, FileMode.Append))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(lineToAppend);
            }

            return filePath;
        }


        public virtual string AppendFileToFile(string filePathToAppendTo, string filePathToAppend)
        {

            using (var fs = new FileStream(filePathToAppendTo, FileMode.Append))
            using (var fsr = new FileStream(filePathToAppend, FileMode.Open))
            {
                var buffer = new byte[16];

                var bytesRead = 0;

                do
                {
                    bytesRead = fsr.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, bytesRead);
                }
                while (bytesRead > 0);
            }

            File.Delete(filePathToAppend);

            return filePathToAppendTo;
        }

        public virtual void createDirectory(string destinationFilePath)
        {
            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
        }

    }

    public static class RandomGen
    {
        private static RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();
        private static Random _local;
        public static int NextInt()
        {
            var inst = _local;
            if (inst == null)
            {
                var buffer = new byte[8];
                _global.GetBytes(buffer);
                _local = inst = new Random(BitConverter.ToInt32(buffer, 0));
            }

            return _local.Next();
        }

        public static string NextString(int length)
        {
            var result = "";

            for (var i = 0; i < length; i++)
            {
                result += Convert.ToChar(NextInt() % ('Z' - 'A') + 'A');
            }

            return result;
        }
    }

    public class cnpTime
    {
        public virtual string getCurrentTime(string format)
        {
            return DateTime.Now.ToString(format);
        }
    }

}

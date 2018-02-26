using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Core
{
    public abstract class RemoteFile
    {
        public virtual string MapPath(string serverUrl, string path)
        {
            path = path.Replace("~/", "").Replace("//", "").TrimStart('/');
            return serverUrl + "/" + path;
        }
        abstract public HttpStatusCode CheckIfFileExistsOnServer(string urlString);
        abstract public bool UploadFile(byte[] fileData, string urlString);
        abstract public byte[] DownloadFile(string urlString);
        abstract public bool DeleteFile(string urlString);
    }

    public class HttpRemoteFile : RemoteFile
    {
        public override string MapPath(string serverUrl, string path)
        {
            serverUrl = "http://" + serverUrl.Replace("http://", "").Replace("ftp://", "");
            return base.MapPath(serverUrl, path);
        }
        public override bool UploadFile(byte[] fileData, string urlString)
        {
            byte[] uploadData;
            try
            {
                uploadData = fileData;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.UploadData(urlString, "PUT", uploadData);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload", ex.InnerException);
            }
        }
        public override byte[] DownloadFile(string fileName)
        {
            byte[] downloadData;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    downloadData = client.DownloadData(fileName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to download", ex.InnerException);
            }
            return downloadData;
        }
        public override HttpStatusCode CheckIfFileExistsOnServer(string urlString)
        {

            // create the request
            HttpWebRequest request = WebRequest.Create(urlString) as HttpWebRequest;
            // instruct the server to return headers only
            request.Method = "HEAD";

            // make the connection
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                // get the status code
                return response.StatusCode;
            }
            catch(Exception ex)
            {
                return HttpStatusCode.NotFound;
            }
        }
        public override bool DeleteFile(string urlString)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(urlString) as HttpWebRequest;
                request.Method = "DELETE";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload", ex.InnerException);
            }
        }
    }
    public enum WebProtocal
    {
        HTTP = 1,
        FTP = 2
    }
}

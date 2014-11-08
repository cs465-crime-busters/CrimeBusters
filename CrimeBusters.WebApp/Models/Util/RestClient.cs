using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CrimeBusters.WebApp.Models.Util
{
    // Got this from here. http://www.codeproject.com/Tips/497123/How-to-make-REST-requests-with-Csharp
    // We will deprecate this one when we migrate to .NET 4.5 wherein we will use the new async features. 
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    /// <summary>
    /// Summary description for RestClient
    /// </summary>
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public Dictionary<String, String> Header { get; set; }

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
            Header = new Dictionary<string, string>();
        }

        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
            Header = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
            Header = new Dictionary<string, string>();
        }

        public RestClient(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = postData;
            Header = new Dictionary<string, string>();
        }

        public void AddHeader(String key, String value)
        {
            Header.Add(key, value);
        }

        public string MakeRequest()
        {
            return MakeRequest("");
        }

        public string MakeRequest(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            foreach (String key in Header.Keys)
            {
                request.Headers.Add(key, Header[key]);
            }
            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                var bytes = Encoding.UTF8.GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;
                
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }
    } // class
}

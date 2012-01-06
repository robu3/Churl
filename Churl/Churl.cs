using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;

namespace churl
{
    /// <summary>
    /// chURL: C# library for sending and receiving data over HTTP(S). Or a peon to do your HTTP bidding.
    /// </summary>
    public static class Churl
    {
        /// <summary>
        /// Makes a request to the specified uri, via the specified method, transmitting any data passed in
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <param name="data">Optional data to be passed; additional values beyond the first will be ignored (a way to handle optional params in 3.5)</param>
        /// <returns></returns>
        public static HttpResponse Request(string method, string uri, IDictionary<string, string> headers, params string[] data)
        {
            // upcase the method for consistency
            method = method.ToUpper();

            // if GET, append data to querystring as is
            if (method == "GET" && data.Length > 0)
            {
                uri = String.Format("{0}?{1}", uri, data[0]);
            }
            
            // form the request
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = method.ToUpper();
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }

            // for non-GET requests, write any data to transmit to the request stream
            if (method != "GET" && data.Length > 0)
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data[0]);
                }
            }

            // get the response
            HttpWebResponse httpResponse = null;
            string responseData = "";
            try
            {
                httpResponse = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseData = reader.ReadToEnd();
                }

            }
            catch (System.Net.WebException ex)
            {
                // don't throw an exception for http error codes
                httpResponse = (HttpWebResponse) ex.Response;
				if (httpResponse != null)
				{
					using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
					{
						responseData = reader.ReadToEnd();
					}
				}
				else
				{
					responseData = ex.Message;
				}
            }

            if (httpResponse != null)
            {
                // able to get some kind of response
                return new HttpResponse((int)httpResponse.StatusCode, uri, responseData, httpResponse.Headers);
            }
            else
            {
                // shouldn't get here often, if ever
				// means there was no response from the server
                return new HttpResponse(500, uri, "No response from the server: " + responseData, null);
            }
        }

        /// <summary>
        /// Makes a request to the specified uri, via the specified method, transmitting any data passed in
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="data">Optional data to be passed; additional values beyond the first will be ignored (a way to handle optional params in 3.5)</param>
        /// <returns></returns>
        public static HttpResponse Request(string method, string uri, params string[] data)
		{
			return Request(method, uri, null, data);
		}

        /// <summary>
        /// Makes a request to the specified uri, via the specified method, transmitting any key-value pair data passed in
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResponse Request(string method, string uri, IDictionary<string, string> headers, Dictionary<string, object> data)
        {
            // make string of key value pairs in the form of application/x-www-form-urlencoded
            StringBuilder buffer = new StringBuilder();
            foreach (string key in data.Keys)
            {
                buffer.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(data[key].ToString()));
            }
            if (buffer.Length > 0) buffer.Remove(buffer.Length - 1, 1);

            return Request(method, uri, headers, buffer.ToString());
        }

        /// <summary>
        /// Makes a request to the specified uri, via the specified method, transmitting any key-value pair data passed in
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResponse Request(string method, string uri, Dictionary<string, object> data)
		{
			return Request(method, uri, null, data);
		}

        /// <summary>
        /// Makes a POST request to the specified uri, via the specified method, transmitting any data passed in
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data">Optional data to be passed; additional values beyond the first will be ignored</param>
        /// <returns></returns>
        public static HttpResponse Post(string uri, params string[] data)
        {
            return Request("POST", uri, data);
        }

        /// <summary>
        /// Makes a POST request to the specified uri, via the specified method, transmitting any data passed in
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data">Key-value pairs to be sent</param>
        /// <returns></returns>
        public static HttpResponse Post(string uri, Dictionary<string, object> data)
        {
            return Request("POST", uri, data);
        }

        /// <summary>
        /// Makes a GET request to the specified uri, via the specified method, transmitting any data passed in
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data">Optional data to be passed; additional values beyond the first will be ignored</param>
        /// <returns></returns>
        public static HttpResponse Get(string uri, params string[] data)
        {
            return Request("GET", uri, data);
        }

        /// <summary>
        /// Makes a GET request to the specified uri, via the specified method, transmitting any data passed in
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data">Key-value pairs to be sent</param>
        /// <returns></returns>
        public static HttpResponse Get(string uri, Dictionary<string, object> data)
        {
            return Request("GET", uri, data);
        }
    }

    /// <summary>
    /// Thin wrapper for a HTML response
    /// </summary>
    public class HttpResponse
    {
        public int ResponseCode { get; set; }
        public string Uri { get; set; }
        public string Data { get; set; }
        public WebHeaderCollection Headers { get; set; }

        public HttpResponse(int responseCode, string uri, string data, WebHeaderCollection headers)
        {
            this.ResponseCode = responseCode;
            this.Data = data;
            this.Headers = headers;
        }
    }
}

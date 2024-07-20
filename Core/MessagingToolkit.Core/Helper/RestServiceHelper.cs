//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Collections.Specialized;
using System.Net.Cache;
using System.Web;

namespace MessagingToolkit.Core.Helper
{
    /// <summary>
    /// Helper class to invoke REST based services.
    /// </summary>
    internal sealed class RestServiceHelper
    {
        private static string JsonContentType = "application/json";
        private static string FormUrlEncodedContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// Perform HTTP GET operation.
        /// </summary>
        /// <typeparam name="T">The returned data type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T Get<T>(string url, string userName, string password)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = JsonContentType;
                client.Headers[HttpRequestHeader.Accept] = JsonContentType;

                if (!string.IsNullOrEmpty(userName))
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(userName, password);
                    client.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
                }

                // invoke the REST method
                byte[] data = client.DownloadData(url);

                // put the downloaded data in a memory stream
                MemoryStream ms = new MemoryStream();
                ms = new MemoryStream(data);

                // deserialize from json
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                T result = (T)ser.ReadObject(ms);
                ms.Close();
                return result;
            }
        }

        /// <summary>
        /// Perform HTTP GET operation.
        /// </summary>
        /// <typeparam name="T">The returned data type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T HttpGet<T>(string url, string userName, string password)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.ContentType = JsonContentType;
            if (!string.IsNullOrEmpty(userName))
            {
                request.UseDefaultCredentials = true;
                request.Credentials = new NetworkCredential(userName, password);
                request.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                T result = (T)ser.ReadObject(response.GetResponseStream());
                return result;
            }
        }


        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="nv">The nv.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T HttpPost<T>(string url, NameValueCollection nv, string userName, string password)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = JsonContentType;
            StringBuilder parameters = new StringBuilder();
            foreach (string key in nv) {
                parameters.Append(key);
                parameters.Append("=");
                parameters.Append(HttpUtility.UrlEncode(nv[key]));
                parameters.Append("&");
            }

            // Encode the parameters as form data:
            byte[] formData = UTF8Encoding.UTF8.GetBytes(parameters.ToString());
            request.ContentLength = formData.Length;

            // Send the request:
            using (Stream post = request.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                T result = (T)ser.ReadObject(response.GetResponseStream());
                return result;
            }
        }


        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The object.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T Post<T>(string url, T obj, string userName, string password)
        {
            return InvokeHttpMethod<T>("POST", url, obj, userName, password);
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="nv">The name value collection.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T Post<T>(string url, NameValueCollection nv, string userName, string password)
        {
            return InvokeHttpMethod<T>("POST", url, nv, userName, password);
        }

        /// <summary>
        /// Updates the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The object.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T Update<T>(string url, T obj, string userName, string password)
        {
            return InvokeHttpMethod<T>("PUT", url, obj, userName, password);
        }

        /// <summary>
        /// Updates the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="nv">The name value collection</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T Update<T>(string url, NameValueCollection nv, string userName, string password)
        {
            return InvokeHttpMethod<T>("PUT", url, nv, userName, password);
        }

        /// <summary>
        /// Deletes the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string Delete(string url, string id, string userName, string password)
        {
            return InvokeHttpMethod<string>("DELETE", url, id, userName, password);
        }

        /// <summary>
        /// Updates the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="nv">The name value collection</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T Delete<T>(string url, NameValueCollection nv, string userName, string password)
        {
            return InvokeHttpMethod<T>("DELETE", url, nv, userName, password);
        }


        /// <summary>
        /// Invokes the HTTP method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="url">The URL.</param>
        /// <param name="nv">The nv.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private static T InvokeHttpMethod<T>(string httpVerb, string url, NameValueCollection nv, string userName, string password)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = FormUrlEncodedContentType;
                client.Headers[HttpRequestHeader.Accept] = JsonContentType;

                if (!string.IsNullOrEmpty(userName))
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(userName, password);
                    client.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
                }

                byte[] data = client.UploadValues(url, httpVerb, nv);

                // deserialize the data returned by the service
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(data);
                T result = (T)ser.ReadObject(ms);
                ms.Close();
                return result;
            }
        }

        /// <summary>
        /// Invokes the HTTP method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The object.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static T InvokeHttpMethod<T>(string httpVerb, string url, T obj, string userName, string password)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = FormUrlEncodedContentType;
                client.Headers[HttpRequestHeader.Accept] = JsonContentType;

                if (!string.IsNullOrEmpty(userName))
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(userName, password);
                    client.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
                }

                // serialize the object data in json format
                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(ms, obj);

                // invoke the REST method
                byte[] data = client.UploadData(url, httpVerb, ms.ToArray());

                // deserialize the data returned by the service
                ms = new MemoryStream(data);
                T result = (T)ser.ReadObject(ms);
                ms.Close();
                return result;
            }
        }

    }
}

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
using System.Text;
using System.Net;
using System.Globalization;
using System.IO;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mm1
{
    /// <summary>
    /// MMS HTTP client for sending and downloading MMS messages
    /// </summary>
    internal class MmsHttpClient
    {
        /// <summary>
        /// HTTP POST method
        /// </summary>
        public const string MethodPost = "POST";

        /// <summary>
        /// HTTP GET method
        /// </summary>
        public const string MethodGet = "GET";

        private const string HeaderAcceptLanguage = "Accept-Language";
        private const string AcceptLangforUsLocale = "en-US";

        // The "Accept" header value
        private const string HeaderValueAccept = "*/*, application/vnd.wap.mms-message, application/vnd.wap.sic";

        // The "Content-Type" header value
        private const string HeaderValueContentTypeWithCharset = "application/vnd.wap.mms-message; charset=utf-8";
        private const string HeaderValueContentTypeWithoutCharset = "application/vnd.wap.mms-message";

        //private static readonly string MacroP = "##(\\S+)##";

        // The raw phone number from TelephonyManager.getLine1Number
        private const string MacroLine1 = "LINE1";

        // The phone number without country code
        private const string MacroLine1CountryCode = "LINE1NOCOUNTRYCODE";

        // NAI (Network Access Identifier), used by Sprint for authentication
        private const string MacroNai = "NAI";


        /// <summary>
        /// Constructor
        /// </summary>
        public MmsHttpClient()
        {
        }

        /// <summary>
        /// Execute an MMS HTTP request, either a POST (sending) or a GET (downloading)
        /// </summary>
        /// <param name="urlString"> The request URL, for sending it is usually the MMSC, and for downloading it is the message URL </param>
        /// <param name="pdu"> For POST (sending) only, the PDU to send </param>
        /// <param name="method"> HTTP method, POST for sending and GET for downloading </param>
        /// <param name="isProxySet"> Is there a proxy for the MMSC </param>
        /// <param name="proxyHost"> The proxy host </param>
        /// <param name="proxyPort"> The proxy port </param>
        /// <param name="mmsConfig"> The MMS config to use </param>
        /// <param name="subId"> The subscription ID used to get line number, etc. </param>
        /// <param name="requestId"> The request ID for logging </param>
        /// <returns> The HTTP response body </returns>
        /// <exception cref="MmsHttpException"> For any failures </exception>
        public virtual byte[] Execute(string urlString, byte[] pdu,
                                        string method, bool isProxySet,
                                        string proxyHost, int proxyPort,
                                        MmsConfig mmsConfig,
                                        int subId, string requestId)
        {
            Logger.LogThis("HTTP: " + method + " " + RedactUrlForNonVerbose(urlString) + (isProxySet ? (", proxy=" + proxyHost + ":" + proxyPort) : "") + ", PDU size=" + (pdu != null ? pdu.Length : 0), LogLevel.Verbose, requestId);

            CheckMethod(method);
            HttpWebRequest httpWebRequest = null;
            try
            {
                WebProxy proxy = null;  // No proxy
                if (isProxySet && !string.IsNullOrEmpty(proxyHost))
                {
                    proxy = new WebProxy(proxyHost, proxyPort);
                }
                Uri uri = new Uri(urlString);

                // Now get the connection
                httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

                if (proxy != null)
                {
                    httpWebRequest.Proxy = proxy;
                }
                httpWebRequest.Timeout = mmsConfig.HttpSocketTimeout;

                // Further testing required
                if (!string.IsNullOrEmpty(mmsConfig.ApnUser))
                {
                    SetBasicAuthHeader(httpWebRequest, uri, mmsConfig.ApnUser, mmsConfig.ApnPassword);
                }

                // ------- COMMON HEADERS ---------

                // Header: Accept
                httpWebRequest.Accept = HeaderValueAccept;

                // Header: Accept-Language
                httpWebRequest.Headers.Add(HeaderAcceptLanguage, GetCurrentAcceptLanguage(CultureInfo.CurrentCulture));

                // Header: User-Agent
                string userAgent = mmsConfig.UserAgent;
                Logger.LogThis("HTTP: User-Agent=" + userAgent, LogLevel.Info, requestId);
                httpWebRequest.UserAgent = userAgent;
                //httpWebRequest.UserAgent =
                //    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

                // Header calling line id
                if (!string.IsNullOrEmpty(mmsConfig.CallingLineId))
                {
                    httpWebRequest.Headers.Add(MmsConfig.HeaderCallingLineId, mmsConfig.CallingLineId);
                    httpWebRequest.Headers.Add(MmsConfig.HeaderMsisdn, mmsConfig.CallingLineId);
                }

                // Header carrier magic
                if (!string.IsNullOrEmpty(mmsConfig.CarrierMagic))
                {
                    httpWebRequest.Headers.Add(MmsConfig.HeaderCarrierMagic, mmsConfig.CarrierMagic);
                }

                // Header: x-wap-profile
                string uaProfUrlTagName = mmsConfig.UaProfileTagName;
                string uaProfUrl = mmsConfig.UaProfileUrl;

                if (!string.IsNullOrEmpty(uaProfUrl))
                {
                    Logger.LogThis("HTTP: UaProfUrl=" + uaProfUrl, LogLevel.Info, requestId);
                    httpWebRequest.Headers.Add(uaProfUrlTagName, uaProfUrl);
                }

                // Add extra headers specified by mms_config.xml's httpparams
                AddExtraHeaders(httpWebRequest, mmsConfig, subId);

                // Different stuff for GET and POST
                if (MethodPost.Equals(method))
                {
                    if (pdu == null || pdu.Length < 1)
                    {
                        Logger.LogThis("HTTP: empty pdu", LogLevel.Error, requestId);
                        throw new MmsHttpException(0, "Sending empty PDU"); //statusCode
                    }
                    httpWebRequest.Method = MethodPost;
                    if (mmsConfig.SupportHttpCharsetHeader)
                    {
                        httpWebRequest.ContentType = HeaderValueContentTypeWithCharset;
                    }
                    else
                    {
                        httpWebRequest.ContentType = HeaderValueContentTypeWithoutCharset;
                    }
                    if (Logger.LogLevel == LogLevel.Verbose)
                    {
                        LogHttpHeaders(httpWebRequest.Headers, requestId);
                    }
                    using (Stream requestStream = httpWebRequest.GetRequestStream())
                    {
                        requestStream.Write(pdu, 0, pdu.Length);
                        requestStream.Flush();
                        requestStream.Close();
                        requestStream.Dispose();
                    }
                }
                else if (MethodGet.Equals(method))
                {
                    httpWebRequest.Method = MethodGet;

                    /* 
                    if (mmsConfig.SupportHttpCharsetHeader)
                    {
                        httpWebRequest.ContentType = HeaderValueContentTypeWithCharset;
                    }
                    else
                    {
                        httpWebRequest.ContentType = HeaderValueContentTypeWithoutCharset;
                    }
                    */

                    if (Logger.LogLevel == LogLevel.Verbose)
                    {
                        LogHttpHeaders(httpWebRequest.Headers, requestId);
                    }

                    //httpWebRequest.KeepAlive = false;

                    // To be further tested
                    //httpWebRequest.AllowAutoRedirect = false;
                    //httpWebRequest.Credentials = new NetworkCredential("mms", "mms");

                    //if (httpWebRequest.Proxy != null)
                    //{
                    //    httpWebRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    //}

                }
                // Get response
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                HttpStatusCode statusCode = httpWebResponse.StatusCode;

                if (Logger.LogLevel == LogLevel.Verbose)
                {
                    LogHttpHeaders(httpWebResponse.Headers, requestId);
                }

                if (statusCode != HttpStatusCode.OK)
                {
                    int responseCode = (int)statusCode;
                    string responseMessage = httpWebResponse.StatusDescription;
                    Logger.LogThis("HTTP: " + responseCode + " " + responseMessage, LogLevel.Info, requestId);
                    throw new MmsHttpException(responseCode, responseMessage);
                }

                // Get the stream containing content returned by the server.
                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] buf = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = responseStream.Read(buf, 0, buf.Length)) > 0)
                    {
                        ms.Write(buf, 0, bytesRead);
                    }
                    byte[] responseBody = ms.ToArray();
                    ms.Close();
                    responseStream.Close();
                    httpWebResponse.Close();
                    Logger.LogThis("HTTP: response size=" + (responseBody != null ? responseBody.Length : 0), LogLevel.Verbose, requestId);
                    return responseBody;
                }
            }
            catch (Exception e)
            {
                string redactedUrl = RedactUrlForNonVerbose(urlString);
                Logger.LogThis("Exception for URL " + redactedUrl, LogLevel.Error, requestId);
                Logger.LogThis(e.StackTrace, LogLevel.Error, requestId);
                throw new MmsHttpException(0, e.Message, e);
            }
            finally
            {
                //if (httpWebRequest != null)
                //{
                httpWebRequest = null;
                //}
            }
        }

        private static void LogHttpHeaders(WebHeaderCollection headers, string requestId)
        {
            StringBuilder sb = new StringBuilder();
            if (headers != null)
            {

                for (int i = 0; i < headers.Count; ++i)
                {
                    string header = headers.GetKey(i);
                    foreach (string value in headers.GetValues(i))
                    {
                        sb.Append(header).Append('=').Append(value).Append('\n');
                    }
                }
                Logger.LogThis("HTTP: headers\n" + sb.ToString(), LogLevel.Verbose, requestId);
            }
        }

        private static void CheckMethod(string method)
        {
            if (!MethodGet.Equals(method) && !MethodPost.Equals(method))
            {
                throw new MmsHttpException(0, "Invalid method " + method); // statusCode
            }
        }


        /// <summary>
        /// Return the Accept-Language header.  Use the current locale plus
        /// US if we are in a different locale than US.
        /// This code copied from the browser's WebSettings.java
        /// </summary>
        /// <returns> Current AcceptLanguage String. </returns>
        public static string GetCurrentAcceptLanguage(CultureInfo locale)
        {
            StringBuilder buffer = new StringBuilder();
            AddLocaleToHttpAcceptLanguage(buffer, locale);

            if (!AcceptLangforUsLocale.Equals(locale.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(", ");
                }
                buffer.Append(AcceptLangforUsLocale);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Convert obsolete language codes, including Hebrew/Indonesian/Yiddish,
        /// to new standard.
        /// </summary>
        private static string ConvertObsoleteLanguageCodeToNew(string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
            {
                return null;
            }
            if ("iw".Equals(langCode))
            {
                // Hebrew
                return "he";
            }
            else if ("in".Equals(langCode))
            {
                // Indonesian
                return "id";
            }
            else if ("ji".Equals(langCode))
            {
                // Yiddish
                return "yi";
            }
            return langCode;
        }

        private static void AddLocaleToHttpAcceptLanguage(StringBuilder builder, CultureInfo locale)
        {
            string language = ConvertObsoleteLanguageCodeToNew(locale.TwoLetterISOLanguageName);
            if (!string.IsNullOrEmpty(language))
            {
                builder.Append(language);

                var region = new RegionInfo(locale.LCID);
                string country = region.TwoLetterISORegionName;
                if (!string.IsNullOrEmpty(country))
                {
                    builder.Append("-");
                    builder.Append(country);
                }
            }
        }

        /// <summary>
        /// Add extra HTTP headers from mms_config.xml's httpParams, which is a list of key/value
        /// pairs separated by "|". Each key/value pair is separated by ":". Value may contain
        /// macros like "##LINE1##" or "##NAI##" which is resolved with methods in this class
        /// </summary>
        /// <param name="connection"> The HttpURLConnection that we add headers to </param>
        /// <param name="mmsConfig"> The MmsConfig object </param>
        /// <param name="subId"> The subscription ID used to get line number, etc. </param>
        private void AddExtraHeaders(HttpWebRequest connection, MmsConfig mmsConfig, int subId)
        {
            string extraHttpParams = mmsConfig.HttpParams;
            if (!string.IsNullOrEmpty(extraHttpParams))
            {
                // Parse the parameter list
                string[] paramList = extraHttpParams.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string paramPair in paramList)
                {
                    string[] splitPair = paramPair.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitPair.Length == 2)
                    {
                        string name = splitPair[0].Trim();
                        string value = ResolveMacro(splitPair[1].Trim(), mmsConfig, subId);
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                        {
                            // Add the header if the param is valid
                            connection.Headers.Add(name, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resolve the macro in HTTP param value text
        /// For example, "something##LINE1##something" is resolved to "something9139531419something"
        /// </summary>
        /// <param name="value"> The HTTP param value possibly containing macros </param>
        /// <param name="mmsConfig">MMS configruation</param>
        /// <param name="subId"> The subscription ID used to get line number, etc. </param>
        /// <returns> The HTTP param with macros resolved to real value </returns>
        private static string ResolveMacro(string value, MmsConfig mmsConfig, int subId)
        {
            /*
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            StringBuilder replaced = null;
            Regex regEx = new Regex(MacroP, RegexOptions.Compiled);
            foreach (Match match in regEx.Matches(value))
            {
                if (replaced == null)
                {
                    replaced = new StringBuilder();
                }
            }
            */

            return string.Empty;
        }

        /// <summary>
        /// Redact the URL for non-VERBOSE logging. Replace url with only the host part and the length
        /// of the input URL string.
        /// </summary>
        /// <param name="urlString">URL string</param>
        /// <returns>Formatted URL</returns>
        public static string RedactUrlForNonVerbose(string urlString)
        {
            if (Logger.LogLevel == LogLevel.Verbose)
            {
                // Don't redact for VERBOSE level logging
                return urlString;
            }
            if (string.IsNullOrEmpty(urlString))
            {
                return urlString;
            }

            string protocol = "http";
            string host = "";
            try
            {
                Uri url = new Uri(urlString);
                protocol = url.Scheme;
                host = url.Host;
            }
            catch (UriFormatException)
            {
                // Ignore
            }

            // Print "http://host[length]"
            StringBuilder sb = new StringBuilder();
            sb.Append(protocol).Append("://").Append(host).Append("[").Append(urlString.Length).Append("]");
            return sb.ToString();
        }


        /// <summary>
        /// Return the HTTP param macro value.
        /// Example: "LINE1" returns the phone number, etc.
        /// </summary>
        /// <param name="macro"> The macro name </param>
        /// <param name="mmsConfig"> The MMS config which contains NAI suffix. </param>
        /// <param name="subId"> The subscription ID used to get line number, etc. </param>
        /// <returns> The value of the defined macro </returns>
        private static string GetMacroValue(string macro, MmsConfig mmsConfig, int subId)
        {
            if (MacroLine1.Equals(macro))
            {
                return GetLine1(subId);
            }
            else if (MacroLine1CountryCode.Equals(macro))
            {
                return GetLine1NoCountryCode(subId);
            }
            else if (MacroNai.Equals(macro))
            {
                return GetNai(mmsConfig, subId);
            }
            Logger.LogThis("Invalid macro " + macro, LogLevel.Error);
            return null;
        }
        /// <summary>
        /// Returns the phone number for the given subscription ID.
        /// </summary>
        private static string GetLine1(int subId)
        {
            return string.Empty;
        }
        /// <summary>
        /// Returns the phone number (without country code) for the given subscription ID.
        /// </summary>
        private static string GetLine1NoCountryCode(int subId)
        {
            return string.Empty;
        }
        /// <summary>
        /// Returns the NAI (Network Access Identifier) from SystemProperties for the given subscription ID.
        /// </summary>
        private static string GetNai(MmsConfig mmsConfig, int subId)
        {
            return string.Empty;
        }

        /// <summary>
        /// Basic authentication in web request.
        /// 
        /// Refer to http://stackoverflow.com/questions/2764577/forcing-basic-authentication-in-webrequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void SetBasicAuthHeader(WebRequest request, string userName, string password)
        {
            request.PreAuthenticate = true;
            string authInfo = userName + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }

        /// <summary>
        /// Basic authentication in web request.
        /// 
        /// Refer to http://stackoverflow.com/questions/2764577/forcing-basic-authentication-in-webrequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userName"></param>
        /// <param name="uri"></param>
        /// <param name="password"></param>
        public void SetBasicAuthHeader(WebRequest request, Uri uri, string userName, string password)
        {
            request.PreAuthenticate = true;
            CredentialCache credentials = new CredentialCache();
            NetworkCredential netCredential = new NetworkCredential(userName, password);
            credentials.Add(uri, "Basic", netCredential);
            request.Credentials = credentials;
        }
    }
}
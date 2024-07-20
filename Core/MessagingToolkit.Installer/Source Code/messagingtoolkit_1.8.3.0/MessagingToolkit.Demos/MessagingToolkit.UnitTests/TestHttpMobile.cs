using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using MessagingToolkit.UnitTests.Models;
using Xunit;

namespace MessagingToolkit.UnitTests
{
    public class TestHttpMobile
    {

        private static void GetResponse(Uri uri, Action<Response> callback)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                    callback(ser.ReadObject(a.Result) as Response);
                }
            };
            wc.OpenReadAsync(uri);
        }

        /*
        private void GetPOSTResponse(Uri uri, string data, Action<Response> callback)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "text/plain;charset=utf-8";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Send the data.
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.BeginGetResponse((x) =>
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                        callback(ser.ReadObject(response.GetResponseStream()) as Response);
                    }
                }
            }, null);
        }

        Uri geocodeRequest = new Uri(string.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", query, key));

GetResponse(geocodeRequest, (x) =>
{
    Console.WriteLine(x.ResourceSets[0].Resources.Length + " result(s) found.");
    Console.ReadLine();
});

    */
        [Fact]
        public void TestMethod1()
        {

            Uri uri = new Uri("http://192.168.0.101:1688/services/api/messaging");

            // web client
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";

            // invoke the REST method
            byte[] data = client.DownloadData(
                   "http://192.168.0.101:1688/services/api/messaging");

            // put the downloaded data in a memory stream
            MemoryStream ms = new MemoryStream();
            ms = new MemoryStream(data);

            // deserialize from json
            DataContractJsonSerializer ser =
                   new DataContractJsonSerializer(typeof(Response));

            Response result = ser.ReadObject(ms) as Response;

            Console.WriteLine(result.RequestMethod);

           

        }
    }
}

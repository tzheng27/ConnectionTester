using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ASPNETConnectionTester.Controllers
{
    public class TesterController : Controller
    {

        [HttpPost]
        public ActionResult GetResponse(string url, List<string> headerNames, List<string> headerValues)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return Json(new { error = "Please enter a target URL" });
            }
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                if (headerNames != null)
                {
                    int headersNum = headerNames.Count();

                    for (int i = 0; i < headersNum; i++)
                    {
                        if (!String.IsNullOrWhiteSpace(headerNames[i]) && !String.IsNullOrWhiteSpace(headerValues[i]))
                            httpWebRequest.Headers.Add(headerNames[i], headerValues[i]);
                    }
                }
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var hReader = new System.IO.StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                {
                    string responseText = hReader.ReadToEnd();
                    return Json(new { statusCode = (int)httpResponse.StatusCode, response = responseText });
                }
            }
            catch (WebException e)
            {

                if (e.Response != null)
                {
                    string responseText;
                    using (var hReader = new System.IO.StreamReader(((HttpWebResponse)e.Response).GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        responseText = hReader.ReadToEnd();
                    }
                    return Json(new
                    {
                        statusCode = ((HttpWebResponse)e.Response).StatusCode,
                        statusDescription = ((HttpWebResponse)e.Response).StatusDescription,
                        errMessage = e.Message,
                        responseContent = responseText
                    });
                }
                else
                {
                    return Json(new { errMessage = e.Message });
                }

            }
            catch (Exception e)
            {
                return Json(new { errMessage = e.Message });
            }
        }



        public ActionResult PostResponse(string url, List<string> headerNames, List<string> headerValues, string content)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return Json(new { error = "Please enter a target URL" });
            }
            try
            {
                JObject.Parse(content);
            }
            catch(JsonException e) 
            {
                return Json(new {
                    error = "Invalid JSON format",
                    errMessage = e.Message
                });
            }

            try
            {
                var encoding = ASCIIEncoding.ASCII;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                if (headerNames != null)
                {
                    int headersNum = headerNames.Count();

                    for (int i = 0; i < headersNum; i++)
                    {
                        if (!String.IsNullOrWhiteSpace(headerNames[i]) && !String.IsNullOrWhiteSpace(headerValues[i]))
                            httpWebRequest.Headers.Add(headerNames[i], headerValues[i]);
                    }
                }
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using(var streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(content);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var hReader = new System.IO.StreamReader(httpResponse.GetResponseStream(), encoding))
                {
                    string responseText = hReader.ReadToEnd();
                    //return View("GetResponse");
                    return Json(new { statusCode = (int)httpResponse.StatusCode, response = responseText });
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    string responseText;
                    using (var hReader = new System.IO.StreamReader(((HttpWebResponse)e.Response).GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        responseText = hReader.ReadToEnd();
                    }
                    return Json(new
                    {
                        statusCode = ((HttpWebResponse)e.Response).StatusCode,
                        statusDescription = ((HttpWebResponse)e.Response).StatusDescription,
                        errMessage = e.Message,
                        responseContent = responseText
                    });
                }
                else
                {
                    return Json(new { errMessage = e.Message });
                }

            }
            catch (Exception e)
            {
                return Json(new { errMessage = e.Message });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using NUnit.Framework;


namespace Umbraco.Backend.Restriction.Test
{
    [TestFixture]
    public class HttpModuleTest
    {
        //make sure this params are correct.
        private const string BASE_URL = "http://localhost";
        private const string PORT = "18077";
        private static Uri baseUri = new Uri(BASE_URL + ":" + PORT);

        
        public void Check()
        {
            try
            {
                Uri testUri = new Uri(baseUri, "test.aspx");

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(testUri);
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                        "Check if the web server for Umbraco.Backend.Restriction.Test is running and has this host: " + BASE_URL);
                }

                HttpWebRequest requestHost = (HttpWebRequest)HttpWebRequest.Create(testUri);
                requestHost.Host = "allowed.local";
                requestHost.Method = "GET";

                using (HttpWebResponse responseHost = (HttpWebResponse)requestHost.GetResponse())
                {
                    Assert.AreEqual(HttpStatusCode.OK, responseHost.StatusCode,
                        "Make sure that host file and iis express has the correct bindings: http://stackoverflow.com/questions/4709014/using-custom-domains-with-iis-express");
                }

                HttpWebRequest requestHost_2 = (HttpWebRequest)HttpWebRequest.Create(testUri);
                requestHost_2.Host = "not.allowed.local";
                requestHost_2.Method = "GET";

                using (HttpWebResponse responseHost_2 = (HttpWebResponse)requestHost_2.GetResponse())
                {
                    Assert.AreEqual(HttpStatusCode.OK, responseHost_2.StatusCode,
                        "Make sure that host file and iis express has the correct bindings: http://stackoverflow.com/questions/4709014/using-custom-domains-with-iis-express");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Check if the web server for Umbraco.Backend.Restriction.Test is running and has this host: " + BASE_URL, ex);
            }
        }

        [Test]
        public void Restricted_UnSafeHost_CommonPath()
        {
            HttpWebRequest request = null;
            Uri testUri = new Uri(baseUri, "Umbraco/login.aspx");
            Uri testUri_2 = new Uri(baseUri, "UMBRACO/melmac/lOGin.aspx");
            Uri testUri_static = new Uri(baseUri, "umbraco/asset/img/pepe.png");
            Uri testUri_static_404 = new Uri(baseUri, "umbraco/asset/img/no_found.png");
            

            request = GetRequest(testUri, "GET", "NOT.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.Forbidden, request);

            request = GetRequest(testUri, "GET", null);
            EnsureStatus(HttpStatusCode.Forbidden, request);

            request = GetRequest(testUri_2, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.Forbidden, request);

            request = GetRequest(testUri_2, "GET", null);
            EnsureStatus(HttpStatusCode.Forbidden, request);

            request = GetRequest(testUri_static, "GET", "not.allowed.loCAL", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_static, "GET", null);
            EnsureStatus(HttpStatusCode.OK, request);

            
            request = GetRequest(testUri_static_404, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_static_404, "GET", null);
            EnsureStatus(HttpStatusCode.NotFound, request);


            
        }

        [Test]
        public void Restricted_SafeHost_CommonPath()
        {
            

            HttpWebRequest request = null;
            Uri testUri = new Uri(baseUri, "Umbraco/login.aspx");
            Uri testUri_2 = new Uri(baseUri, "UMBRACO/melmac/lOGin.aspx");
            Uri testUri_static = new Uri(baseUri, "umbraco/asset/img/pepe.png");
            Uri testUri_static_404 = new Uri(baseUri, "umbraco/asset/img/no_found.png");


            request = GetRequest(testUri, "GET", "aLLowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_2, "GET", "allowed.LOcal", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_static, "GET", "allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_static_404, "GET", "ALLOWED.local", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            

        }

        [Test]
        [Ignore("cant emulate another IP")]
        public void Restricted_SafeIp_CommonPath()
        {
            
            HttpWebRequest request = null;
            Uri testUri = new Uri(baseUri, "Umbraco/login.aspx");
            Uri testUri_2 = new Uri(baseUri, "UMBRACO/melmac/lOGin.aspx");
            Uri testUri_static = new Uri(baseUri, "umbraco/asset/img/pepe.png");
            Uri testUri_static_404 = new Uri(baseUri, "umbraco/asset/img/no_found.png");


            request = GetRequest(testUri, null, "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_2, null, "localhost", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_static, null, "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_static_404, null, "localhost", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);
        }

        [Test]
        public void Restricted_SafeHost_ServicesPath()
        {
            

            HttpWebRequest request = null;
            
            Uri testUri = new Uri(baseUri, "umbraco/plugins/DigibizAdvancedMediaPicker/DigibizService.asmx/GetInitDigibizTreeData");
            Uri testUri_2 = new Uri(baseUri, "umbraco/webservices/TreeDataService.ashx?rnd=cab48ce11f0947a59c64d7c37483063f&id=-1&treeType=DigibizMediaTree&contextMenu=false&isDialog=true&dialogMode=id&nodeKey=1000&rnd2=75.1");
            Uri testUri_3 = new Uri(baseUri, "umbraco/plugins/DigibizAdvancedMediaPicker/DigibizService.asmx/GetPickerData");
            Uri testUri_4 = new Uri(baseUri, "umbraco/webservices/legacyAjaxCalls.asmx/GetSecondsBeforeUserLogout");
            Uri testUri_5 = new Uri(baseUri, "umbraco/plugins/DigibizAdvancedMediaPicker/SelectMediaItem.aspx?startNodeId=-1&allowedExtensions=&allowedMediaTypes=1032&selectMultipleNodes=False&cropPropertyAlias=&cropName=&searchEnabled=True&autoSuggestEnabled=False&searchMethod=all&rndo=54.6");


            request = GetRequest(testUri, "GET", "aLLowed.local", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_2, "GET", "allowed.LOcal", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_3, "GET", "allowed.local", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_4, "GET", "ALLOWED.local", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_5, "GET", "ALLOWED.local", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);


            request = GetRequest(testUri, "GET", "ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_2, "GET", "ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_3, "GET", "ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_4, "GET", "ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_5, "GET", "ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            //ONE.SecuRe.largeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee.hoSt

        }

        [Test]
        [TestCase(
            "localhost",
            "umbraco/plugins/DigibizAdvancedMediaPicker/DigibizService.asmx/GetInitDigibizTreeData",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: localhost, resource: DigibizService.")]
        [TestCase(
            "localhost",
            "umbraco/webservices/TreeDataService.ashx?rnd=cab48ce11f0947a59c64d7c37483063f&id=-1&treeType=DigibizMediaTree&contextMenu=false&isDialog=true&dialogMode=id&nodeKey=1000&rnd2=75.1",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: localhost, resource: TreeDataService.")]
        [TestCase(
            "localhost",
            "umbraco/plugins/DigibizAdvancedMediaPicker/DigibizService.asmx/GetPickerData",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: localhost, resource: GetPickerData.")]
        [TestCase(
            "localhost",
            "umbraco/webservices/legacyAjaxCalls.asmx/GetSecondsBeforeUserLogout",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: localhost, resource: GetSecondsBeforeUserLogout.")]
        [TestCase(
            "localhost",
            "umbraco/plugins/DigibizAdvancedMediaPicker/SelectMediaItem.aspx?startNodeId=-1&allowedExtensions=&allowedMediaTypes=1032&selectMultipleNodes=False&cropPropertyAlias=&cropName=&searchEnabled=True&autoSuggestEnabled=False&searchMethod=all&rndo=54.6",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: localhost, resource: SelectMediaItem.")]

        [TestCase(
            "not.allowed.local",
            "umbraco/plugins/DigibizAdvancedMediaPicker/DigibizService.asmx/GetInitDigibizTreeData",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: not.allowed.local, resource: DigibizService.")]
        [TestCase(
            "not.allowed.local",
            "umbraco/webservices/TreeDataService.ashx?rnd=cab48ce11f0947a59c64d7c37483063f&id=-1&treeType=DigibizMediaTree&contextMenu=false&isDialog=true&dialogMode=id&nodeKey=1000&rnd2=75.1",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: not.allowed.local, resource: TreeDataService.")]
        [TestCase(
            "not.allowed.local",
            "umbraco/plugins/DigibizAdvancedMediaPicker/DigibizService.asmx/GetPickerData",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: not.allowed.local, resource: GetPickerData.")]
        [TestCase(
            "not.allowed.local",
            "umbraco/webservices/legacyAjaxCalls.asmx/GetSecondsBeforeUserLogout",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: not.allowed.local, resource: GetSecondsBeforeUserLogout.")]
        [TestCase(
            "not.allowed.local",
            "umbraco/plugins/DigibizAdvancedMediaPicker/SelectMediaItem.aspx?startNodeId=-1&allowedExtensions=&allowedMediaTypes=1032&selectMultipleNodes=False&cropPropertyAlias=&cropName=&searchEnabled=True&autoSuggestEnabled=False&searchMethod=all&rndo=54.6",
            Result = HttpStatusCode.Forbidden,
            TestName = "Restricted un Safe Host: not.allowed.local, resource: SelectMediaItem.")]
        public HttpStatusCode Restricted_UnSafeHost_ServicesPath(string host, string uri)
        {
            HttpWebRequest request = null;

            request = GetRequest(new Uri(baseUri, uri), "GET", host, PORT);
            return GetHttpStatus(request);
        }

        [TestCase(
            "not.allowed.local",
            "test",
            "user",
            "pinocho",
            Result = HttpStatusCode.Unauthorized,
            TestName = "Restricted by basic auth - wrong credentials")]
        [TestCase(
            "not.allowed.local",
            "test",
            "user",
            "pass",
            Result = HttpStatusCode.OK,
            TestName = "Restricted by basic auth - OK credentials")]
        [Ignore]
        public HttpStatusCode Restricted_By_BasicAuth(string host, string uri, string user, string pass)
        {
            HttpWebRequest request = null;
            NetworkCredential myCreds = new NetworkCredential(user, pass);

            request = GetRequest(new Uri(baseUri, uri), "GET", host, PORT, myCreds);
            return GetHttpStatus(request);
        }

        

        [Test]
        public void Allowed_FrontEnd_Navigation()
        {
            HttpWebRequest request = null;

            Uri testUri_index = new Uri(baseUri, "test");
            Uri testUri_details = new Uri(baseUri, "test/details/20");
            Uri testUri_post = new Uri(baseUri, "test/Create?askhda=asdas");
            Uri testUri_post_ii = new Uri(baseUri, "test/Create");// no trailing /


            request = GetRequest(testUri_index, "GET", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_details, "GET", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post, "POST", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post_ii, "GET", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_index, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_details, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post, "POST", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post_ii, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);
        }

        [Test]
        public void Allowed_Surface_Navigation()
        {
            HttpWebRequest request = null;

            Uri testUri_lost = new Uri(baseUri, "umbraco/surface/HomeSurface/imlost");
            Uri testUri_index = new Uri(baseUri, "umbraco/surface/HomeSurface");
            Uri testUri_details = new Uri(baseUri, "umbraco/surface/HomeSurface/Edit/589");
            Uri testUri_post = new Uri(baseUri, "umbraco/surface/HomeSurface/Create");
            Uri testUri_post_ii = new Uri(baseUri, "umbraco/surface/HomeSurface/Edit/589");


            request = GetRequest(testUri_lost, "GET", "localhost", PORT);
            EnsureStatus(HttpStatusCode.NotFound, request);

            request = GetRequest(testUri_index, "GET", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_details, "GET", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post, "POST", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post_ii, "POST", "localhost", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            ///

            request = GetRequest(testUri_index, "GET");
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_details, "GET");
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post, "POST");
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post_ii, "POST");
            EnsureStatus(HttpStatusCode.OK, request);
            
            ///

            request = GetRequest(testUri_index, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_details, "GET", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post, "POST", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post_ii, "POST", "not.allowed.local", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            ///

            request = GetRequest(testUri_index, "GET", "127.0.0.1", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_details, "GET", "127.0.0.1", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post, "POST", "127.0.0.1", PORT);
            EnsureStatus(HttpStatusCode.OK, request);

            request = GetRequest(testUri_post_ii, "POST", "127.0.0.1", PORT);
            EnsureStatus(HttpStatusCode.OK, request);
        }

        private void EnsureStatus(HttpStatusCode httpStatusCode, HttpWebRequest request)
        {
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Assert.AreEqual(httpStatusCode, response.StatusCode);
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Assert.AreEqual(httpStatusCode, httpResponse.StatusCode);
                }
            }
        }

        private HttpStatusCode GetHttpStatus(HttpWebRequest request)
        {
            HttpStatusCode result = HttpStatusCode.BadRequest;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    result = response.StatusCode;
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    result = httpResponse.StatusCode;
                }
            }
            return result;
        }


        private HttpWebRequest GetRequest(Uri url, string method = null, string host = null, string port = null, NetworkCredential credentials = null)
        {
            HttpWebRequest request = null;

            request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Host = host != null ? host : request.Host;
            request.Host += port != null ? ":"+port : string.Empty;
            request.Method = method != null ? method : request.Method;
            request.Credentials = credentials;

            return request;
        }
    }
}
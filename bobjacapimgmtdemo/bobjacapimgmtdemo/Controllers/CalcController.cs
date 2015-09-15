using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using bobjacapimgmtdemo.Models;

namespace bobjacapimgmtdemo.Controllers
{
    [Authorize]
    public class CalcController : ApiController
    {
        [Route("api/add")]
        [HttpGet]
        public HttpResponseMessage Add(int a, int b)
        {
            return GetResponseMessage(a + b);
        }
        //[HttpPost]
        //public HttpResponseMessage Add([FromBody]CalcInput input)
        //{
        //    return GetResponseMessage(input.a + input.b);
        //}

        [Route("api/sub")]
        [HttpPost]
        public HttpResponseMessage Sub([FromBody]CalcInput input)
        {
            return GetResponseMessage(input.a - input.b);
        }

        [Route("api/mul")]
        [HttpPost]
        public HttpResponseMessage Mul([FromBody]CalcInput input)
        {
            return GetResponseMessage(input.a * input.b);
        }

        [Route("api/div")]
        [HttpPost]
        public HttpResponseMessage Div([FromBody]CalcInput input)
        {
            return GetResponseMessage(input.a / input.b);
        }

        private HttpResponseMessage GetResponseMessage(int responseValue)
        {
            string xml = String.Format(
                "<result><value>{0}</value><broughtToYouBy>Azure API Management - http://azure.microsoft.com/apim/ </broughtToYouBy></result>", responseValue);

            var response = Request.CreateResponse();
            response.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");

            return response;
        }
    }
}

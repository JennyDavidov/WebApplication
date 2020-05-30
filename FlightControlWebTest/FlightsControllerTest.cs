using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControl;
using FlightControl.Controllers;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FlightControl.Models;
using System;

namespace FlightControlWebTest
{
    [TestClass]
    public class FlightsControllerTest
    {
        [TestMethod]
        public async Task ShouldReturnStatusCode()
        {
            //ARRANGE
            var mockFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient(new HttpMessageHandlerStub());
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            IHttpClientFactory factory = mockFactory.Object;

            FlightsController controller = new FlightsController(factory);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            //prepare the query parameters with external servers
            controller.ControllerContext.HttpContext.Request.QueryString = new QueryString("?sync_all");

            MyServersManager myServersManager = new MyServersManager();
            Servers server = new Servers();
            server.ServerId = "123";
            server.ServerURL = "http://mock.com";
            myServersManager.AddServer(server);


            //ACT
            var res = await controller.Get("2020-12-27T01:56:21Z");

            //ASSERT
            Assert.AreEqual(StatusCodes.Status400BadRequest, (res.Result as StatusCodeResult).StatusCode);


        }
    }

    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            throw new WebException("Bad Request", WebExceptionStatus.UnknownError);
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("This is a reply")
            };

            return await Task.FromResult(responseMessage);
        }
    }
}

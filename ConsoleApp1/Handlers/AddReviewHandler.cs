
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.Review.Requests;
using Messages.ServiceBusRequest.Review.Responses;
using ReviewService.Models;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace ReviewService.Handlers
{

    /// <summary>
    /// This is the handler class for the company search request 
    /// This class is created and its methods called by the NServiceBus framework
    /// </summary>
    public class AddReviewHandler : IHandleMessages<AddReviewRequest>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<AddReviewHandler>();

        /// <summary>
        /// Searches the db for the company, and returns its results back to the calling endpoint.
        /// </summary>
        /// <param name="message">Information about the echo</param>
        /// <param name="context">Used to access information regarding the endpoints used for this handle</param>
        /// <returns>The response to be sent back to the calling process</returns>
        public Task Handle(AddReviewRequest request, IMessageHandlerContext context)
        {
            Review reviewObject = new Review()
            {
                companyName = request.companyName,
                username = request.username,
                review = request.review,
                stars = request.stars,
                timestamp = request.timestamp
            };
            Reviews reviewsObject = new Reviews()
            {
                review = reviewObject
            };
            //Create client and submit post
            var companyReviewClient = new HttpClient();



            /////////////KEEP WORKING FROM HERE////////////////////////////




            var jsonObject = JsonConvert.SerializeObject(reviewsObject);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            //This url is the one where the rest call it to
            var res = companyReviewClient.PostAsync("http://35.188.47.76/Home/SaveCompanyReview", stringContent).Result.Content.ReadAsStringAsync().Result;
            Response resObject = JsonConvert.DeserializeObject<Response>(res);
            //If response bad, redirect indicating bad request, otherwise, good
            if (resObject.response == "failure")
            {
                return RedirectToAction("DisplayCompany", new { id = companyName, responseStatus = "Error occured when submitting review" });
            }
            else
                return RedirectToAction("DisplayCompany", new { id = companyName, responseStatus = "Review successfully saved" });
        }

        //Save the echo to the database
        ServiceBusResponse response = CompanyDirectoryServiceDatabase.getInstance().searchCompanyInfo(request.searchDeliminator);

            //The context is used to give a reply back to the endpoint that sent the request
            return context.Reply(response);
        }
    }
}
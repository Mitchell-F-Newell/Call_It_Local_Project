using Messages.NServiceBus.Commands;
using Messages.NServiceBus.Events;
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.AccuWeather;
using Messages.ServiceBusRequest.AccuWeather.Requests;
using Messages.ServiceBusRequest.AccuWeather.Responses;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Communication
{
    /// <summary>
    /// This portion of the class contains methods specifically for accessing the echo service.
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Listens for the client to secifify which task is being requested from the echo service
        /// </summary>
        /// <param name="request">Includes which task is being requested and any additional information required for the task to be executed</param>
        /// <returns>A response message</returns>
        private ServiceBusResponse accuWeatherRequest(AccuWeatherServiceRequest request)
        {
            switch (request.requestType)
            {
                case (AccuWeatherRequest.getAccuWeather):
                    return getAccuWeather((GetWeatherInfoRequest)request);
                default:
                    return new ServiceBusResponse(false, "Error: Invalid Request. Request received was:" + request.requestType.ToString());
            }
        }

        /// <summary>
        /// Publishes an EchoEvent.
        /// </summary>
        /// <param name="request">The data to be echo'd back to the client</param>
        /// <returns>The data sent by the client</returns>
        private GetWeatherInfoResponse getAccuWeather(GetWeatherInfoRequest request)
        {
          if(authenticated == false)
            {
                return new GetWeatherInfoResponse(false, "Error: you mussed be logged in to use this feature", null);
            };

 
            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("AccuWeather");

            // The Request<> funtion itself is an asynchronous operation. However, since we do not want to continue execution until the Request
            // function runs to completion, we call the ConfigureAwait, GetAwaiter, and GetResult functions to ensure that this thread
            // will wait for the completion of Request before continueing. 
            return requestingEndpoint.Request<GetWeatherInfoResponse>(request, sendOptions).
                ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}

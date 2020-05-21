using NServiceBus;
using System;
using Messages.NServiceBus.Commands;

namespace Messages.ServiceBusRequest.AccuWeather.Requests
{
    [Serializable]
    public class GetWeatherInfoRequest : AccuWeatherServiceRequest
    {
        public GetWeatherInfoRequest(string searchItem)
            : base(AccuWeatherRequest.getAccuWeather)
        {
            this.searchItem = searchItem;
        }

        /// <summary>
        /// Information used to search the database for companies
        /// </summary>
        public string searchItem;
    }
}
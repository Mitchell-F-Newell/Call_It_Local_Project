using Messages.DataTypes.Database.AccuWeather;

using System;

namespace Messages.ServiceBusRequest.AccuWeather.Responses
{
    [Serializable]
    public class GetWeatherInfoResponse : ServiceBusResponse
    {
        public GetWeatherInfoResponse(bool result, string response, AccuWeatherInfo data)
            : base(result, response)
        {
            this.data = data;
        }

        /// <summary>
        /// A list of companies matching the search criteria given by the client
        /// </summary>
        public AccuWeatherInfo data;
    }
}

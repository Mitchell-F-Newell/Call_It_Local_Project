using System;

namespace Messages.ServiceBusRequest.AccuWeather
{
    [Serializable]
    public class AccuWeatherServiceRequest : ServiceBusRequest
    {
        public AccuWeatherServiceRequest(AccuWeatherRequest requestType)
            : base(Service.AccuWeather)
        {
            this.requestType = requestType;
        }

        /// <summary>
        /// Indicates the type of request the client is seeking from the Company Directory Service
        /// </summary>
        public AccuWeatherRequest requestType;
    }

    public enum AccuWeatherRequest { getAccuWeather };
}

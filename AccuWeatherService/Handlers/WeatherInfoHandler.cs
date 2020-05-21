using WeatherService.AccuweatherAPI;

using Messages.ServiceBusRequest.AccuWeather.Requests;
using Messages.ServiceBusRequest.AccuWeather.Responses;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;

namespace AccuWeatherService.Handlers
{
    public class GetWeatherHandler : IHandleMessages<GetWeatherInfoRequest>
    {
        static ILog log = LogManager.GetLogger<GetWeatherHandler>();

        public Task Handle(GetWeatherInfoRequest request, IMessageHandlerContext context)
        {
            GetWeatherInfoResponse response = AccuweatherAPIReference.getInstance().getWeatherInfo(request.searchItem);

            return context.Reply(response);
        }
    }
}
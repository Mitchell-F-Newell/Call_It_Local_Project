using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.DataTypes.Database.AccuWeather
{
    /// <summary>
    /// This class represents all messages sent between two users since the accounts creation
    /// </summary>
    [Serializable]
    public partial class AccuWeatherInfo
    {
        public string locationName { get; set; }
        public string temperatureMetric { get; set; }
        public string realFeelTemperatureMetric { get; set; }
        public string weatherText { get; set; }
        public Int32 weatherIcon { get; set; }
    }
}
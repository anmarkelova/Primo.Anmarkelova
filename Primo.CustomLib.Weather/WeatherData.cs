using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Primo.CustomLib
{
    public enum WeatherAttribute
    {
        Temperature,
        MinTemperature,
        MaxTemperature,
        Pressure,
        Humidity
    }

    public class WeatherData
    {
        private const string GroupName = "main";

        public WeatherData() { }

        public string GetFullWeatherJson(string apiKey, string city)
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        public string GetWeatherAttribute(string apiKey, string city, WeatherAttribute attribute)
        {
            return ParseJsonByAttribute(GetFullWeatherJson(apiKey, city), GetJsonAttribute(attribute));
        }

        public static string ParseJsonByAttribute(string weatherJson, string attributeName)
        {
            var jsonObject = JObject.Parse(weatherJson);
            var nestedObject = jsonObject[GroupName] as JObject;
            var attributeValue = nestedObject?[attributeName]?.ToString();

            return attributeValue;
        }

        private static string GetJsonAttribute(WeatherAttribute attribute)
        {
            switch(attribute){
                case(WeatherAttribute.Temperature):
                    return "temp";
                case(WeatherAttribute.MinTemperature):
                    return "temp_min";
                case(WeatherAttribute.MaxTemperature):
                    return "temp_max";
                case (WeatherAttribute.Pressure):
                    return "pressure";
                case (WeatherAttribute.Humidity):
                    return "humidity";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using LTools.Common.Helpers;
using LTools.Common.Model;
using LTools.Common.UIElements;
using LTools.SDK;
using System.Net.Http;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows;
using System.Net;
using ValidationResult = LTools.Common.Model.ValidationResult;

namespace Primo.CustomLib.Weather.Activities
{
    public class CityTemperature : PrimoComponentTO<Design.CityTemperature_Form>
    {
        private const string GROUP_NAME = "CustomLib" + WFPublishedElementBase.TREE_SEPARATOR + "Погода OpenWeatherMap",
                             INPUT_CATEGORY = "Входящие параметры",
                             OUTPUT_CATEGORY = "Исходящие параметры",
                             ACTIVITY_NAME = "Температура в городе",
                             ACTIVITY_DESCRIPTION = "Активность позволяет получить текущую температуру в указанном городе.",
                             SUCCESS_MESSAGE = "Данные о погоде получены.";

        private const int ACTIVITY_TIMEOUT = 60000;

        #region [GROUP NAME]
        public override string GroupName
        {
            get => GROUP_NAME;
            protected set { }
        }
        #endregion

        #region [SDK TimeOut]
        protected override int sdkTimeOut
        {
            get => ACTIVITY_TIMEOUT;
            set { }
        }
        #endregion


        #region [GLOBAL VARIABLE]
        private string apiKey;
        private string city;
        private string temperature;
        #endregion


        #region [PROPERTIES]
        /// <summary>
        /// Свойство, которое хранит ключ API OpenWeatherMap.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("API Key")]
        public string ApiKey
        {
            get { return this.apiKey; }
            set { this.apiKey = value; this.InvokePropertyChanged(this, nameof(ApiKey)); }
        }

        /// <summary>
        /// Свойство, которое хранит название города.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("Город")]
        public string City
        {
            get { return this.city; }
            set { this.city = value; this.InvokePropertyChanged(this, nameof(City)); }
        }

        /// <summary>
        /// Свойство, которое хранит температуру в городе.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(OUTPUT_CATEGORY), System.ComponentModel.DisplayName("Температура")]
        public string Temperature
        {
            get { return this.temperature; }
            set { this.temperature = value; this.InvokePropertyChanged(this, nameof(Temperature)); }
        }
        #endregion


        #region [PROPERTIES][TYPES]
        public CityTemperature(IWFContainer container) : base(container)
        {
            sdkComponentName = ACTIVITY_NAME;
            sdkComponentHelp = ACTIVITY_DESCRIPTION;
            sdkComponentHelpUrl = WFElementBase.HELP_URL_ROOT;
            sdkComponentIcon = "pack://application:,,/Primo.CustomLib.Weather;component/Images/wether_icon.png";

            ///[PROPERTIES]
            ///[Input]
            sdkComponentHelp += "\r\n" + "\r\n" + "[Input]";

            sdkComponentHelp += "\r\n"
                             + nameof(ApiKey) + "*: API ключ к OpenWeatherMap.";

            sdkComponentHelp += "\r\n"
                             + nameof(City) + "*: Город, погоду которого нужно узнать.";

            ///[Output]
            sdkComponentHelp += "\r\n" + "\r\n" + "[Output]";

            sdkComponentHelp += "\r\n"
                             + nameof(Temperature) + "*: Температура в городе.";


            sdkProperties = new List<LTools.Common.Helpers.WFHelper.PropertiesItem>()
            {
                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(ApiKey),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.VARIABLE,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] API ключ к OpenWeatherMap", IsReadOnly = false
                },

                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(City),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.VARIABLE,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] Город поиска", IsReadOnly = false
                },

                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(Temperature),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.VARIABLE,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] Температура в городе по данным OpenWeatherMap", IsReadOnly = false
                },
            };
            InitClass(container);
        }
        #endregion


        #region [EXECUTION]
        /// <summary>
        /// Main action
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public override ExecutionResult TimedAction(ScriptingData sd)
        {
            try
            {
                string key = GetPropertyValue<string>(this.ApiKey, nameof(ApiKey), sd);
                string chosenCity = GetPropertyValue<string>(this.City, nameof(City), sd);

                var response = new WeatherData().GetWeatherAttribute(key, chosenCity, WeatherAttribute.Temperature);

                WFHelper.AssignToVariable(
                    this.Temperature,
                    response,
                    response.GetType(),
                    sd.Variables
                );

                return new ExecutionResult() { IsSuccess = true, SuccessMessage = SUCCESS_MESSAGE };
            }
            catch (Exception ex)
            {
                return new ExecutionResult() { IsSuccess = false, ErrorMessage = ex?.Message };
            }
        }
        #endregion


        #region [VALIDATE]
        /// <summary>
        /// Syntax check
        /// </summary>
        /// <returns></returns>
        public override ValidationResult Validate()
        {
            ValidationResult ret = new ValidationResult();
            if (String.IsNullOrEmpty(this.ApiKey)) ret.Items.Add(new ValidationResult.ValidationItem() { PropertyName = nameof(ApiKey), Error = "Ключ не указан!" });
            if (String.IsNullOrEmpty(this.City)) ret.Items.Add(new ValidationResult.ValidationItem() { PropertyName = nameof(City), Error = "Город не указан!" }); 
            return ret;
        }
        #endregion
    }
}

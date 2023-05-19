using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LTools.Common.Model;
using LTools.Common.UIElements;
using LTools.SDK;
using ValidationResult = LTools.Common.Model.ValidationResult;

namespace Primo.CustomLib.KPI.Activities
{
    public class ProcessingItemKpi_StopTimer : PrimoComponentTO<Design.ProcessingItemKpi_StopTimer_Form>
    {
        private const string GROUP_NAME = "CustomLib" + WFPublishedElementBase.TREE_SEPARATOR + "KPI",
                             INPUT_CATEGORY = "Входящие параметры",
                             ACTIVITY_NAME = "Выключить таймер",
                             ACTIVITY_DESCRIPTION = "Активность, которая позволяет завершить обработку элемента.",
                             VALIDATION_ERROR = "Не указана переменная!",
                             SUCCESS_MESSAGE = "Таймер выключен, обработка элемента завершена.";

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
        private string itemKpi;
        private string errorMessage;
        #endregion


        #region [PROPERTIES]
        /// <summary>
        /// Свойство, которое хранит данные о KPI элемента.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("ProcessingItemKpi")]
        public string ItemKpi
        {
            get { return this.itemKpi; }
            set { this.itemKpi = value; this.InvokePropertyChanged(this, nameof(ItemKpi)); }
        }

        /// <summary>
        /// Свойство, которое хранит сообщение об ошибке в случае неуспеха.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("Сообщение об ошибке")]
        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set { this.errorMessage = value; this.InvokePropertyChanged(this, nameof(ErrorMessage)); }
        }
        #endregion


        #region [PROPERTIES][TYPES]
        public ProcessingItemKpi_StopTimer(IWFContainer container) : base(container)
        {
            sdkComponentName = ACTIVITY_NAME;
            sdkComponentHelp = ACTIVITY_DESCRIPTION;
            sdkComponentHelpUrl = WFElementBase.HELP_URL_ROOT;
            sdkComponentIcon = "pack://application:,,/Primo.CustomLib.KPI;component/Images/item_kpi_icon.png";

            ///[PROPERTIES]
            ///[Input]
            sdkComponentHelp += "\r\n" + "\r\n" + "[Input]";

            sdkComponentHelp += "\r\n"
                             + nameof(ItemKpi) + "*: Объект типа ProcessingItemKpi";

            sdkComponentHelp += "\r\n"
                             + nameof(ErrorMessage) + ": Сообщение об ошибке (в случае неудачи)";


            sdkProperties = new List<LTools.Common.Helpers.WFHelper.PropertiesItem>()
            {
                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(ItemKpi),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.VARIABLE,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[ProcessingItemKpi] Переменная типа ProcessingItemKpi", IsReadOnly = false
                },

                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(ErrorMessage),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] Сообщение об ошибке", IsReadOnly = false
                }
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
                var processingItemKpi = GetPropertyValue<ProcessingItemKpi>(this.ItemKpi, nameof(ItemKpi), sd);

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    processingItemKpi.Stop();
                }
                else
                {
                    processingItemKpi.Stop(ErrorMessage);
                }

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
            if (this.ItemKpi is null) ret.Items.Add(new ValidationResult.ValidationItem() { PropertyName = nameof(ItemKpi), Error = VALIDATION_ERROR });
            return ret;
        }
        #endregion
    }
}

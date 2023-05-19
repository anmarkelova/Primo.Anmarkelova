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
using LTools.Common.Helpers;
using ValidationResult = LTools.Common.Model.ValidationResult;

namespace Primo.CustomLib.KPI.Activities
{
    public class ProcessingItemKpi_StartTimer : PrimoComponentTO<Design.ProcessingItemKpi_StartTimer_Form>
    {
        private const string GROUP_NAME = "CustomLib" + WFPublishedElementBase.TREE_SEPARATOR + "KPI",
                             INPUT_CATEGORY = "Входящие параметры",
                             ACTIVITY_NAME = "Включить таймер",
                             ACTIVITY_DESCRIPTION = "Активность, которая позволяет включить таймер обработки элемента.",
                             VALIDATION_ERROR = "Не указана переменная!",
                             SUCCESS_MESSAGE = "Таймер включен.";

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
        private string itemId;
        private string itemKpi;
        #endregion


        #region [PROPERTIES]
        /// <summary>
        /// Свойство, которое хранит уникальный идентификатор элемента.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("Item ID")]
        public string ItemId
        {
            get { return this.itemId; }
            set { this.itemId = value; this.InvokePropertyChanged(this, nameof(ItemId)); }
        }

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
        #endregion


        #region [PROPERTIES][TYPES]
        public ProcessingItemKpi_StartTimer(IWFContainer container) : base(container)
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
                             + nameof(ItemId) + ": Идентификатор элемента (при наличии)";


            sdkProperties = new List<LTools.Common.Helpers.WFHelper.PropertiesItem>()
            {
                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(ItemKpi),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.VARIABLE,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[ProcessingItemKpi] Объект item KPI", IsReadOnly = false
                },

                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(ItemId),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] ID элемента (при наличии)", IsReadOnly = false
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

                if (string.IsNullOrEmpty(ItemId))
                {
                    processingItemKpi.Start();
                }
                else
                {
                    processingItemKpi.Start(ItemId);
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
            if (String.IsNullOrEmpty(this.itemKpi)) ret.Items.Add(new ValidationResult.ValidationItem() { PropertyName = nameof(itemKpi), Error = VALIDATION_ERROR });
            return ret;
        }
        #endregion
    }
}

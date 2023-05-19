using LTools.Common.Helpers;
using LTools.Common.Model;
using LTools.Common.UIElements;
using LTools.Common.WFItems;
using LTools.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Primo.CustomLib.Text.Activities
{
    public class ChangeTextRegistry : PrimoComponentTO<Design.ChangeTextRegistry_Form>
    {
        private const string GROUP_NAME = "CustomLib" + WFPublishedElementBase.TREE_SEPARATOR + "Текст",
                             INPUT_CATEGORY = "Входящие параметры",
                             OUTPUT_CATEGORY = "Исходящие параметры",
                             ACTIVITY_NAME = "Изменение регистра текста",
                             ACTIVITY_DESCRIPTION = "Активность позволяет изменить регистр текста на выбранный (нижний, верхний или комбинированный).",
                             VALIDATION_ERROR = "Не определен!",
                             SUCCESS_MESSAGE = "Регистр текста изменен.";

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
        private string inputText;
        private Registry registry;
        private string outputText;
        #endregion


        #region [PROPERTIES]
        /// <summary>
        /// Свойство, которое хранит входящий текст, регистр которого требуется изменить.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [LTools.Common.Model.Studio.ValidateReturnScript(DataType = typeof(string))]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("Входящий текст")]
        public string InputText
        {
            get { return this.inputText; }
            set { this.inputText = value; this.InvokePropertyChanged(this, nameof(InputText)); }
        }

        /// <summary>
        /// Свойство, которое хранит регистр для применения.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [System.ComponentModel.Category(INPUT_CATEGORY), System.ComponentModel.DisplayName("Регистр написания")]
        [DefaultValue(Registry.LowerCase)]
        public Registry Registry
        {
            get { return this.registry; }
            set { this.registry = value; this.InvokePropertyChanged(this, nameof(Registry)); }
        }

        /// <summary>
        /// Свойство, которое хранит результат изменения регистра текста.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [LTools.Common.Model.Studio.ValidateReturnScript(DataType = typeof(string))]
        [System.ComponentModel.Category(OUTPUT_CATEGORY), System.ComponentModel.DisplayName("Результат изменения регистра")]
        public string OutputText
        {
            get { return this.outputText; }
            set { this.outputText = value; this.InvokePropertyChanged(this, nameof(OutputText)); }
        }
        #endregion


        #region [PROPERTIES][TYPES]
        public ChangeTextRegistry(IWFContainer container) : base(container)
        {
            sdkComponentName = ACTIVITY_NAME;
            sdkComponentHelp = ACTIVITY_DESCRIPTION;
            sdkComponentHelpUrl = WFElementBase.HELP_URL_ROOT;
            sdkComponentIcon = "pack://application:,,/Primo.CustomLib.Text;component/Images/text_registry_icon.png";

            ///[PROPERTIES]
            ///[Input]
            sdkComponentHelp += "\r\n" + "\r\n" + "[Input]";

            sdkComponentHelp += "\r\n"
                             + nameof(InputText) + "*: Входящий текст.";

            sdkComponentHelp += "\r\n"
                             + nameof(Registry) + "*: LowerCase (по умолчанию), UpperCase или TitleCase.";

            ///[Output]
            sdkComponentHelp += "\r\n" + "\r\n" + "[Output]";

            sdkComponentHelp += "\r\n"
                             + nameof(OutputText) + "*: Текст с измененным регистром написания.";


            sdkProperties = new List<LTools.Common.Helpers.WFHelper.PropertiesItem>()
            {
                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(InputText),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] Входящий текст", IsReadOnly = false
                },

                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(Registry),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(Registry), ToolTip = "[Registry] LowerCase/UpperCase/TitleCase", IsReadOnly = false
                },

                new LTools.Common.Helpers.WFHelper.PropertiesItem()
                {
                    PropName = nameof(OutputText),
                    PropertyType = LTools.Common.Helpers.WFHelper.PropertiesItem.PropertyTypes.VARIABLE,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] Переменная для передачи результата", IsReadOnly = false
                },
            };
            InitClass(container);
            this.InputText = this.IsNoCode(nameof(InputText)) ? "Замените на ваш текст..." : "\"Замените на ваш текст...\"";
            this.Registry = Registry.LowerCase;
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
                string inputText = GetPropertyValue<string>(this.InputText, nameof(InputText), sd);

                string result = "";

                switch (this.Registry)
                {
                    case Registry.LowerCase:
                        result = inputText.ToString().ToLower();
                        break;

                    case Registry.UpperCase:
                        result = inputText.ToString().ToUpper();
                        break;

                    case Registry.TitleCase:
                        result = RegistryClass.ToTitleCase(inputText);
                        break;
                }

                WFHelper.AssignToVariable(
                    this.OutputText,
                    result,
                    result.GetType(),
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
            if (String.IsNullOrEmpty(this.InputText)) ret.Items.Add(new ValidationResult.ValidationItem() { PropertyName = nameof(InputText), Error = VALIDATION_ERROR });
            return ret;
        }
        #endregion
    }
}

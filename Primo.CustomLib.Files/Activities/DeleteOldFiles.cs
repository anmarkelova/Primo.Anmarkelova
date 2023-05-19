using LTools.Common.Helpers;
using LTools.Common.Model;
using LTools.Common.UIElements;
using LTools.Common.WFItems;
using LTools.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Primo.CustomLib.Files.Activities
{
    public class DeleteOldFiles : PrimoComponentTO<Design.DeleteOldFiles_Form>
    {
        private const string GROUP_NAME = "CustomLib" + WFPublishedElementBase.TREE_SEPARATOR + "Файлы",
                             INPUT_CATEGORY = "Входящие параметры",
                             ACTIVITY_NAME = "Удаление старых файлов",
                             ACTIVITY_DESCRIPTION = "Активность позволяет удалить старые файлы за указанный период времени (например, старые лог-файлы робота).",
                             FOLDER_VALIDATION_ERROR = "Путь не указан!",
                             DAYS_VALIDATION_ERROR = "Количество дней должно быть положительным числом!",
                             EXTENSION_VALIDATION_ERROR = "Расширение файла не должно начинаться с точки!",
                             SUCCESS_MESSAGE = "Файлы удалены.";

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
        private Design.DeleteOldFiles_Form cbase;
        private string folder;
        private string days;
        private string extension;
        #endregion


        #region [PROPERTIES]
        /// <summary>
        /// Свойство, которое хранит в себе путь до целевой папки.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [LTools.Common.Model.Studio.ValidateReturnScript(DataType = typeof(string))]
        [Category(INPUT_CATEGORY), DisplayName("Путь до папки")]
        public string Folder
        {
            get { return this.folder; }
            set { this.folder = value; this.InvokePropertyChanged(this, nameof(Folder)); }
        }

        /// <summary>
        /// Свойство, которое хранит в себе количество дней, в рамках которых требуется хранить файлы.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [Category(INPUT_CATEGORY), DisplayName("Срок хранения файлов в днях")]
        public string Days
        {
            get { return this.days; }
            set { this.days = value; this.InvokePropertyChanged(this, nameof(Days)); }
        }

        /// <summary>
        /// Свойство, которое хранит в расширение файлов для проверки. Иные расширения игнорируются.
        /// </summary>
        [LTools.Common.Model.Serialization.StoringProperty]
        [LTools.Common.Model.Studio.ValidateReturnScript(DataType = typeof(string))]
        [Category(INPUT_CATEGORY), DisplayName("Расширение файла")]
        public string Extension
        {
            get { return this.extension; }
            set { this.extension = value; this.InvokePropertyChanged(this, nameof(Extension)); }
        }
        #endregion

        #region [PROPERTIES][TYPES]
        public DeleteOldFiles(IWFContainer container) : base(container)
        {
            sdkComponentName = ACTIVITY_NAME;
            sdkComponentHelp = ACTIVITY_DESCRIPTION;
            sdkComponentHelpUrl = WFElementBase.HELP_URL_ROOT;
            sdkComponentIcon = "pack://application:,,/Primo.CustomLib.Files;component/Images/files_deleting_icon.png";

            ///[PROPERTIES]
            ///[Input]
            sdkComponentHelp += "\r\n" + "\r\n" + "[Input]";

            sdkComponentHelp += "\r\n"
                             + nameof(Folder) + "*: Путь до папки.";

            sdkComponentHelp += "\r\n"
                             + nameof(Days) + "*: Число дней, по окончанию  которых необходимо удалить файл.";

            sdkComponentHelp += "\r\n"
                             + nameof(Extension) + ": Расширение файлов, которые требуется удалить (по умолчанию отсутствует). Пример: txt";


            sdkProperties = new List<WFHelper.PropertiesItem>()
            {
                new WFHelper.PropertiesItem()
                {
                    PropName = nameof(Folder),
                    PropertyType = WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.FOLDER_SELECTOR,
                    DataType = typeof(string), ToolTip = "[string] Путь", IsReadOnly = false
                },

                new WFHelper.PropertiesItem()
                {
                    PropName = nameof(Days),
                    PropertyType = WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[int] Количество дней хранения файлов", IsReadOnly = false
                },

                new WFHelper.PropertiesItem()
                {
                    PropName = nameof(Extension),
                    PropertyType = WFHelper.PropertiesItem.PropertyTypes.SCRIPT,
                    EditorType = ScriptEditorTypes.NONE,
                    DataType = typeof(string), ToolTip = "[string] Расширение файлов для проверки и удаления (без точки)", IsReadOnly = false
                }
            };
            InitClass(container);
            Days = "30";

            ///Create Instance of MyFirstActivity_Form
            try
            {
                Design.DeleteOldFiles_Form inputform = new Design.DeleteOldFiles_Form();


                this.cbase = inputform;
                this.cbase.DataContext = (object)this;
                WFElement wfElement = new WFElement((IWFElement)this, container);
                this.element = wfElement;

                ///Create Event if Buttons is clicked
                ///
                this.cbase.Form_btn.Click += new RoutedEventHandler(this.DeleteOldFiles_Form_btn_Click);
                this.element.Container = (FrameworkElement)this.cbase;
            }
            catch
            {
            }
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
                var folderPath = GetPropertyValue<string>(this.Folder, nameof(Folder), sd);
                var ext = GetPropertyValue<string>(this.Extension, nameof(Extension), sd);
                
                var daysObj = GetPropertyValue<object>(this.Days, nameof(Days), sd);
                var isSuccess = Int32.TryParse(daysObj.ToString(), out int daysInt);

                if (!isSuccess)
                {
                    throw new ArgumentException(DAYS_VALIDATION_ERROR);
                }

                var filesArr = GetFilesInFolder(folderPath, ext);
                DeleteFilesOlderThan(filesArr, daysInt);

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
            if (String.IsNullOrEmpty(this.Folder)) ret.Items.Add(new ValidationResult.ValidationItem() { PropertyName = nameof(Folder), Error = FOLDER_VALIDATION_ERROR });
            return ret;
        }
        #endregion

        #region [ADVANCED]

        /// <summary>
        /// Метод для отображения диалогового окна для выбора папки.
        /// </summary>
        private void DeleteOldFiles_Form_btn_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    if (this.IsNoCode(nameof(Folder)))
                    {
                        this.Folder = Path.GetFullPath(folderBrowserDialog.SelectedPath);
                    }
                    else
                    {
                        this.Folder = "@\"" + Path.GetFullPath(folderBrowserDialog.SelectedPath) + "\"";
                    }
                }
            }
        }

        /// <summary>
        /// Метод для определения списка файлов указанного расширения, которые хранятся в указанной папке.
        /// </summary>
        /// <param name="folderPath">Путь до целевой папки.</param>
        /// <param name="extension">Расширение файлов, которые требуется получить.</param>
        /// <returns>Перечень путей до файлов.</returns>
        static string[] GetFilesInFolder(string folderPath, string extension)
        {            
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException($"Папка '{folderPath}' не существует.", nameof(folderPath));
            }

            try
            {
                if (string.IsNullOrEmpty(extension))
                {
                    return Directory.GetFiles(folderPath);
                }
                else
                {
                    if (extension.Trim().StartsWith("."))
                    {
                        throw new ArgumentException(EXTENSION_VALIDATION_ERROR);
                    }

                    string searchPattern = $"*.{extension.Trim()}";
                    return Directory.GetFiles(folderPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch
            {
                throw new ArgumentException($"Файлы в папке '{folderPath}' не найдены.", nameof(folderPath));
            }
        }

        /// <summary>
        /// Метод для удаления файлов, если срок их хранения истек.
        /// </summary>
        /// <param name="files">Перечень файлов для проверки.</param>
        /// <param name="days">Срок хранения файлов в днях.</param>
        /// <returns></returns>
        static void DeleteFilesOlderThan(string[] files, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException(DAYS_VALIDATION_ERROR);
            }

            DateTime thresholdDate = DateTime.Now.AddDays(-days);

            foreach (string file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < thresholdDate)
                    {
                        fileInfo.Delete();
                        Console.WriteLine($"Удален: {file}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка удаления файла {file}: {ex.Message}");
                }
            }
        }
        #endregion
    }
}

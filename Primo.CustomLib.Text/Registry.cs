using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace Primo.CustomLib.Text
{
    /// <summary>
    /// Перечисление основных типов регистра.
    /// </summary>
    public enum Registry
    {
        /// <summary>
        /// Регистр написания текста, при котором заглавные буквы не пишутся.
        /// </summary>
        LowerCase,
        /// <summary>
        /// Регистр написания текста, при котором каждый символ пишется с большой буквы.
        /// </summary>
        UpperCase,
        /// <summary>
        /// Регистр написания текста, при котором первая буква каждого слова заглавная.
        /// </summary>
        TitleCase
        
    }

    public class RegistryClass
    {
        /// <summary>
        /// Метод для приведения текста к регистру Title.
        /// </summary>
        /// <param name="text">Входящий текст.</param>
        /// <returns>Текст, в котором первая буква каждого слова - заглавная.</returns>/returns>
        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text.ToLower());
        }
    }
}

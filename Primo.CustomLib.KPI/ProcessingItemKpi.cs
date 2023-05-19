using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primo.CustomLib.KPI
{
    /// <summary>
    /// Класс для определения KPI одного элемента.
    /// </summary>
    public class ProcessingItemKpi
    {
        public string Id { get; set; }
        public Stopwatch TimeCounter { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public ProcessingItemKpi()
        {
            Id = "";
            IsSuccess = true;
            ErrorMessage = "";
            TimeCounter = new Stopwatch();
        }

        /// <summary>
        /// Метод для запуска отсчета времени обработки KPI элемента.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public void Start()
        {
            Id = Guid.NewGuid().ToString();

            TimeCounter = new Stopwatch();
            TimeCounter.Start();
        }

        /// <summary>
        /// Метод для запуска отсчета времени обработки KPI элемента с указанием уникального идентификатора элемента.
        /// </summary>
        /// <param name="id">Уникальный идентификатор элемента.</param>
        /// <returns></returns>
        public void Start(string id)
        {
            Id = id;

            TimeCounter = new Stopwatch();
            TimeCounter.Start();
        }

        /// <summary>
        /// Метод для завершения отсчета времени обработки KPI элемента в случае успеха.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public void Stop()
        {
            TimeCounter.Stop();
            IsSuccess = true;
            ErrorMessage = "";
        }

        /// <summary>
        /// Метод для завершения отсчета времени обработки KPI элемента в случае ошибки.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке, возникшей на этапе обработки элемента.</param>
        /// <returns></returns>
        public void Stop(string errorMessage)
        {
            TimeCounter.Stop();
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }
    }
}

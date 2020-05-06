using System;
using System.Collections.Generic;
using System.Text;
using LinqToExcel;

namespace scheduler
{
    /// <summary>
    /// Класс необходимы для работы с библиотекой LinqToExcel
    /// </summary>
    class ConnectionExcel
    {
        private string _pathExcelFile;
        private ExcelQueryFactory _urlConnetion;
        public ConnectionExcel(string path)
        {
            this._pathExcelFile = path;
            this._urlConnetion = new ExcelQueryFactory(_pathExcelFile);
        }
        public string PathExcelFile
        {
            get
            {
                return _pathExcelFile;
            }
        }
        public ExcelQueryFactory UrlConnetion
        {
            get
            {
                return _urlConnetion;
            }
        }
    }
}

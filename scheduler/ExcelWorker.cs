using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace scheduler
{
    class ExcelWorker
    {
        _Application excel;
        Excel.Workbook wb;
        Excel.Worksheet ws;

        public ExcelWorker()
        {
            excel = new Excel.Application();
            excel.SheetsInNewWorkbook = 1;
            wb = excel.Workbooks.Add(Type.Missing);
            ws = (Excel.Worksheet)excel.Worksheets.get_Item(1);
            ws.Name = "Результат планирования";
        }

        public void CreateReport(List<int> ScheduleResult, ObservableCollection<Gantt> GanttTree)
        {
            Excel.Range range = (Excel.Range)ws.Range[ws.Cells[2, 2], ws.Cells[2, 4]];
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.Font.Size = 16;
            ws.Columns[1].ColumnWidth = 1.5;
            ws.Columns[2].ColumnWidth = 7.5;
            ws.Columns[3].ColumnWidth = 65;
            ws.Columns[4].ColumnWidth = 33;
            ws.Columns[5].ColumnWidth = 1.5;
            ws.Columns[6].ColumnWidth = 39;
            range.Merge(Type.Missing);
            ws.Cells[2, 2] = "Результат планирования загрузки Печей";

            int offset = 0;;
            for (int t = 0; t < GanttTree.Count; t++)
            {
                ws.Cells[2][3 + offset] = GanttTree[t].MachineTool;
                offset++;

                G_Nomenclature nom = null;
                for (int n = 0; n < GanttTree[t].Nomenclatures.Count; n++) 
                {
                    nom = GanttTree[t].Nomenclatures[n];
                    ws.Cells[3][3+offset] = $"Партии с номенклатурой {nom.GN_NomenclatureName} (Начало: {nom.GN_Start} - Конец: {nom.GN_End}) обработки";
                    offset++;

                    G_PartiesParametrs pp = null;
                    for (int p = 0; p < nom.PartiesParametrs.Count; p++)
                    {
                        pp = nom.PartiesParametrs[p]; 
                        ws.Cells[4][3 + offset] = $"Партия {pp.GP_Batch_ID} (Начало: {pp.GP_Start} - Конец: {pp.GP_End})";
                        offset++;
                    }
                }
            }

            ws.Cells[2, 6] = "Полное время обработки всех партий:";
            ws.Cells[2, 7] = GanttTree.LastOrDefault().Nomenclatures.LastOrDefault().PartiesParametrs.LastOrDefault().GP_End.ToString().ToString() +" у.е.";
            range = (Excel.Range)ws.Range[ws.Cells[2, 6], ws.Cells[2, 7]];
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            range.Font.Size = 14;
            range.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(185,146,89));

            range = (Excel.Range)ws.Range[ws.Cells[2, 2], ws.Cells[2+offset, 4]];
            range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;
        }

        public void Save()
        {
            wb.Save();
        }

        public void SaveAs(string path)
        {
            wb.SaveAs(path);
        }

        public void Close()
        {
            wb.Close();
        }
    }
}

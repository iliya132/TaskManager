using OfficeOpenXml;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml.Style;
using TaskManager_redesign.Model.DataProviders.Interfaces;
using DataTable = System.Data.DataTable;

namespace TaskManager_redesign.Model.DataProviders.Implementations
{
    public class ExcelExporter : IExporter
    {
        public string Export(DataTable input)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string reportFileName = $"Report{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorkbook workbook = excel.Workbook;
                ExcelWorksheet sheet = workbook.Worksheets.Add("Report");
                sheet.Cells[1, 1].LoadFromDataTable(input, true);
                FormatQuarterReport(sheet); //TODO вынести вызов метода в MainViewModel
                excel.SaveAs(new FileInfo(reportFileName)); //TODO сохранение вынести в отдельный метод
            }
            return reportFileName;
        }

        
        public void FormatQuarterReport(ExcelWorksheet sheet)
        {
            sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            short maxLevel = 0;
            for (int i = 1; i <= sheet.Dimension.Columns; i++)
            {
                string header = sheet.Cells[1, i].Text; 
                if (header.Contains("Level"))
                {
                    maxLevel++;
                }
                if (header.Contains("Level") ||
                    header.Contains("Q") ||
                    header.Equals("AssignedTo"))//длинный текст
                {
                    sheet.Column(i).Width = 30;
                    sheet.Column(i).Style.WrapText = true;
                    sheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                }

                if(header.Equals("Start") || //Формат дат
                    header.Equals("End"))
                {
                    sheet.Column(i).Style.Numberformat.Format = @"dd.MM.yyyy";
                    sheet.Column(i).Width = 10;
                    sheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }

            #region Grouping
            for (int i = 2; i <= sheet.Dimension.Rows; i++)
            {
                for (int j = 1; j <= maxLevel; j++)
                {
                    if (!string.IsNullOrWhiteSpace(sheet.Cells[i, j].Text))
                    {
                        sheet.Row(i).OutlineLevel = j-1;
                        sheet.Row(i).Collapsed = true;
                    }
                }
            }
            #endregion

            #region quarterFill
            for (int i = 1; i <= sheet.Dimension.Rows; i++)
            {

            }
            #endregion
        }

    }
}

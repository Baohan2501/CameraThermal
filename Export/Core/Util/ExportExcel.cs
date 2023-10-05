using Core.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public class ExportExcel
    {
        public void ExportTemperature(List<TemperatureInfo> temperatures, string path)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Temparetures");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "No";
            workSheet.Cells[1, 2].Value = "DateTime";
            workSheet.Cells[1, 3].Value = "Camera name";
            workSheet.Cells[1, 4].Value = "Zone name";
            workSheet.Cells[1, 5].Value = "Temperature";
            //Body of table  
            //  
            int recordIndex = 2;
            foreach (var temperature in temperatures)
            {
                workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
                workSheet.Cells[recordIndex, 2].Value = temperature.DateTime;
                workSheet.Cells[recordIndex, 2].Style.Numberformat.Format = "MM/dd/yyyy hh:mm:ss";
                workSheet.Cells[recordIndex, 3].Value = temperature.CameraName;
                workSheet.Cells[recordIndex, 4].Value = temperature.ZoneName;
                workSheet.Cells[recordIndex, 5].Value = temperature.AverageTemperature;
                //workSheet.Cells[recordIndex, 5].Style.Numberformat.Format = "#,##0.00";
                recordIndex++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            //set format
            var modelTable = workSheet.Cells[1, 1, temperatures.Count + 1, 5];
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //set background
            workSheet.Cells["A1:E1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            workSheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Silver);
            //Check file exists
            if (File.Exists(path))
                File.Delete(path);
            //Create excel file on physical disk    
            FileStream objFileStrm = File.Create(path);
            objFileStrm.Close();
            //Write content to excel file    
            File.WriteAllBytes(path, excel.GetAsByteArray());
        }
    }
}

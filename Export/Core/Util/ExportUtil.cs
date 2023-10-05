using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing;
using Core.Model;

namespace Export.Util
{
    public class ExportUtil
    {
        public void Export(string pathfile, List<ThermalInfo> datas)
        {
            using (ExcelPackage objExcelPackage = new ExcelPackage())
            {
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
                int startRow = 1;
                int startColumn = 1;
                //Create the worksheet    
                ExcelWorksheet objWorksheet = objExcelPackage.Workbook.Worksheets.Add("Export data");

                foreach (var item in datas)
                {
                    objWorksheet.Cells[startRow, startColumn].Value = "Inspection Date:";
                    objWorksheet.Cells[startRow + 1, startColumn].Value = "Equipment";
                    objWorksheet.Cells[startRow + 2, startColumn].Value = "Potential Problem:";
                    objWorksheet.Cells[startRow + 3, startColumn].Value = "Emissivity:";
                    objWorksheet.Cells[startRow + 4, startColumn].Value = "Camera Manufacturer";
                    objWorksheet.Cells[startRow + 5, startColumn].Value = "Hot Image Marker:";

                    objWorksheet.Cells[startRow, startColumn + 1].Value = item.InspectionDate;
                    objWorksheet.Cells[startRow + 1, startColumn + 1].Value = item.Equipment;
                    objWorksheet.Cells[startRow + 2, startColumn + 1].Value = item.PotentialProblem;
                    objWorksheet.Cells[startRow + 3, startColumn + 1].Value = item.Emissivity;
                    objWorksheet.Cells[startRow + 4, startColumn + 1].Value = item.CameraManufacturer;
                    objWorksheet.Cells[startRow + 5, startColumn + 1].Value = item.HotImageMarker;

                    objWorksheet.Cells[startRow, startColumn + 2].Value = "Location";
                    objWorksheet.Cells[startRow + 1, startColumn + 2].Value = "Equipment Name:";
                    objWorksheet.Cells[startRow + 2, startColumn + 2].Value = "Repair Priority:";
                    objWorksheet.Cells[startRow + 3, startColumn + 2].Value = "Reflected Temperature:";
                    objWorksheet.Cells[startRow + 4, startColumn + 2].Value = "Camera:";
                    objWorksheet.Cells[startRow + 5, startColumn + 2].Value = "Cold Image Marker";

                    objWorksheet.Cells[startRow, startColumn + 3].Value = item.Location;
                    objWorksheet.Cells[startRow + 1, startColumn + 3].Value = item.EquipmentName;
                    objWorksheet.Cells[startRow + 2, startColumn + 3].Value = item.RepairPriority;
                    objWorksheet.Cells[startRow + 3, startColumn + 3].Value = item.ReflectedTemperature;
                    objWorksheet.Cells[startRow + 4, startColumn + 3].Value = item.Camera;
                    objWorksheet.Cells[startRow + 5, startColumn + 3].Value = item.ColdImageMarker;


                    objWorksheet.Cells[startRow, startColumn, startRow + 5, startColumn + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    objWorksheet.Cells[startRow, startColumn, startRow + 5, startColumn + 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    objWorksheet.Cells[startRow, startColumn, startRow + 5, startColumn + 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;

                    objWorksheet.Cells[startRow, startColumn, startRow + 5, startColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    objWorksheet.Cells[startRow, startColumn, startRow + 5, startColumn].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    objWorksheet.Cells[startRow, startColumn + 2, startRow + 5, startColumn + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    objWorksheet.Cells[startRow, startColumn + 2, startRow + 5, startColumn + 2].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                    ExcelPicture pic = objWorksheet.Drawings.AddPicture("Sample" + startRow, item.ImageData);
                    pic.SetPosition(startRow + 6, 0, startColumn - 1, 20);
                    pic.SetSize(400, 300);
                    objWorksheet.Protection.IsProtected = false;

                    startRow = startRow + 30;
                }

                objWorksheet.Cells.AutoFitColumns();

                //Write it back to the client    
                if (File.Exists(pathfile))
                    File.Delete(pathfile);
                //Create excel file on physical disk    
                FileStream objFileStrm = File.Create(pathfile);
                objFileStrm.Close();
                //Write content to excel file    
                File.WriteAllBytes(pathfile, objExcelPackage.GetAsByteArray());
            }
        }

    }
}

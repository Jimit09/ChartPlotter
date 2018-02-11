using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using TabularDataToChartConvertor.Models;
using Newtonsoft.Json;

namespace TabularDataToChartConvertor.Controllers
{
    public class ChartGeneratorController : Controller
    {
        private const int _departmentNameIndex = 1;
        private const int _salesIndex = 2;
        private const int _salesInPercentageIndex = 3;

        // GET: ChartGenerator
        [HttpPost]
        public ActionResult GeneratePieChart(HttpPostedFileBase file)
        {
            var salesList = new List<SalesData>();
            try
            {
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var workSheets = package.Workbook.Worksheets;
                        var workSheet = workSheets.First();
                        var noOfRows = workSheet.Dimension.End.Row;
                        //Skipping the header row and total row
                        for (int i = 2; i < noOfRows; i++)
                        {
                            var salesData = new SalesData();
                            salesData.DepartmentName = workSheet.Cells[i, _departmentNameIndex].Value.ToString();
                            salesData.Sales = Convert.ToInt32(workSheet.Cells[i, _salesIndex].Value);
                            salesData.SalesInPercentage = Convert.ToInt32(workSheet.Cells[i, _salesInPercentageIndex].Value);
                            salesList.Add(salesData);
                        }
                    }

                    //if (salesList.Count > 0)
                    //{
                    //    ViewBag.SalesData = JsonConvert.SerializeObject(salesList);
                    //}

                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View(salesList);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
    }
}
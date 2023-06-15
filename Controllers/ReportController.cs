using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdventureWorks.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace AdventureWorks.Controllers
{
    [Route("[controller]/[action]")]
    public class ReportController : Controller
    {

        private readonly string _connString = "Server=localhost;Database=AdventureWorks2019;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{value}")]
        public IActionResult ShiftEmployees(int value)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "EXEC ShiftEmployees @id = " + value + ";";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", value);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string results = reader.GetString(0);
                        return Content(results);
                    }
                }
                conn.Close();
                return NotFound();
            }
        }



        [HttpGet]
        public IActionResult SalesmenByTerritory()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "EXECUTE SalesmenByTerritory;";

                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string results = reader.GetString(0);
                        return Content(results);
                    }
                }
                conn.Close();
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public IActionResult EmployeesByDepartment(int id)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.Cells[1, 1].Value = "Employee Name";
            workSheet.Cells[1, 2].Value = "Job Title";
            workSheet.Cells[1, 3].Value = "Birth Date";
            workSheet.Cells[1, 4].Value = "Hire Date";
            workSheet.Cells[1, 5].Value = "Department";
            workSheet.Cells[1, 6].Value = "Group";
            workSheet.Cells[1, 7].Value = "Shift";
            int recordIndex = 2;

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "EXEC EmployeesByDepartment @id = " + id + ";";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            workSheet.Cells[recordIndex, 1].Value = reader.GetString(0);
                            workSheet.Cells[recordIndex, 2].Value = reader.GetString(1);
                            workSheet.Cells[recordIndex, 3].Value = reader["BirthDate"].ToString();
                            workSheet.Cells[recordIndex, 4].Value = reader["HireDate"].ToString();
                            workSheet.Cells[recordIndex, 5].Value = reader.GetString(4);
                            workSheet.Cells[recordIndex, 6].Value = reader.GetString(5);
                            workSheet.Cells[recordIndex, 7].Value = reader.GetString(6);
                            recordIndex++;
                        }
                        reader.NextResult();
                    }
                    var xlFile = new FileInfo(@"C:\Temp\employee_department" + id + ".xlsx");
                    excel.SaveAs(xlFile);//save the Excel file
                    return Json("here");
                }
                conn.Close();
                return NotFound();
            }
        }
    }
}

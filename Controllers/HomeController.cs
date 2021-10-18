using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using democloudapplication.Models;
namespace democloudapplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Getdatabases(string Uname, string LoginId, string Password)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            SqlConnection con = new SqlConnection("Data Source=" + Uname + ";User Id=" + LoginId + ";Password=" + Password + ";Database=StarsDB");
            string query = "select name from sys.sysdatabases";
            Session["Uname"] = Uname;
            Session["LoginId"] = LoginId;
            Session["Password"] = Password;
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = sdr["name"].ToString(),
                        });
                    }
                }
                con.Close();
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MainPage(Logins m)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            SqlConnection con = new SqlConnection("Data Source=" + m.ServerName + ";User Id=" + m.LoginID + ";Password=" + m.Password + ";Database=" + m.Database + "");
            string query = "select name from sys.tables";
            Session["Uname"] = m.ServerName;
            Session["LoginId"] = m.LoginID;
            Session["Password"] = m.Password;
            Session["Database"] = m.Database;
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = sdr["name"].ToString(),
                        });
                    }
                }
                con.Close();
            }
            ViewBag.items = items;
            return View();
        }
        public ActionResult ViewDataManagement(string TableName = "", string condition = "",
       string SerialNumberFilter = "", string tablecolumns = "", string tblformat = "",
      int pageno = 1, int pagesize = 10)
        {
            try
            {
                if (TableName != "--select--")
                {

                    DataTable dt = new DataTable();
                    if (tblformat == "genaratepdf")
                    {
                        ViewBag.pdf = "pdf";
                        dt = Multipletablereportfun(TableName, tablecolumns, condition, "Report", pageno, pagesize);
                        ViewBag.FinalList = dt;
                        return new PartialViewAsPdf("_multipletbldata")
                        {
                            FileName = "" + TableName + ".pdf"
                        };
                    }
                    else if (tblformat == "genaratexls")
                    {
                        dt = Multipletablereportfun(TableName, "*", "", "Report", pageno, pagesize);

                        Generate_excel(TableName, dt);
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    else if (tblformat == "genaratecsv")
                    {
                        dt = Multipletablereportfun(TableName, "*", "", "Report", pageno, pagesize);

                        Generate_CSV(TableName, dt);
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        dt = Multipletablereportfun(TableName, "*", "", "", pageno, pagesize);
                        return PartialView(@"~/Views/_multipletbldata.cshtml");
                    }
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        id = 1,
                        responseText = "Please Select the Table Name to preview "
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false,
                    id = 1,
                    responseText = ex.Message
                });
            }
        }


        private ActionResult Generate_excel(string tableName, DataTable dt)
        {

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;
            //string reportName = (tableName.Replace(" ", "") + "_" + DateTime.Now.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString()).Replace(":", "-").Replace("/", "-");

            //var memoryStream = new MemoryStream();
            //using (var excelPackage = new ExcelPackage(memoryStream))
            //{
            //    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

            //    worksheet.Cells["A1"].LoadFromDataTable(dt, true, TableStyles.None);
            //    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
            //    worksheet.DefaultRowHeight = 18;


            //    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            //    worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //    worksheet.DefaultColWidth = 20;
            //    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            //    byte[] data = excelPackage.GetAsByteArray();
            //    Response.Clear();
            //    Response.Buffer = true;
            //    Response.AddHeader("content-disposition", "attachment;filename=" + reportName + ".xlsx");
            //    Response.Charset = "";
            //    Response.ContentType = "application/vnd.ms-excel";
            //    StringWriter sw = new StringWriter();
            //    Response.BinaryWrite(data);
            //    //Response.Close();
            //    Response.End();
            return View();
            //}
        }


        private ActionResult Generate_CSV(string tableName, DataTable dt)
        {
            string reportName = (tableName.Replace(" ", "") + "_" + DateTime.Now.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString()).Replace(":", "-").Replace("/", "-");
            string csv = string.Empty;
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Header row for CSV file.
                csv += column.ColumnName + ',';
            }

            //Add new line.
            csv += "\r\n";

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    //Add the Data rows.
                    csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
                }

                //Add new line.
                csv += "\r\n";
            }

            //Download the CSV file.
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + reportName + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv);
            Response.Flush();
            Response.End();
            return View();
        }





        public DataTable Multipletablereportfun(string TableName, string tablecolumns, string condition, string flag, int pageno, int pagesize)
        {
            SqlCommand cmd = new SqlCommand();
            SqlParameter output = new SqlParameter("@totalRecords", typeof(long));
            List<SqlParameter> sp = new List<SqlParameter>()
                {
                    new SqlParameter("@tblname", TableName),
                    new SqlParameter("@tablecolumns", tablecolumns),
                    new SqlParameter("@condition", condition),
                    new SqlParameter("@flag", flag),
                    new SqlParameter("@pageno", pageno),
                    new SqlParameter("@pagesize", pagesize)
                };
            sp.Add(output);
            output.DbType = DbType.Int64;
            output.Direction = ParameterDirection.Output;
            DataTable dt = ExecuteProcedure("gettblreports", sp);
            ViewBag.FinalList = dt;
            if (flag != "Report")
            {
                Pagingutility obj = new Pagingutility();
                if (output.GetType().Name != "DBNULL")
                {
                    output.Value = output.Value;
                }
                if (dt.Rows.Count > 0)
                {
                    obj = GetPaging(obj, pageno, pagesize, output);
                    ViewBag.TotalRecords = output.Value;
                    ViewBag.Totalpages = obj.TotalPages;
                }
            }
            return dt;
        }

        public T GetPaging<T>(T objlst, int pageNumber, int pageSize,
         SqlParameter obj2) where T : Pagingutility
        {
            int totalCount = obj2.GetType().Name != "DBNull" ? Convert.ToInt32(obj2.Value) : 0; if (objlst != null || objlst != null)
            {
                objlst.TotalPages = objlst != null ? Convert.ToInt32(Math.Ceiling((decimal)totalCount / pageSize)) : 0;
                objlst.Pages = pageNumber + " of " + objlst.TotalPages;
                objlst.PageNumber = pageNumber;
                objlst.RecordCount = pageSize;
                var pageCount = ((pageNumber - 1) * pageSize + pageSize) > totalCount ? totalCount : ((pageNumber - 1) * pageSize + pageSize);
                objlst.Record = ((pageNumber - 1) * pageSize) + " - " + pageCount + " of " + totalCount;
            }
            return objlst;
        }
        public DataTable ExecuteProcedure(string ProcedureName, List<SqlParameter> lstpara)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=" + Session["Uname"].ToString() + ";User Id=" + Session["LoginId"].ToString() + ";Password=" + Session["Password"].ToString() + ";Database=" + Session["Database"].ToString() + ""))
                {
                    using (SqlCommand cmd = new SqlCommand(ProcedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter sp in lstpara)
                            cmd.Parameters.Add(sp);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.SelectCommand.CommandTimeout = 6000;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                //BAL.ErrorLog.LogError(ex);

                throw ex;
            }

        }

        public ActionResult UploadFiles(HttpPostedFileBase FileUpload)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
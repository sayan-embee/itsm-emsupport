using Common.Layer.Models.Report;
using DataAccess.Layer.DbAccess;
using Microsoft.ApplicationInsights;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Data;
using System.IO;

namespace WebAPI.Layer.Services
{
    public class GenExcelService : IGenExcelService
    {
        private readonly ILogger _logger;
        private readonly ISQLDataAccess _db;
        private readonly IConfiguration _config;

        public GenExcelService
        (
            ILogger<GenExcelService> logger
            , TelemetryClient telemetryClient
            , IConfiguration config
            , ISQLDataAccess db
        )
        {
            this._logger = logger;
            this._db = db;
            this._config = config;
        }

        #region Helper

        public string CheckOrCreateDirectory(string directoryName)
        {
            try
            {
                if (!string.IsNullOrEmpty(directoryName))
                {
                    var mainDirectoryPath = System.IO.Directory.GetCurrentDirectory() + @"\Documents";

                    if (!System.IO.Directory.Exists(mainDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(mainDirectoryPath);
                    }

                    var subDirectoryPath = Path.Combine(mainDirectoryPath, directoryName);
                    if (!System.IO.Directory.Exists(subDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(subDirectoryPath);
                    }

                    return subDirectoryPath;
                }


                return string.Empty;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at CheckOrCreateDirectory()");
                return string.Empty;
            }
        }

        private Cell CreateTextCell(string text)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text)
            };
        }

        public string CreateExcel(DataTable data, string outputFilePath)
        {
            if (data == null || data.Rows.Count == 0)
            {
                throw new ArgumentException("Data cannot be null or empty.", nameof(data));
            }

            try
            {
                // Create the spreadsheet document
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(outputFilePath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    // Add a WorkbookPart
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Add a WorksheetPart
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Create Sheets collection
                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    // Create a Sheet
                    Sheet sheet = new Sheet
                    {
                        Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Sheet1"
                    };
                    sheets.Append(sheet);

                    // Add data to SheetData
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Add column headers
                    Row headerRow = new Row();
                    foreach (DataColumn column in data.Columns)
                    {
                        Cell headerCell = CreateTextCell(column.ColumnName);
                        headerRow.AppendChild(headerCell);
                    }
                    sheetData.AppendChild(headerRow);

                    // Add rows
                    foreach (DataRow row in data.Rows)
                    {
                        Row newRow = new Row();
                        foreach (var cellValue in row.ItemArray)
                        {
                            Cell cell = CreateTextCell(cellValue?.ToString());
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }

                    workbookPart.Workbook.Save();
                }

                return outputFilePath; // Return the file path
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred at GenerateExcel(): {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Excel

        public string GenerateExcelNReturnPath(DataSet data, ParamModel paramModel)
        {
            try
            {
                string finalDirectoryName = paramModel.Filter.departmentId + "\\" + DateTime.Now.ToString("MMMMyyyy") + "\\Excel";
                string directoryPath = this.CheckOrCreateDirectory(finalDirectoryName);
                if (string.IsNullOrEmpty(directoryPath))
                {
                    throw new ArgumentException("Directory path is null or empty.");
                }
                string outputPath = Path.Combine(directoryPath, $"Excel.xlsx");

                //DataTable excelDataTable = data.Tables[0].Copy();
                DataTable excelDataTable = data.Tables[0].DefaultView
                        .ToTable(false, "id", "created_at_display", "category", "closed_at_display", "name", "first_resp_time_in_secs", "ResponseStatus", "location", "nsd_member_name", "on_roaster_engineer", "priorityname", "RequesterName", "RequesterEmail", "resolution_remarks", "ResolutionStatus", "StatusName", "sub_category", "subject", "tenant", "type", "status_updated_at_display");
                
                if (excelDataTable.Columns.Contains("first_resp_time_in_secs"))
                {
                    DataColumn newColumn = new DataColumn("First Response Time (in Hrs)", typeof(string));
                    excelDataTable.Columns.Add(newColumn);

                    foreach (DataRow row in excelDataTable.Rows)
                    {
                        if (row["first_resp_time_in_secs"] != DBNull.Value &&
                            int.TryParse(row["first_resp_time_in_secs"].ToString(), out int seconds))
                        {
                            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

                            // Format the TimeSpan to HH:mm:ss
                            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                (int)timeSpan.TotalHours,
                                timeSpan.Minutes,
                                timeSpan.Seconds);

                            row[newColumn] = formattedTime;
                        }
                        else
                        {
                            row[newColumn] = "";
                        }
                    }

                    excelDataTable.Columns.Remove("first_resp_time_in_secs");

                    newColumn.ColumnName = "First Response Time (in Hrs)";
                }

                if (excelDataTable.Columns.Contains("id"))
                {
                    excelDataTable.Columns["id"].ColumnName = "Ticket Id";
                }

                if (excelDataTable.Columns.Contains("created_at_display"))
                {
                    excelDataTable.Columns["created_at_display"].ColumnName = "Created Time";
                }

                if (excelDataTable.Columns.Contains("category"))
                {
                    excelDataTable.Columns["category"].ColumnName = "Category";
                }

                if (excelDataTable.Columns.Contains("closed_at_display"))
                {
                    excelDataTable.Columns["closed_at_display"].ColumnName = "Closed Time";
                }

                if (excelDataTable.Columns.Contains("name"))
                {
                    excelDataTable.Columns["name"].ColumnName = "Company";
                }

                //if (excelDataTable.Columns.Contains("first_resp_time_in_secs"))
                //{
                //    excelDataTable.Columns["first_resp_time_in_secs"].ColumnName = "First Response Time (in Hrs)";
                //}

                if (excelDataTable.Columns.Contains("ResponseStatus"))
                {
                    excelDataTable.Columns["ResponseStatus"].ColumnName = "First Response Status";
                }

                if (excelDataTable.Columns.Contains("location"))
                {
                    excelDataTable.Columns["location"].ColumnName = "Location";
                }

                if (excelDataTable.Columns.Contains("nsd_member_name"))
                {
                    excelDataTable.Columns["nsd_member_name"].ColumnName = "NSD Member Name";
                }

                if (excelDataTable.Columns.Contains("on_roaster_engineer"))
                {
                    excelDataTable.Columns["on_roaster_engineer"].ColumnName = "On Roaster Engineer";
                }

                if (excelDataTable.Columns.Contains("priorityname"))
                {
                    excelDataTable.Columns["priorityname"].ColumnName = "Priority";
                }

                if (excelDataTable.Columns.Contains("RequesterEmail"))
                {
                    excelDataTable.Columns["RequesterEmail"].ColumnName = "Requester Email";
                }

                if (excelDataTable.Columns.Contains("RequesterName"))
                {
                    excelDataTable.Columns["RequesterName"].ColumnName = "Requester Name";
                }

                if (excelDataTable.Columns.Contains("resolution_remarks"))
                {
                    excelDataTable.Columns["resolution_remarks"].ColumnName = "Resolution Remarks";
                }

                if (excelDataTable.Columns.Contains("ResolutionStatus"))
                {
                    excelDataTable.Columns["ResolutionStatus"].ColumnName = "Resolution Status";
                }

                if (excelDataTable.Columns.Contains("StatusName"))
                {
                    excelDataTable.Columns["StatusName"].ColumnName = "Status";
                }

                if (excelDataTable.Columns.Contains("sub_category"))
                {
                    excelDataTable.Columns["sub_category"].ColumnName = "Sub-Category";
                }

                if (excelDataTable.Columns.Contains("subject"))
                {
                    excelDataTable.Columns["subject"].ColumnName = "Subject";
                }

                if (excelDataTable.Columns.Contains("tenant"))
                {
                    excelDataTable.Columns["tenant"].ColumnName = "Tenant";
                }

                if (excelDataTable.Columns.Contains("type"))
                {
                    excelDataTable.Columns["type"].ColumnName = "Type";
                }

                if (excelDataTable.Columns.Contains("status_updated_at_display"))
                {
                    excelDataTable.Columns["status_updated_at_display"].ColumnName = "Last Updated Time";
                }

                string resultPath = CreateExcel(excelDataTable, outputPath);
                return resultPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred at GenerateExcelNReturnPath(): {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}

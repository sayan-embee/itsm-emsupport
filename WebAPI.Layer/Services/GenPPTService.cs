using System.Data;
using System.Dynamic;
using SkiaSharp;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;
using System.Text.Json;
using DataTable = System.Data.DataTable;
using Shape = DocumentFormat.OpenXml.Presentation.Shape;
using ShapeProperties = DocumentFormat.OpenXml.Presentation.ShapeProperties;
using Common.Layer.Models.Report;
using DocumentFormat.OpenXml.Vml.Office;

namespace WebAPI.Layer.Services
{
    public class GenPPTService : IGenPPTService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly double SLIDE_MAX_HEIGHT = 14.28;
        private readonly double SLIDE_MAX_WIDTH = 25.36;

        private SlideSettings slideSettings;

        // Temporary storage for C022 pair
        private DataTable? pendingC022Table = null;

        public GenPPTService
        (
            ILogger<GenPPTService> logger
            , IConfiguration config
        )
        {
            this._logger = logger;
            this._config = config;

            ReadFromJson();
        }



        #region Color Codes

        static string HEADING_TEXT_COLOR_CODE = "10446F";

        static string TBL_HEADER_FONT_COLOR_CODE = "FFFFFF";
        static string TBL_ROW_LIGHT_COLOR_CODE = "FFFFFF";
        static string TBL_ROW_MEDIUM_COLOR_CODE = "DCEAF7";
        static string TBL_FOOTER_COLOR_CODE = "A6CAEC";

        static string CELL_FONT_COLOR_CODE = "10446F";

        static string GraphAreaBgColor = "#ffffff";

        static string[] BarColors_ClosedTickets = new[]
        {
            "#156082", // Blue / Incident
            "#E97132" // Orange / Service Request
        };

        static string[] BarColors = new[]
        {
            "#156082", // Blue / Change Request
            "#E97132", // Orange / Incident
            "#196B24", // Green / Service Request
            "#0F9ED5", // Sky blue / Problem
        };

        #endregion

        #region Constant

        static int TBL_MAX_ROW_COUNT = 20;

        static int TBL_HEADER_ROW_HEIGHT = 275000;
        static int TBL_ROW_HEIGHT = 175000;

        //static double SLIDE_MAX_HEIGHT = 19.05;
        //static double SLIDE_MAX_WIDTH = 33.87;

        #endregion

        #region Helpers

        private void ReadFromJson()
        {
            try
            {
                string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Settings", "SlideSettings.json");
                string json = File.ReadAllText(path);

                if (string.IsNullOrEmpty(json))
                {
                    throw new ArgumentNullException("Error reading SlideSettings.json");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                slideSettings = JsonSerializer.Deserialize<SlideSettings>(json, options);

                if (slideSettings == null)
                {
                    throw new ArgumentNullException("Error parsing SlideSettings.json");
                }

                // Convert BarColor from "color1;color2" to array
                //slideSettings.Config.BarGraph.ClosedTickets.BarColor = slideSettings.Config.BarGraph.ClosedTickets.BarColor[0].Split(';');
                //slideSettings.Config.BarGraph.Last3Months.BarColor = slideSettings.Config.BarGraph.Last3Months.BarColor[0].Split(';');
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataTable> CreateDynamicDataTable<T>(IEnumerable<T> data, Func<T, bool> filter = null)
        {
            var dataTable = new DataTable();

            // Get properties of the type T
            var properties = typeof(T).GetProperties();

            // Define columns dynamically based on the properties of T
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Apply filter if provided
            var filteredData = filter != null ? data.Where(filter) : data;

            // Populate rows dynamically
            foreach (var item in filteredData)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            await Task.Delay(0);
            return dataTable;
        }

        public async Task<string> CheckOrCreateDirectory(string directoryName)
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

                    await Task.Delay(0);
                    return subDirectoryPath;
                }

                await Task.Delay(0);
                return string.Empty;
            }
            catch (Exception ex)
            {
                await Task.Delay(0);
                return string.Empty;
            }
        }

        public async Task<bool> CopyPowerPointFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    await Task.Delay(0);
                    return false;
                }
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                await Task.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                await Task.Delay(0);
                return false;
            }
        }

        public async Task CopySlide(string sourcePptPath, PresentationPart presentationPart)
        {
            try
            {
                using (PresentationDocument sourcePresentation = PresentationDocument.Open(sourcePptPath, false))
                {
                    PresentationPart sourcePresentationPart = sourcePresentation.PresentationPart;
                    if (sourcePresentationPart == null) return;

                    // Get slides in correct order
                    var slideIds = sourcePresentationPart.Presentation.SlideIdList.Elements<DocumentFormat.OpenXml.Presentation.SlideId>();
                    foreach (var slideId in slideIds)
                    {
                        string relId = slideId.RelationshipId;
                        SlidePart sourceSlidePart = (SlidePart)sourcePresentationPart.GetPartById(relId);
                        if (sourceSlidePart == null) continue;

                        // Create a new slide in the target presentation
                        SlidePart newSlidePart = await this.AddSlide(presentationPart);

                        // Get shape trees
                        var sourceShapeTree = sourceSlidePart.Slide.CommonSlideData.ShapeTree;
                        var newShapeTree = newSlidePart.Slide.CommonSlideData.ShapeTree;

                        // Copy shapes
                        foreach (var sourceShape in sourceShapeTree.Elements())
                        {
                            if (sourceShape is DocumentFormat.OpenXml.Presentation.Picture)
                            {
                                var newPicture = (DocumentFormat.OpenXml.Presentation.Picture)sourceShape.CloneNode(true);
                                var sourceImagePart = sourceSlidePart.GetPartById(((DocumentFormat.OpenXml.Drawing.Blip)newPicture.BlipFill.Blip).Embed);
                                var newImagePart = newSlidePart.AddPart(sourceImagePart);
                                ((DocumentFormat.OpenXml.Drawing.Blip)newPicture.BlipFill.Blip).Embed = newSlidePart.GetIdOfPart(newImagePart);
                                newShapeTree.Append(newPicture);
                            }
                            else
                            {
                                var newShape = (DocumentFormat.OpenXml.OpenXmlElement)sourceShape.CloneNode(true);
                                newShapeTree.Append(newShape);
                            }
                        }

                        // Save the new slide
                        newSlidePart.Slide.Save();
                    }

                    // Save the target presentation
                    presentationPart.Presentation.Save();

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                await Task.Delay(0);
                return;
            }
        }

        public async Task<DataTable> ProcessAndMapColumnsWithCustomNames(DataTable originalTable, string columns, string customColumnNames)
        {
            // Parse the column list and corresponding custom column names
            var columnList = columns.Split(',').Select(c => c.Trim()).ToList();
            var customColumnList = customColumnNames?.Split(',').Select(c => c.Trim()).ToList();

            // Validate column and custom name counts
            if (customColumnList != null && columnList.Count != customColumnList.Count)
                throw new ArgumentException("Mismatch between Columns and CustomColumnNames counts.");

            // Create a new DataTable for transformed data
            var transformedTable = new DataTable();

            // Map original columns to custom names
            for (int i = 0; i < columnList.Count; i++)
            {
                string originalColumn = columnList[i];
                string customColumn = customColumnList?[i] ?? originalColumn; // Use custom name or fallback to original name

                if (originalTable.Columns.Contains(originalColumn))
                {
                    transformedTable.Columns.Add(customColumn);
                }
            }

            // Add rows with the selected and renamed columns
            foreach (DataRow row in originalTable.Rows)
            {
                var newRow = transformedTable.NewRow();
                for (int i = 0; i < columnList.Count; i++)
                {
                    string originalColumn = columnList[i];
                    string customColumn = customColumnList?[i] ?? originalColumn;

                    if (originalTable.Columns.Contains(originalColumn))
                    {
                        newRow[customColumn] = row[originalColumn];
                    }
                }
                transformedTable.Rows.Add(newRow);
            }

            await Task.Delay(0);
            return transformedTable;
        }

        public async Task<(double widthCm, double heightCm)> GetImageDimensionsInCm(string imagePath)
        {
            using (var bitmap = SKBitmap.Decode(imagePath))
            {
                if (bitmap == null)
                {
                    return (0, 0);
                }

                // Get the dimensions in pixels
                int widthInPixels = bitmap.Width;
                int heightInPixels = bitmap.Height;

                // Define the DPI (you may need to adjust this based on your image)
                float dpi = 124; // Common DPI for screen images

                // Convert pixels to centimeters (1 inch = 2.54 cm)
                double widthCm = (widthInPixels / dpi) * 2.54;
                double heightCm = (heightInPixels / dpi) * 2.54;

                await Task.Delay(0);
                return (widthCm, heightCm);
            }
        }

        #endregion

        #region Bar Chart

        private async Task<string> GenerateNSaveBarChart(IDictionary<string, object>[] aggregatedData, string directoryName, string fileName, SlideEnum slideEnum)
        {
            try
            {
                string returnString = string.Empty;

                if (!await this.IsValidData(aggregatedData) || string.IsNullOrWhiteSpace(directoryName) || string.IsNullOrWhiteSpace(fileName))
                {

                }

                if (slideEnum == SlideEnum.IncidentSlide)
                {
                    // Extract all dynamic property names(columns except Type)
                    var properties = aggregatedData.First()
                                   .Keys
                                   .Where(key => !key.Equals("Ticket Type", StringComparison.OrdinalIgnoreCase))
                                   .ToArray();

                    // Calculate dynamic canvas width and height
                    int ticketCount = aggregatedData.Length;
                    int propertyCount = properties.Length;
                    int barWidth = propertyCount > 2 ? 40 : 60; // Base width for each bar
                    int barSpacing = 1; // Spacing between bars

                    int ticketSpacing = propertyCount * (barWidth + barSpacing) + 60; // Space between ticket types
                    int width = ticketCount * ticketSpacing + (propertyCount * 100); // Adjust width dynamically
                    int contentHeight = 550; // Fixed content height
                    int height = contentHeight;

                    // Find the maximum value for each property and determine the overall max value
                    int maxBarValue = aggregatedData
                                     .SelectMany(row => properties
                                    .Select(key => Convert.ToInt32(row[key] ?? 0))) // Convert values to int, handle nulls
                                    .Max(); // Find the maximum value

                    // Dynamically calculate the Y-axis max value (round up to nearest 100)
                    int maxValue = (int)Math.Ceiling(maxBarValue / 100.0) * 100;

                    // Padding from Y-axis to the first bar
                    int yAxisPadding = 30;

                    // Create a new image surface
                    using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
                    {
                        var canvas = surface.Canvas;

                        // Clear the canvas with a white background
                        //canvas.Clear(SKColors.White);
                        canvas.Clear(SKColors.Transparent);

                        string[] colorArray = [];
                        if (!string.IsNullOrEmpty(slideSettings?.Config?.BarGraph.ClosedTickets.BarColor))
                        {
                            colorArray = slideSettings.Config.BarGraph.ClosedTickets.BarColor.Split(";");
                        }
                        else
                        {
                            colorArray = BarColors_ClosedTickets;
                        }

                        // Define SKPaint objects for bars
                        var paints = colorArray.Take(properties.Length).Select(color => new SKPaint
                        {
                            Color = SKColor.Parse(color),
                            Style = SKPaintStyle.Fill
                        }).ToArray();

                        // Set up paint for axes and labels
                        var axisPaint = new SKPaint { Color = SKColors.Empty, StrokeWidth = 0 };
                        var textPaint = new SKPaint { Color = SKColor.Parse("595959"), TextSize = 20, IsAntialias = true };

                        // Define the graph area
                        int graphTop = 80;
                        int graphBottom = height - 150;
                        int graphHeight = graphBottom - graphTop;
                        int graphWidth = width - 100;

                        // Set background color of graph area (with transparency)

                        string areaColor = GraphAreaBgColor;
                        if (!string.IsNullOrEmpty(slideSettings?.Config?.BarGraph.ClosedTickets.BackgroundColor))
                        {
                            areaColor = slideSettings.Config.BarGraph.ClosedTickets.BackgroundColor;
                        }

                        var graphBackgroundPaint = new SKPaint
                        {
                            Color = SKColor.Parse(areaColor).WithAlpha(128)
                        };
                        canvas.DrawRect(50, graphTop, graphWidth, graphHeight, graphBackgroundPaint);

                        // Draw X and Y axis
                        canvas.DrawLine(50, height - 150, width - 50, height - 150, axisPaint); // X Axis
                        canvas.DrawLine(50, 50, 50, height - 150, axisPaint); // Y Axis


                        // Draw Y-axis labels and gridlines
                        // Define gridline paints
                        var mainGridlinePaint = new SKPaint { Color = SKColor.Parse("d9d9d9"), StrokeWidth = 2, IsAntialias = true };
                        var lighterGridlinePaint = new SKPaint { Color = SKColor.Parse("e6e6e6"), StrokeWidth = 1, IsAntialias = true };


                        int yAxisCount = 5;
                        //if (slideSettings?.Config?.BarGraph?.ClosedTickets?.YAxisCount != null)
                        //{
                        //    yAxisCount = slideSettings.Config.BarGraph.ClosedTickets.YAxisCount;
                        //}

                        for (int i = 0; i <= yAxisCount; i++) // 5 main labels on Y-axis
                        {
                            int yValue = i * maxValue / yAxisCount;
                            string label = yValue.ToString();
                            float yPosition = height - 150 - (i * (graphHeight / yAxisCount));

                            // Draw Y-axis labels
                            canvas.DrawText(label, 10, yPosition, textPaint);

                            // Draw main horizontal gridlines
                            canvas.DrawLine(50, yPosition, width - 50, yPosition, mainGridlinePaint);

                            // Draw 5 lighter gridlines between current and next main gridline
                            if (i < yAxisCount) // Avoid drawing below the last line
                            {
                                float nextY = height - 150 - ((i + 1) * (graphHeight / yAxisCount));
                                float step = (yPosition - nextY) / 6; // Divide into 6 sections

                                for (int j = 1; j <= yAxisCount; j++) // Draw 5 lighter gridlines
                                {
                                    float midY = yPosition - (j * step);
                                    canvas.DrawLine(50, midY, width - 50, midY, lighterGridlinePaint);
                                }
                            }
                        }


                        // Draw the bars with Zero
                        for (int i = 0; i < aggregatedData.Length; i++)
                        {
                            var item = aggregatedData[i];
                            float xPos = 50 + yAxisPadding + i * ticketSpacing;

                            for (int j = 0; j < properties.Length; j++)
                            {
                                var property = properties[j];
                                int value = Convert.ToInt32(item[property] ?? 0); // Safely retrieve and convert the value
                                float barHeight = graphHeight * value / maxValue;

                                // Draw the bar
                                canvas.DrawRect(
                                    xPos + j * (barWidth + barSpacing),
                                    height - barHeight - 150,
                                    barWidth,
                                    barHeight,
                                    paints[j]);

                                // Draw value above the bar
                                canvas.DrawText(value.ToString(),
                                    xPos + j * (barWidth + barSpacing) + barWidth / 2 - 10,
                                    height - barHeight - 160,
                                    textPaint);
                            }
                        }

                        // Draw the bars without Zero
                        //var xPos = 50 + yAxisPadding;

                        //// Loop through aggregated data to draw bars
                        //for (int i = 0; i < aggregatedData.Length; i++)
                        //{
                        //    var item = aggregatedData[i]; // Current data item (ticket)

                        //    for (int j = 0; j < properties.Length; j++)
                        //    {
                        //        var property = properties[j];
                        //        int value = Convert.ToInt32(item[property] ?? 0); // Safely retrieve and convert the value

                        //        if (value > 0) // Skip 0 value bars
                        //        {
                        //            float barHeight = graphHeight * value / maxValue; // Calculate bar height

                        //            // Draw the bar (skip 0 value)
                        //            canvas.DrawRect(
                        //                xPos,                              // X position for the current bar
                        //                height - barHeight - 150,          // Y position for the bar (from bottom)
                        //                barWidth,                          // Width of the bar
                        //                barHeight,                         // Height of the bar
                        //                paints[j]);                        // Color of the bar

                        //            // Draw the value above the bar
                        //            canvas.DrawText(
                        //                value.ToString(),
                        //                xPos + barWidth / 2 - 10,         // Position text at the center of the bar
                        //                height - barHeight - 160,         // Position the text above the bar
                        //                textPaint);                       // Text styling

                        //            // Increment xPos for the next valid bar (skip the space for zero-value bars)
                        //            xPos += barWidth + barSpacing;
                        //        }
                        //    }

                        //    xPos += ticketSpacing;
                        //}


                        // Draw the labels for ticket types (X-axis labels) V1
                        //for (int i = 0; i < aggregatedData.Length; i++)
                        //{
                        //    var item = aggregatedData[i]; // Dictionary<string, object>

                        //    // Get the "type" value
                        //    var typeValue = item.ContainsKey("Ticket Type") ? item["Ticket Type"]?.ToString() : string.Empty;

                        //    var xPoss = 50 + yAxisPadding + i * ticketSpacing;
                        //    canvas.DrawText(typeValue, xPoss + (propertyCount * (barWidth + barSpacing)) / 2 - 20, height - 120, textPaint);
                        //}


                        // Set a maximum width for the label
                        float maxLabelWidth = ticketSpacing - 40; // Adjust this value to provide enough space for labels

                        // Draw the labels for ticket types (X-axis labels)
                        for (int i = 0; i < aggregatedData.Length; i++)
                        {
                            var item = aggregatedData[i]; // Dictionary<string, object>

                            // Get the "type" value
                            var typeValue = item.ContainsKey("Ticket Type") ? item["Ticket Type"]?.ToString() : string.Empty;

                            var xPos = 5 + i * ticketSpacing;

                            // Dynamically adjust font size based on label width
                            float labelWidth = textPaint.MeasureText(typeValue);
                            float fontSize = textPaint.TextSize;
                            if (labelWidth > maxLabelWidth)
                            {
                                fontSize = textPaint.TextSize * (maxLabelWidth / labelWidth);  // Scale down font size
                                textPaint.TextSize = fontSize;  // Apply the new font size
                            }

                            // Draw the label
                            canvas.DrawText(typeValue, xPos + ticketSpacing / 2 - 20, height - 120, textPaint);
                        }

                        // Adjust legend Y position to be at the top
                        int legendStartY = 1; // Set Y position to the top (e.g., 50px from the top)

                        // Calculate total width for all legends
                        float totalLegendWidth = 0;
                        List<float> legendWidths = new List<float>();

                        // Calculate the width for each legend (rectangle + label) and accumulate
                        for (int j = 0; j < properties.Length; j++)
                        {
                            float labelWidth = textPaint.MeasureText(properties[j]); // Calculate width of label text
                            float legendItemWidth = labelWidth + 40; // Rectangle width (20px) + space between label and box (20px)

                            // Store the calculated width for each legend
                            legendWidths.Add(legendItemWidth);

                            // Accumulate total width of all legends
                            totalLegendWidth += legendItemWidth;
                        }

                        // Calculate the starting X position to center the legends horizontally
                        int legendStartX = (width - (int)totalLegendWidth) / 2;

                        // Draw each legend
                        for (int j = 0; j < properties.Length; j++)
                        {
                            // Get the calculated width for the current legend
                            float legendItemWidth = legendWidths[j];

                            // Draw the legend box
                            canvas.DrawRect(legendStartX, legendStartY, 20, 20, paints[j]);

                            // Draw the legend label
                            canvas.DrawText(properties[j], legendStartX + 30, legendStartY + 15, textPaint); // Align better with box

                            // Move to the next legend position
                            legendStartX += (int)legendItemWidth; // Add width of the current legend to move to the next one
                        }

                        // Add Text
                        int textStartY = height - 100;

                        // Define the text to be drawn
                        string text = "Closed Tickets";

                        // Measure the width of the text
                        float textWidth = textPaint.MeasureText(text);

                        // Calculate X and Y positions for centering
                        float textStartX = (width - textWidth) / 2; // Center horizontally

                        // Draw the text at the calculated position
                        canvas.DrawText(text, textStartX, textStartY, textPaint);



                        // Save Image
                        string finalpath = directoryName + "\\" + DateTime.Now.ToString("MMMMyyyy") + "\\BarCharts";
                        var directoryPath = await this.CheckOrCreateDirectory(finalpath);
                        if (!string.IsNullOrEmpty(directoryPath))
                        {

                            // Delete all existing files in the directory
                            //foreach (var file in Directory.GetFiles(directoryPath))
                            //{
                            //    File.Delete(file);
                            //}

                            string formattedDate = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                            string filePath = Path.Combine(directoryPath, $"{fileName}.png");

                            // Save the image to a file
                            using (var image = surface.Snapshot())
                            using (var dataImage = image.Encode(SKEncodedImageFormat.Png, 100))
                            using (var stream = File.OpenWrite(filePath))
                            {
                                dataImage.SaveTo(stream);
                                returnString = filePath;
                            }
                        }
                    }
                }
                else if (slideEnum == SlideEnum.Last3MonthsSlide)
                {
                    // Extract all dynamic property names(columns except Type)
                    var properties = aggregatedData.First()
                                   .Keys
                                   .Where(key => !key.Equals("Ticket Type", StringComparison.OrdinalIgnoreCase))
                                   .ToArray();

                    // Calculate dynamic canvas width and height
                    int ticketCount = aggregatedData.Length;
                    int propertyCount = properties.Length;
                    int barWidth = 60; // Base width for each bar
                    int barSpacing = 20; // Spacing between bars

                    int ticketSpacing = propertyCount * (barWidth + barSpacing) + 80; // Space between ticket types
                    int width = ticketCount * ticketSpacing + 100; // Adjust width dynamically
                    int contentHeight = 580; // Fixed content height
                    int height = contentHeight;

                    // Find the maximum value for each property and determine the overall max value
                    int maxBarValue = aggregatedData
                                     .SelectMany(row => properties
                                    .Select(key => Convert.ToInt32(row[key] ?? 0))) // Convert values to int, handle nulls
                                    .Max(); // Find the maximum value

                    // Dynamically calculate the Y-axis max value (round up to nearest 100)
                    int stepSize;
                    if (maxBarValue > 1000) stepSize = 500;
                    else if (maxBarValue > 500) stepSize = 250;
                    else if (maxBarValue > 200) stepSize = 100;
                    else stepSize = 50;

                    int maxValue = (int)Math.Ceiling(maxBarValue / (double)stepSize) * stepSize;

                    // Padding from Y-axis to the first bar
                    int yAxisPadding = 30;

                    // Create a new image surface
                    using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
                    {
                        var canvas = surface.Canvas;

                        // Clear the canvas with a white background
                        //canvas.Clear(SKColors.White);
                        canvas.Clear(SKColors.Transparent);

                        string[] colorArray = [];
                        if (!string.IsNullOrEmpty(slideSettings?.Config?.BarGraph.Last3Months.BarColor))
                        {
                            colorArray = slideSettings.Config.BarGraph.Last3Months.BarColor.Split(";");
                        }
                        else
                        {
                            colorArray = BarColors;
                        }

                        // Define SKPaint objects for bars
                        var paints = colorArray.Take(properties.Length).Select(color => new SKPaint
                        {
                            Color = SKColor.Parse(color),
                            Style = SKPaintStyle.Fill
                        }).ToArray();

                        // Set up paint for axes and labels
                        var axisPaint = new SKPaint { Color = SKColor.Parse("d9d9d9"), StrokeWidth = 0 };
                        var textPaint = new SKPaint { Color = SKColor.Parse("595959"), TextSize = 20, IsAntialias = true };

                        // Define the graph area
                        int graphTop = 80;
                        int graphBottom = height - 150;
                        int graphHeight = graphBottom - graphTop;
                        int graphWidth = width - 100;

                        string areaColor = GraphAreaBgColor;
                        if (!string.IsNullOrEmpty(slideSettings?.Config?.BarGraph.Last3Months.BackgroundColor))
                        {
                            areaColor = slideSettings.Config.BarGraph.Last3Months.BackgroundColor;
                        }

                        // Set background color of graph area (with transparency)
                        var graphBackgroundPaint = new SKPaint
                        {
                            Color = SKColor.Parse(areaColor).WithAlpha(128)
                        };
                        canvas.DrawRect(50, graphTop, graphWidth, graphHeight, graphBackgroundPaint);

                        // Draw X and Y axis
                        canvas.DrawLine(50, height - 150, width - 50, height - 150, axisPaint); // X Axis
                        canvas.DrawLine(50, 50, 50, height - 150, axisPaint); // Y Axis


                        // Draw the bars with Zero
                        for (int i = 0; i < aggregatedData.Length; i++)
                        {
                            var item = aggregatedData[i];
                            float xPos = 50 + yAxisPadding + i * ticketSpacing;

                            for (int j = 0; j < properties.Length; j++)
                            {
                                var property = properties[j];
                                int value = Convert.ToInt32(item[property] ?? 0); // Safely retrieve and convert the value

                                float scaleFactor = 0.9f; // Adjust scale slightly for better visibility
                                float barHeight = (graphHeight * value / maxValue) * scaleFactor;

                                // Draw the bar
                                canvas.DrawRect(
                                    xPos + j * (barWidth + barSpacing),
                                    height - barHeight - 150,
                                    barWidth,
                                    barHeight,
                                    paints[j]);

                                string val = value.ToString() == "0" ? " " : value.ToString();

                                // Draw value above the bar
                                canvas.DrawText(val,
                                    xPos + j * (barWidth + barSpacing) + barWidth / 2 - 10,
                                    height - barHeight - 160,
                                    textPaint);


                                if (i < aggregatedData.Length - 1)
                                {
                                    float separatorX = xPos + propertyCount * (barWidth + barSpacing) + barSpacing / 2; // X position for vertical line
                                    canvas.DrawLine(separatorX, height - 150, separatorX, 50, axisPaint); // Draw vertical line
                                }
                            }
                        }

                        float finalSeparatorX = width - 50; // Ensures it reaches the rightmost side
                        canvas.DrawLine(finalSeparatorX, height - 150, finalSeparatorX, 50, axisPaint);


                        // Draw the bars without Zero
                        //var xPos = 50 + yAxisPadding;

                        //// Loop through aggregated data to draw bars
                        //for (int i = 0; i < aggregatedData.Length; i++)
                        //{
                        //    var item = aggregatedData[i]; // Current data item (ticket)

                        //    for (int j = 0; j < properties.Length; j++)
                        //    {
                        //        var property = properties[j];
                        //        int value = Convert.ToInt32(item[property] ?? 0); // Safely retrieve and convert the value

                        //        if (value > 0) // Skip 0 value bars
                        //        {
                        //            float barHeight = graphHeight * value / maxValue; // Calculate bar height

                        //            // Draw the bar (skip 0 value)
                        //            canvas.DrawRect(
                        //                xPos,                              // X position for the current bar
                        //                height - barHeight - 150,          // Y position for the bar (from bottom)
                        //                barWidth,                          // Width of the bar
                        //                barHeight,                         // Height of the bar
                        //                paints[j]);                        // Color of the bar

                        //            // Draw the value above the bar
                        //            canvas.DrawText(
                        //                value.ToString(),
                        //                xPos + barWidth / 2 - 10,         // Position text at the center of the bar
                        //                height - barHeight - 160,         // Position the text above the bar
                        //                textPaint);                       // Text styling

                        //            // Increment xPos for the next valid bar (skip the space for zero-value bars)
                        //            xPos += barWidth + barSpacing;
                        //        }
                        //    }

                        //    xPos += ticketSpacing;
                        //}


                        // Draw the labels for ticket types (X-axis labels) V1
                        //for (int i = 0; i < aggregatedData.Length; i++)
                        //{
                        //    var item = aggregatedData[i]; // Dictionary<string, object>

                        //    // Get the "type" value
                        //    var typeValue = item.ContainsKey("Ticket Type") ? item["Ticket Type"]?.ToString() : string.Empty;

                        //    var xPoss = 50 + yAxisPadding + i * ticketSpacing;
                        //    canvas.DrawText(typeValue, xPoss + (propertyCount * (barWidth + barSpacing)) / 2 - 20, height - 120, textPaint);
                        //}


                        // Set a maximum width for the label
                        float maxLabelWidth = ticketSpacing - 40; // Adjust this value to provide enough space for labels

                        // Draw the labels for ticket types (X-axis labels)
                        for (int i = 0; i < aggregatedData.Length; i++)
                        {
                            var item = aggregatedData[i]; // Dictionary<string, object>

                            // Get the "type" value
                            var typeValue = item.ContainsKey("Ticket Type") ? item["Ticket Type"]?.ToString() : string.Empty;

                            var xPos = 5 + i * ticketSpacing;

                            // Dynamically adjust font size based on label width
                            float labelWidth = textPaint.MeasureText(typeValue);
                            float fontSize = textPaint.TextSize;
                            if (labelWidth > maxLabelWidth)
                            {
                                fontSize = textPaint.TextSize * (maxLabelWidth / labelWidth);  // Scale down font size
                                textPaint.TextSize = fontSize;  // Apply the new font size
                            }

                            // Draw the label
                            canvas.DrawText(typeValue, xPos + ticketSpacing / 2 - 20, height - 120, textPaint);
                        }


                        // Adjust legend Y position to be at the top
                        int legendStartY = 1; // Set Y position to the top (e.g., 50px from the top)

                        // Calculate total width for all legends
                        float totalLegendWidth = 0;
                        List<float> legendWidths = new List<float>();

                        // Calculate the width for each legend (rectangle + label) and accumulate
                        for (int j = 0; j < properties.Length; j++)
                        {
                            float labelWidth = textPaint.MeasureText(properties[j]); // Calculate width of label text
                            float legendItemWidth = labelWidth + 40; // Rectangle width (20px) + space between label and box (20px)

                            // Store the calculated width for each legend
                            legendWidths.Add(legendItemWidth);

                            // Accumulate total width of all legends
                            totalLegendWidth += legendItemWidth;
                        }

                        // Calculate the starting X position to center the legends horizontally
                        int legendStartX = (width - (int)totalLegendWidth) / 2;

                        // Draw each legend
                        for (int j = 0; j < properties.Length; j++)
                        {
                            // Get the calculated width for the current legend
                            float legendItemWidth = legendWidths[j];

                            // Draw the legend box
                            canvas.DrawRect(legendStartX, legendStartY, 20, 20, paints[j]);

                            // Draw the legend label
                            canvas.DrawText(properties[j], legendStartX + 30, legendStartY + 15, textPaint); // Align better with box

                            // Move to the next legend position
                            legendStartX += (int)legendItemWidth; // Add width of the current legend to move to the next one
                        }



                        // Save Image
                        string finalpath = directoryName + "\\" + DateTime.Now.ToString("MMMMyyyy") + "\\BarCharts";
                        var directoryPath = await this.CheckOrCreateDirectory(finalpath);
                        if (!string.IsNullOrEmpty(directoryPath))
                        {

                            // Delete all existing files in the directory
                            //foreach (var file in Directory.GetFiles(directoryPath))
                            //{
                            //    File.Delete(file);
                            //}

                            string formattedDate = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                            string filePath = Path.Combine(directoryPath, $"{fileName}.png");

                            // Save the image to a file
                            using (var image = surface.Snapshot())
                            using (var dataImage = image.Encode(SKEncodedImageFormat.Png, 100))
                            using (var stream = File.OpenWrite(filePath))
                            {
                                dataImage.SaveTo(stream);
                                returnString = filePath;
                            }
                        }
                    }
                }
                await Task.Delay(0);
                return returnString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> IsValidData(IDictionary<string, object>[] aggregatedData)
        {
            // Invalid if the array is null or empty
            if (aggregatedData == null || aggregatedData.Length == 0)
            {
                await Task.Delay(0);
                return false;
            }

            var data = aggregatedData.All(item =>
            {
                // Check if the dictionary contains a "type" key and its value is valid
                if (!item.ContainsKey("type"))
                    return false;

                var typeValue = item["type"] as string;
                return !string.IsNullOrWhiteSpace(typeValue);
            });

            await Task.Delay(0);
            return data;
        }

        #endregion

        #region PPT Gen

        public async Task<string> GeneratePpt(DataSet Datas, HelperModel helperModel)
        {
            try
            {
                // Validate inputs
                if (Datas == null || Datas.Tables.Count == 0)
                {
                    throw new ArgumentException("The provided Data Set is null or empty.");
                }

                if (string.IsNullOrWhiteSpace(helperModel.DirectoryName))
                {
                    throw new ArgumentException("The CompanyId is null or empty.");
                }

                // Create presentation
                //using (PresentationDocument presentationDoc = PresentationDocument.Create(filePath, PresentationDocumentType.Presentation))
                //{

                //    PresentationPart presentationPart = presentationDoc.AddPresentationPart();
                //    presentationPart.Presentation = new Presentation();

                //    // Add Slide Master and Layout
                //    AddSlideMasterAndLayout(presentationPart);

                //    int rowsPerSlide = 8;

                //    // Generate First Cover slide
                //    GenerateSlidesForData(presentationPart, SlideEnum.FirstSlide, helperModel);

                //    // Generate Agenda slide
                //    GenerateSlidesForData(presentationPart, SlideEnum.AgendaSlide, helperModel);

                //    // Generate Slide Cover Page
                //    GenerateSlidesForData(presentationPart, SlideEnum.MonthlyServiceCover, helperModel);

                //    // Access the data set and generate slides for each data category
                //    foreach (DataTable table in Datas.Tables)
                //    {
                //        GenerateSlidesForData(presentationPart, table, table.TableName, helperModel.DirectoryName, helperModel.MonthName, helperModel.DataTableForChart);
                //    }

                //    // Generate ThankYou slide
                //    GenerateSlidesForData(presentationPart, SlideEnum.ThankYouSlide, helperModel);

                //    // Save the presentation
                //    presentationPart.Presentation.Save();
                //};

                string sourceFilePath = System.IO.Directory.GetCurrentDirectory() + @"\Documents_Internal\PPT\Presentation.pptx";
                string customDirectoryName = helperModel.DirectoryName + "\\" + DateTime.Now.ToString("MMMMyyyy") + "\\Presentations";
                string outputDirectoryPath = await this.CheckOrCreateDirectory(customDirectoryName);

                if (string.IsNullOrEmpty(outputDirectoryPath))
                {
                    throw new ArgumentException("DirectoryPath is null or empty.");
                }

                string outputFilePath = Path.Combine(outputDirectoryPath, $"Presentation.pptx");
                if (!await this.CopyPowerPointFile(sourceFilePath, outputFilePath))
                {
                    throw new ArgumentException("Unable to execute CopyPowerPointFile()");
                }

                // Edit presentation
                using (PresentationDocument presentation = PresentationDocument.Open(outputFilePath, true))
                {
                    // Access the presentation part
                    PresentationPart presentationPart = presentation.PresentationPart;

                    bool isTechnicalDomainCoverPrinted = false;
                    bool isMonthlyServiceCoverPrinted = false;

                    if (presentationPart != null)
                    {
                        // Generate First Cover slide
                        await GenerateSlidesForData(presentationPart, SlideEnum.FirstSlide, helperModel);

                        // Generate Agenda slide
                        await GenerateSlidesForData(presentationPart, SlideEnum.AgendaSlide, helperModel);

                        // Access the data set and generate slides for each data category
                        foreach (DataTable table in Datas.Tables)
                        {
                            try
                            {
                                if (!isTechnicalDomainCoverPrinted)
                                {
                                    string SlideType = string.Empty;

                                    if (table.ExtendedProperties.ContainsKey("Code"))
                                    {
                                        SlideType = table.ExtendedProperties["Code"].ToString();
                                    }

                                    if (!string.IsNullOrEmpty(SlideType)
                                        && (
                                            SlideType == SlideCodeEnum.C011.ToString()
                                            || SlideType == SlideCodeEnum.C012.ToString()
                                            || SlideType == SlideCodeEnum.C013.ToString())
                                        )
                                    {
                                        string sourcePath8 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "TechnicalDomain.pptx");
                                        await this.CopySlide(sourcePath8, presentationPart);

                                        isTechnicalDomainCoverPrinted = true;
                                    }
                                }

                                if (!isMonthlyServiceCoverPrinted)
                                {
                                    string SlideType = string.Empty;

                                    if (table.ExtendedProperties.ContainsKey("Code"))
                                    {
                                        SlideType = table.ExtendedProperties["Code"].ToString();
                                    }

                                    if (!string.IsNullOrEmpty(SlideType)
                                        && (
                                            SlideType == SlideCodeEnum.C001.ToString()
                                            || SlideType == SlideCodeEnum.C002.ToString()
                                            || SlideType == SlideCodeEnum.C003.ToString()
                                            || SlideType == SlideCodeEnum.C004.ToString()
                                            || SlideType == SlideCodeEnum.C005.ToString()
                                            || SlideType == SlideCodeEnum.C006.ToString()
                                            || SlideType == SlideCodeEnum.C007.ToString()
                                            || SlideType == SlideCodeEnum.C008.ToString()
                                            || SlideType == SlideCodeEnum.C009.ToString()
                                            || SlideType == SlideCodeEnum.C010.ToString()

                                            )
                                        )
                                    {
                                        // Generate Slide Cover Page
                                        await this.GenerateSlidesForData(presentationPart, SlideEnum.MonthlyServiceCover, helperModel);

                                        isMonthlyServiceCoverPrinted = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                
                            }
                            //string TableName = table.TableName.Contains("CO23") ? "Daily / Monthly activity details:" : table.TableName;

                            await this.GenerateSlidesForData(presentationPart, table, table.TableName, helperModel.DirectoryName, helperModel.MonthName, helperModel.DataTableForChart);
                        }

                        // Generate ThankYou slide
                        await this.GenerateSlidesForData(presentationPart, SlideEnum.ThankYouSlide, helperModel);
                    }

                    // Delete the first slide
                    await this.DeleteSlide(presentationPart, 0); // 0 means the first slide

                    presentationPart.Presentation.Save();
                    await Task.Delay(100);
                }

                await Task.Delay(100);
                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        private async Task DeleteSlide(PresentationPart presentationPart, int slideIndex)
        {
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;
            SlideId slideId = slideIdList.ChildElements[slideIndex] as SlideId;

            if (slideId != null)
            {
                // Get the SlidePart associated with the SlideId
                SlidePart slidePart = presentationPart.GetPartById(slideId.RelationshipId) as SlidePart;

                // Delete the SlidePart
                if (slidePart != null)
                {
                    presentationPart.DeletePart(slidePart);
                }

                // Remove the SlideId from the SlideIdList
                slideIdList.RemoveChild(slideId);

                // Save the presentation
                presentationPart.Presentation.Save();

                await Task.Delay(100);
            }
        }

        private async Task AddSlideMasterAndLayout(PresentationPart presentationPart)
        {
            // Add Slide Master Part
            SlideMasterPart slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>("rId1");
            slideMasterPart.SlideMaster = new P.SlideMaster(
                new P.CommonSlideData(new P.ShapeTree()));

            // Add Slide Layout Part
            SlideLayoutPart slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>("rId2");
            slideLayoutPart.SlideLayout = new P.SlideLayout(
                new P.CommonSlideData(new P.ShapeTree()));

            // Link Slide Layout to Slide Master
            slideMasterPart.SlideMaster.Append(
                new P.SlideLayoutIdList(
                    new P.SlideLayoutId
                    {
                        Id = 1U,
                        RelationshipId = "rId1"
                    }));

            slideMasterPart.SlideMaster.Save();

            await Task.Delay(0);
        }

        private async Task GenerateSlidesForData(PresentationPart presentationPart, SlideEnum slideEnum, HelperModel? helperModel)
        {
            if (slideEnum == SlideEnum.FirstSlide)
            {
                // Add a new slide
                SlidePart slidePart = await this.AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBg.png";

                await this.AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(19.5 * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgGradient.png";

                await this.AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                await this.AddImageToSlide(slidePart, EmbeeLogo_Path,
                    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);

                // Company Logo
                long CompanyLogo_ImageWidthEmu = (long)(4.79 * 360000);  // Width in EMU
                long CompanyLogo_ImageHeightEmu = (long)(2.52 * 360000); // Height in EMU
                long CompanyLogo_OffsetXEmu = (long)(1.02 * 360000);    // Horizontal position in EMU
                long CompanyLogo_OffsetYEmu = (long)(2.68 * 360000);    // Vertical position in EMU (negative for above the origin)

                string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                await this.AddImageToSlide(slidePart, CompanyLogo_Path,
                    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                string slideName = "Monthly MSD Review";
                string key = "C000";
                if (slideSettings.SlideHeadingText.TryGetValue(key, out string headingText) && !string.IsNullOrEmpty(headingText))
                {
                    slideName = headingText;
                }

                // Add title text to the slide
                await this.AddTextToSlide(slidePart, slideName, SlideEnum.FirstSlide, helperModel);

                /*

                // Background Image
                long Bg_ImageWidthEmu = (long)(21.42 * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(4 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBg.png";

                AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgGradient.png";

                AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                AddImageToSlide(slidePart, EmbeeLogo_Path,
                    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);


                // Company Logo
                long CompanyLogo_ImageWidthEmu = (long)(4.79 * 360000);  // Width in EMU
                long CompanyLogo_ImageHeightEmu = (long)(2.52 * 360000); // Height in EMU
                long CompanyLogo_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long CompanyLogo_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                AddImageToSlide(slidePart, CompanyLogo_Path,
                    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                // Add title text to the slide
                AddTextToSlide(slidePart, "Monthly MSD Review", SlideEnum.FirstSlide, helperModel);

                */
            }

            if (slideEnum == SlideEnum.AgendaSlide)
            {
                /*
                // Add a new slide
                SlidePart slidePart = AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(21.42 * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(4 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\AgendaPageBg.png";

                AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgGradient.png";

                AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);

                // Embee Logo
                long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                AddImageToSlide(slidePart, EmbeeLogo_Path,
                    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);


                // Add title text to the slide
                AddTextToSlide(slidePart, "Agenda", SlideEnum.AgendaSlide);

                */

                string sourcePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "Agenda.pptx");
                await this.CopySlide(sourcePath, presentationPart);
            }

            if (slideEnum == SlideEnum.MonthlyServiceCover)
            {
                /*

                // Add a new slide
                SlidePart slidePart = AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\MonthlyServiceBg.png";

                AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\MonthlyServiceGradient.png";

                AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                long EmbeeLogo_ImageWidthEmu = (long)(8.37 * 360000);  // Width in EMU
                long EmbeeLogo_ImageHeightEmu = (long)(2.33 * 360000); // Height in EMU
                long EmbeeLogo_OffsetXEmu = (long)(0.45 * 360000);    // Horizontal position in EMU
                long EmbeeLogo_OffsetYEmu = (long)(3.52 * 360000);    // Vertical position in EMU (negative for above the origin)

                string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo_White.png";

                AddImageToSlide(slidePart, EmbeeLogo_Path,
                    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);


                // Company Logo
                //long CompanyLogo_ImageWidthEmu = (long)(8.1 * 360000);  // Width in EMU
                //long CompanyLogo_ImageHeightEmu = (long)(4.26 * 360000); // Height in EMU
                //long CompanyLogo_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                //long CompanyLogo_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                //AddImageToSlide(slidePart, CompanyLogo_Path,
                //    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                // Add title text to the slide
                AddTextToSlide(slidePart, "Discussion on last\nMonths services", SlideEnum.MonthlyServiceCover, helperModel);

                */

                string sourcePath1 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "LastMonthServices.pptx");
                await this.CopySlide(sourcePath1, presentationPart);
            }

            if (slideEnum == SlideEnum.ThankYouSlide)
            {

                /*
               // Add a new slide
               SlidePart slidePart = AddSlide(presentationPart);

               // Background Image
               long Bg_ImageWidthEmu = (long)(13 * 360000);  // Width in EMU
               long Bg_ImageHeightEmu = (long)(10 * 360000); // Height in EMU
               long Bg_OffsetXEmu = (long)(11.5 * 360000);    // Horizontal position in EMU
               long Bg_OffsetYEmu = (long)(0.5 * 360000);    // Vertical position in EMU (negative for above the origin)

               string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\ThankYouBg.png";

               AddImageToSlide(slidePart, Bg_Path,
                   Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);


               // Embee Logo
               long EmbeeLogo_ImageWidthEmu = (long)(4.32 * 360000);  // Width in EMU
               long EmbeeLogo_ImageHeightEmu = (long)(1.6 * 360000); // Height in EMU
               long EmbeeLogo_OffsetXEmu = (long)(0.35 * 360000);    // Horizontal position in EMU
               long EmbeeLogo_OffsetYEmu = (long)(3.9 * 360000);    // Vertical position in EMU (negative for above the origin)

               string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

               AddImageToSlide(slidePart, EmbeeLogo_Path,
                   EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);


               // Globe Logo
               long Globe_ImageWidthEmu = (long)(0.8 * 360000);  // Width in EMU
               long Globe_ImageHeightEmu = (long)(0.8 * 360000); // Height in EMU
               long Globe_OffsetXEmu = (long)(0.35 * 360000);    // Horizontal position in EMU
               long Globe_OffsetYEmu = (long)(11.18 * 360000);    // Vertical position in EMU (negative for above the origin)

               string Globe_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\Icon_Globe.png";

               AddImageToSlide(slidePart, Globe_Path,
                   Globe_ImageWidthEmu, Globe_ImageHeightEmu, Globe_OffsetXEmu, Globe_OffsetYEmu);


               // Email Logo
               long Email_ImageWidthEmu = (long)(0.8 * 360000);  // Width in EMU
               long Email_ImageHeightEmu = (long)(0.8 * 360000); // Height in EMU
               long Email_OffsetXEmu = (long)(6.8 * 360000);    // Horizontal position in EMU
               long Email_OffsetYEmu = (long)(11.18 * 360000);    // Vertical position in EMU (negative for above the origin)

               string Email_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\Icon_Email.png";

               AddImageToSlide(slidePart, Email_Path,
                   Email_ImageWidthEmu, Email_ImageHeightEmu, Email_OffsetXEmu, Email_OffsetYEmu);


               // Phone Logo
               long Phone_ImageWidthEmu = (long)(0.8 * 360000);  // Width in EMU
               long Phone_ImageHeightEmu = (long)(0.8 * 360000); // Height in EMU
               long Phone_OffsetXEmu = (long)(13.25 * 360000);    // Horizontal position in EMU
               long Phone_OffsetYEmu = (long)(11.18 * 360000);    // Vertical position in EMU (negative for above the origin)

               string Phone_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\Icon_Phone.png";

               AddImageToSlide(slidePart, Phone_Path,
                   Phone_ImageWidthEmu, Phone_ImageHeightEmu, Phone_OffsetXEmu, Phone_OffsetYEmu);


               // Location Logo
               long Location_ImageWidthEmu = (long)(1 * 360000);  // Width in EMU
               long Location_ImageHeightEmu = (long)(1 * 360000); // Height in EMU
               long Location_OffsetXEmu = (long)(0.25 * 360000);    // Horizontal position in EMU
               long Location_OffsetYEmu = (long)(12.33 * 360000);    // Vertical position in EMU (negative for above the origin)

               string Location_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\Icon_Location.png";

               AddImageToSlide(slidePart, Location_Path,
                   Location_ImageWidthEmu, Location_ImageHeightEmu, Location_OffsetXEmu, Location_OffsetYEmu);


               // Add title text to the slide
               AddTextToSlide(slidePart, "Thank You", SlideEnum.ThankYouSlide);
               */

                string sourcePath2 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "ThankYou.pptx");
                await this.CopySlide(sourcePath2, presentationPart);
            }
            
            //soumik rev 27-2025
            if (slideEnum == SlideEnum.FirstSlideOnMobile)
            {
                // Add a new slide
                SlidePart slidePart = await this.AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgOnMobile.png";

                await this.AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                //long Gradient_ImageWidthEmu = (long)(19.5 * 360000);  // Width in EMU
                //long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                //long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                //long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgOnMobile.png";

                //await this.AddImageToSlide(slidePart, Gradient_Path,
                //    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                //long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                //long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                //long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                //long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                //await this.AddImageToSlide(slidePart, EmbeeLogo_Path,
                //    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);

                // Company Logo
                long CompanyLogo_ImageWidthEmu = (long)(4.79 * 360000);  // Width in EMU
                long CompanyLogo_ImageHeightEmu = (long)(2.52 * 360000); // Height in EMU
                long CompanyLogo_OffsetXEmu = (long)(1.02 * 360000);    // Horizontal position in EMU
                long CompanyLogo_OffsetYEmu = (long)(2.68 * 360000);    // Vertical position in EMU (negative for above the origin)

                string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                await this.AddImageToSlide(slidePart, CompanyLogo_Path,
                    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                string slideName = "Monthly MSD Review";
                string key = "C000";
                if (slideSettings.SlideHeadingText.TryGetValue(key, out string headingText) && !string.IsNullOrEmpty(headingText))
                {
                    slideName = headingText;
                }

                // Add title text to the slide
                await this.AddTextToSlide(slidePart, slideName, SlideEnum.FirstSlide, helperModel);

            }
 

            if (slideEnum == SlideEnum.IncidentTrendAnalysis)
            {
                // Add a new slide
                SlidePart slidePart = await this.AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\IncidentTrendAnalysisCover.jpg";

                await this.AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(19.5 * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgGradient.png";

                await this.AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                await this.AddImageToSlide(slidePart, EmbeeLogo_Path,
                    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);

                // Company Logo
                //long CompanyLogo_ImageWidthEmu = (long)(4.79 * 360000);  // Width in EMU
                //long CompanyLogo_ImageHeightEmu = (long)(2.52 * 360000); // Height in EMU
                //long CompanyLogo_OffsetXEmu = (long)(1.02 * 360000);    // Horizontal position in EMU
                //long CompanyLogo_OffsetYEmu = (long)(2.68 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                //await this.AddImageToSlide(slidePart, CompanyLogo_Path,
                //    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                string slideName = "Incident trend analysis \r\n(last 3 months)\r\n";
                //string key = "C000";
                //if (slideSettings.SlideHeadingText.TryGetValue(key, out string headingText) && !string.IsNullOrEmpty(headingText))
                //{
                //    slideName = headingText;
                //}

                // Add title text to the slide
                await this.AddTextToSlide(slidePart, slideName, SlideEnum.MonthlyServiceCover, helperModel);

            }
            
            if (slideEnum == SlideEnum.ResponseResolutionPerformanceDetails)
            {
                // Add a new slide
                SlidePart slidePart = await this.AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\Response&ResolutionDetails.jpg";

                await this.AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(19.5 * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgGradient.png";

                await this.AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                //long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                //long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                //long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                //long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                //await this.AddImageToSlide(slidePart, EmbeeLogo_Path,
                //    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);

                // Company Logo
                //long CompanyLogo_ImageWidthEmu = (long)(4.79 * 360000);  // Width in EMU
                //long CompanyLogo_ImageHeightEmu = (long)(2.52 * 360000); // Height in EMU
                //long CompanyLogo_OffsetXEmu = (long)(1.02 * 360000);    // Horizontal position in EMU
                //long CompanyLogo_OffsetYEmu = (long)(2.68 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                //await this.AddImageToSlide(slidePart, CompanyLogo_Path,
                //    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                string slideName = "Response & Resolution\r\nperformance details";
                //string key = "C000";
                //if (slideSettings.SlideHeadingText.TryGetValue(key, out string headingText) && !string.IsNullOrEmpty(headingText))
                //{
                //    slideName = headingText;
                //}

                // Add title text to the slide
                await this.AddTextToSlide(slidePart, slideName, SlideEnum.MonthlyServiceCover, helperModel);

            }
            if (slideEnum == SlideEnum.NetworkCategory)
            {
                // Add a new slide
                SlidePart slidePart = await this.AddSlide(presentationPart);

                // Background Image
                long Bg_ImageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
                long Bg_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Bg_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Bg_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Bg_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\Response&ResolutionDetails.jpg";

                await this.AddImageToSlide(slidePart, Bg_Path,
                    Bg_ImageWidthEmu, Bg_ImageHeightEmu, Bg_OffsetXEmu, Bg_OffsetYEmu);

                // Gradient Effect on Background Image
                long Gradient_ImageWidthEmu = (long)(19.5 * 360000);  // Width in EMU
                long Gradient_ImageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
                long Gradient_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                long Gradient_OffsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

                string Gradient_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\FirstPageBgGradient.png";

                await this.AddImageToSlide(slidePart, Gradient_Path,
                    Gradient_ImageWidthEmu, Gradient_ImageHeightEmu, Gradient_OffsetXEmu, Gradient_OffsetYEmu);


                // Embee Logo
                //long EmbeeLogo_ImageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
                //long EmbeeLogo_ImageHeightEmu = (long)(0.92 * 360000); // Height in EMU
                //long EmbeeLogo_OffsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
                //long EmbeeLogo_OffsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string EmbeeLogo_Path = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";

                //await this.AddImageToSlide(slidePart, EmbeeLogo_Path,
                //    EmbeeLogo_ImageWidthEmu, EmbeeLogo_ImageHeightEmu, EmbeeLogo_OffsetXEmu, EmbeeLogo_OffsetYEmu);

                // Company Logo
                //long CompanyLogo_ImageWidthEmu = (long)(4.79 * 360000);  // Width in EMU
                //long CompanyLogo_ImageHeightEmu = (long)(2.52 * 360000); // Height in EMU
                //long CompanyLogo_OffsetXEmu = (long)(1.02 * 360000);    // Horizontal position in EMU
                //long CompanyLogo_OffsetYEmu = (long)(2.68 * 360000);    // Vertical position in EMU (negative for above the origin)

                //string CompanyLogo_Path = Directory.GetCurrentDirectory() + $@"\Documents_Internal\Images\{helperModel.DepartmentId}.png";

                //await this.AddImageToSlide(slidePart, CompanyLogo_Path,
                //    CompanyLogo_ImageWidthEmu, CompanyLogo_ImageHeightEmu, CompanyLogo_OffsetXEmu, CompanyLogo_OffsetYEmu);


                string slideName = "Network Category wise ticket analysis\r\n";
                //string key = "C000";
                //if (slideSettings.SlideHeadingText.TryGetValue(key, out string headingText) && !string.IsNullOrEmpty(headingText))
                //{
                //    slideName = headingText;
                //}

                // Add title text to the slide
                await this.AddTextToSlide(slidePart, slideName, SlideEnum.MonthlyServiceCover, helperModel);

            }

        }

        private async Task GenerateSlidesForData(PresentationPart presentationPart, DataTable dataTable, string slideText, string companyName = "", string MonthName = "", DataTable? dataTableForChart = null)
        {



            string SlideType = string.Empty;

            if (dataTable.ExtendedProperties.ContainsKey("Code"))
            {
                SlideType = dataTable.ExtendedProperties["Code"].ToString();
            }


            //if (SlideType == SlideCodeEnum.C022_1.ToString())
            //{
            //    // Store first table and return (wait for C022-2)
            //    pendingC022Table = dataTable;
            //    return;
            //}

            if (SlideType == SlideCodeEnum.C001.ToString()
                || SlideType == SlideCodeEnum.C002.ToString()
                || SlideType == SlideCodeEnum.C003.ToString()
                || SlideType == SlideCodeEnum.C004.ToString()
                || SlideType == SlideCodeEnum.C005.ToString()
                || SlideType == SlideCodeEnum.C006.ToString()
                || SlideType == SlideCodeEnum.C007.ToString()
                || SlideType == SlideCodeEnum.C008.ToString()
                || SlideType == SlideCodeEnum.C022.ToString()

                )
            {
                slideText += " for " + MonthName;
            }


            if (SlideType == SlideCodeEnum.C016.ToString())
            {
                string sourcePath6 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "MOM.pptx");
                await this.CopySlide(sourcePath6, presentationPart);
                return;
            }


            if (SlideType == SlideCodeEnum.C017.ToString())
            {
                string sourcePath3 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "ProjectHighlights.pptx");
                await this.CopySlide(sourcePath3, presentationPart);
                return;
            }


            if (SlideType == SlideCodeEnum.C018.ToString())
            {
                string sourcePath4 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "ITLandscape.pptx");
                await this.CopySlide(sourcePath4, presentationPart);
                return;
            }


            if (SlideType == SlideCodeEnum.C019.ToString())
            {
                string sourcePath5 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "CSAT.pptx");
                await this.CopySlide(sourcePath5, presentationPart);
                return;
            }


            if (SlideType == SlideCodeEnum.C020.ToString())
            {
                string sourcePath7 = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "MajorActivities.pptx");
                await this.CopySlide(sourcePath7, presentationPart);
                return;
            }


            if (SlideType == SlideCodeEnum.C021.ToString())
            {
                string sourcePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents_Internal", "PPT", "EscalationMatrix.pptx");
                await this.CopySlide(sourcePath, presentationPart);
                return;
            }


            // Add logo
            string EmbeelogoPath = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";
            long imageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
            long imageHeightEmu = (long)(0.92 * 360000); // Height in EMU
            long offsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
            long offsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU

            // Return if the DataTable is null or empty
            if (dataTable == null || dataTable.Rows.Count == 0)
                return;

            const long SlideHeightEmu = 6543500; // Total available slide height in EMUs
            const long TopMarginEmu = 475200;   // Top margin (1.32 cm in EMUs)
            const long RowHeightBaseEmu = 370000; // Base height for a row in EMUs
            int MaxRowsPerSlide = TBL_MAX_ROW_COUNT; // Max rows per slide as a fallback for low-content rows

            if (slideSettings?.Config?.TableRows?.MaxCountPerTable != null)
            {
                MaxRowsPerSlide = slideSettings.Config.TableRows.MaxCountPerTable;
            }

            int totalRows = dataTable.Rows.Count;
            int currentRowIndex = 0;

            SlidePart first_slidePart = null;
            bool isSameSlide = false;

            if (SlideType == SlideCodeEnum.C014.ToString() || SlideType == SlideCodeEnum.C015.ToString())
            {
                SlidePart slidePart = null;

                while (currentRowIndex < totalRows)
                {
                    // Add a new slide only when starting a new pair of tables
                    if (!isSameSlide)
                    {
                        slidePart = await this.AddSlide(presentationPart);

                        if (currentRowIndex == 0
                            && (SlideType == SlideCodeEnum.C001.ToString() || SlideType == SlideCodeEnum.C009.ToString()))
                        {
                            first_slidePart = slidePart;
                        }

                        // Add title text and image only once per slide
                        await this.AddTextToSlide(slidePart, slideText);
                        await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                    }

                    // Create a new DataTable for the current table
                    DataTable slideTable = dataTable.Clone();
                    long currentTableHeight = TopMarginEmu;

                    int rowCountOnTable = 0;
                    while (currentRowIndex < totalRows)
                    {
                        string rowText = string.Join(" ", dataTable.Rows[currentRowIndex].ItemArray);
                        long estimatedRowHeight = await this.EstimateRowHeight(rowText, RowHeightBaseEmu);

                        if (currentTableHeight + estimatedRowHeight > SlideHeightEmu || rowCountOnTable >= MaxRowsPerSlide)
                            break;

                        slideTable.ImportRow(dataTable.Rows[currentRowIndex]);
                        currentRowIndex++;
                        currentTableHeight += estimatedRowHeight;
                        rowCountOnTable++;
                    }

                    // Determine table placement: left table (first) or right table (second)
                   await this.AddTableToSlide(slidePart, slideTable, SlideType, isSameSlide);

                    // Flip `isSameSlide` flag
                    isSameSlide = !isSameSlide;
                }
            }
            else if (SlideType == SlideCodeEnum.C022.ToString())
            {

                DataTable NonPlantUserTable = this.BuildUserTable("Non-Plant User", dataTable);
                DataTable PlantUserTable = this.BuildUserTable("Plant User", dataTable);

                SlidePart slidePart = null;
                slidePart = await this.AddSlide(presentationPart);
                await this.AddTextToSlide(slidePart, slideText);
                await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                //await this.AddTextToSlide(slidePart, slideText, null, null, 0.75, 1.13);


                await this.AddTextToSlide(slidePart, "Non Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { },1.49);
                await this.AddTableToSlide(slidePart, NonPlantUserTable, SlideType,false,0.75, 2.41);


                await this.AddTextToSlide(slidePart, "Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { }, 7.83);
                await this.AddTableToSlide(slidePart, PlantUserTable, SlideType,false, 0.75,  8.81);

               

            }

            else if (SlideType == SlideCodeEnum.C023.ToString())
            {

                DataTable SourceDataTable = dataTable;

                DataTable DataTable1 = SourceDataTable.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table1")
                    .CopyToDataTable();

                // Keep only the needed columns
                DataTable1 = DataTable1.DefaultView.ToTable(false,
                    "Daily and Monthly SR Activity Analysis",
                    "Activity Count");

                DataTable DataTable2 = SourceDataTable.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table2")
                    .CopyToDataTable();

                // Keep only the needed columns
                DataTable2 = DataTable2.DefaultView.ToTable(false,
                    "Incident",
                    "Service Request",
                    "Daily / Monthly / Operational Calls");

                DataTable DataTable3 = SourceDataTable.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table3")
                    .CopyToDataTable();

                // Keep only the needed columns
                DataTable3 = DataTable3.DefaultView.ToTable(false,
                    "Daily & Monthly SR Activity (Team Wise Bifurcation)",
                    "Count");


                SlidePart slidePart = null;
                slidePart = await this.AddSlide(presentationPart);
                await this.AddTextToSlide(slidePart, slideText);
                await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                ////await this.AddTextToSlide(slidePart, slideText, null, null, 0.75, 1.13);


                //await this.AddTextToSlide(slidePart, "Non Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { }, 1.49);
                await this.AddTableToSlide(slidePart, DataTable1, SlideType, false,0.75,1.32, 10.82);
                await this.AddTableToSlide(slidePart, DataTable2, SlideType, false, 0.75, 8.81,10.82);
                await this.AddTableToSlide(slidePart, DataTable3, SlideType, false, 13.00, 5.76, 10.82);



                //await this.AddTextToSlide(slidePart, "Monthly activities consists of DLP report, O365 & " +
                //    "OneDrive monthly reports, Health checkup report, Plant user data etc.", SlideEnum.DailyMonthlyActivity, new HelperModel { });
                

                //soumikk

            }

            else
            {
                while (currentRowIndex < totalRows)
                {
                    // Add a new slide
                    SlidePart slidePart = await this.AddSlide(presentationPart);

                    if (currentRowIndex == 0
                        && (SlideType == SlideCodeEnum.C001.ToString() || SlideType == SlideCodeEnum.C009.ToString()))
                    {
                        first_slidePart = slidePart;
                    }

                    // Add title text to the slide
                    await this.AddTextToSlide(slidePart, slideText);

                    await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                    // Create a new DataTable for the current slide's rows
                    DataTable slideTable = dataTable.Clone();
                    long currentTableHeight = TopMarginEmu; // Start with top margin

                    // Add rows dynamically based on content height
                    int rowCountOnSlide = 0;
                    while (currentRowIndex < totalRows)
                    {
                        string rowText = string.Join(" ", dataTable.Rows[currentRowIndex].ItemArray);
                        long estimatedRowHeight = await this.EstimateRowHeight(rowText, RowHeightBaseEmu);

                        // Check if adding this row exceeds the slide height
                        if (currentTableHeight + estimatedRowHeight > SlideHeightEmu || rowCountOnSlide >= MaxRowsPerSlide)
                            break;

                        slideTable.ImportRow(dataTable.Rows[currentRowIndex]);
                        currentRowIndex++;
                        currentTableHeight += estimatedRowHeight;
                        rowCountOnSlide++;
                    }

                    // Add the table to the slide
                    await this.AddTableToSlide(slidePart, slideTable, SlideType);
                }
            }

            // Handle specific SlideType with charts
            if (SlideType == SlideCodeEnum.C001.ToString())
            {
                var removeCategory = true;

                if (removeCategory)
                {
                    // Define columns to exclude
                    var columnsToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "Category", "Grand Total"
                    };

                    // Create a new DataTable to hold transformed data
                    DataTable resultTable = new DataTable();

                    // Add the dynamic "Ticket Type" column
                    resultTable.Columns.Add("Ticket Type", typeof(string));

                    // Dynamically add other columns from inputTable except excluded ones
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (!columnsToExclude.Contains(column.ColumnName))
                        {
                            resultTable.Columns.Add(column.ColumnName, column.DataType);
                        }
                    }

                    // Filter rows where RowType == 0
                    var filteredRows = dataTable.AsEnumerable();
                    var lastRow = filteredRows.LastOrDefault();

                    if (lastRow != null)
                    {
                        // Create a new row for resultTable
                        DataRow newRow = resultTable.NewRow();

                        // newRow["Ticket Type"] = lastRow.Field<string>("Category");
                        newRow["Ticket Type"] = "";

                        // Populate other columns dynamically
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (!columnsToExclude.Contains(column.ColumnName))
                            {
                                newRow[column.ColumnName] = lastRow[column];
                            }
                        }

                        // Add the populated row to the result table
                        resultTable.Rows.Add(newRow);

                        SlidePart chartSlidePart = null;

                        var height = 4.78;
                        var width = 25.4;

                        long ChartWidthEmu = (long)(width * 360000);
                        long ChartHeightEmu = (long)(height * 360000);
                        long ChartoffsetXEmu = (long)(9.61 * 360000);
                        long ChartoffsetYEmu = (long)(1.51 * 360000);

                        if (first_slidePart != null)
                        {
                            chartSlidePart = first_slidePart;

                            if (dataTable.Columns.Count == 3)
                            {
                                ChartWidthEmu = (long)(4.4 * 360000);
                                ChartHeightEmu = (long)(11.27 * 360000);
                                ChartoffsetXEmu = (long)(16.9 * 360000);
                                ChartoffsetYEmu = (long)(1.32 * 360000);
                            }
                            else if (dataTable.Columns.Count == 4)
                            {
                                ChartWidthEmu = (long)(6.27 * 360000);
                                ChartHeightEmu = (long)(11.27 * 360000);
                                ChartoffsetXEmu = (long)(16.9 * 360000);
                                ChartoffsetYEmu = (long)(1.32 * 360000);
                            }
                            else
                            {
                                ChartWidthEmu = (long)(8.10 * 360000);
                                ChartHeightEmu = (long)(11.27 * 360000);
                                ChartoffsetXEmu = (long)(17.10 * 360000);
                                ChartoffsetYEmu = (long)(1.51 * 360000);
                            }
                        }
                        else
                        {
                            chartSlidePart = await this.AddSlide(presentationPart);
                            await this.AddTextToSlide(chartSlidePart, slideText + " Graphical View");
                            await this.AddImageToSlide(chartSlidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                        }

                        try
                        {
                            string BarChartfilePath = await this.GetBarChartFilePath(resultTable, companyName, SlideEnum.IncidentSlide);

                            var dimensions = await this.GetImageDimensionsInCm(BarChartfilePath);

                            if (dimensions.widthCm > 0)
                            {
                                width = dimensions.widthCm;
                            }

                            if (dimensions.heightCm > 0)
                            {
                                height = dimensions.heightCm;
                            }

                            await this.AddImageToSlide(chartSlidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    // Define columns to exclude
                    var columnsToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "Category", "Grand Total"
                    };

                    // Create a new DataTable to hold transformed data
                    DataTable resultTable = new DataTable();

                    // Add the dynamic "Ticket Type" column
                    resultTable.Columns.Add("Ticket Type", typeof(string));


                    if (dataTable.Rows.Count > 0)
                    {
                        dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
                    }

                    // Dynamically add other columns from inputTable except excluded ones
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (!columnsToExclude.Contains(column.ColumnName))
                        {
                            resultTable.Columns.Add(column.ColumnName, column.DataType);
                        }
                    }

                    // Filter rows where RowType == 0
                    var filteredRows = dataTable.AsEnumerable();

                    // Populate the new DataTable
                    foreach (var row in filteredRows)
                    {
                        // Create a new row for resultTable
                        DataRow newRow = resultTable.NewRow();

                        // Set the "Ticket Type" column value
                        //string category = row.Field<string>("Category");
                        //string months = row.Field<string>("Months");
                        //newRow["Ticket Type"] = $"{category} {months}";

                        string months = row.Field<string>("Category");
                        newRow["Ticket Type"] = $"{months}";

                        // Populate other columns dynamically
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (!columnsToExclude.Contains(column.ColumnName))
                            {
                                newRow[column.ColumnName] = row[column];
                            }
                        }

                        // Add the populated row to the result table
                        resultTable.Rows.Add(newRow);
                    }


                    SlidePart chartSlidePart = await this.AddSlide(presentationPart);
                    await this.AddTextToSlide(chartSlidePart, slideText);

                    await this.AddImageToSlide(chartSlidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                    try
                    {
                        string BarChartfilePath = await this.GetBarChartFilePath(resultTable, companyName, SlideEnum.IncidentSlide);

                        var dimensions = await this.GetImageDimensionsInCm(BarChartfilePath);

                        var height = 4.78;
                        var width = 25.4;

                        if (dimensions.widthCm > 0)
                        {
                            width = dimensions.widthCm;
                        }

                        if (dimensions.heightCm > 0)
                        {
                            height = dimensions.heightCm;
                        }

                        long ChartWidthEmu = (long)(width * 360000);
                        long ChartHeightEmu = (long)(height * 360000);
                        long ChartoffsetXEmu = (long)(0.2 * 360000);
                        long ChartoffsetYEmu = (long)(1.51 * 360000);

                        await this.AddImageToSlide(chartSlidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            if (SlideType == SlideCodeEnum.C009.ToString())
            {
                if (dataTableForChart != null)
                {
                    #region For all Category

                    // Define columns to exclude
                    var columnsToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                       "Months", "Grand Total", "RowType"
                    };

                    // Create a new DataTable to hold transformed data
                    DataTable resultTable = new DataTable();

                    // Add the dynamic "Ticket Type" column
                    resultTable.Columns.Add("Ticket Type", typeof(string));

                    // Dynamically add other columns from inputTable except excluded ones
                    foreach (DataColumn column in dataTableForChart.Columns)
                    {
                        if (!columnsToExclude.Contains(column.ColumnName))
                        {
                            resultTable.Columns.Add(column.ColumnName, column.DataType);
                        }
                    }

                    // Filter rows where RowType == 0
                    var filteredRows = dataTableForChart.AsEnumerable()
                        .Where(row => row.Field<int>("RowType") == 0);

                    // Populate the new DataTable
                    foreach (var row in filteredRows)
                    {
                        // Create a new row for resultTable
                        DataRow newRow = resultTable.NewRow();

                        // Set the "Ticket Type" column value
                        //string category = row.Field<string>("Category");
                        //string months = row.Field<string>("Months");
                        //newRow["Ticket Type"] = $"{category} {months}";

                        string months = row.Field<string>("Months");
                        newRow["Ticket Type"] = $"{months}";

                        // Populate other columns dynamically
                        foreach (DataColumn column in dataTableForChart.Columns)
                        {
                            if (!columnsToExclude.Contains(column.ColumnName))
                            {
                                newRow[column.ColumnName] = row[column];
                            }
                        }

                        // Add the populated row to the result table
                        resultTable.Rows.Add(newRow);
                    }


                    SlidePart chartSlidePart = null;

                    long ChartWidthEmu = (long)(23.54 * 360000);
                    long ChartHeightEmu = (long)(14.29 * 360000);
                    long ChartoffsetXEmu = (long)(0.93 * 360000);
                    long ChartoffsetYEmu = (long)(0.63 * 360000);

                    if (first_slidePart != null)
                    {
                        chartSlidePart = first_slidePart;

                        ChartWidthEmu = (long)(22.06 * 360000);
                        ChartHeightEmu = (long)(8.54 * 360000);
                        ChartoffsetXEmu = (long)(1.71 * 360000);
                        ChartoffsetYEmu = (long)(6.90 * 360000);
                    }
                    else
                    {
                        chartSlidePart = await this.AddSlide(presentationPart);
                        await this.AddTextToSlide(chartSlidePart, slideText + " Graphical View");
                        await this.AddImageToSlide(chartSlidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                    }

                    try
                    {
                        string BarChartfilePath = await this.GetBarChartFilePath(resultTable, companyName, SlideEnum.Last3MonthsSlide);
                        var dimensions = await this.GetImageDimensionsInCm(BarChartfilePath);
                        await this.AddImageToSlide(chartSlidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);
                    }
                    catch (Exception ex)
                    {

                    }

                    #endregion

                    #region For each category
                    /*

                    // Define columns to exclude
                    var columnsToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "Category", "Months", "Grand Total", "RowType"
                    };

                    // Group the data by "Category"
                    var groupedData = dataTableForChart.AsEnumerable()
                        .Where(row => row.Field<int>("RowType") == 0) // Filter only RowType == 0
                        .GroupBy(row => row.Field<string>("Category"))
                        .ToList();

                    // Iterate through each category group
                    foreach (var group in groupedData)
                    {
                        // Create a new DataTable for this category
                        DataTable resultTable = new DataTable();

                        // Add the dynamic "Ticket Type" column
                        resultTable.Columns.Add("Ticket Type", typeof(string));

                        // Dynamically add other columns from the input table (excluding excluded columns)
                        foreach (DataColumn column in dataTableForChart.Columns)
                        {
                            if (!columnsToExclude.Contains(column.ColumnName))
                            {
                                resultTable.Columns.Add(column.ColumnName, column.DataType);
                            }
                        }

                        // Get the distinct months for the current category
                        var monthsList = group.Select(row => row.Field<string>("Months")).Distinct().ToList();

                        // Populate the resultTable with transformed data for this category
                        foreach (var month in monthsList)
                        {
                            // Create a new row for the resultTable
                            DataRow newRow = resultTable.NewRow();

                            // Set the "Ticket Type" value (Category + Month)
                            var category = group.Key;
                            newRow["Ticket Type"] = $"{category} {month?.Substring(0, 3)}";

                            // Populate other columns (like ChangeRequest, Incident, etc.) dynamically
                            foreach (DataColumn column in dataTableForChart.Columns)
                            {
                                if (!columnsToExclude.Contains(column.ColumnName))
                                {
                                    // Aggregate the values for the current month
                                    var value = group.Where(row => row.Field<string>("Months") == month)
                                                     .Sum(row => row.Field<int>(column.ColumnName)); // Sum values for each month

                                    newRow[column.ColumnName] = value;
                                }
                            }

                            // Add the populated row to the resultTable
                            resultTable.Rows.Add(newRow);
                        }

                        // Now that we have the resultTable for this category, pass it to GetBarChartFilePath
                        string BarChartfilePath = GetBarChartFilePath(resultTable, companyName, SlideEnum.Last3MonthsSlide);

                        // Add chart to slide for this category (assuming slide creation logic is correct)
                        SlidePart chartSlidePart = AddSlide(presentationPart);
                        AddTextToSlide(chartSlidePart, slideText);

                        AddTextToSlide(chartSlidePart, group.Key, SlideEnum.Last3MonthsSlide);

                        // Add the logo (assuming logic for adding image is correct)
                        AddImageToSlide(chartSlidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                        // Set chart dimensions and position
                        long ChartWidthEmu = (long)(15.92 * 360000);
                        long ChartHeightEmu = (long)(11.49 * 360000);
                        long ChartoffsetXEmu = (long)(4.72 * 360000);
                        long ChartoffsetYEmu = (long)(1.39 * 360000);

                        // Add the generated chart image to the slide
                        AddImageToSlide(chartSlidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);
                    }
                    
                    */
                    #endregion
                }
            
            }
        }

        private DataTable BuildUserTable(string userType, DataTable sourceTable)
        {
            // Filter with Select()
            var filtered = sourceTable
                .Select($"UserType = '{userType}'")
                .CopyToDataTable();

            // Project only the required columns
            var dv = new DataView(filtered);
            var result = dv.ToTable(false,
                "PriorityCode",
                "AgreedSLAForResponse",
                "AgreedSLAForResolution",
                "AverageReponseTimeTaken",
                "AverageResolutionTimeTaken",
                "TotalCalls"
            );

            // Calculate grand total
            int grandTotal = result.AsEnumerable()
                .Sum(r => r.Field<int>("TotalCalls"));

            // Add total row
            DataRow totalRow = result.NewRow();
            totalRow["PriorityCode"] = "Grand Total";
            totalRow["TotalCalls"] = grandTotal;
            result.Rows.Add(totalRow);

            // Rename columns
            var renameMap = new Dictionary<string, string>
                {
                    { "PriorityCode", "Priority" },
                    { "AgreedSLAForResponse", "Agreed SLA for Response" },
                    { "AgreedSLAForResolution", "Agreed SLA for Resolution" },
                    { "AverageReponseTimeTaken", "Average Response Time Taken" },
                    { "AverageResolutionTimeTaken", "Average Resolution Time Taken" },
                    { "TotalCalls", "Total Calls" }
                };

            foreach (var kvp in renameMap)
            {
                if (result.Columns.Contains(kvp.Key))
                {
                    result.Columns[kvp.Key].ColumnName = kvp.Value;
                }
            }

            return result;
        }

        private async Task<long> EstimateRowHeight(string text, long baseRowHeight)
        {
            const int CharactersPerLine = 60; // Adjust based on your font size and slide layout
            int lines = (int)Math.Ceiling((double)text.Length / CharactersPerLine);

            await Task.Delay(0);
            return baseRowHeight * lines;
        }

        private async Task<string> GetBarChartFilePath(DataTable Data, string departmentId, SlideEnum slideEnum)
        {
            if (slideEnum == SlideEnum.IncidentSlide)
            {
                //var aggregatedData = Data.AsEnumerable()
                //.Select(row =>
                //{
                //    // Create a dictionary to store the object's properties
                //    var obj = new Dictionary<string, object>();


                //    // Add all other columns as properties
                //    foreach (DataColumn column in Data.Columns)
                //    {
                //        if (!column.ColumnName.Equals("Ticket Type", StringComparison.OrdinalIgnoreCase))
                //        {
                //            string columnName = column.ColumnName.Replace("ZZZ-", "").Trim();
                //            object value = row[column];

                //            // Replace null/empty values with 0
                //            if (value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                //            {
                //                value = 0;
                //            }
                //            else
                //            {
                //                // Convert to integer if possible
                //                if (int.TryParse(value.ToString(), out int intValue))
                //                {
                //                    value = intValue;
                //                }
                //                else
                //                {
                //                    value = 0; // Default to 0 if conversion fails
                //                }
                //            }

                //            obj[columnName] = value;
                //        }
                //    }

                //    // Dynamically create an anonymous object from the dictionary
                //    return obj;
                //})
                //.Select(obj =>
                //{
                //    // Flatten the dictionary into a dynamic object
                //    var dynamicObject = new ExpandoObject() as IDictionary<string, object>;
                //    foreach (var kvp in obj)
                //    {
                //        dynamicObject[kvp.Key] = kvp.Value;
                //    }
                //    return dynamicObject;
                //})
                //.ToArray();

                var aggregatedData = Data.AsEnumerable()
                .Select(row =>
                {
                    // Convert each row of the DataTable into a dictionary
                    var rowDictionary = Data.Columns.Cast<DataColumn>()
                        .ToDictionary(
                            column => column.ColumnName.Replace("ZZZ-", "").Trim(), // Clean column name
                            column =>
                            {
                                var value = row[column];

                                // Replace null/empty values with 0
                                if (value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                                {
                                    return "";
                                }

                                // Convert to integer if possible
                                if (int.TryParse(value.ToString(), out int intValue))
                                {
                                    return intValue;
                                }

                                return value; // Fallback to original value
                            }
                        );

                    // Convert to IDictionary<string, object>
                    return (IDictionary<string, object>)rowDictionary;
                })
                .ToArray();

                await Task.Delay(0);
                return await this.GenerateNSaveBarChart(aggregatedData, departmentId, "BarChart_Incident", slideEnum);
            }

            if (slideEnum == SlideEnum.Last3MonthsSlide)
            {
                // Convert each row of the DataTable into a dictionary and return as an array
                var aggregatedData = Data.AsEnumerable()
                    .Select(row => Data.Columns.Cast<DataColumn>()
                        .ToDictionary(
                            column => column.ColumnName,  // Key: Column Name
                            column => row[column]         // Value: Row value for that column
                        )
                    )
                    .ToArray();

                await Task.Delay(0);
                return await this.GenerateNSaveBarChart(aggregatedData, departmentId, "BarChart_LastThreeMonths", slideEnum);
            }

            await Task.Delay(0);
            return "";
        }

        private async Task<DataTable> ConvertToDataTable<T>(List<T> data)
        {
            DataTable table = new DataTable(typeof(T).Name);

            if (data != null && data.Count > 0)
            {
                // Get properties of the type
                var properties = typeof(T).GetProperties();
                foreach (var prop in properties)
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                // Add rows
                foreach (var item in data)
                {
                    var values = properties.Select(p => p.GetValue(item, null)).ToArray();
                    table.Rows.Add(values);
                }
            }

            await Task.Delay(0);
            return table;
        }

        private async Task<SlidePart> AddSlide(PresentationPart presentationPart)
        {
            // Ensure there is at least one slide layout
            SlideLayoutPart slideLayoutPart = presentationPart.SlideMasterParts
                .First()
                .SlideLayoutParts
                .First();

            presentationPart.Presentation.SlideSize = new SlideSize
            {
                Cx = 9144000, // Width (10 inches in EMU)
                Cy = 5143500, // Height (5.625 inches in EMU)
                Type = SlideSizeValues.Screen16x9 // Set to widescreen 16:9
            };

            // Add a new slide part
            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();

            // Link the slide to the slide layout
            slidePart.AddPart(slideLayoutPart);

            // Create a blank slide
            slidePart.Slide = new Slide(
                new CommonSlideData(
                    new ShapeTree(
                        new NonVisualGroupShapeProperties(
                            new NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                            new NonVisualGroupShapeDrawingProperties(),
                            new ApplicationNonVisualDrawingProperties()),
                        new GroupShapeProperties(
                            new DocumentFormat.OpenXml.Drawing.TransformGroup()),
                        // Add a shape with the specified text
                        new Shape(
                            new NonVisualShapeProperties(
                                new NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "Title" },
                                new NonVisualShapeDrawingProperties(),
                                new ApplicationNonVisualDrawingProperties()),
                            new ShapeProperties(),
                            new TextBody(
                                new DocumentFormat.OpenXml.Drawing.BodyProperties(),
                                new DocumentFormat.OpenXml.Drawing.ListStyle(),
                                new DocumentFormat.OpenXml.Drawing.Paragraph(
                                    new DocumentFormat.OpenXml.Drawing.Run(
                                        new DocumentFormat.OpenXml.Drawing.Text() { Text = " " })))))));

            slidePart.Slide.Save();

            // Add the new slide to the SlideIdList
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;
            uint maxSlideId = slideIdList.ChildElements.OfType<SlideId>().Max(s => s.Id.Value);
            slideIdList.AppendChild(new SlideId()
            {
                Id = maxSlideId + 1,
                RelationshipId = presentationPart.GetIdOfPart(slidePart)
            });

            //Presentation presentation = presentationPart.Presentation;
            //presentation.SlideSize = new SlideSize
            //{
            //    Cx = 9144000, // Width (10 inches in EMU)
            //    Cy = 5143500, // Height (5.625 inches in EMU)
            //    Type = SlideSizeValues.Screen16x9 // Set to widescreen 16:9
            //};

            //SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();
            //slidePart.Slide = new P.Slide(
            //    new P.CommonSlideData(new P.ShapeTree())
            //);

            //// Link to Slide Layout
            //slidePart.AddPart(presentationPart.SlideMasterParts.First().SlideLayoutParts.First(), "rId1");

            //// Add the slide to the presentation's slide list
            ////Presentation presentation = presentationPart.Presentation;
            //if (presentation.SlideIdList == null)
            //    presentation.SlideIdList = new SlideIdList();

            //uint slideId = presentation.SlideIdList.ChildElements.Count > 0
            //    ? presentation.SlideIdList.Elements<SlideId>().Max(s => s.Id.Value) + 1
            //    : 256;

            //presentation.SlideIdList.Append(new SlideId
            //{
            //    Id = slideId,
            //    RelationshipId = presentationPart.GetIdOfPart(slidePart)
            //});

            long imageWidthEmu = (long)(SLIDE_MAX_WIDTH * 360000);  // Width in EMU
            long imageHeightEmu = (long)(SLIDE_MAX_HEIGHT * 360000); // Height in EMU
            long offsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
            long offsetYEmu = (long)(0 * 360000);    // Vertical position in EMU (negative for above the origin)

            string bgPath = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\BackGroundImage.jpg";

            await this.AddImageToSlide(slidePart, bgPath,
                imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

            await Task.Delay(100);
            return slidePart;
        }

        private async Task AddTableToSlide(SlidePart slidePart, DataTable dataTable, string SlideType, bool isRightTable = false,
            double offsetX = 0.75,    // Horizontal position in EMU
        double offsetY = 1.32,double width = 23.98)
        {
            int tblHeaderHeight = TBL_HEADER_ROW_HEIGHT;
            int tblRowHeight = TBL_ROW_HEIGHT;
            int tblFooterHeight = TBL_ROW_HEIGHT;

            string evenBgColor = TBL_ROW_LIGHT_COLOR_CODE;
            string oddBgColor = TBL_ROW_MEDIUM_COLOR_CODE;

            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableRows.EvenBackgroundColor))
            {
                evenBgColor = slideSettings.Config.TableRows.EvenBackgroundColor;
            }

            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableRows.OodBackgroundColor))
            {
                oddBgColor = slideSettings.Config.TableRows.OodBackgroundColor;
            }

            if (slideSettings?.Config?.TableRows.Height != null)
            {
                tblRowHeight = slideSettings.Config.TableRows.Height;
            }

            if (slideSettings?.Config?.TableHeader.Height != null)
            {
                tblHeaderHeight = slideSettings.Config.TableHeader.Height;
            }

            if (slideSettings?.Config?.TableFooter.Height != null)
            {
                tblFooterHeight = slideSettings.Config.TableFooter.Height;
            }


            // Get the ShapeTree from the slide
            ShapeTree shapeTree = slidePart?.Slide?.CommonSlideData?.ShapeTree;

            // Create a GraphicFrame to hold the table
            P.GraphicFrame graphicFrame = shapeTree?.AppendChild(new P.GraphicFrame());

            // Assign unique ID and name
            graphicFrame.NonVisualGraphicFrameProperties = new P.NonVisualGraphicFrameProperties(
                new P.NonVisualDrawingProperties { Id = 1U, Name = "Table" },
                new P.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoGrouping = true }),
                new ApplicationNonVisualDrawingProperties());

            long widthEmu   = (long)(width * 360000);  // Width in EMU
            long heightEmu  = (long)(0 * 360000); // Height in EMU
            long offsetXEmu = (long)(offsetX * 360000);    // Horizontal position in EMU
            long offsetYEmu = (long)(offsetY * 360000);    // Vertical position in EMU

            if (isRightTable)
            {
                offsetXEmu = (long)(11.65 * 360000);
            }

            graphicFrame.Transform = new Transform(
                new A.Offset { X = offsetXEmu, Y = offsetYEmu }, // Horizontal and vertical position                                                           
                new A.Extents { Cx = widthEmu, Cy = heightEmu } // Fixed width and height
            );


            // Create the table
            A.Table table = new A.Table();

            A.TableProperties tableProperties = new A.TableProperties(
                                new A.NoFill()
                                , new A.TableStyleId { Text = "{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}" }
                            );

            table.Append(tableProperties);

            // Add table grid with fixed column width
            A.TableGrid tableGrid = new A.TableGrid();

            int totalColumns = dataTable.Columns.Count;
            double remainingWidth = widthEmu;

            // Define a dictionary for column names and their corresponding widths in cm
            var columnWidthsInCm = new Dictionary<string, double>
            {
                { "SL", 1.5 }, // Width for 'SL' column in cm
                { "Ticket Id", 2.0 }, // Width for 'Ticket Id' column in cm
                { "Created Time", 2.5 }, // Width for 'Created Time' column in cm
                { "Ticket Type", 5.0 }, // Width for 'Ticket Type' column in cm
                { "Status", 2.5 }, // Width for 'Status' column in cm
                { "Months", 2.5 }, // Width for 'Months' column in cm
                { "Response Status", 5.0 }, // Width for 'Months' column in cm
                { "Resolution Status", 5.0 }, // Width for 'Months' column in cm
            };

            if (SlideType == SlideCodeEnum.C001.ToString())
            {
                if (totalColumns <= 4)
                {
                    columnWidthsInCm.Add("Category", 6.0);
                    columnWidthsInCm.Add("Change Request", 3.5);
                    columnWidthsInCm.Add("Incident", 2.5);
                    columnWidthsInCm.Add("Service Request", 3.5);
                    columnWidthsInCm.Add("Grand Total", 2.5);
                }
                else
                {
                    columnWidthsInCm.Add("Category", 6.0);
                    columnWidthsInCm.Add("Change Request", 2.5);
                    columnWidthsInCm.Add("Incident", 2.5);
                    columnWidthsInCm.Add("Service Request", 2.5);
                    columnWidthsInCm.Add("Grand Total", 2.5);
                }
            }

            if (SlideType == SlideCodeEnum.C007.ToString())
            {
                columnWidthsInCm.Add("Category", 4.0);
                columnWidthsInCm.Add("Incident", 2.0);
                columnWidthsInCm.Add("Change Request", 3.5);
                columnWidthsInCm.Add("Service Request", 3.5);
                columnWidthsInCm.Add("Problem", 2.0);
                columnWidthsInCm.Add("Grand Total", 2.0);
            }

            if (SlideType == SlideCodeEnum.C003.ToString()
                || SlideType == SlideCodeEnum.C002.ToString()
                || SlideType == SlideCodeEnum.C008.ToString())
            {
                columnWidthsInCm.Add("Category", 5.0);

                columnWidthsInCm.Add("Urgent", 2.0);
                columnWidthsInCm.Add("High", 2.0);
                columnWidthsInCm.Add("Medium", 2.0);
                columnWidthsInCm.Add("Low", 2.0);

                if (SlideType == SlideCodeEnum.C008.ToString())
                {
                    columnWidthsInCm.Add("Grand Total", 2.0);
                }
                else
                {
                    columnWidthsInCm.Add("Grand Total", 2.5);
                }

                columnWidthsInCm.Add("Achieved Percentage (%)", 3.0);
            }

            if (SlideType == SlideCodeEnum.C010.ToString())
            {
                columnWidthsInCm.Add("Incident", 2.0);
                columnWidthsInCm.Add("Service Request", 3.5);
                columnWidthsInCm.Add("Problem", 2.0);
                columnWidthsInCm.Add("Grand Total", 2.0);
                columnWidthsInCm.Add("Change Request", 3.5);
            }

            if (SlideType == SlideCodeEnum.C011.ToString()
                || SlideType == SlideCodeEnum.C013.ToString()
                || SlideType == SlideCodeEnum.C012.ToString())
            {
                columnWidthsInCm.Add("Average (%)", 3.5);
                columnWidthsInCm.Add("Minimum (%)", 3.5);
                columnWidthsInCm.Add("Maximum (%)", 3.5);
            }

            if (SlideType == SlideCodeEnum.C014.ToString())
            {
                columnWidthsInCm.Add("Resource Name", 8.0);
                columnWidthsInCm.Add("Count", 2.5);
            }

            if (SlideType == SlideCodeEnum.C023.ToString())
            {
                columnWidthsInCm.Add("Daily and Monthly SR Activity Analysis", 8.0);
                
                columnWidthsInCm.Add("Daily & Monthly SR Activity (Team Wise Bifurcation)", 9.0);

                columnWidthsInCm.Add("Activity Count", 3.5);
                columnWidthsInCm.Add("Count", 2.5);

            }

            if (SlideType == SlideCodeEnum.C015.ToString())
            {
                columnWidthsInCm.Add("Requester", 8.0);
                columnWidthsInCm.Add("Count", 2.5);
            }

            // Convert cm to EMUs and calculate the total width for predefined columns
            var predefinedColumnWidths = new Dictionary<string, double>();
            foreach (var kvp in columnWidthsInCm)
            {
                if (dataTable.Columns.Contains(kvp.Key))
                {
                    double columnWidthInEmus = kvp.Value * 360000; // Convert cm to EMUs
                    predefinedColumnWidths[kvp.Key] = columnWidthInEmus;
                    remainingWidth -= columnWidthInEmus; // Subtract the assigned width from remaining width
                }
            }

            // Distribute remaining width among columns not specified in columnWidthsInCm
            int remainingColumnsCount = totalColumns - predefinedColumnWidths.Count;
            double remainingColumnWidth = remainingColumnsCount > 0 ? remainingWidth / remainingColumnsCount : 0;

            // Add columns to the table grid in the order they appear in dataTable
            foreach (DataColumn column in dataTable.Columns)
            {
                string columnName = column.ColumnName;
                if (predefinedColumnWidths.ContainsKey(columnName))
                {
                    // Use predefined width
                    tableGrid.Append(new A.GridColumn { Width = (int)predefinedColumnWidths[columnName] });
                }
                else
                {
                    // Use calculated remaining width
                    tableGrid.Append(new A.GridColumn { Width = (int)remainingColumnWidth });
                }
            }

            table.Append(tableGrid);


            // Add header row
            A.TableRow headerRow = new A.TableRow { Height = tblHeaderHeight };
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnName.ToLower() == "grand total")
                {
                    var tbl_cell = await this.CreateTableCell("Total", isHeader: true, isFooter: false, SlideType);
                    headerRow.Append(tbl_cell);
                }
                else
                {
                    var tbl_cell = await this.CreateTableCell(column.ColumnName, isHeader: true, isFooter: false, SlideType);
                    headerRow.Append(tbl_cell);
                }
            }
            table.Append(headerRow);

            // Add data rows
            bool IsFooter = false;
            int rowIndex = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                A.TableRow tableRow = new A.TableRow { Height = tblRowHeight }; // Table Row height Except Header

                bool isAlternateRow = rowIndex % 2 == 1;
                string rowColor = isAlternateRow ? oddBgColor : evenBgColor;

                foreach (object cell in row.ItemArray)
                {
                    if (
                        //SlideType == "Incident, SR Report" && (
                        cell?.ToString() == "Grand Total")
                    //))
                    {
                        IsFooter = true;
                        var tbl_cell = await CreateTableCell("Total", false, IsFooter, SlideType, rowColor);
                        tableRow.Append(tbl_cell);
                    }
                    else
                    {
                        var tbl_cell = await CreateTableCell(cell?.ToString(), false, IsFooter, SlideType, rowColor);
                        tableRow.Append(tbl_cell);
                    }
                }

                table.Append(tableRow);
                rowIndex++;
            }

            // Add the table to the graphic frame
            graphicFrame.Graphic = new A.Graphic(new A.GraphicData(table)
            {
                Uri = "http://schemas.openxmlformats.org/drawingml/2006/table"
            });
        }

        private async Task<A.TableCell> CreateTableCell(string text, bool isHeader, bool isFooter, string SlideType = "", string rowColor = "")
        {

            int fontSize = 10;
            string fontColor = CELL_FONT_COLOR_CODE;
            bool isBold = false;
            bool hasUnderline = false;

            string tblHeaderFontColor = TBL_HEADER_FONT_COLOR_CODE;
            string tblHeaderBgColor = HEADING_TEXT_COLOR_CODE;
            int tblHeaderFontSize = 10;
            bool tblHeaderIsBold = true;

            string tblFooterFontColor = CELL_FONT_COLOR_CODE;
            string tblFooterBgColor = TBL_FOOTER_COLOR_CODE;
            int tblFooterFontSize = 10;
            bool tblFooterIsBold = true;

            if (rowColor == "") rowColor = TBL_ROW_LIGHT_COLOR_CODE;

            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableRows.FontColor))
            {
                fontColor = slideSettings.Config.TableRows.FontColor;
            }

            if (slideSettings?.Config?.TableRows.FontSize != null)
            {
                fontSize = slideSettings.Config.TableRows.FontSize;
            }


            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableHeader.FontColor))
            {
                tblHeaderFontColor = slideSettings.Config.TableHeader.FontColor;
            }

            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableHeader.BackgroundColor))
            {
                tblHeaderBgColor = slideSettings.Config.TableHeader.BackgroundColor;
            }

            if (slideSettings?.Config?.TableHeader.FontSize != null)
            {
                tblHeaderFontSize = slideSettings.Config.TableHeader.FontSize;
            }

            if (slideSettings?.Config?.TableHeader.IsBold != null)
            {
                tblHeaderIsBold = slideSettings.Config.TableHeader.IsBold;
            }


            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableFooter.FontColor))
            {
                tblFooterFontColor = slideSettings.Config.TableFooter.FontColor;
            }

            if (!string.IsNullOrEmpty(slideSettings?.Config?.TableFooter.BackgroundColor))
            {
                tblFooterBgColor = slideSettings.Config.TableFooter.BackgroundColor;
            }

            if (slideSettings?.Config?.TableFooter.FontSize != null)
            {
                tblFooterFontSize = slideSettings.Config.TableFooter.FontSize;
            }

            if (slideSettings?.Config?.TableFooter.IsBold != null)
            {
                tblFooterIsBold = slideSettings.Config.TableFooter.IsBold;
            }

            // Create a new TableCell
            A.TableCell cell = new A.TableCell();

            // Create the text body and add a paragraph with the text
            A.RunProperties runProperties = new A.RunProperties();

            if (string.IsNullOrEmpty(text))
            {
                text = " ";
            }

            A.TextBody textBody = new A.TextBody(
                new A.BodyProperties { Wrap = A.TextWrappingValues.Square, Anchor = A.TextAnchoringTypeValues.Center },
                new A.ListStyle(),
                new A.Paragraph(
                    new A.ParagraphProperties
                    {
                        Alignment = A.TextAlignmentTypeValues.Center
                    },
                    new A.Run(
                        new A.Text(text)
                    )
                )
            );

            // Set horizontal alignment
            A.ParagraphProperties paragraphProperties = new A.ParagraphProperties
            {
                Alignment = A.TextAlignmentTypeValues.Left   // Horizontally center the text
                                                             //,LineSpacing = new A.LineSpacing(
                                                             //new A.SpacingPercent { Val = 0 },
                                                             //new A.SpacingPoints { Val = 0 },
                                                             //new SpacingBetweenLines() { Before = "0", After = "0", Line = "0", LineRule = LineSpacingRuleValues.Exact },
                                                             //new ContextualSpacing() { Val = false }
                                                             //), // 100% line spacing (1.0 in Open XML)--100000

            };

            textBody.GetFirstChild<A.Paragraph>().ParagraphProperties = paragraphProperties;


            string CellColor = rowColor;
            string CellFontColor = fontColor;

            // Apply styles for header cells
            //A.RunProperties runProperties = new A.RunProperties();
            if (isHeader)
            {
                // Create RunProperties for text styling
                runProperties.Bold = tblHeaderIsBold;
                runProperties.FontSize = tblHeaderFontSize * 100;

                // Set font color
                runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = tblHeaderFontColor }));

                //if (SlideType == "TicketDetailsAnalyzationObj" || SlideType == "SLAnotMetTicketDetailsObj")
                //{
                //    CellColor = "0E58C4";//blue
                //}
                //else if (SlideType == "Ticket Not Closed")
                //{
                //    CellColor = "7D7D7D";// grey
                //}
                //else if (SlideType == "CategoryWiseCallBifurcationObj")
                //{
                //    CellColor = "E97132";//orange
                //}
                //else
                //{
                CellColor = tblHeaderBgColor;
                //}

                A.TableCellProperties cellProperties = new A.TableCellProperties(
                    new A.SolidFill(new A.RgbColorModelHex { Val = CellColor }),
                    new A.TableCellBorders()
                );

                // Apply the runProperties and cell properties for the header
                textBody.GetFirstChild<A.Paragraph>().GetFirstChild<A.Run>().RunProperties = runProperties;
                cell.Append(textBody);
                cell.Append(cellProperties);
            }
            else if (isFooter)
            {
                // Create RunProperties for text styling
                runProperties.Bold = tblFooterIsBold;
                runProperties.FontSize = tblFooterFontSize * 100;

                // Set font color
                runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = tblFooterFontColor }));

                //if (SlideType == "TicketDetailsAnalyzationObj" || SlideType == "SLAnotMetTicketDetailsObj")
                //{
                //    CellColor = "0E58C4";//blue
                //}
                //else if (SlideType == "Ticket Not Closed")
                //{
                //    CellColor = "7D7D7D";// grey
                //}
                //else if (SlideType == "CategoryWiseCallBifurcationObj")
                //{
                //    CellColor = "E97132";//orange
                //}
                //else
                //{
                CellColor = tblFooterBgColor;
                //}

                A.TableCellProperties cellProperties = new A.TableCellProperties(
                    new A.SolidFill(new A.RgbColorModelHex { Val = CellColor }),
                    new A.TableCellBorders()
                );

                // Apply the runProperties and cell properties for the header
                textBody.GetFirstChild<A.Paragraph>().GetFirstChild<A.Run>().RunProperties = runProperties;
                cell.Append(textBody);
                cell.Append(cellProperties);
            }
            else if (text?.Length > 50)
            {
                runProperties.FontSize = 800;

                // For regular cells, just add textBody and a default TableCellProperties
                runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = CellFontColor }));

                A.TableCellProperties cellProperties = new A.TableCellProperties(
                   new A.TableCellBorders()
                );

                cellProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = CellColor }));

                textBody.GetFirstChild<A.Paragraph>().GetFirstChild<A.Run>().RunProperties = runProperties;



                cell.Append(textBody);
                cell.Append(cellProperties);
            }
            else
            {
                runProperties.FontSize = fontSize * 100;

                // For regular cells, just add textBody and a default TableCellProperties
                runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = CellFontColor }));

                A.TableCellProperties cellProperties = new A.TableCellProperties(
                    new A.TableCellBorders()
                );

                cellProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = rowColor }));

                if ((SlideType == SlideCodeEnum.C002.ToString() || SlideType == SlideCodeEnum.C003.ToString()) && text == "SLA Violated")
                {
                    // cellProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "FFFF00" }));
                }
                else if (SlideType == "CategoryWiseCallBifurcationObj" && text.Contains("Total", StringComparison.OrdinalIgnoreCase))
                {
                    //cellProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "84E291" }));
                }
                else if (SlideType == "CategoryWiseCallBifurcationObj" && text == "Grand Total")
                {
                    //cellProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "E97132" }));
                    //runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "E97132" }));
                    //textBody.GetFirstChild<A.Paragraph>().<A.Run>().RunProperties = runProperties;
                }


                textBody.GetFirstChild<A.Paragraph>().GetFirstChild<A.Run>().RunProperties = runProperties;



                cell.Append(textBody);
                cell.Append(cellProperties);
            }

            await Task.Delay(0);
            return cell;
        }

        private async Task AddImageToSlide(SlidePart slidePart, string imagePath, long imageWidthEmu, long imageHeightEmu, long offsetXEmu, long offsetYEmu)
        {
            // Check if the file exists
            if (!File.Exists(imagePath))
                return;


            // Add the image to the slide part
            ImagePart imagePart = slidePart.AddImagePart(ImagePartType.Jpeg);

            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                imagePart.FeedData(stream); // Load the image into the presentation
            }

            // Get the relationship ID of the image
            string relationshipId = slidePart.GetIdOfPart(imagePart);

            // Define the image dimensions (adjust as needed)
            //long imageWidthEmu = (long)(5.32 * 360000);  // Width in EMU
            //long imageHeightEmu = (long)(1.97 * 360000); // Height in EMU
            //long offsetXEmu = (long)(20.04 * 360000);    // Horizontal position in EMU
            //long offsetYEmu = (long)(0.03 * 360000);    // Vertical position in EMU (negative for above the origin)

            // Add the image to the slide's shape tree
            ShapeTree shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;

            // Create a Picture object to hold the image
            P.Picture picture = new P.Picture(
                new P.NonVisualPictureProperties(
                    new P.NonVisualDrawingProperties { Id = 4U, Name = "Picture 1" },
                    new P.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true }),
                    new ApplicationNonVisualDrawingProperties()
                ),
                new P.BlipFill(
                    new A.Blip { Embed = relationshipId }, // Link the image
                    new A.Stretch(new A.FillRectangle())
                ),
                new P.ShapeProperties(
                    new A.Transform2D(
                        new A.Offset { X = offsetXEmu, Y = offsetYEmu }, // Top-left corner of the slide
                        new A.Extents { Cx = imageWidthEmu, Cy = imageHeightEmu } // Dimensions
                    ),
                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                )
            );

            // Append the picture to the shape tree
            shapeTree.AppendChild(picture);

            await Task.Delay(0);
            return;
        }

        private async Task AddTextToSlide(SlidePart slidePart, string text, SlideEnum? slideEnum = null, HelperModel? helperModel = null,

        double OffsetY = 0
            )
        {
            int fontSize = 20;
            string fontColor = HEADING_TEXT_COLOR_CODE;
            bool isBold = true;
            bool hasUnderline = false;

            if (!string.IsNullOrEmpty(slideSettings?.Config?.Heading.FontColor))
            {
                fontColor = slideSettings.Config.Heading.FontColor;
            }

            if (slideSettings?.Config?.Heading.FontSize != null)
            {
                fontSize = slideSettings.Config.Heading.FontSize;
            }

            if (slideSettings?.Config?.Heading.IsBold != null)
            {
                isBold = slideSettings.Config.Heading.IsBold;
            }

            if (slideSettings?.Config?.Heading.HasUnderline != null)
            {
                hasUnderline = slideSettings.Config.Heading.HasUnderline;
            }

            // Access the ShapeTree of the slide
            ShapeTree shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;

            if (slideEnum == SlideEnum.FirstSlide)
            {
                // Heading
                try
                {
                    // Create a new shape for the text box
                    P.Shape textShape = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());


                    long Heading_WidthEmu = (long)(11.94 * 360000);     // Width in EMU
                    long Heading_HeightEmu = (long)(2.38 * 360000);     // Height in EMU
                    long Heading_OffsetXEmu = (long)(0.75 * 360000);    // Horizontal position in EMU
                    long Heading_OffsetYEmu = (long)(5.95 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShape.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Heading_OffsetXEmu, Y = Heading_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Heading_WidthEmu, Cy = Heading_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runProperties = new A.RunProperties
                    {
                        FontSize = 3000, // multiply the desired font size by 100
                        Bold = true, // Make the text bold
                                     //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "FFFFFF" }));


                    textShape.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runProperties
                                , new A.Text(text)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShape);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Sub-Heading
                try
                {
                    //// Define Indian Standard Time (IST)
                    //TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //// Get the current date and time in IST
                    //DateTime istDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istTimeZone);
                    //// Format the date to "November 2024"
                    //string formattedDate = istDateTime.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

                    // Create a new shape for the text box
                    P.Shape textShapeV2 = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShapeV2.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long SubHeading_WidthEmu = (long)(11.94 * 360000);     // Width in EMU
                    long SubHeading_HeightEmu = (long)(1.79 * 360000);     // Height in EMU
                    long SubHeading_OffsetXEmu = (long)(0.75 * 360000);    // Horizontal position in EMU
                    long SubHeading_OffsetYEmu = (long)(7.45 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShapeV2.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = SubHeading_OffsetXEmu, Y = SubHeading_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = SubHeading_WidthEmu, Cy = SubHeading_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runPropertiesV2 = new A.RunProperties
                    {
                        FontSize = 2000, // multiply the desired font size by 100
                        Bold = true, // Make the text bold
                                     //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runPropertiesV2.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "FFFFFF" })); // Black color


                    textShapeV2.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runPropertiesV2
                                , new A.Text(helperModel.MonthName)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShapeV2);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (slideEnum == SlideEnum.AgendaSlide)
            {
                // Heading
                try
                {
                    // Create a new shape for the text box
                    P.Shape textShape = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Heading_WidthEmu = (long)(4.75 * 360000);     // Width in EMU
                    long Heading_HeightEmu = (long)(1.5 * 360000);     // Height in EMU
                    long Heading_OffsetXEmu = (long)(1.15 * 360000);    // Horizontal position in EMU
                    long Heading_OffsetYEmu = (long)(0.25 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShape.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Heading_OffsetXEmu, Y = Heading_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Heading_WidthEmu, Cy = Heading_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runProperties = new A.RunProperties
                    {
                        FontSize = 2800, // multiply the desired font size by 100
                        Bold = true, // Make the text bold
                                     //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "000000" })); // Black color


                    textShape.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runProperties
                                , new A.Text(text)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShape);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //try
                //{
                //    // Agenda List
                //    string[] agendaItems = new string[]
                //    {
                //    "Introduction & Previous MOM Discussion",
                //    "Previous Month Server Utilization Report",
                //    "Discussion on Last Month's Services",
                //    "Observation & Recommendation",
                //    "Previous Month VM and Active User Details",
                //    "Major Activities/Incidents",
                //    "SLA & Escalation Matrix",
                //    "Conclusion"
                //    };

                //    // Assuming you have a shapeTree already defined and initialized
                //    long Agenda_OffsetXEmu = (long)(1.15 * 360000); // Horizontal position in EMU
                //    long Agenda_OffsetYEmu = (long)(1.75 * 360000); // Starting vertical position in EMU

                //    // Adjust the height based on your design requirements
                //    long Agenda_HeightEmu = (long)(1 * 360000); // Height in EMU (adjusted to be smaller)

                //    foreach (var item in agendaItems)
                //    {
                //        // Create a new shape for the text box
                //        P.Shape textShapeV2 = new P.Shape();

                //        // Set NonVisual properties for the text box
                //        textShapeV2.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                //            new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                //            new P.NonVisualShapeDrawingProperties(),
                //            new ApplicationNonVisualDrawingProperties());

                //        long Agenda_WidthEmu = (long)(23.10 * 360000); // Width in EMU

                //        // Set the position and size of the text box
                //        textShapeV2.ShapeProperties = new P.ShapeProperties(
                //              new A.Transform2D(
                //                  new A.Offset { X = Agenda_OffsetXEmu, Y = Agenda_OffsetYEmu }, // Position
                //                  new A.Extents { Cx = Agenda_WidthEmu, Cy = Agenda_HeightEmu } // Size
                //              )
                //          );

                //        A.RunProperties runPropertiesV2 = new A.RunProperties
                //        {
                //            FontSize = 1400, // Default font size
                //        };
                //        runPropertiesV2.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "000000" })); // Black color

                //        // Create the text body for the agenda item with a bullet character
                //        string bullet = "• "; // Bullet character
                //                              // string bullet = "■ "; // Square bullet character
                //        textShapeV2.TextBody = new P.TextBody(
                //            new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping
                //            new A.ListStyle(),
                //            new A.Paragraph(
                //                new A.Run(
                //                    runPropertiesV2,
                //                    new A.Text(bullet + item) // Use bullet character and the current agenda item
                //                )
                //            )
                //        );

                //        // Append the text box to the ShapeTree
                //        shapeTree.AppendChild(textShapeV2); // Append text box

                //        // Update the Y offset for the next item (adjust as needed)
                //        Agenda_OffsetYEmu += Agenda_HeightEmu + (long)(0.1 * 360000); // Reduced space between items
                //    }
                //}
                //catch (Exception ex)
                //{
                //    this._logger.LogError(ex, $"Execution failed at AddTextToSlide()");
                //}
            }

            if (slideEnum == SlideEnum.MonthlyServiceCover)
            {
                // Heading
                try
                {
                    // Create a new shape for the text box
                    P.Shape textShape = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Heading_WidthEmu = (long)(10.53 * 360000);     // Width in EMU
                    long Heading_HeightEmu = (long)(2.52 * 360000);     // Height in EMU
                    long Heading_OffsetXEmu = (long)(1.15 * 360000);    // Horizontal position in EMU
                    long Heading_OffsetYEmu = (long)(5.85 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShape.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Heading_OffsetXEmu, Y = Heading_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Heading_WidthEmu, Cy = Heading_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runProperties = new A.RunProperties
                    {
                        FontSize = 3000, // multiply the desired font size by 100
                        Bold = true, // Make the text bold
                                     //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "ffffff" }));


                    textShape.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runProperties
                                , new A.Text(text)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShape);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (slideEnum == SlideEnum.ThankYouSlide)
            {
                // Heading
                try
                {
                    // Create a new shape for the text box
                    P.Shape textShape = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Heading_WidthEmu = (long)(8.56 * 360000);     // Width in EMU
                    long Heading_HeightEmu = (long)(2.05 * 360000);     // Height in EMU
                    long Heading_OffsetXEmu = (long)(0 * 360000);    // Horizontal position in EMU
                    long Heading_OffsetYEmu = (long)(5.5 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShape.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Heading_OffsetXEmu, Y = Heading_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Heading_WidthEmu, Cy = Heading_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runProperties = new A.RunProperties
                    {
                        FontSize = 4000, // multiply the desired font size by 100
                        Bold = true, // Make the text bold
                                     //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "14436c" })); // Embee Color


                    textShape.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runProperties
                                , new A.Text(text)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShape);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Details
                try
                {
                    // Creating a dynamic object
                    dynamic contactInfo = new ExpandoObject();
                    contactInfo.Website = "www.embee.co.in";
                    contactInfo.Email = "connect@embee.co.in";
                    contactInfo.Phone = "+91 9711544074";
                    contactInfo.Locations = new[]
                    {
                        "Mumbai", "Pune", "New Delhi", "Gurugram", "Noida",
                        "Kolkata", "Jamshedpur", "Bhubaneshwar", "Guwahati", "Raipur"
                    };


                    // Create a new shape for the text box - Website
                    P.Shape textShapeV2 = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShapeV2.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Website_WidthEmu = (long)(5.18 * 360000);     // Width in EMU
                    long Website_HeightEmu = (long)(1.2 * 360000);     // Height in EMU
                    long Website_OffsetXEmu = (long)(1.03 * 360000);    // Horizontal position in EMU
                    long Website_OffsetYEmu = (long)(11.18 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShapeV2.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Website_OffsetXEmu, Y = Website_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Website_WidthEmu, Cy = Website_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runPropertiesV2 = new A.RunProperties
                    {
                        FontSize = 1400, // multiply the desired font size by 100
                                         //Bold = true, // Make the text bold
                                         //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runPropertiesV2.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "14436c" })); // Embee Color


                    textShapeV2.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runPropertiesV2
                                , new A.Text(contactInfo.Website)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShapeV2);



                    // Create a new shape for the text box - Email
                    P.Shape textShapeV3 = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShapeV3.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Email_WidthEmu = (long)(5.18 * 360000);     // Width in EMU
                    long Email_HeightEmu = (long)(1.2 * 360000);     // Height in EMU
                    long Email_OffsetXEmu = (long)(7.59 * 360000);    // Horizontal position in EMU
                    long Email_OffsetYEmu = (long)(11.18 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShapeV3.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Email_OffsetXEmu, Y = Email_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Email_WidthEmu, Cy = Email_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runPropertiesV3 = new A.RunProperties
                    {
                        FontSize = 1400, // multiply the desired font size by 100
                                         //Bold = true, // Make the text bold
                                         //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runPropertiesV3.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "14436c" })); // Embee Color


                    textShapeV3.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runPropertiesV3
                                , new A.Text(contactInfo.Email)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShapeV3);




                    // Create a new shape for the text box - Phone
                    P.Shape textShapeV4 = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShapeV4.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Phone_WidthEmu = (long)(5.18 * 360000);     // Width in EMU
                    long Phone_HeightEmu = (long)(1.2 * 360000);     // Height in EMU
                    long Phone_OffsetXEmu = (long)(13.95 * 360000);    // Horizontal position in EMU
                    long Phone_OffsetYEmu = (long)(11.18 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShapeV4.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Phone_OffsetXEmu, Y = Phone_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Phone_WidthEmu, Cy = Phone_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runPropertiesV4 = new A.RunProperties
                    {
                        FontSize = 1400, // multiply the desired font size by 100
                                         //Bold = true, // Make the text bold
                                         //Underline = A.TextUnderlineValues.Single, // Add underline

                    };
                    runPropertiesV4.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "14436c" })); // Embee Color


                    textShapeV4.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runPropertiesV4
                                , new A.Text(contactInfo.Phone)
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShapeV4);



                    // Create a new shape for the combined text box - Locations
                    P.Shape textShapeCombined = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShapeCombined.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "CombinedTextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Combined_WidthEmu = (long)(23.35 * 360000); // Width in EMU
                    long Combined_HeightEmu = (long)(1.35 * 360000); // Height in EMU
                    long Combined_OffsetXEmu = (long)(1.15 * 360000); // Horizontal position in EMU
                    long Combined_OffsetYEmu = (long)(12.43 * 360000); // Vertical position in EMU

                    // Set the position and size of the text box
                    textShapeCombined.ShapeProperties = new P.ShapeProperties(
                        new A.Transform2D(
                            new A.Offset { X = Combined_OffsetXEmu, Y = Combined_OffsetYEmu }, // Position
                            new A.Extents { Cx = Combined_WidthEmu, Cy = Combined_HeightEmu } // Size
                        )
                    );

                    // Set text properties
                    A.RunProperties runPropertiesCombined = new A.RunProperties
                    {
                        FontSize = 1300, // Font size in EMU
                    };
                    runPropertiesCombined.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = "14436c" })); // Embee Color

                    // Create a list to store the formatted text for locations
                    List<string> locationTexts = new List<string>();

                    // Iterate through each location and prepare the text
                    foreach (var location in contactInfo.Locations)
                    {
                        locationTexts.Add(location);
                    }

                    // Join the locations with a gap (space or any other character)
                    string combinedLocationText = string.Join("   ", locationTexts); // Adjust the gap as needed

                    // Set the text body for the combined text
                    textShapeCombined.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Enable wrapping
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.Run(
                                runPropertiesCombined,
                                new A.Text(combinedLocationText) // Use the combined location text
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShapeCombined);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (slideEnum == SlideEnum.Last3MonthsSlide)
            {
                // Sub-Heading
                try
                {
                    // Create a new shape for the text box
                    P.Shape textShape = new P.Shape();

                    // Set NonVisual properties for the text box
                    textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                        new P.NonVisualShapeDrawingProperties(),
                        new ApplicationNonVisualDrawingProperties());

                    long Heading_WidthEmu = (long)(23.38 * 360000);     // Width in EMU
                    long Heading_HeightEmu = (long)(0.94 * 360000);     // Height in EMU
                    long Heading_OffsetXEmu = (long)(0.99 * 360000);    // Horizontal position in EMU
                    long Heading_OffsetYEmu = (long)(12.41 * 360000);    // Vertical position in EMU (negative for above the origin)

                    // Set the position and size of the text box
                    textShape.ShapeProperties = new P.ShapeProperties(
                          new A.Transform2D(
                              new A.Offset { X = Heading_OffsetXEmu, Y = Heading_OffsetYEmu }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                              new A.Extents { Cx = Heading_WidthEmu, Cy = Heading_HeightEmu } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                          )
                      );

                    A.RunProperties runProperties = new A.RunProperties
                    {
                        FontSize = 1600, // multiply the desired font size by 100
                        Bold = isBold, // Make the text bold
                        Underline = hasUnderline ? A.TextUnderlineValues.Single : A.TextUnderlineValues.None, // Add underline
                    };
                    runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = fontColor }));


                    textShape.TextBody = new P.TextBody(
                        new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                        new A.ListStyle(),
                        new A.Paragraph(
                            new A.ParagraphProperties
                            {
                                // Set horizontal alignment to center
                                Alignment = A.TextAlignmentTypeValues.Center
                            },
                            new A.Run(
                                runProperties,
                                new A.Text(text) // The text content to be displayed
                            )
                        )
                    );

                    // Append the text box to the ShapeTree
                    shapeTree.AppendChild(textShape);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (slideEnum == SlideEnum.TicketByAvgResponseResolution)
            {
                // Create a new shape for the text box
                P.Shape textShape = new P.Shape();

                // Set NonVisual properties for the text box
                textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                    new P.NonVisualShapeDrawingProperties(),
                    new ApplicationNonVisualDrawingProperties());

                // Set the position and size of the text box


                textShape.ShapeProperties = new P.ShapeProperties(
                      new A.Transform2D(
                          new A.Offset { X = (long)(0.67 * 360000), Y = (long)(OffsetY * 360000) }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                          new A.Extents { Cx = (long)(19.64 * 360000), Cy = (long)(1.45 * 360000) } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                      )
                  );

                A.RunProperties runProperties = new A.RunProperties
                {
                    FontSize = 15 * 100, // multiply the desired font size by 100
                    Bold = true, // Make the text bold
                    Underline = A.TextUnderlineValues.Single, // Add underline
                };
                runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = fontColor }));


                textShape.TextBody = new P.TextBody(
                    new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                    new A.ListStyle(),
                    new A.Paragraph(
                        new A.Run(
                            runProperties
                            , new A.Text(text)
                        )
                    )
                );

                // Append the text box to the ShapeTree
                shapeTree.AppendChild(textShape);
            }
            //if (slideEnum == SlideEnum.DailyMonthlyActivity)
            //{
            //    // Create a new shape for the text box
            //    P.Shape textShape = new P.Shape();

            //    // Set NonVisual properties for the text box
            //    textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
            //        new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
            //        new P.NonVisualShapeDrawingProperties(),
            //        new ApplicationNonVisualDrawingProperties());

            //    // Set the position and size of the text box


            //    textShape.ShapeProperties = new P.ShapeProperties(
            //          new A.Transform2D(
            //              new A.Offset { X = (long)(13 * 360000), Y = (long)(2.39 * 360000) }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
            //              new A.Extents { Cx = (long)(22 * 360000), Cy = (long)(4 * 360000) } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
            //          )
            //      );

            //    A.RunProperties runProperties = new A.RunProperties
            //    {
            //        FontSize = 15 * 100, // multiply the desired font size by 100
            //        Bold = false, // Make the text bold
            //        Underline = A.TextUnderlineValues.None, // Add underline
            //    };
            //    runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = fontColor }));

            //    var lines = SplitIntoThreeLines(text);

            //    // Create Paragraph[] explicitly
            //    A.Paragraph[] paragraphArray = lines.Select(line =>
            //        new A.Paragraph(
            //            new A.Run(runProperties, new A.Text(line))
            //        )
            //    ).ToArray(); // Important: .ToArray()

            //    // Assign to TextBody
            //    textShape.TextBody = new P.TextBody(
            //        new A.BodyProperties { Wrap = A.TextWrappingValues.Square },
            //        new A.ListStyle(),
            //        paragraphArray // ✅ array works, IEnumerable fails
            //    );


            //    // Append the text box to the ShapeTree
            //    shapeTree.AppendChild(textShape);
            //}



            if (slideEnum == null)
            {
                // Create a new shape for the text box
                P.Shape textShape = new P.Shape();

                // Set NonVisual properties for the text box
                textShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties { Id = 2U, Name = "TextBox" },
                    new P.NonVisualShapeDrawingProperties(),
                    new ApplicationNonVisualDrawingProperties());

                // Set the position and size of the text box
                textShape.ShapeProperties = new P.ShapeProperties(
                      new A.Transform2D(
                          new A.Offset { X = (long)(0.39 * 360000), Y = (long)(0.16 * 360000) }, // Position: 0.39 cm from the left, 0.16 cm from the top (converted to EMUs)
                          new A.Extents { Cx = (long)(19.64 * 360000), Cy = (long)(1.45 * 360000) } // Size: Width = 19.64 cm, Height = 1.45 cm (converted to EMUs)
                      )
                  );

                A.RunProperties runProperties = new A.RunProperties
                {
                    FontSize = fontSize * 100, // multiply the desired font size by 100
                    Bold = isBold, // Make the text bold
                    Underline = hasUnderline ? A.TextUnderlineValues.Single : A.TextUnderlineValues.None, // Add underline
                };
                runProperties.AppendChild(new A.SolidFill(new A.RgbColorModelHex { Val = fontColor }));


                textShape.TextBody = new P.TextBody(
                    new A.BodyProperties { Wrap = A.TextWrappingValues.None }, // Disable wrapping to ensure it stays in one line
                    new A.ListStyle(),
                    new A.Paragraph(
                        new A.Run(
                            runProperties
                            , new A.Text(text)
                        )
                    )
                );

                // Append the text box to the ShapeTree
                shapeTree.AppendChild(textShape);
            }

            await Task.Delay(0);
            return;
        }
        //private static string[] SplitIntoThreeLines(string text)
        //{
        //    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        //    int totalWords = words.Count;

        //    int chunkSize = (int)Math.Ceiling(totalWords / 3.0);

        //    var lines = new List<string>();
        //    for (int i = 0; i < totalWords; i += chunkSize)
        //        lines.Add(string.Join(" ", words.Skip(i).Take(chunkSize)));

        //    while (lines.Count < 3)
        //        lines.Add(string.Empty);

        //    return lines.Take(3).ToArray();
        //}

        #endregion PPT Gen

        //soumik rev
        
        private async Task GenerateSlidesForDataForOnMobile(PresentationPart presentationPart, DataTable dataTable, string slideText, string companyName = "", string MonthName = "", DataTable? dataTableForChart = null)
        {

            string SlideType = string.Empty;

            if (dataTable.ExtendedProperties.ContainsKey("Code"))
            {
                SlideType = dataTable.ExtendedProperties["Code"].ToString();
            }


            if (SlideType == SlideCodeEnum.C027.ToString() || SlideType == SlideCodeEnum.C030.ToString()

                )
            {
                slideText += " for " + MonthName;
            }


            // Add logo
            string EmbeelogoPath = Directory.GetCurrentDirectory() + @"\Documents_Internal\Images\EmbeeLogo.png";
            long imageWidthEmu = (long)(3.31 * 360000);  // Width in EMU
            long imageHeightEmu = (long)(0.92 * 360000); // Height in EMU
            long offsetXEmu = (long)(21.7 * 360000);    // Horizontal position in EMU
            long offsetYEmu = (long)(0.4 * 360000);    // Vertical position in EMU

            // Return if the DataTable is null or empty
            if (dataTable == null || dataTable.Rows.Count == 0)
                return;

            const long SlideHeightEmu = 6543500; // Total available slide height in EMUs
            const long TopMarginEmu = 475200;   // Top margin (1.32 cm in EMUs)
            const long RowHeightBaseEmu = 370000; // Base height for a row in EMUs
            int MaxRowsPerSlide = TBL_MAX_ROW_COUNT; // Max rows per slide as a fallback for low-content rows

            if (slideSettings?.Config?.TableRows?.MaxCountPerTable != null)
            {
                MaxRowsPerSlide = slideSettings.Config.TableRows.MaxCountPerTable;
            }

            int totalRows = dataTable.Rows.Count;
            int currentRowIndex = 0;

            SlidePart first_slidePart = null;
            bool isSameSlide = false;

            if (SlideType == SlideCodeEnum.C024.ToString())
            {

                DataTable SourceDataTable = dataTable;

                DataTable DataTable1 = SourceDataTable.AsEnumerable()
                    .Where(r => r.Field<string>("Flag") == "TAB1")
                    .CopyToDataTable();

                string[] MonthNames = new string[0];
                if (DataTable1.Rows.Count > 0)
                {
                    MonthNames = DataTable1.Rows[0]["MonthNames"].ToString().Split(",");
                }

                // Rename column
                DataTable1.Columns["PriorityCode"].ColumnName = "Ticket Type";
                DataTable1.Columns["Month1"].ColumnName = MonthNames[0];
                DataTable1.Columns["Month2"].ColumnName = MonthNames[1];
                DataTable1.Columns["Month3"].ColumnName = MonthNames[2];




                // Keep only the needed columns

                string[] colsToRemove = { "Id", "DepartmentId", "DepartmentName", "PriorityId", "PriorityName", "MonthNames",
                "Flag","MSDHelpdesk","NocAlerts","GrandTotal"
                };

                foreach (var col in colsToRemove)
                {
                    if (DataTable1.Columns.Contains(col))
                        DataTable1.Columns.Remove(col);
                }

                dataTableForChart = DataTable1.Copy();

                // Create a new row for totals
                DataRow totalRow = DataTable1.NewRow();
                // Label for the first column (e.g., "Ticket Type")
                totalRow["Ticket Type"] = "Grand Total";

                for (int i = 1; i < DataTable1.Columns.Count; i++)
                {
                    string colName = DataTable1.Columns[i].ColumnName;

                    // Sum the column values (assuming numeric)
                    int sum = DataTable1.AsEnumerable()
                        .Sum(r => r.Field<int>(colName));

                    totalRow[colName] = sum;
                }
                // Add the total row to the end of the table
                DataTable1.Rows.Add(totalRow);


                DataTable DataTable2 = SourceDataTable.AsEnumerable()
                    .Where(r => r.Field<string>("Flag") == "TAB2")
                    .CopyToDataTable();

                // Keep only the needed columns
                DataTable2 = DataTable2.DefaultView.ToTable(false,
                    "PriorityName",
                    "MSDHelpdesk",
                    "NocAlerts", "GrandTotal");

                // Create a new row for totals
                DataRow totalRow1 = DataTable2.NewRow();
                // Label for the first column (e.g., "Ticket Type")
                totalRow1["PriorityName"] = "Grand Total";

                for (int i = 1; i < DataTable2.Columns.Count; i++)
                {
                    string colName = DataTable2.Columns[i].ColumnName;

                    // Sum the column values (assuming numeric)
                    int sum = DataTable2.AsEnumerable()
                        .Sum(r => r.Field<int>(colName));

                    totalRow1[colName] = sum;
                }
                // Add the total row to the end of the table
                DataTable2.Rows.Add(totalRow1);
                DataTable2.Columns["PriorityName"].ColumnName = "Priority Name";
                DataTable2.Columns["GrandTotal"].ColumnName = "Grand Total";
                DataTable2.Columns["MSDHelpdesk"].ColumnName = "MSD Helpdesk";
                DataTable2.Columns["NocAlerts"].ColumnName = "Noc Alerts";

                SlidePart slidePart = null;
                slidePart = await this.AddSlide(presentationPart);
                await this.AddTextToSlide(slidePart, slideText);
                await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                ////await this.AddTextToSlide(slidePart, slideText, null, null, 0.75, 1.13);


                //await this.AddTextToSlide(slidePart, "Non Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { }, 1.49);
                await this.AddTableToSlide(slidePart, DataTable1, SlideType, false, 13.26, 1.32, 11.82);
                await this.AddTableToSlide(slidePart, DataTable2, SlideType, false, 13.26, 8.81, 11.82);


                // charts

                if (dataTableForChart != null)

                {
                    var chartTable = PivotDataForChart(dataTableForChart);



                    //SlidePart chartSlidePart = null;

                    long ChartWidthEmu = (long)(12.18 * 360000);
                    long ChartHeightEmu = (long)(9.46 * 360000);
                    long ChartoffsetXEmu = (long)(0.93 * 360000);
                    long ChartoffsetYEmu = (long)(3.05 * 360000);

                    //if (first_slidePart != null)
                    //{
                    //    chartSlidePart = first_slidePart;

                    //    ChartWidthEmu = (long)(22.06 * 360000);
                    //    ChartHeightEmu = (long)(8.54 * 360000);
                    //    ChartoffsetXEmu = (long)(1.71 * 360000);
                    //    ChartoffsetYEmu = (long)(6.90 * 360000);
                    //}
                    //else
                    //{
                        //chartSlidePart = await this.AddSlide(presentationPart);
                        //await this.AddTextToSlide(slidePart, slideText + " Graphical View");
                        await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                    //}

                    try
                    {
                        string BarChartfilePath = await this.GetBarChartFilePath(chartTable, companyName, SlideEnum.Last3MonthsSlide);
                        var dimensions = await this.GetImageDimensionsInCm(BarChartfilePath);
                        await this.AddImageToSlide(slidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);
                    }
                    catch (Exception ex)
                    {

                    }
                }


                // charts


                //soumikk

            }
            else if (SlideType == SlideCodeEnum.C028.ToString())
            {


                SlidePart slidePart = null;
                slidePart = await this.AddSlide(presentationPart);
                await this.AddTextToSlide(slidePart, slideText);
                await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                ////await this.AddTextToSlide(slidePart, slideText, null, null, 0.75, 1.13);


                //await this.AddTextToSlide(slidePart, "Non Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { }, 1.49);
                //await this.AddTableToSlide(slidePart, DataTable1, SlideType, false, 13.26, 1.32, 11.82);
                await this.AddTableToSlide(slidePart, dataTable, SlideType, false, 13.81, 10.77, 11.2);


                // charts

                if (dataTableForChart != null)
                {
                    var chartTable1 = dataTableForChart.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table1")
                    .CopyToDataTable().DefaultView.ToTable(false,
                    "TicketType",
                    "Total");
                    chartTable1.Columns["TicketType"].ColumnName = "Ticket Type";

                    //
                    var chartTable2 = dataTableForChart.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table2")
                    .CopyToDataTable().DefaultView.ToTable(false,
                    "Priority",
                    "WithInSLA", "SLAViolated");
                    chartTable2.Columns["Priority"].ColumnName = "Ticket Type";
                    //
                    var chartTable3 = dataTableForChart.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table3")
                    .CopyToDataTable().DefaultView.ToTable(false,
                    "Priority",
                    "WithInSLA", "SLAViolated");
                    chartTable3.Columns["Priority"].ColumnName = "Ticket Type";

                    long ChartWidthEmu = (long)(8.1 * 360000);
                    long ChartHeightEmu = (long)(6.29 * 360000);
                    long ChartoffsetXEmu = (long)(1.32 * 360000);
                    long ChartoffsetYEmu = (long)(1.32 * 360000);

                    await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                    //}

                    try
                    {
                        //
                        string BarChartfilePath = await this.GetBarChartFilePath(chartTable1, companyName, SlideEnum.Last3MonthsSlide);
                        await this.AddImageToSlide(slidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);
                        
                        // Response 

                        long ChartWidthEmu2     = (long)(13.76 * 360000);
                        long ChartHeightEmu2    = (long)(8.75 * 360000);
                        long ChartoffsetXEmu2   = (long)(0.05 * 360000);
                        long ChartoffsetYEmu2   = (long)(7.14 * 360000);

                        string BarChartfilePath2 = await this.GetBarChartFilePath(chartTable2, companyName, SlideEnum.Last3MonthsSlide);
                        await this.AddImageToSlide(slidePart, BarChartfilePath2, ChartWidthEmu2, ChartHeightEmu2, ChartoffsetXEmu2, ChartoffsetYEmu2);
                        
                        //  Resolution
                        
                        long ChartWidthEmu3     = (long)(13.76 * 360000);
                        long ChartHeightEmu3    = (long)(8.75 * 360000);
                        long ChartoffsetXEmu3   = (long)(11.56 * 360000);
                        long ChartoffsetYEmu3   = (long)(1.08 * 360000);

                        string BarChartfilePath3 = await this.GetBarChartFilePath(chartTable3, companyName, SlideEnum.Last3MonthsSlide);

                        await this.AddImageToSlide(slidePart, BarChartfilePath2, ChartWidthEmu3, ChartHeightEmu3, ChartoffsetXEmu3, ChartoffsetYEmu3);

                    }
                    catch (Exception ex)
                    {

                    }
                }


            }
            else if (SlideType == SlideCodeEnum.C030.ToString())
            {


                SlidePart slidePart = null;
                slidePart = await this.AddSlide(presentationPart);
                await this.AddTextToSlide(slidePart, slideText);
                await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                ////await this.AddTextToSlide(slidePart, slideText, null, null, 0.75, 1.13);


                //await this.AddTextToSlide(slidePart, "Non Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { }, 1.49);
                //await this.AddTableToSlide(slidePart, DataTable1, SlideType, false, 13.26, 1.32, 11.82);
                //await this.AddTableToSlide(slidePart, dataTable, SlideType, false, 13.81, 10.77, 11.2);


                // charts

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var chartTable1 = dataTable.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table1")
                    .CopyToDataTable();
                    chartTable1.Columns.Remove("TableType");
                    //
                    var chartTable2 = dataTable.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table2")
                    .CopyToDataTable();
                    chartTable2.Columns.Remove("TableType");

                    //
                    var chartTable3 = dataTable.AsEnumerable()
                    .Where(r => r.Field<string>("TableType") == "Table3")
                    .CopyToDataTable();
                    chartTable3.Columns.Remove("TableType");


                    long ChartWidthEmu = (long)(9.53 * 360000);
                    long ChartHeightEmu = (long)(11.58 * 360000);
                    long ChartoffsetXEmu = (long)(0.68 * 360000);
                    long ChartoffsetYEmu = (long)(2.49 * 360000);

                    await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                    //}

                    try
                    {
                        //
                        string BarChartfilePath = await this.GetBarChartFilePath(chartTable1, companyName, SlideEnum.Last3MonthsSlide);
                        await this.AddImageToSlide(slidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);

                        // Response 

                        long ChartWidthEmu2 = (long)(15.19 * 360000);
                        long ChartHeightEmu2 = (long)(7.58 * 360000);
                        long ChartoffsetXEmu2 = (long)(9.53 * 360000);
                        long ChartoffsetYEmu2 = (long)(1.32 * 360000);

                        string BarChartfilePath2 = await this.GetBarChartFilePath(chartTable2, companyName, SlideEnum.Last3MonthsSlide);
                        await this.AddImageToSlide(slidePart, BarChartfilePath2, ChartWidthEmu2, ChartHeightEmu2, ChartoffsetXEmu2, ChartoffsetYEmu2);

                        //  Resolution

                        long ChartWidthEmu3 = (long)(15.19 * 360000);
                        long ChartHeightEmu3 = (long)(7.58 * 360000);
                        long ChartoffsetXEmu3 = (long)(9.53 * 360000);
                        long ChartoffsetYEmu3 = (long)(7.8 * 360000);

                        string BarChartfilePath3 = await this.GetBarChartFilePath(chartTable3, companyName, SlideEnum.Last3MonthsSlide);

                        await this.AddImageToSlide(slidePart, BarChartfilePath2, ChartWidthEmu3, ChartHeightEmu3, ChartoffsetXEmu3, ChartoffsetYEmu3);

                    }
                    catch (Exception ex)
                    {

                    }
                }


            }
            else if (SlideType == SlideCodeEnum.C032.ToString())
            {


                SlidePart slidePart = null;
                slidePart = await this.AddSlide(presentationPart);
                await this.AddTextToSlide(slidePart, slideText);
                await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                ////await this.AddTextToSlide(slidePart, slideText, null, null, 0.75, 1.13);


                //await this.AddTextToSlide(slidePart, "Non Plant User Details:", SlideEnum.TicketByAvgResponseResolution, new HelperModel { }, 1.49);
                //await this.AddTableToSlide(slidePart, DataTable1, SlideType, false, 13.26, 1.32, 11.82);
                //await this.AddTableToSlide(slidePart, dataTable, SlideType, false, 13.81, 10.77, 11.2);


                // charts

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var chartTable1 = dataTable.AsEnumerable()
                    .Where(r => r.Field<string>("Flag") == "TAB1")
                    .CopyToDataTable();
                    chartTable1.Columns.Remove("Flag");
                    chartTable1.Columns.Remove("Patch");
                    chartTable1.Columns.Remove("Windows");
                    chartTable1.Columns.Remove("Network");
                    chartTable1.Columns.Remove("BackUp");
                    chartTable1.Columns.Remove("Linux");

                    var ChartTable = this.PivotDataForChart(chartTable1);


                    //
                    var chartTable2 = dataTable.AsEnumerable()
                    .Where(r => r.Field<string>("Flag") == "TAB2")
                    .CopyToDataTable();
                    chartTable2.Columns.Remove("Flag");
                    chartTable2.Columns.Remove("Emergency");
                    chartTable2.Columns.Remove("Normal");
                    chartTable2.Columns.Remove("Standard");
                    var ChartTable1 = this.PivotDataForChart(chartTable2);
                    //



                    long ChartWidthEmu = (long)(9.53 * 360000);
                    long ChartHeightEmu = (long)(11.58 * 360000);
                    long ChartoffsetXEmu = (long)(0.68 * 360000);
                    long ChartoffsetYEmu = (long)(2.49 * 360000);

                    await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);
                    //}

                    try
                    {
                        //
                        string BarChartfilePath = await this.GetBarChartFilePath(ChartTable, companyName, SlideEnum.Last3MonthsSlide);
                        await this.AddImageToSlide(slidePart, BarChartfilePath, ChartWidthEmu, ChartHeightEmu, ChartoffsetXEmu, ChartoffsetYEmu);

                        // Response 

                        long ChartWidthEmu2     = (long)(14.64 * 360000);
                        long ChartHeightEmu2    = (long)(7.31 * 360000);
                        long ChartoffsetXEmu2   = (long)(10.37 * 360000);
                        long ChartoffsetYEmu2   = (long)(4.44 * 360000);

                        string BarChartfilePath2 = await this.GetBarChartFilePath(ChartTable1, companyName, SlideEnum.Last3MonthsSlide);
                        await this.AddImageToSlide(slidePart, BarChartfilePath2, ChartWidthEmu2, ChartHeightEmu2, ChartoffsetXEmu2, ChartoffsetYEmu2);

                       

                    }
                    catch (Exception ex)
                    {

                    }
                }


            }

            else
            {
                while (currentRowIndex < totalRows)
                {
                    // Add a new slide
                    SlidePart slidePart = await this.AddSlide(presentationPart);

                    if (currentRowIndex == 0
                        && (SlideType == SlideCodeEnum.C001.ToString() || SlideType == SlideCodeEnum.C009.ToString()))
                    {
                        first_slidePart = slidePart;
                    }

                    // Add title text to the slide
                    await this.AddTextToSlide(slidePart, slideText);

                    await this.AddImageToSlide(slidePart, EmbeelogoPath, imageWidthEmu, imageHeightEmu, offsetXEmu, offsetYEmu);

                    // Create a new DataTable for the current slide's rows
                    DataTable slideTable = dataTable.Clone();
                    long currentTableHeight = TopMarginEmu; // Start with top margin

                    // Add rows dynamically based on content height
                    int rowCountOnSlide = 0;
                    while (currentRowIndex < totalRows)
                    {
                        string rowText = string.Join(" ", dataTable.Rows[currentRowIndex].ItemArray);
                        long estimatedRowHeight = await this.EstimateRowHeight(rowText, RowHeightBaseEmu);

                        // Check if adding this row exceeds the slide height
                        if (currentTableHeight + estimatedRowHeight > SlideHeightEmu || rowCountOnSlide >= MaxRowsPerSlide)
                            break;

                        slideTable.ImportRow(dataTable.Rows[currentRowIndex]);
                        currentRowIndex++;
                        currentTableHeight += estimatedRowHeight;
                        rowCountOnSlide++;
                    }

                    // Add the table to the slide
                    await this.AddTableToSlide(slidePart, slideTable, SlideType);
                }
            }
        }

        public async Task<string> GeneratePptForOnMobile(DataSet Datas, HelperModel helperModel)
        {
            try
            {
                // Validate inputs
                if (Datas == null || Datas.Tables.Count == 0)
                {
                    throw new ArgumentException("The provided Data Set is null or empty.");
                }

                if (string.IsNullOrWhiteSpace(helperModel.DirectoryName))
                {
                    throw new ArgumentException("The CompanyId is null or empty.");
                }



                string sourceFilePath = System.IO.Directory.GetCurrentDirectory() + @"\Documents_Internal\PPT\Presentation.pptx";
                string customDirectoryName = helperModel.DirectoryName + "\\" + DateTime.Now.ToString("MMMMyyyy") + "\\Presentations";
                string outputDirectoryPath = await this.CheckOrCreateDirectory(customDirectoryName);

                if (string.IsNullOrEmpty(outputDirectoryPath))
                {
                    throw new ArgumentException("DirectoryPath is null or empty.");
                }

                string outputFilePath = Path.Combine(outputDirectoryPath, $"Presentation.pptx");
                if (!await this.CopyPowerPointFile(sourceFilePath, outputFilePath))
                {
                    throw new ArgumentException("Unable to execute CopyPowerPointFile()");
                }

                // Edit presentation
                using (PresentationDocument presentation = PresentationDocument.Open(outputFilePath, true))
                {
                    // Access the presentation part
                    PresentationPart presentationPart = presentation.PresentationPart;

                    if (presentationPart != null)
                    {
                        // Generate First Cover slide
                        await GenerateSlidesForData(presentationPart, SlideEnum.FirstSlideOnMobile, helperModel);

                        // Generate Agenda slide
                        await GenerateSlidesForData(presentationPart, SlideEnum.AgendaSlide, helperModel);

                        await GenerateSlidesForData(presentationPart, SlideEnum.IncidentTrendAnalysis, helperModel);


                        // Access the data set and generate slides for each data category
                        foreach (DataTable table in Datas.Tables)
                        {

                            await this.GenerateSlidesForDataForOnMobile(presentationPart, table, table.TableName, helperModel.DirectoryName, helperModel.MonthName, helperModel.DataTableForChart);

                            string SlideType = string.Empty;

                            if (table.ExtendedProperties.ContainsKey("Code"))
                            {
                                SlideType = table.ExtendedProperties["Code"].ToString();
                            }

                            if (!string.IsNullOrEmpty(SlideType))
                               {
                                if(SlideType == SlideCodeEnum.C024.ToString())
                                {
                                    // Generate Slide Cover Page
                                    await this.GenerateSlidesForData(presentationPart, SlideEnum.ResponseResolutionPerformanceDetails, helperModel);
                                }
                                if (SlideType == SlideCodeEnum.C029.ToString())
                                {
                                    // Generate Slide Cover Page
                                    await this.GenerateSlidesForData(presentationPart, SlideEnum.NetworkCategory, helperModel);
                                }
                            } 



                        }

                        // Generate ThankYou slide
                        await this.GenerateSlidesForData(presentationPart, SlideEnum.ThankYouSlide, helperModel);
                    }

                    // Delete the first slide
                    await this.DeleteSlide(presentationPart, 0); // 0 means the first slide

                    presentationPart.Presentation.Save();
                    await Task.Delay(100);
                }

                await Task.Delay(100);
                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        private DataTable PivotDataForChart(DataTable dt)
        {
            var result = new DataTable();
            result.Columns.Add("Ticket Type", typeof(string));

            // Get all ticket types from the original table
            var ticketTypes = dt.AsEnumerable()
                                .Select(r => r.Field<string>("Ticket Type"))
                                .ToList();

            // Add ticket types as columns
            foreach (var tt in ticketTypes)
            {
                result.Columns.Add(tt, typeof(int));
            }

            // Get all month columns (excluding "Ticket Type")
            var monthColumns = dt.Columns.Cast<DataColumn>()
                                 .Where(c => c.ColumnName != "Ticket Type")
                                 .Select(c => c.ColumnName)
                                 .ToList();

            // For each month, create a new row
            foreach (var month in monthColumns)
            {
                var row = result.NewRow();
                row["Ticket Type"] = month;

                foreach (var tt in ticketTypes)
                {
                    row[tt] = dt.AsEnumerable()
                                .Where(r => r.Field<string>("Ticket Type") == tt)
                                .Select(r => r[month])
                                .FirstOrDefault();
                }

                result.Rows.Add(row);
            }

            return result;
        }


        //soumik rev
    }
}

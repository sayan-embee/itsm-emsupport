using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Schedular.ExceptionLog
{
    public static class ExceptionLogging
    {
        private static String? ErrorlineNo, Errormsg, extype, ErrorLocation;
        public static void SendErrorToText(Exception ex)
        {
            var line = Environment.NewLine + Environment.NewLine;

            string? stackTrace = ex.StackTrace;

            if (!string.IsNullOrEmpty(stackTrace))
            {
                // Match something like "line 123" or "line 4567"
                Match match = Regex.Match(stackTrace, @"line (\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    ErrorlineNo = match.Groups[1].Value;
                }
            }

            if (string.IsNullOrEmpty(ErrorlineNo))
            {
                ErrorlineNo = ex.StackTrace?.Substring(ex.StackTrace.Length - 6, 7);
            }

            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            // exurl = context.Current.Request.Url.ToString();
            ErrorLocation = ex.Message.ToString();

            try
            {
                string filepath = Directory.GetCurrentDirectory() + @"\ExceptionFiles";
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);

                }
                filepath = filepath + @"\" + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {

                    File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = line + "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

        public static void WriteMessageToText(string message)
        {
            var line = Environment.NewLine + Environment.NewLine;
            try
            {
                string filepath = Directory.GetCurrentDirectory() + @"\ExceptionDetailsFile";
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + @"\Message_" + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                //filepath = filepath + @"\" + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                { File.Create(filepath).Dispose(); }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string msg = line + "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Message :" + " " + message;
                    //sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    //sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(msg);
                    sw.WriteLine("-------------------------------------------------------------------------");
                    //sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}

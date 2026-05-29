using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.Site24x7
{
    public class Rpt_PerformaceReportModel
    {
        public string zaaid { get; set; }
        public string name { get; set; }
        public string ServerName { get; set; }
        public int RowIndex { get; set; }
        public string Average { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public string UtilizationType { get; set; }
    }
}

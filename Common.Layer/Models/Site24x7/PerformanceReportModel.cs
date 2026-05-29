using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Layer.Models.Site24x7
{

    public class PerformanceReportModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public PRM_Data data { get; set; }
    }
    public class PRM_Data
    {
        public PRM_GroupData group_data { get; set; }
        public Info info { get; set; }
    }
    public class PRM_GroupData
    {
        public PRM_ServerData SERVER { get; set; }
    }
    public class PRM_ServerData
    {
        public List<string> name { get; set; }
        public List<string> availability { get; set; }
        public List<PRM_AttributeData> attribute_data { get; set; }
        public List<List<object>> tags { get; set; }
    }

    public class PRM_AttributeData
    {
        [JsonProperty("0")]
        public PRM_ServerMetrics metrics { get; set; }
    }
    public class PRM_ServerMetrics
    {
        public string DISKUSEDPERCENT { get; set; }
        public string MEMUSEDPERCENT { get; set; }
        public string CPUUSEDPERCENT { get; set; }
    }
    //public class PRM_ServerMetrics
    //{
    //    public string DISKUSEDPERCENT { get; set; }
    //    public string MEMUSEDPERCENT { get; set; }
    //    public string CPUUSEDPERCENT { get; set; }
    //}
    public class Info
    {
        public int period { get; set; }
        public string resource_type_name { get; set; }
        public int resource_type { get; set; }
        public string end_time { get; set; }
        public string period_name { get; set; }
        public string formatted_start_time { get; set; }
        public string metric_aggregation_name { get; set; }
        public int report_type { get; set; }
        public string formatted_generated_time { get; set; }
        public string formatted_end_time { get; set; }
        public string generated_time { get; set; }
        public string start_time { get; set; }
        public int metric_aggregation { get; set; }
        public string resource_name { get; set; }
        public string report_name { get; set; }
        public string monitor_type { get; set; }
    }
}

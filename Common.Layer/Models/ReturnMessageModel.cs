using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Layer.Models
{
    public class ReturnMessageModel
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public int Status { get; set; }
        public string Id { get; set; }
        public string ReferenceNo { get; set; }
        public string ExecutionTime { get; set; }
        public string? JwtToken { get; set; }
        public DateTime? JwtTokenExpiry { get; set; }
        public string? SessionId { get; set; }
    }
}

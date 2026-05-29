using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.AdaptiveCard
{
    public class WelcomeCardModel
    {
        public string? ShortDesc { get; set; } = String.Empty;
        public string? ImageUrl { get; set; } = String.Empty;
        public string? LongDesc { get; set; } = String.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafetyRatings.Models
{
    public class JokeViewModel
    {
        public int id { get; set; }
        public string type { get; set; }
        public string setup { get; set; }
        public string punchline { get; set; }
    }
}
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SafetyRatings.Models
{
    public class SafetyViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubType { get; set; }
        public string SafetyScore { get; set; }
        public SafetyScores Scores { get; set; }
        public string SafetyComment { get; set; }
        public PlaceName Place { get; set; }  
    }

    public enum PlaceName
    {
        NewYork,
        Barcelona,
        London,
        Paris,
        Berlin
    }

    public enum SafetyScores
    {
        overall,
        lgbtq,
        medical,
        physicalHarm,
        politicalFreedom,
        theft,
        women
    }


    internal class Global
    {
        public Uri baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=2&page%5Blimit%5D=2");
        public JObject jObject = null;
        public string fetchScore = "overall";
        public string comment = "An average of the 6 “sub”-categories.Score go from 1 (very safe) to 100 (very dangerous).";
        public string token = System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusToken"];
    }
}
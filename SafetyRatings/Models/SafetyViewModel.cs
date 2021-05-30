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


        public int PlaceId { get; set; }
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

    public class SafetyScores
    {
        public string LGBTQ { get; set; }
        public string Women { get; set; }
        public string Overall { get; set; }

    }


    

    //get_new_token()
    //{
    //    var client = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token");
    //    client.Timeout = -1;
    //    var request = new RestRequest(Method.POST);
    //    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
    //    request.AddParameter("grant_type", "client_credentials");
    //    request.AddParameter("client_id", "JxADxJiWnn4tlRdnKPR5U1duX3kLnCaY");
    //    request.AddParameter("client_secret", "wclab2e0JLTdT0bT");
    //    IRestResponse response = client.Execute(request);
    //    Console.WriteLine(response.Content);
    //}
}
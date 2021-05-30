using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SafetyRatings.Models;

namespace SafetyRatings.Controllers
{
    public class SafetyController : Controller
    {
        // init viewmodel to display
        SafetyViewModel safetyMod = new SafetyViewModel
        {
            Id = "Emp",
            Name = "Empty",
            SubType = "Empty",
            SafetyScore = "Empty",
        };

        // oAuth Token
        string token = "84v2CwQXJO3NwzO14Zpc27YqN5cQ";
        // NewYork
        Uri baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");


        [HttpGet]
        // GET: Safety
        public ActionResult Index()
        {
            
            // see if token is still valid. if not, get new token
            token = Check_token_validity(ref token);

            // Call API and return response
            IRestResponse response = get_info(ref token, ref baseUrl);

            int statusCode = (int)response.StatusCode;
            Debug.WriteLine(statusCode);

            var jObject = JObject.Parse(response.Content);

            if (statusCode == 200)
            {
                
                safetyMod = set_view(ref safetyMod, ref jObject);
            }
            else
            {
                safetyMod = new SafetyViewModel
                {
                    Id = "Err",
                    Name = "Error Description" + response.StatusDescription.ToString(),
                    SubType = "Error Code: " + statusCode.ToString(),
                    SafetyScore = "Sorry, try again",
                };
                ModelState.AddModelError(string.Empty, "An error occured");
                token = Get_new_token2(ref token);
            }
            return View(safetyMod);
        }


        [HttpPost]
        // POST
        public ActionResult Index(SafetyViewModel _safetyView)
        {
            Debug.WriteLine("TESTING POST");

            // NewYork
            //var baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");

            if (_safetyView != null)
            {

                Debug.WriteLine(_safetyView.Place); // can access this!!!


                // set URI
                switch(_safetyView.Place.ToString())
                {
                    case "NewYork":
                        baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                        break;
                    case "Barcelona":
                        baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=41.385064&longitude=2.173404&radius=1");
                        break;
                    case "London":
                        baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=51.511214&longitude=-0.119824&radius=1");
                        break;
                    case "Paris":
                        baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=48.856614&longitude=2.3522219&radius=1");
                        break;
                    case "Berlin":
                        baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=52.519171&longitude=13.406091&radius=1");
                        break;
                    default:
                        baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                        break;

                }

                Debug.WriteLine(baseUrl);

                //var baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                // new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                //New York: "latitude": 40.755653, "longitude": -73.985303 // new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                //    // Barcelona: "latitude": 41.385064, "longitude": 2.173404 // new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                //    // London: "latitude": 51.511214, "longitude": -0.119824 // new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1"); 
                //    // Paris: "latitude": 48.856614, "longitude": 2.3522219 // new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");
                //    // Berlin: "latitude": 52.519171, "longitude": 13.406091 // new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");



                // see if token is still valid. if not, get new token
                token = Check_token_validity(ref token);

                // Call API and return response
                IRestResponse response = get_info(ref token, ref baseUrl);

                int statusCode = (int)response.StatusCode;
                Debug.WriteLine(statusCode);

                var jObject = JObject.Parse(response.Content);

                if (statusCode == 200)
                {

                    safetyMod = set_view(ref safetyMod, ref jObject);
                }
                else
                {
                    safetyMod = new SafetyViewModel
                    {
                        Id = "Err",
                        Name = "Error Description" + response.StatusDescription.ToString(),
                        SubType = "Error Code: " + statusCode.ToString(),
                        SafetyScore = "Sorry, try again",
                    };
                    ModelState.AddModelError(string.Empty, "An error occured");
                    token = Get_new_token2(ref token);
                }
                //return View(safetyMod);
            }
            return View(safetyMod);
        }

            public string Check_token_validity(ref string token)
        {
            var baseUrl = new Uri("https://test.api.amadeus.com/v1/security/oauth2/token/");

            //var client2 = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token/");
            var client2 = new RestClient(baseUrl + token);
            client2.Timeout = -1;
            var request2 = new RestRequest(Method.GET);
            IRestResponse response2 = client2.Execute(request2);
            Debug.WriteLine("TESTING AUTH THOKEN");
            Debug.WriteLine(response2.Content);

            var jObjecttokenStat = JObject.Parse(response2.Content);

            foreach (JProperty property2 in jObjecttokenStat.Properties())
            {
                if (property2.Name.ToString() == "state" && property2.Value.ToString() == "expired")
                {
                    token = Get_new_token2(ref token);
                    Debug.WriteLine("new token: " + token);
                }
            }
            return token;
        }

        public string Get_new_token2(ref string token)
        {
            var client3 = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token");
            client3.Timeout = -1;
            var request3 = new RestRequest(Method.POST);
            request3.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request3.AddParameter("grant_type", "client_credentials");
            request3.AddParameter("client_id", "JxADxJiWnn4tlRdnKPR5U1duX3kLnCaY");
            request3.AddParameter("client_secret", "wclab2e0JLTdT0bT");
            IRestResponse response3 = client3.Execute(request3);
            Debug.WriteLine(response3.Content);


            var jObjectToken = JObject.Parse(response3.Content);

            foreach (JProperty property in jObjectToken.Properties())
            {
                if (property.Name == "access_token")
                {
                    token = property.Value.ToString();
                }
            }
            return token;
        }

        public IRestResponse get_info(ref string token, ref Uri baseUrl)
        {
            //var baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=41.397158&longitude=2.160873&radius=1");
            //Debug.WriteLine("This is baseUrl: ");
            //Debug.WriteLine(baseUrl);
            var client1 = new RestClient(baseUrl);
            //var client1 = new RestClient("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=41.397158&longitude=2.160873&radius=1");
            client1.Timeout = -1;
            var request = new RestRequest(Method.GET);
            //request.AddQueryParameter("latitude", latitude);
            //request.AddQueryParameter("longitude", longitude);
            //request.AddQueryParameter("radius", "2");
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client1.Execute(request);
            Debug.WriteLine(response.Content);
            
            return response;
        }


        private SafetyViewModel set_view(ref SafetyViewModel safetyMod, ref JObject jObject)
        {
            int count = 0;
            int maxScore = 100; // 100 is most dangerous, 0 is safest
            int maxScoreId = 0;
            int currScore = 0;

            string fetchScore = "overall";

            if (jObject["data"] != null)
            {

                foreach (var item in jObject["data"])
                {
                    Debug.WriteLine(count);
                    currScore = int.Parse(jObject["data"][count]["safetyScores"][fetchScore].ToString());
                    if (currScore < maxScore)
                    {
                        maxScore = currScore;
                        maxScoreId = count;

                    }
                    Debug.WriteLine(jObject["data"][count]["id"].ToString());
                    Debug.WriteLine(jObject["data"][count]["name"].ToString());
                    Debug.WriteLine(jObject["data"][count]["subType"].ToString());
                    Debug.WriteLine(currScore);
                    count += 1;
                }


                safetyMod = new SafetyViewModel
                {
                    Id = jObject["data"][maxScoreId]["id"].ToString(),
                    Name = jObject["data"][maxScoreId]["name"].ToString(),
                    SubType = jObject["data"][maxScoreId]["subType"].ToString(),
                    SafetyScore = jObject["data"][maxScoreId]["safetyScores"][fetchScore].ToString(),
                    //Id = jObject["data"][0]["id"].ToString(),
                    //Name = jObject["data"][0]["name"].ToString(),
                    //SubType = jObject["data"][0]["subType"].ToString(),
                    //SafetyScore = jObject["data"][0]["safetyScores"][fetchScore].ToString(),
                };

            }
            return safetyMod;
        }
    }
}
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
        // GET: Safety
        public ActionResult Index()
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
            var token = "FGMfVJGub3oSOsVMzFCRAgRCFjGT";

            // see if token is still valid. if not, get new token
            token = Check_token_validity(ref token);

            // Call API and return response
            IRestResponse response = get_info(ref token);

            int statusCode = (int)response.StatusCode;
            Debug.WriteLine(statusCode);

            var jObject = JObject.Parse(response.Content);

            if (statusCode == 200)
            {
                int i = (int)jObject["meta"]["count"];
                int count = 0;
                int maxScore = 100; // 100 is most dangerous, 0 is safest
                int maxScoreId = 0;
                int currScore = 0;

                string fetchScore = "overall";

                //IEnumerable<int> list = new List<int> { 1, 2, 3 };

                //IEnumerable<string> PlaceName = new List<string>();
                //IEnumerable<string> Scores = new List<string>();

                //IEnumerable<string[]> PlaceName = new list<string[]>;
                //IEnumerable<string[]> Scores = new IEnumerable<string[]>;
                //string[] PlaceName = enumerable;
                //string[] Scores;

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

                    //PlaceName.Append(jObject["data"][count]["name"].ToString());
                    //Scores.Append(jObject["data"][count]["safetyScores"][fetchScore].ToString());

                }

                //Debug.WriteLine("this is PlaceName: ");
                //Debug.WriteLine(PlaceName);


                //Debug.WriteLine("this is Scores: ");
                //Debug.WriteLine(Scores);

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

        public IRestResponse get_info(ref string token)
        {
            var client1 = new RestClient("https://test.api.amadeus.com/v1/safety/safety-rated-locations?");
            client1.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddParameter("latitude", 41.397158);
            request.AddParameter("longitude", 2.160873);
            request.AddParameter("radius", "2");
            request.AddHeader("Authorization", "Bearer " + token);
            //IRestResponse<List<SafetyViewModel>> response = client1.Execute<List<SafetyViewModel>>(request);
            IRestResponse response = client1.Execute(request);
            Debug.WriteLine(response.Content);
            
            return response;
        }


        //private string get_new_token2(ref string token)
        //{
        //    throw new NotImplementedException();
        //}
    }
        //private static string get_new_token(ref string token)
        //{

        //    var client3 = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token");
        //    client3.Timeout = -1;
        //    var request3 = new RestRequest(Method.POST);
        //    request3.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //    request3.AddParameter("grant_type", "client_credentials");
        //    request3.AddParameter("client_id", "JxADxJiWnn4tlRdnKPR5U1duX3kLnCaY");
        //    request3.AddParameter("client_secret", "wclab2e0JLTdT0bT");
        //    IRestResponse response3 = client3.Execute(request3);
        //    Debug.WriteLine(response3.Content);


        //    var jObjectToken = JObject.Parse(response3.Content);

        //    Debug.WriteLine("jObjectToken QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ");
        //    Debug.WriteLine(jObjectToken);



        //    foreach (JProperty property in jObjectToken.Properties())
        //    {
        //        //Debug.WriteLine("one: ");
        //        //Debug.WriteLine(property.Name + " - " + property.Value);

        //        if (property.Name == "access_token")
        //        {
        //            Debug.WriteLine("actual token? QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ: ");

        //            Debug.WriteLine("one: ");
        //            Debug.WriteLine(property.Name + " - " + property.Value);
        //            token = property.Value.ToString();

        //            Debug.WriteLine("two: ");
        //            Debug.WriteLine(token);

        //        }
        //        else
        //            return "invalid";
        //    }
        //    return token;
        //}
}
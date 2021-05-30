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
        string token = "yBgXDYjImceQ6220VyEiBSDmFwdU";
        string fetchScore = "overall";
        string comment = "An average of the 6 “sub”-categories.Score go from 1 (very safe) to 100 (very dangerous).";
        // NewYork
        Uri baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");

        [HttpGet]
        // GET: Safety
        public ActionResult Index()
        {
            // testing here?
            //test_set_view();


            // see if token is still valid. if not, get new token
            token = Check_token_validity(ref token);

            // Call API and return response
            IRestResponse response = get_info(ref token, ref baseUrl);

            int statusCode = (int)response.StatusCode;

            Debug.Assert(statusCode == 200, "Test #1: Valid response received");
            //Debug.WriteLine(statusCode);

            var jObject = JObject.Parse(response.Content);

            if (statusCode == 200)
            {
                
                safetyMod = set_view(ref safetyMod, ref jObject);
            }
            else
            {
                Debug.Assert(statusCode != 200, "Test #2: invalid response received: " + statusCode + " in else.");
                safetyMod = new SafetyViewModel
                {
                    Id = "Err",
                    Name = "Error Description" + response.StatusDescription.ToString(),
                    SubType = "Error Code: " + statusCode.ToString(),
                    SafetyScore = "Sorry, try again",
                    safetyComment = "Error occured",
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
            Debug.Assert(_safetyView != null, "Test #3: items allocated to model.");

            // NewYork
            //var baseUrl = new Uri("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=1");

            if (_safetyView != null)
            {
                Debug.Assert(_safetyView.Scores.ToString() != null, "Test #4: Score type is set successfully.");

                if (_safetyView.Scores.ToString() != null)
                {
                    // set score rating param
                    try
                    {
                        fetchScore = _safetyView.Scores.ToString();


                        // set score rating comment

                        switch (_safetyView.Scores.ToString())
                        {
                            case "women":
                                comment = "Likelihood of inappropriate behavior against females.Score go from 1 (not likely) to 100 (very likely).";
                                break;
                            case "overall":
                                comment = "An average of the 6 “sub”-categories.Score go from 1(very safe) to 100(very dangerous).";
                                break;
                            case "lgbtq":
                                comment = "Likelihood of harm or discrimination against LGBTQ persons or groups and level of caution required at location.Score go from 1(not likely) to 100(very likely).";
                                break;
                            case "medical":
                                comment = "Likelihood of illness or disease, assessment of water and air quality, and access to reliable medical care. Score go from 1 (not likely) to 100 (very likely).";
                                break;
                            case "physicalHarm":
                                comment = "Likelihood of injury due to harmful intent. Score go from 1 (not likely) to 100 (very likely).";
                                break;
                            case "politicalFreedom":
                                comment = "Potential for infringement of political rights or political unrest. Score go from 1 (not likely) to 100 (very likely).";
                                break;
                            case "theft":
                                comment = "Likelihood of theft. Score go from 1 (not likely) to 100 (very likely).";
                                break;
                        }
                    }
                    catch
                    {
                        Debug.Assert(_safetyView.Scores.ToString() == null, "Test #5: Score type is not set.");
                    }
                }

                Debug.Assert(_safetyView.Place.ToString() != null, "Test #6: Place is set successfully.");
                
                // set URI
                switch (_safetyView.Place.ToString())
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
                        safetyComment = "Err",
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

            if (token is null)
            {
                token = Get_new_token2(ref token);
            }
            else
            {
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
            }
            return token;
        }

        public string Get_new_token2(ref string token)
        {

            string AccessID = System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusClientID"];
            string AccessSecret = System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusClientSecret"];

            var client3 = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token");
            client3.Timeout = -1;
            var request3 = new RestRequest(Method.POST);
            request3.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request3.AddParameter("grant_type", "client_credentials");
            request3.AddParameter("client_id", AccessID);
            request3.AddParameter("client_secret", AccessSecret);
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


            Debug.WriteLine("this is fetchScore: " + fetchScore);

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
                    safetyComment = comment,
                    //Id = jObject["data"][0]["id"].ToString(),
                    //Name = jObject["data"][0]["name"].ToString(),
                    //SubType = jObject["data"][0]["subType"].ToString(),
                    //SafetyScore = jObject["data"][0]["safetyScores"][fetchScore].ToString(),
                };

            }
            return safetyMod;
        }

        // tests:

        /*
        private void test_set_view()
        {
            string json = @"{'data':[{'type':'safety - rated - location','id':'Q930402719','self':{ 'type':'https://test.api.amadeus.com/v1/safety/safety-rated-locations/Q930402719','methods':['GET']},'subType':'CITY','name':'Barcelona','geoCode':{ 'latitude':41.385064,'longitude':2.173404},'safetyScores':{ 'lgbtq':39,'medical':69,'overall':45,'physicalHarm':36,'politicalFreedom':50,'theft':44,'women':34}},{'type':'safety -rated-location','id':'Q930402720','self':{ 'type':'https://test.api.amadeus.com/v1/safety/safety-rated-locations/Q930402720','methods':['GET']},'subType':'DISTRICT','name':'Antiga Esquerra de l'Eixample (Barcelona)','geoCode':{ 'latitude':41.3885573,'longitude':2.1573033},'safetyScores':{ 'lgbtq':37,'medical':69,'overall':44,'physicalHarm':34,'politicalFreedom':50,'theft':42,'women':33}}}";
            JObject jObjectTst;
            try
            {
                jObjectTst = JObject.Parse(json);


            }
            catch(Exception e)
            {
                Console.WriteLine("Could not set value, see #101. Error Message: " + e);
            }

            SafetyViewModel safetyModtst = new SafetyViewModel
                 {
                     Id = "Emp",
                     Name = "Empty",
                     SubType = "Empty",
                     SafetyScore = "Empty",
                 };

            try
            {
                safetyModtst = set_view(ref safetyModtst, ref jObjectTst);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not set value, see #102. Error Message: " + e);
            }
            Debug.Assert(safetyModtst != null, "Test set_view() successfully returned a result");
            Debug.Assert(safetyModtst.Id.ToString() != "Emp", "Test set_view() successfully set new values");
        }
        */
    }
}
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
        // init global variables
        Global g = new Global();

        SafetyViewModel safetyMod = new SafetyViewModel
        {
            Id = "Emp",
            Name = "Empty",
            SubType = "Empty",
            SafetyScore = "Empty",
        };


        [HttpGet]
        // GET: Safety
        public ActionResult Index()
        {
            // Uncomment below to RUN TESTS
            //run_tests();

            safetyMod = populate_view(ref safetyMod);

            return View(safetyMod);
        }


        [HttpPost]
        // POST
        public ActionResult Index(SafetyViewModel _safetyView)
        {
            Debug.Assert(_safetyView != null, "Test #3: items allocated to model.");

            if (_safetyView != null)
            {
                set_selected_values(ref _safetyView);
            }
            safetyMod = populate_view(ref safetyMod);
            return View(safetyMod);
        }


        // Ensure token is still valid. Even if invalid or expired tokens passed, get new token
        public string check_token_validity(ref string token)
        {
            var baseUrl = new Uri("https://test.api.amadeus.com/v1/security/oauth2/token/");

            if (token is null)
            {
                token = get_new_token(ref token);
                System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusToken"] = token;
                Debug.WriteLine("new token: " + token);
            }
            else
            {
                var client = new RestClient(baseUrl + token);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                var jObjecttokenStat = JObject.Parse(response.Content);

                foreach (JProperty property in jObjecttokenStat.Properties())
                {
                    if (property.Name.ToString() == "state")
                    {
                        if (property.Value.ToString() == "expired")
                        {
                            token = get_new_token(ref token);
                            System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusToken"] = token;
                        }
                    }
                    else if (property.Name.ToString() == "error")
                    {
                        token = get_new_token(ref token);
                        System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusToken"] = token;
                    }
                    // no else statement, as token must remain as is, if not expired and no error is returned
                }
            }
            return token;
        }


        // Get new token using AmadeusClientID and AmadeusClientSecret
        public string get_new_token(ref string token)
        {
            string AccessID = System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusClientID"];
            string AccessSecret = System.Web.Configuration.WebConfigurationManager.AppSettings["AmadeusClientSecret"];

            var client = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", AccessID);
            request.AddParameter("client_secret", AccessSecret);
            IRestResponse response = client.Execute(request);

            var jObjectToken = JObject.Parse(response.Content);

            foreach (JProperty property in jObjectToken.Properties())
            {
                if (property.Name == "access_token")
                {
                    token = property.Value.ToString();
                }
                else if (property.Name == "error")
                {
                    Debug.Assert(property.Name.ToString() != "error", "check error: " + property.Value.ToString());
                    Console.WriteLine(property.Value.ToString());
                }
            }
            return token;
        }


        // Set global variable values, selected from dropdown. 
        public void set_selected_values(ref SafetyViewModel safetyView)
        {
            if (safetyView.Scores.ToString() != null)
            {
                // set score rating paramater
                g.fetchScore = safetyView.Scores.ToString();

                // set score rating comment
                switch (safetyView.Scores.ToString())
                {
                    case "women":
                        g.comment = "Likelihood of inappropriate behavior against females.Score go from 1 (not likely) to 100 (very likely).";
                        break;
                    case "overall":
                        g.comment = "An average of the 6 “sub”-categories.Score go from 1(very safe) to 100(very dangerous).";
                        break;
                    case "lgbtq":
                        g.comment = "Likelihood of harm or discrimination against LGBTQ persons or groups and level of caution required at location.Score go from 1(not likely) to 100(very likely).";
                        break;
                    case "medical":
                        g.comment = "Likelihood of illness or disease, assessment of water and air quality, and access to reliable medical care. Score go from 1 (not likely) to 100 (very likely).";
                        break;
                    case "physicalHarm":
                        g.comment = "Likelihood of injury due to harmful intent. Score go from 1 (not likely) to 100 (very likely).";
                        break;
                    case "politicalFreedom":
                        g.comment = "Potential for infringement of political rights or political unrest. Score go from 1 (not likely) to 100 (very likely).";
                        break;
                    case "theft":
                        g.comment = "Likelihood of theft. Score go from 1 (not likely) to 100 (very likely).";
                        break;
                }
            }

            Debug.Assert(safetyView.Place.ToString() != null, "Test #6: Place is set successfully.");

            // set URI for API call
            switch (safetyView.Place.ToString())
            {
                case "NewYork":
                    g.baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=2&page%5Blimit%5D=30");
                    break;
                case "Barcelona":
                    g.baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=41.385064&longitude=2.173404&radius=2&page%5Blimit%5D=30");
                    break;
                case "London":
                    g.baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=51.511214&longitude=-0.119824&radius=2&page%5Blimit%5D=30");
                    break;
                case "Paris":
                    g.baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=48.856614&longitude=2.3522219&radius=2&page%5Blimit%5D=30");
                    break;
                case "Berlin":
                    g.baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=52.519171&longitude=13.406091&radius=2&page%5Blimit%5D=30");
                    break;
                default:
                    g.baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=2&page%5Blimit%5D=30");
                    break;
            }
        }


        // Call API and return response
        public IRestResponse get_info(ref string token, ref Uri baseUrl)
        {
            var client = new RestClient(baseUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            
            return response;
        }


        // populate view that will be returned in Index, GET and POST
        private SafetyViewModel populate_view(ref SafetyViewModel safetyMod)
        {
            g.token = check_token_validity(ref g.token);
            IRestResponse response = get_info(ref g.token, ref g.baseUrl);
            int statusCode = (int)response.StatusCode;

            try
            {
                g.jObject = JObject.Parse(response.Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (statusCode == 200)
            {
                safetyMod = set_view_values(ref safetyMod, ref g.jObject, ref g.fetchScore, ref g.comment);
            }
            else
            {
                safetyMod = new SafetyViewModel
                {
                    Id = "Err",
                    Name = "Error Description " + response.StatusDescription.ToString(),
                    SubType = "Error Code: " + statusCode.ToString(),
                    SafetyScore = "Sorry, try again",
                    SafetyComment = "Error occured",
                };
                g.token = get_new_token(ref g.token);
            }
            return safetyMod;
        }


        // Called in populate_view()
        private SafetyViewModel set_view_values(ref SafetyViewModel safetyMod, ref JObject jObject, ref string fetchScore, ref string comment)
        {
            int count = 0;
            int minScore = 100; // 100 is most dangerous, 0 is safest
            int minScoreId = 0;
            int currScore;

            Debug.Assert(jObject["data"] != null, "FT set_view, Test #1: Object passed has no data");

            if (jObject["data"] != null)
            {
                foreach (var item in jObject["data"])
                {
                    // set the lowest score for score paramater
                    currScore = int.Parse(jObject["data"][count]["safetyScores"][fetchScore].ToString());
                    if (currScore < minScore)
                    {
                        minScore = currScore;
                        minScoreId = count;
                    }
                    count += 1;
                }

                safetyMod = new SafetyViewModel
                {
                    Id = jObject["data"][minScoreId]["id"].ToString(),
                    Name = jObject["data"][minScoreId]["name"].ToString(),
                    SubType = jObject["data"][minScoreId]["subType"].ToString(),
                    SafetyScore = jObject["data"][minScoreId]["safetyScores"][fetchScore].ToString(),
                    SafetyComment = comment,
                };
            }
            return safetyMod;
        }


        // TESTS
        public void test_check_token_validity()
        {            
            string testToken = "123456asdfed45678segfff481d";
            string newToken = null;
            string returnedToken;

            returnedToken = check_token_validity(ref testToken);
            newToken = check_token_validity(ref newToken);

            Debug.Assert(returnedToken.Length == 28, "FT test_check_token_validity() TEST #1: length of token returned should be 28 characters");
            Debug.Assert(newToken.Length == 28, "FT test_check_token_validity() TEST #2: length of token returned should be 28 characters");
            Debug.WriteLine("new token: " + newToken);
        }


        private void test_populate_view()
        {
            string json = @"{'data':[{'type':'safety - rated - location','id':'Q930206666','self':{ 'type':'https://test.api.amadeus.com/v1/safety/safety-rated-locations/Q930206666','methods':['GET']},'subType':'CITY','name':'New York','geoCode':{ 'latitude':40.755653,'longitude':-73.985303},'safetyScores':{ 'lgbtq':35,'medical':73,'overall':39,'physicalHarm':30,'politicalFreedom':40,'theft':27,'women':26}},{'type':'safety -rated-location','id':'Q930212195','self':{ 'type':'https://test.api.amadeus.com/v1/safety/safety-rated-locations/Q930212195','methods':['GET']},'subType':'DISTRICT','name':'Manhattan - NoHo (New York)','geoCode':{ 'latitude':40.728726,'longitude':-73.992879},'safetyScores':{ 'lgbtq':33,'medical':68,'overall':36,'physicalHarm':28,'politicalFreedom':41,'theft':22,'women':23}}],'meta':{ 'count':20,'links':{ 'self':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026radius=2','next':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026page%5Boffset%5D=1\u0026radius=2','last':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026page%5Boffset%5D=10\u0026radius=2','first':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026page%5Boffset%5D=0\u0026radius=2'}}}";
            JObject jObjectTst = JObject.Parse(json);

            SafetyViewModel safetyModtst = new SafetyViewModel
            {
                Id = "Emp",
                Name = "Empty",
                SubType = "Empty",
                SafetyScore = "Empty",
            };

            safetyModtst = populate_view(ref safetyModtst);

            Debug.Assert(safetyModtst != null, "FT test_populate_view() TEST #1: returned view is empty");
            Debug.Assert(safetyModtst.Id.ToString() != "Emp", "FT test_populate_view() TEST #2: the Id from returned view should not be Emp");
        }


        private void test_set_selected_values()
        {
            // this is for Berlin
            Uri baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=52.519171&longitude=13.406091&radius=2&page%5Blimit%5D=30");
            string scoresComment = "Likelihood of theft. Score go from 1 (not likely) to 100 (very likely).";

            SafetyViewModel safetyModtst = new SafetyViewModel
            {
                Id = "Emp",
                Name = "Empty",
                SubType = "Empty",
                Scores = SafetyScores.theft,
                Place = PlaceName.Berlin,
            };

            set_selected_values(ref safetyModtst);

            Debug.Assert(g.fetchScore == "theft", "FT test_set_selected_values() TEST #1: Did not return value passed in for fetchScore");
            Debug.Assert(g.comment == scoresComment, "FT test_set_selected_values() TEST #2: Did not return value passed in for Comment");
            Debug.Assert(g.baseUrl == baseUrl, "FT test_set_selected_values() TEST #3: Did not return value passed in for Place / URI");
        }


        private void test_set_view_values()
        {
            string fetchScore = "overall";
            string comment = "random comment";
            string json = @"{'data':[{'type':'safety - rated - location','id':'Q930206666','self':{ 'type':'https://test.api.amadeus.com/v1/safety/safety-rated-locations/Q930206666','methods':['GET']},'subType':'CITY','name':'New York','geoCode':{ 'latitude':40.755653,'longitude':-73.985303},'safetyScores':{ 'lgbtq':35,'medical':73,'overall':39,'physicalHarm':30,'politicalFreedom':40,'theft':27,'women':26}},{'type':'safety -rated-location','id':'Q930212195','self':{ 'type':'https://test.api.amadeus.com/v1/safety/safety-rated-locations/Q930212195','methods':['GET']},'subType':'DISTRICT','name':'Manhattan - NoHo (New York)','geoCode':{ 'latitude':40.728726,'longitude':-73.992879},'safetyScores':{ 'lgbtq':33,'medical':68,'overall':36,'physicalHarm':28,'politicalFreedom':41,'theft':22,'women':23}}],'meta':{ 'count':20,'links':{ 'self':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026radius=2','next':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026page%5Boffset%5D=1\u0026radius=2','last':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026page%5Boffset%5D=10\u0026radius=2','first':'https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653\u0026longitude=-73.985303\u0026page%5Blimit%5D=2\u0026page%5Boffset%5D=0\u0026radius=2'}}}";
            JObject jObjectTst = JObject.Parse(json);

            SafetyViewModel safetyModtst = new SafetyViewModel
                 {
                     Id = "Emp",
                     Name = "Empty",
                     SubType = "Empty",
                     SafetyScore = "Empty",
                 };

            safetyModtst = set_view_values(ref safetyModtst, ref jObjectTst, ref fetchScore, ref comment);
            
            Debug.Assert(safetyModtst != null, "FT set_view() TEST #1: returned view is empty");
            Debug.Assert(safetyModtst.Id.ToString() != "Emp", "FT set_view() TEST #2: the Id from returned view should not be Emp");
        }


        public void test_get_new_token()
        {
            string validToken = "bNNYKF5ZI7ktN1RXUzN80z7I4HUw";
            string invalidToken = "priYzGUhD4HoC34dLxbO8s84Z5N";
            string expiredToken = "priYzGUhD4HoC34dLxbO8s84Z5NS";

            string responseVal = get_new_token(ref validToken);
            string responseInval = get_new_token(ref invalidToken);
            string responseExp = get_new_token(ref expiredToken);

            Debug.Assert(responseVal.Length == 28, "FT get_new_token() TEST #1: Invalid response returned for valid token" + responseVal.Length);
            Debug.Assert(responseInval.Length == 28, "FT get_new_token() TEST #2: Invalid response returned for invalid token, should have corrected in the function" + responseInval.Length); // because i correct it in the function
            Debug.Assert(responseExp.Length == 28, "FT get_new_token() TEST #3: Invalid response returned for expired token, should have corrected in the function" + responseExp.Length);
        }


        private void test_get_info()
        {   
            string validToken = "bNNYKF5ZI7ktN1RXUzN80z7I4HUw";
            string invalidToken = "priYzGUhD4HoC34dLxbO8s84Z5N";
            string expiredToken = "priYzGUhD4HoC34dLxbO8s84Z5NS";
            Uri baseUrl = new Uri(uriString: "https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=40.755653&longitude=-73.985303&radius=2&page%5Blimit%5D=2");
            
            IRestResponse responseVal = get_info(ref validToken, ref baseUrl);
            IRestResponse responseInval = get_info(ref invalidToken, ref baseUrl);
            IRestResponse responseExp = get_info(ref expiredToken, ref baseUrl);

            Debug.Assert((int)responseVal.StatusCode == 200, "FT test_get_info() TEST #1: Invalid response returned for valid token" + (int)responseVal.StatusCode);
            Debug.Assert((int)responseInval.StatusCode != 200, "FT test_get_info() TEST #2: Valid response returned for invalid token, should not have corrected in the function" + (int)responseInval.StatusCode);
            Debug.Assert((int)responseExp.StatusCode != 200, "FT test_get_info() TEST #3: Valid response returned for expired token, should not have corrected in the function" + (int)responseExp.StatusCode);
        }


        public void run_tests()
        {
            test_check_token_validity();
            test_get_new_token();
            test_set_selected_values();
            test_get_info();
            test_set_view_values();
            test_populate_view();
        }
    }
}
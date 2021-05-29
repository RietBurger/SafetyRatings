using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using SafetyRatings.Models;
using RestSharp;
using System.Diagnostics;

namespace SafetyRatings.Controllers
{
    public class JokeController : Controller
    {
        // GET: Joke
        public ActionResult Index()
        {
            //return View();
            /*
            var baseUrl = new Uri("https://test.api.amadeus.com/v1/security/oauth2/token/66cyZxsGOs91v2yqn4hQFiPHAe8l");


            //var client2 = new RestClient("https://test.api.amadeus.com/v1/security/oauth2/token/");
            var client2 = new RestClient(baseUrl);
            client2.Timeout = -1;
            var request2 = new RestRequest(Method.GET);
            IRestResponse response2 = client2.Execute(request2);
            Debug.WriteLine("TESTING AUTH THOKEN");
            Debug.WriteLine(response2.Content);
                       


            var client1 = new RestClient("https://test.api.amadeus.com/v1/safety/safety-rated-locations?latitude=41.397158&longitude=2.160873&radius=2");
            client1.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 66cyZxsGOs91v2yqn4hQFiPHAe8l");
            //IRestResponse<List<SafetyViewModel>> response = client1.Execute<List<SafetyViewModel>>(request);
            IRestResponse response = client1.Execute(request);
            Debug.WriteLine(response.Content);
            */


            IEnumerable<JokeViewModel> joke = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://official-joke-api.appspot.com/");
                var responseTask = client.GetAsync("random_ten");
                responseTask.Wait();

                var result = responseTask.Result;

                if(result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<IList<JokeViewModel>>();
                    readJob.Wait();

                    joke = readJob.Result;
                }
                else
                {
                    // return error code
                    joke = Enumerable.Empty<JokeViewModel>();
                    ModelState.AddModelError(string.Empty, "An error occured");


                }
            }
            return View(joke);

        }
    }
}
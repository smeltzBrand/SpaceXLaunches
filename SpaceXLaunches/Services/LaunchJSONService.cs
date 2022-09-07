using SpaceXLaunches.Models;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using SpaceXLaunches.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;

namespace SpaceXLaunches.Services
{
    public class LaunchJSONService : ILaunchJSONService
    {
        public async Task<List<Launch>> GetLaunchDataAsync()
        {


            List<Launch> launchData = new ();
            List<Launch> orderedLaunchData = new ();

            List<RootLaunch> rootLaunches = new();
            List<Payload> payloads = new();

            payloads = await GetPayloadRankAsync();

            try
            {

                //var webRequest = WebRequest.Create("https://api.spacexdata.com/v4/launches") as HttpWebRequest;
                //if (webRequest == null)
                //{
                //    return launchData;
                //}

                //webRequest.ContentType = "application/json";
                //webRequest.UserAgent = "Nothing";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://api.spacexdata.com/v5/launches");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                rootLaunches = JsonConvert.DeserializeObject<List<RootLaunch>>(responseBody);

                //using (var response = await webRequest.GetResponseAsync())
                //{
                //    using (var responseStream = new StreamReader(response.GetResponseStream()))
                //    {
                //        var launchesAsJson = await responseStream.ReadToEndAsync();
                //        rootLaunches = JsonConvert.DeserializeObject<List<RootLaunch>>(launchesAsJson);
                //    }
                //}









                foreach (var data in rootLaunches)
                {
                if (data.name != null)
                {
                    var launchSingle = new Launch();

                    launchSingle.Id = data.id.ToString();

                    launchSingle.RocketName = data.name.ToString();

                    //Use the original utc data for the LaunchTime
                    var dateTimeString = data.date_utc.ToString();

                    //Convert UTC to CST
                    var CST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    var launchCSTDateTime = TimeZoneInfo.ConvertTimeFromUtc(data.date_utc, CST);
                    var launchDateTime = launchCSTDateTime.ToString();
                    var dateTimeSplit = launchDateTime.Split(' ');
                    launchSingle.LaunchDate = launchCSTDateTime;
                    launchSingle.LaunchTime = dateTimeSplit[1] + " " + dateTimeSplit[2];


                    if (data.success != null)
                    {
                        if (data.success.ToString() == "true")
                        {
                            launchSingle.LaunchStatus = "Success!";
                        }
                        else if (data.success.ToString() == "false")
                        {
                            launchSingle.LaunchStatus = "Failed :(";
                        }

                    }
                    else if (data.success is null)
                    {
                        launchSingle.LaunchStatus = "Null :/";

                    }

                    var tempMass = (payloads.Where(a => a.LaunchId == launchSingle.Id).Select(a => a.Mass.Value)).ToList();
                    if (tempMass.Count() != 0)
                    {
                        launchSingle.PayloadMass = Convert.ToSingle(tempMass[0]);
                    }

                    var tempRank = (payloads.Where(a => a.LaunchId == launchSingle.Id).Select(a => a.Rank.Value)).ToList();
                    if (tempRank.Count() != 0)
                    {
                        launchSingle.PayloadRank = Convert.ToInt32(tempRank[0]);
                    }


                    launchData.Add(launchSingle);

                }
                }





                


            }
            catch(WebException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

            orderedLaunchData = launchData.OrderByDescending(x => x.LaunchDate).ToList();

            return orderedLaunchData;

        }

        public async Task<List<Payload>> GetPayloadRankAsync()
        {
           
            var rootPayloads = new List<RootPayload>();
            var payloads = new List<Payload>();

            //var webRequest = WebRequest.Create("https://api.spacexdata.com/v4/payloads") as HttpWebRequest;
            //if (webRequest == null)
            //{
            //    return payloads;
            //}

            //webRequest.ContentType = "application/json";
            //webRequest.UserAgent = "Nothing";



            try
            {

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://api.spacexdata.com/v4/payloads");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                rootPayloads = JsonConvert.DeserializeObject<List<RootPayload>>(responseBody);


                //using (var s = await webRequest.GetResponseAsync())
                //{
                //    using (var sr = new StreamReader(s.GetResponseStream()))
                //    {
                //        var payloadsAsJson = await sr.ReadToEndAsync();
                //        rootPayloads = JsonConvert.DeserializeObject<List<RootPayload>>(payloadsAsJson).ToList();

                //    }
                //}


                try
                {

                    //List<RootPayload.Rootobject> testRootPayload = new();
                    //RootPayload.Rootobject singleRoot = new();
                    //testRootPayload.Add(singleRoot);
                    //testRootPayload[0].launch = "5e29830dka897AD";
                    //testRootPayload[0].mass_kg = "3847";

                    
                    //Create a list of payload objects
                    foreach (var data in rootPayloads)
                    {
                        Payload payload = new Payload();

                        if (data.mass_kg != null)
                        {
                            payload.Mass = Convert.ToSingle(data.mass_kg);
                        }

                        if (data.launch != null && payload.Mass != null)
                        {
                            payload.LaunchId = data.launch.ToString();
                            payloads.Add(payload);
                        }
                        else if (data.launch != null && payload.Mass == null)
                        {
                            payload.Mass = 0.0f;
                            payload.LaunchId = data.launch.ToString();
                            payloads.Add(payload);
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }

                
                //Sort the Payload list by Mass and then add indexing as the ranking, accounting for duplicate masses with the same rank
                payloads.Sort((a, b) => b.Mass.Value.CompareTo(a.Mass.Value));

                var n = payloads.Count() - 1;
                
                for (int i = 0; i < n; i++)
                {
                    if (i == 0)
                    {
                        payloads[i].Rank = 1;
                    }

                    else if (i > 0 && payloads[i].Mass == payloads[i - 1].Mass)
                    {
                        payloads[i].Rank = payloads[i - 1].Rank;
                    }
                    else if (i > 0)
                    {
                        payloads[i].Rank = (payloads[i - 1].Rank) + 1;
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return payloads;
        }

    }
}

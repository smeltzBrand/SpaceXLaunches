The application was built in Visual Studio as .NET Core Web API. A Launch Controller and View were added in the MVC pattern.

I used an abstracted Service class for the 2 web requests to the API server and the injected the interface into the Launch controller constructor.

A big issue that I ran into was a current and documented bug with the SpaceXAPI that returns a status code 429 "too many requests". I wasn't sure that this was a problem with the API server side, so I changed the HttpClient to a WebRequest with the same disappointing results. I was unable to find a way to asynchronously get a response and a response stream with the webRequest instance. I had to be mindful of my time in getting the challenge complete. I also added a for loop to make 3 requests as long as a result is not returned.

Another issue was the incorrenct name on some of the recent launches, which is why a few of the latest launches on the table don't have a Payload ranking because the Id's may be missing or got switched.

The JSON strings are deserialized, using the Newtonsoft NuGet package, into objects with limited properties.

Each payload had it's own Id instead of just the mass_kg, so I also consumed the Payloads API data and then sorted the list by Payload, using indexing for rank.

The time is converted using methods from the TimeZoneInfo class.

Surprisingly, the OrderByDesc LINQ worked well for sorting the list by LaunchDate.

At times the Payload was documented as null, which I really can't imagine, but regardless I decided to still rank the null Payloads and put a zero in place of the null. However, the "0" is not displayed on the table in reference to its original null value. I made the mistake of creating a huge JSON object by "Paste as JSON" which ended up causing lots of grief by trying to figure out which values to make nullable. Limited properties for each JSON object class did the trick.

I used the DataTables CDN, a JQuery CDN, a Bootstrap CDN and small JS script. The JS disables the sorting function available, which unfortunately also disables the pagination and search features in turn.

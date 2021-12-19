using Coreopsis.Common.WebAPI.Exceptions;
using Coreopsis.Interfaces.WebApi;
using Coreopsis.WebApi;
using System;
using System.Net;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            WebProxy myProxy = new WebProxy("http://45.132.76.202:8000");

            myProxy.Credentials = new NetworkCredential("HaxXqf", "W29YBc");

            IHttpQueryFactory<string> httpQueryFactory =
                new GetQueryFactory<string>(new TestApiData("https://zkillboard.com/kill/97397405/"),
                "application/x-www-form-urlencoded",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 0, 1), myProxy);

            IHttpQuery<string> coreopsisSetTokenResponse = httpQueryFactory.Create();
            try
            {
                string coreopsisSetToken = coreopsisSetTokenResponse.SendRequest(false);
            }
            catch(Error404Exception ex)
            {
                Console.WriteLine(ex.ServerError);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}

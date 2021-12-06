using Coreopsis.Common.WebAPI.Exceptions;
using Coreopsis.Interfaces.WebApi;
using Coreopsis.WebApi;
using System;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IHttpQueryFactory<string> httpQueryFactory =
                new GetQueryFactory<string>(new TestApiData("https://zkillboard.com/kill/97165968/"),
                "application/x-www-form-urlencoded",
                "PostmanRuntime/7.26.2",
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 0, 1));

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

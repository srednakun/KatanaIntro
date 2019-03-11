using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
/// <summary>
/// App that starts up a server
/// 
/// </summary>
namespace KatanaIntro
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	class Program
	{
		static void Main(string[] args)
		{
			//turn this katana web application into a server
			string uri = "http://localhost:8080";
			using(WebApp.Start<Startup>(uri))
			{
				Console.WriteLine("Started...");
				Console.ReadKey();
				Console.WriteLine("Stopping...");
			}
		}
	}
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.Use(async (environment, next) =>
			{
				Console.WriteLine("Requesting: " + environment.Request.Path);
				await next();
				Console.WriteLine("Response: " + environment.Response.StatusCode);
			});

			ConfigureWebApi(app);
			app.UseHelloWorld();
		}

		private void ConfigureWebApi(IAppBuilder app)
		{
			var config = new HttpConfiguration();
			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

			app.UseWebApi(config);
		}
	
		public void TestingResponseRequest(IAppBuilder app)
		{
			app.Use(async (enviornment, next) =>
			{
				foreach (var pair in enviornment.Environment)
				{
					Console.WriteLine("{0}:{1}, pair.Key, pair.Value");

				}
				await next();
			});
			//app.UseWelcomePage();

			//katana will call into to process at http requests
			//app.Run(context =>
			//{
			//	//every request that comes in will respond with Hello World
			//	return context.Response.WriteAsync("Hello World");
			//});

			//app.Use<HelloWorldComponent>();
			app.UseHelloWorld();
		}
	}

public static class AppBuilderExtenstions
	{
		public static void UseHelloWorld(this IAppBuilder app)
		{
			app.Use<HelloWorldComponent>();
		}
	}

	public class HelloWorldComponent
	{
		AppFunc _next;
		public HelloWorldComponent(AppFunc next)
		{
			_next = next;
		}

		public Task Invoke(IDictionary<string, object> environment)
		{
			//await must be used with the async modifier, ex. public async Task Invoke...
			//await _next(environment);
			var response = environment["owin.ResponseBody"] as Stream;
			using (var writer = new StreamWriter(response))
			{
				return writer.WriteAsync("Hello!!");
			}
		}
	}
}
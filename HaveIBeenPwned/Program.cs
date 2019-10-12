using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HaveIBeenPwned
{
    class ConsoleOption
    {
        public ConsoleOption(int id, string text)
        {
            ID = id;
            Text = text;
        }

        public int ID { get; set; } = 0;
        public string Text { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{ID}. {Text}";
        }
    }

    class Program
    {
        static int WriteConsoleOptionsAndGetSelection(List<ConsoleOption> options)
        {
            foreach (var option in options)
                Console.WriteLine(option.ToString());

            int number;
            if (int.TryParse(Console.ReadLine(), out number))
                return number;
            return 0;
        }

        static List<ConsoleOption> LocalConsoleOptions => new List<ConsoleOption>()
            {
                new ConsoleOption(1, "Add email"),
                new ConsoleOption(2, "Is email Pwned?"),
                new ConsoleOption(3, "Close the application"),
            };

        static List<ConsoleOption> WebConsoleOptions => new List<ConsoleOption>()
            {
                new ConsoleOption(1, "Add email"),
                new ConsoleOption(2, "Is email Pwned?"),
                new ConsoleOption(3, "Close the application"),
            };

        static List<ConsoleOption> RootConsoleOptions => new List<ConsoleOption>()
            {
                new ConsoleOption(1, "Use Orleans over WebApi"),
                new ConsoleOption(2, "Use Orleans directly"),
                new ConsoleOption(3, "Close the application"),
            };

        static async Task<int> Main()
        {
            int choise = WriteConsoleOptionsAndGetSelection(RootConsoleOptions);
            switch (choise)
            {
                case 1: await DoWebApiWork();  break;
                case 2: await ConnectToSiloAsync();  break;
                default: break;
            }
            return 0;
        }

        #region WebAPI "Client"
        private static HttpWebRequest CreateWebRequest(string email, string webRequestMethod)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:56961/{email}");
            webRequest.Method = webRequestMethod;
            webRequest.ContentType = "application/json";
            webRequest.Timeout = 30000;
            webRequest.ContentLength = 0;
            return webRequest;
        }

        private static Task<string> AddEmail(string email)
        {
            return Task.FromResult(GetRespond(CreateWebRequest(email, "POST")));
        }

        private static Task<string> GetEmail(string email)
        {
            return Task.FromResult(GetRespond(CreateWebRequest(email, "GET")));
        }

        static string GetRespond(HttpWebRequest webRequest)
        {
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponseAsync().Result;
                using (Stream ResponseStream = webResponse.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(ResponseStream);
                    return sr.ReadToEnd();
                }
            }
            catch (AggregateException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
            catch (ProtocolViolationException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
            catch (ObjectDisposedException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
            catch (ArgumentNullException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
            catch (ArgumentException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
            catch (OutOfMemoryException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
            catch (IOException exc)
            {
                return $"EXCEPTION -> {exc.GetType().FullName}: {exc.Message}";
            }
        }

        private static async Task<int> DoWebApiWork()
        {
            Console.WriteLine("Welcome to application - HaveIBeenPwned using WebApi and Orleans! Choose option:");
            int selectedOption;
            do
            {
                switch (selectedOption = WriteConsoleOptionsAndGetSelection(WebConsoleOptions))
                {
                    case 1:
                        Console.Write("Enter email: ");
                        Console.WriteLine($"Respond: { await AddEmail(Console.ReadLine())}");
                        break;
                    case 2:
                        Console.Write("Enter email: ");
                        Console.WriteLine($"Respond: { await GetEmail(Console.ReadLine())}");
                        break;
                    case 3: break;
                    default: break;
                }
            } while (selectedOption != 3);
            return 0;
        }
        #endregion

        #region Orleans Client

        private static async Task<int> ConnectToSiloAsync()
        {
            try
            {
                Console.Write("Connecting to Silo. Please wait...");
                using (var client = await ConnectClientToSiloAsync())
                {
                    await DoOrleansClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo, the client is trying to connect to, is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClientToSiloAsync()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "dev";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoOrleansClientWork(IClusterClient clusterClient)
        {
            Console.WriteLine("Welcome to application - HaveIBeenPwned using Orelans! Choose option:");
            var grain = clusterClient.GetGrain<IUserEmailGrain>("nomnio.com");
            
            int selectedOption;
            do
            {
                switch (selectedOption = WriteConsoleOptionsAndGetSelection(LocalConsoleOptions))
                {
                    case 1:
                        Console.Write("Enter email: ");
                        var email = Console.ReadLine();

                        Console.Write("Enter description: ");
                        var description = Console.ReadLine();

                        var userEmail = await grain.AddEmailAddress(email, description);
                        if (userEmail.Message.Status == GrainAccessibility.Status.PWNED)
                            Console.WriteLine("Email sucessfully added");
                        else
                            Console.WriteLine(userEmail.Message.ToString());
                        break;
                    case 2:
                        Console.Write("Enter email: ");
                        email = Console.ReadLine();

                        var userEmail2 = await grain.IsEmailAddressPwned(email);
                        Console.WriteLine(userEmail2.Message.ToString());

                        break;
                    case 3: break;
                    default: break;
                }
            } while (selectedOption != 3);
        }
        #endregion

    }
}

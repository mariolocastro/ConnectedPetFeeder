using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualConnectedCiotola
{
    static class Program
    {
        private static int ciotola = 10;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var deviceId = configuration["deviceId"];
            var authenticationMethod =
                new DeviceAuthenticationWithRegistrySymmetricKey(
                    deviceId,
                    configuration["deviceKey"]
                )
            ;

            var transportType = TransportType.Mqtt;
            if (!string.IsNullOrWhiteSpace(configuration["transportType"]))
            {
                transportType = (TransportType)
                    Enum.Parse(typeof(TransportType),
                    configuration["transportType"], true);

            }

            var client = DeviceClient.Create(
                configuration["hostName"],
                authenticationMethod,
                transportType
            );
   
            while (true)
            {

                Console.WriteLine($"Ciao, vuoi aggiungere una dose?(aggiungi) Quantità attuale " + ciotola + " unità");
                var text = Console.ReadLine();

                var message = await client.ReceiveAsync();
                if (message == null)
                {
                    continue;
                }
                else
                {
                    var bytes = message.GetBytes();
                    if (bytes == null)
                    {
                        continue;
                    }
                    else
                    {
                        text = Encoding.UTF8.GetString(bytes);

                    }
                }
  
                Console.WriteLine($"Messaggio ricevuto: {text}");

                switch (text)
                {
                    case "aggiungi":
                        await Aggiungi(client);
                        break;
                    case "capture":
                        //await Aggiungi(client);
                        break;
                    default:
                        Console.WriteLine("Syntax error");
                        break;
                }
            }
        }

        private static async Task Aggiungi(DeviceClient client)
        {
            ciotola++;
            Console.WriteLine("La dose è stata incrementata di 1");

            var coll = new TwinCollection();
            coll["ciotola"] = ciotola;
            await client.UpdateReportedPropertiesAsync(coll);
        }

        private static string DesiredProperty(this Twin that, string name)
        {
            if (that == null) return string.Empty;
            if (!that.Properties.Desired.Contains(name)) return string.Empty;
            return that.Properties.Desired[name];
        }

        private static bool IsDesiredPropertyEmpty(this Twin that, string name)
        {
            return string.IsNullOrWhiteSpace(DesiredProperty(that, name));
        }
    }
}

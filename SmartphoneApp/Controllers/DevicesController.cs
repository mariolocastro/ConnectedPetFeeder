using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartphoneApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SmartphoneApp.Controllers
{
    public class DevicesController : Controller
    {

        private IConfiguration _configuration;

        public DevicesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {

            var rm = RegistryManager.CreateFromConnectionString(_configuration["IotHubConnectionString"]);
            var devicesQuery = rm.CreateQuery("SELECT DeviceId FROM devices");
            var devices = await devicesQuery.GetNextAsJsonAsync();

            var list = new List<DeviceListDto>();

            foreach (var device in devices)
            {
                var json = JsonConvert.DeserializeObject<JObject>(device);
                var dto = new DeviceListDto
                {
                    DeviceId = json.Value<string>("DeviceId")
                };
                list.Add(dto);
            }

            return View(list);
        }

        public async Task<IActionResult> Details(string id)
        {
            var dto = new DeviceListDto();
            dto.DeviceId = id;

            var rm = RegistryManager.CreateFromConnectionString(_configuration["IotHubConnectionString"]);

            return View(dto);
        }

        public async Task Capture(string id)
        {
            var rm = RegistryManager.CreateFromConnectionString(_configuration["IoTHubConnectionString"]);
            var bytes = Encoding.UTF8.GetBytes("capture");
            var message = new Message(bytes);
            var serviceClient =
                ServiceClient.CreateFromConnectionString(
                    _configuration["IoTHubConnectionString"]);
            try
            {
                await serviceClient.SendAsync(id, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Comando non valido(capture)");
            }
        }
    }
}
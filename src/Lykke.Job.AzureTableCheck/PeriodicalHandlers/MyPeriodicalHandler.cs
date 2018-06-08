using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.AzureTableCheck.Settings;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Lykke.Job.AzureTableCheck.PeriodicalHandlers
{
    public class MyPeriodicalHandler : TimerPeriod
    {
        public MyPeriodicalHandler(ILog log) :
            // TODO: Sometimes, it is enough to hardcode the period right here, but sometimes it's better to move it to the settings.
            // Choose the simplest and sufficient solution
            base(nameof(MyPeriodicalHandler), (int)TimeSpan.FromSeconds(AppSettings.CheckPeriodInSeconds).TotalMilliseconds, log)
        {
        }

        public override async Task Execute()
        {
            // TODO: Orchestrate execution flow here and delegate actual business logic implementation to services layer
            // Do not implement actual business logic here

            HttpWebRequest request = WebRequest.Create(AppSettings.SettingsApiLink) as HttpWebRequest;
            string json = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                json = reader.ReadToEnd();
                Console.WriteLine(json);
            }

            var result = JsonConvert.DeserializeObject<string>(json);
            //Console.WriteLine(result);
            var azureTableList = result.ToList();

            Console.WriteLine(result);

            foreach (var str in azureTableList)
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("Repeat after - " + AppSettings.CheckPeriodInSeconds);



            await Task.CompletedTask;
        }
    }
}

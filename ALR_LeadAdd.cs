using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ALR
{
    public static class ALR_LeadAdd
    {
        [FunctionName("ALR_LeadAdd")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<LeelooModel>((JObject.Parse(requestBody)["data"]).ToString());
            
            SendLeadEmail(data, req.Query["email"]);

            return new OkResult();
        }

        private static void SendLeadEmail(LeelooModel data, string to)
        {
            var body = $@"Name: {data.Name}
                        Name: {data.Name}
                        Phone: {data.Phone}
                        Email: {data.Email}
                        LeelooId: {data.Id}
                        LeelooLink: https://app.leeloo.ai/chats/all/{data.Id}/?personCard=true
                        Utm_source: {data.UtmMarks.UtmSource}
                        Utm_medium: {data.UtmMarks.UtmMedium}
                        Utm_campaign: {data.UtmMarks.UtmCampaign}
                        Utm_content: {data.UtmMarks.UtmContent}
                        Roistat: {data.UtmMarks.UtmContent}

                        "; //Empty string at the end is important fro amocrm parser

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("alrintegrationgv@gmail.com", "932%3A16"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            client.Send("alrintegrationgv@gmail.com", to, "New Lead", body);
        }
    }

    public class LeelooModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        
        [JsonProperty("utm_marks")]
        public UtmMarks UtmMarks { get; set; }
    }

    public class UtmMarks
    {
        [JsonProperty("utm_source")]
        public string UtmSource { get; set; }
        [JsonProperty("utm_medium")]
        public string UtmMedium { get; set; }
        [JsonProperty("utm_campaign")]
        public string UtmCampaign { get; set; }
        [JsonProperty("utm_content")]
        public string UtmContent { get; set; }
    }
}

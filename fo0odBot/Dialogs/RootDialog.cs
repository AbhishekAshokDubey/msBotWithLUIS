using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Newtonsoft.Json;

namespace fo0odBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            LuisReponse luis_response = await HelpLUIS(activity.Text);

            string bot_response = "";
            if (luis_response.intents.Length > 0 && luis_response.entities.Length > 0)
            {
                if (luis_response.intents[0].intent == "suggestHotel" && luis_response.intents[0].score >= 0.5)
                {
                    bot_response = await DoREST.GetData(luis_response.entities[0].entity, 3);
                }
                else if (luis_response.intents[0].intent == "suggestHotel" && luis_response.intents[0].score < 0.5)
                {
                    bot_response = "Assuming you asked for " + luis_response.entities[0].entity + ": " + await DoREST.GetData(luis_response.entities[0].entity, 3);
                }
                else
                {
                    bot_response = "I am not sure what you are asking, I guess, I need more training ...";
                }
            }
            else {
                bot_response = "Guess some query issue ?";
            }

            await context.PostAsync(bot_response);

            context.Wait(MessageReceivedAsync);
        }

        private static async Task<LuisReponse> HelpLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LuisReponse Data = new LuisReponse();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/b97c1f83-70e2-4f3e-a330-d46f8b0b192b?subscription-key=5e1f37b5bf8644dca0fba20da65e01d1&verbose=true&timezoneOffset=330&spellCheck=true&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LuisReponse>(JsonDataResponse);
                }
            }
            return Data;
        }
    }
}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Discord_csharp_bot
{
    public class MemeApi
    {
        public int id { get; set; }
        public string image { get; set; }
        public string caption { get; set; }
        public string category { get; set; }
    }

    internal class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    
        private Task Log(LogMessage Message)
        {
            Console.WriteLine(Message.ToString());
            return Task.CompletedTask;
        }
        
        private Task LoggedIn()
        {
            Console.WriteLine("Logged in to csharp bot!");
            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage Message)
        {
            if (Message.Author.IsBot) return Task.CompletedTask;
            if (!Message.Content.StartsWith("!")) return Task.CompletedTask;

            if (Message.Content == "!meme")
            {
                string url = Uri.EscapeUriString("https://some-random-api.ml/meme");
                string doc = "";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    // Get JSON data
                    doc = client.DownloadString(url);
                    MemeApi data = JsonConvert.DeserializeObject<MemeApi>(doc);
                    
                    // Create Embed
                    EmbedFooterBuilder Footer = new EmbedFooterBuilder { Text = $"ID: {data.id} | Category: {data.category}" };
                    EmbedBuilder Embed = new EmbedBuilder
                    {
                        Color = Color.Green,
                        Title = "MEME",
                        Description = data.caption,
                        ImageUrl = data.image,
                        Footer = Footer,
                        Timestamp = DateTimeOffset.Now
                    };
                    
                    // Send Embed
                    Message.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            
            return Task.CompletedTask;
        }
        
        public async Task MainAsync()
        {
            // Create new Socket Client
            DiscordSocketClient Client = new DiscordSocketClient();

            // Register Events
            Client.Log += Log;
            Client.MessageReceived += MessageReceived;
            Client.LoggedIn += LoggedIn;

            // Login to client
            await Client.LoginAsync(TokenType.Bot, "Your Bot Token");
            await Client.StartAsync();

            // Loop Async Task
            await Task.Delay(-1);
        }
    }
}
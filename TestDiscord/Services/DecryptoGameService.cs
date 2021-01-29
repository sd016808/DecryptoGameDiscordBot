using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestDiscord.Services
{
    public class DecryptoGameService
    {
        public IConfigurationRoot Configuration { get; set; }
        public Random Random { get; set; }
        public DecryptoGameService()
        {
            var builder = new ConfigurationBuilder()        // Create a new instance of the config builder
                .SetBasePath(AppContext.BaseDirectory)      // Specify the default location for the config file
                .AddJsonFile("_descrypto.json");                // Add this (json encoded) file to the configuration
                Configuration = builder.Build();                // Build the configuration
        }

        public IEnumerable<string> GetRandomWordList(int wordCount)
        {
            var wordList = Configuration.GetSection("wordList").Get<List<string>>();

            if (wordList.Count < wordCount)
                return new List<string>();

            var randomWordLsit = Randomize(wordList).Take(wordCount).ToList();
            for(int i=0; i< randomWordLsit.Count(); i++)
            {
                randomWordLsit[i] = $"{i+1}: {randomWordLsit[i]}";
            }
            return randomWordLsit;
        }

        public IEnumerable<string> GetRandomNumberList(int numberCount)
        {
            List<string> randomNumberList = new List<string>() { "1", "2", "3", "4" };
            return Randomize(randomNumberList).Take(numberCount);
        }

        public static IEnumerable<t> Randomize<t>(IEnumerable<t> target)
        {
            Random r = new Random();
            return target.OrderBy(x => (r.Next()));
        }
    }
}

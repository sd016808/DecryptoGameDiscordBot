using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestDiscord.Services
{
    public class DecryptoGameService
    {
        private object _lock = new object();
        private readonly string _dbPath = "_descrypto.json";
        public IConfigurationRoot Configuration { get; set; }
        public Random Random { get; set; }
        public DecryptoGameService()
        {
            var builder = new ConfigurationBuilder()        // Create a new instance of the config builder
                            .SetBasePath(AppContext.BaseDirectory)      // Specify the default location for the config file
                            .AddJsonFile(_dbPath);                // Add this (json encoded) file to the configuration
            Configuration = builder.Build();                // Build the configuration
        }

        public IEnumerable<string> GetRandomWordList(int wordCount)
        {
            List<string> wordList = new List<string>();
            lock (_lock)
            {
                wordList = Configuration.GetSection("wordList").Get<List<string>>();
            }
            if (wordList.Count < wordCount)
                return wordList;

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

        public List<string> GetWordList()
        {
            lock (_lock)
            {
                return Configuration.GetSection("wordList").Get<List<string>>();
            }
        }

        public bool AddNewWords(List<string> newWords)
        {
            try
            {
                lock (_lock)
                {
                    var json = File.ReadAllText(_dbPath);
                    DecryptoGameDatabase db = JsonConvert.DeserializeObject<DecryptoGameDatabase>(json);
                    foreach (var word in newWords)
                    {
                        if (!db.wordList.Contains(word))
                            db.wordList.Add(word);
                    }

                    json = JsonConvert.SerializeObject(db);
                    File.WriteAllText(_dbPath, json);

                    var builder = new ConfigurationBuilder()        // Create a new instance of the config builder
                                    .SetBasePath(AppContext.BaseDirectory)      // Specify the default location for the config file
                                    .AddJsonFile("_descrypto.json");                // Add this (json encoded) file to the configuration
                    Configuration = builder.Build();                // Build the configuration
                }
                return true;
            }
            catch
            {
                // 之後再研究如何結合LOG
                return false;
            }
        }
    }

    public class DecryptoGameDatabase
    {
        public List<string> wordList { get; set; }
    }
}

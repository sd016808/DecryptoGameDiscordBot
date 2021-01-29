using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestDiscord
{
    class Program
    {
        public static Task Main(string[] args)
        { 
            return Startup.RunAsync(args);
        }
    }
}

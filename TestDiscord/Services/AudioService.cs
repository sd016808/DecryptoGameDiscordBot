using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TestDiscord.Modules
{
    [Name("YoutubePlayer")]
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string song)
        {
            //IAudioClient client;
            //if (!ConnectedChannels.TryGetValue(guild.Id, out client))
            //{
            //    await channel.SendMessageAsync("請先將機器人加入到語音頻道");
            //    return;
            //}

            ////Uri uri = new Uri(@"https://www.youtube.com/");
            ////HttpClientHandler handler = new HttpClientHandler();
            ////handler.CookieContainer = new CookieContainer();
            ////handler.UseCookies = true;
            ////handler.CookieContainer.Add(uri, new Cookie("name", "value")); // Adding a Cookie
            ////HttpClient httpClient = new HttpClient(handler);
            ////HttpResponseMessage response = await httpClient.GetAsync(uri);
            ////CookieCollection collection = handler.CookieContainer.GetCookies(uri); // Retrieving a Cookie

            //YoutubeClient youtube = new YoutubeClient();
            //var search = await youtube.Search.GetVideosAsync(song, 0 , 1);

            //if (search.Count <= 0)
            //{
            //    await channel.SendMessageAsync("找不到任何歌曲");
            //    return;
            //}

            //var streamManifest = await youtube.Videos.Streams.GetManifestAsync(search[0].Id);
            //var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();
            //var stream = youtube.Videos.Streams.GetAsync(streamInfo).Result;
            //var memoryStream = new MemoryStream();
            //await Cli.Wrap("ffmpeg")
            //    .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
            //    .WithStandardInputPipe(PipeSource.FromStream(stream))
            //    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
            //    .ExecuteAsync();

            //using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            //{
            //    try { await discord.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length); }
            //    finally { await discord.FlushAsync(); }
            //}

            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(song))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                using (var ffmpeg = CreateProcess(song))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}

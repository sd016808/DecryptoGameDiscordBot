using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestDiscord.Services;

namespace TestDiscord.Modules
{
    [Name("DecryptoGame")]
    public class DecryptoGameModel:ModuleBase
    {
        public DecryptoGameService DecryptoGameService { get; set; }

        [Command("getWordList")]
        [Alias("get", "g")]
        public Task GetRandomWordList()
        {
            var wordList = DecryptoGameService.GetRandomWordList(4); // default get 4 word
            return ReplyAsync(string.Join(" ", wordList));
        }

        [Command("randomNumberList")]
        [Alias("n")]
        public Task GetRandomList()
        {
            var numberList = DecryptoGameService.GetRandomNumberList(3); // default get 4 word
            return Context.User.SendMessageAsync(string.Join(" ", numberList));
        }

        // 'params' will parse space-separated elements into a list
        [Command("addWordList")]
        [Alias("add", "a")]
        public Task AddNewWords(params string[] objects)
        {
            var result = DecryptoGameService.AddNewWords(new List<string>(objects));
            if(result)
                return ReplyAsync("Add success!");
            else
                return ReplyAsync("Add failed!");
        }

        // 'params' will parse space-separated elements into a list
        [Command("!wordList")]
        [Alias("list", "l")]
        public Task GetWordList()
        {
            var wordList = DecryptoGameService.GetWordList();
            return ReplyAsync(string.Join(Environment.NewLine, wordList));
        }

        [Command("clear", RunMode = RunMode.Async)]
        [Alias("c")]
        [Summary("Deletes the specified amount of messages.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeChat()
        {
            var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            foreach (IUserMessage message in messages)
            {
                try
                {
                    await message.DeleteAsync(); //rate limit exception if too many messages!!!
                }
                catch
                {
                    continue;
                }
            }
            const int delay = 1000;
            var m = await this.ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        [Command("help", RunMode = RunMode.Async)]
        [Alias("h")]
        public Task Help()
        {
            string helpString = "!get: 取得題庫" + Environment.NewLine;
            helpString += "!n: 取得數字卡" + Environment.NewLine;
            helpString += "!add: 增加題庫" + Environment.NewLine;
            helpString += "!list: 取得所有題庫" + Environment.NewLine;
            helpString += "!clear: 清空頻道訊息" + Environment.NewLine;
            helpString += "!help: 幫助清單" + Environment.NewLine;

            return ReplyAsync(helpString);
        }
    }
}

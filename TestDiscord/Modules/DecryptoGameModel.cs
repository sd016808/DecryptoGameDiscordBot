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

        [Command("!wordList")]
        [Alias("!w", "!word")]
        public Task GetWordList()
        {
            var wordList = DecryptoGameService.GetRandomWordList(4); // default get 4 word
            return ReplyAsync(string.Join(" ", wordList));
        }

        [Command("!randomNumberList")]
        [Alias("!r", "!rn")]
        public Task GetRandomList()
        {
            var wordList = DecryptoGameService.GetRandomNumberList(3); // default get 4 word
            return Context.User.SendMessageAsync(string.Join(" ", wordList));
        }

        // 'params' will parse space-separated elements into a list
        [Command("!list")]
        public Task ListAsync(params string[] objects)
        { 
            return ReplyAsync("You listed: " + string.Join("; ", objects));
        }

        [Command("!clear", RunMode = RunMode.Async)]
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

        [Command("!help", RunMode = RunMode.Async)]
        [Alias("!h")]
        public Task Help()
        {
            string helpString = "!word: 取得題庫" + Environment.NewLine;
            helpString += "!rn: 取得數字卡" + Environment.NewLine;
            helpString += "!clear: 清空頻道訊息" + Environment.NewLine;
            helpString += "!h: 幫助清單" + Environment.NewLine;

            return ReplyAsync(helpString);
        }
    }
}

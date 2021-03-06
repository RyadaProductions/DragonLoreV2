﻿using Discord.Commands;
using DragonLore.Managers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private readonly IBotMessageManager _botMessage;

        public FunModule(IBotMessageManager botMessage)
        {
            _botMessage = botMessage;
        }

        [Command("Fail", RunMode = RunMode.Async)]
        [Summary("Show a random csgo fail gif")]
        public async Task Fail()
        {
            string filePath = "";
            var randomNumber = new Random();
            switch (randomNumber.Next(3))
            {
                case 0:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "FeFailNade.gif");
                    break;

                case 1:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "KillTheDefuser.gif");
                    break;

                case 2:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "NadeTheAFK.gif");
                    break;

                case 3:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "DuckToDodge.gif");
                    break;
            }
            await Context.Channel.SendFileAsync(filePath);
            await _botMessage.RemoveCommandMessageAsync(Context.Message, Context.Channel);
        }
    }
}
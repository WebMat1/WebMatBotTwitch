using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebMatBotV3.Server.Hubs
{
    public class SubtitleHub : Hub
    {
        public async Task ToTranslate(string text,string user)
        {

            //subtitle está ativo
            if (WebMatBot.Subtitle.IsActive)
            {
                //traduz
                var textTranslated = await WebMatBot.Translate.TranslateCore(text, false, WebMatBot.Subtitle.TargetLanguage, user);

                //envia para os clientes
                await Clients.All.SendAsync("receiveTranslated", textTranslated);
            }
        }
    }
}

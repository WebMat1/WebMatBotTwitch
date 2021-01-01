using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMatBotV3.Shared
{
    public class UserTitle
    {

        public List<Permissions> Permissions { get; set; }


        public UserTitle(string badges) {

            //decodificar a string da twitch

            // @badge-info=subscriber/1;badges=subscriber/0,premium/1,moderator/1,bits/1;client-nonce=3c7925c70412b0b55144227f60764baf;color=;display-name=allexx3x;emotes=;flags=;id=61742c2b-5ea1-4d40-9b35-c971b12c8812;mod=0;room-id=45168403;subscriber=1;tmi-sent-ts=1606745430489;turbo=0;user-id=55290495;user-type= :allexx3x!allexx3x@allexx3x.tmi.twitch.tv PRIVMSG #webmat1 :dotnet -version

            #region Pegando string Badges
            string[] parameters = badges.Split(";");
            string userBadge = null;

            for (int i = 0; i < parameters.Length && userBadge == null; i++)
                userBadge = (parameters[i].StartsWith("badges=") ? parameters[i] : null);

            if (userBadge == null)
                throw new Exception("Informações de usuário não carregadas");

            #endregion

            Permissions = new List<WebMatBotV3.Shared.Permissions>() { WebMatBotV3.Shared.Permissions.Viewer};

            #region Populando instancia

            if (userBadge.Contains("subscriber") || userBadge.Contains("founder"))
                Permissions.Add(WebMatBotV3.Shared.Permissions.Subscriber);

            if (userBadge.Contains("moderator"))
                Permissions.Add(WebMatBotV3.Shared.Permissions.Moderator);


            if (userBadge.Contains("bits"))
                Permissions.Add(WebMatBotV3.Shared.Permissions.Bits);

            if (userBadge.Contains("vip"))
                Permissions.Add(WebMatBotV3.Shared.Permissions.VIP);

            if (userBadge.Contains("broadcaster"))
                Permissions.Add(WebMatBotV3.Shared.Permissions.Broadcaster);
            #endregion

        }
    }
}

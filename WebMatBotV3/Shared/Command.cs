using System;
using System.Collections.Generic;
using System.Text;

namespace WebMatBotV3.Shared
{
    public class Command
    {
        public string Key { get; set; }
        public Action<string, string, UserTitle> Action { get; set; }
        public Permissions[] Permissions { get; set; }
        public string Description { get; set; }
        public Command(string _Key, Action<string, string, UserTitle> _Action, string _Description,params Permissions[] _permissions )
        {
            Key = _Key;
            Action = _Action;
            Description = _Description;
            Permissions = _permissions == null || _permissions.Length == 0 ? new Permissions[1] { Shared.Permissions.Viewer} : _permissions;
        }
        
    }
    public enum Permissions
    {
        Viewer,
        Subscriber,
        Bits,
        Moderator,
        VIP,
        Broadcaster,
    }
}

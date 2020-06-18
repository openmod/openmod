using Rocket.Core.Assets;
using System.Xml.Serialization;
using System;
using Rocket.API;
using System.Collections.Generic;

namespace Rocket.Core.Serialization
{
    public enum CommandPriority { Low = -1, Normal = 0, High = 1 };

    public sealed class RemoteConsole
    {
        [XmlAttribute]
        public bool Enabled = false;
        [XmlAttribute]
        public ushort Port = 27115;
        [XmlAttribute]
        public string Password = "changeme";
        [XmlAttribute]
        public bool EnableMaxGlobalConnections = true;
        [XmlAttribute]
        public ushort MaxGlobalConnections = 10;
        [XmlAttribute]
        public bool EnableMaxLocalConnections = true;
        [XmlAttribute]
        public ushort MaxLocalConnections = 3;
    }

    public sealed class AutomaticShutdown
    {
        [XmlAttribute]
        public bool Enabled = false;
        [XmlAttribute]
        public int Interval = 86400;
    }

    public sealed class WebPermissions
    {
        [XmlAttribute]
        public bool Enabled = false;
        [XmlAttribute]
        public string Url = "";
        [XmlAttribute]
        public int Interval = 180;
    }

    public sealed class WebConfigurations
    {
        [XmlAttribute]
        public bool Enabled = false;
        [XmlAttribute]  
        public string Url = "";
    }

    public sealed class CommandMapping
    {
        [XmlAttribute]
        public string Name = "";

        [XmlAttribute]
        public bool Enabled = true;

        [XmlAttribute]
        public CommandPriority Priority = CommandPriority.Normal;

        [XmlText]
        public string Class = "";
        public CommandMapping()
        {

        }

        public CommandMapping(string name,string @class, bool enabled = true, CommandPriority priority = CommandPriority.Normal)
        {
            Name = name;
            Enabled = enabled;
            Class = @class;
            Priority = priority;
        }
    }

    public sealed class RocketSettings : IDefaultable
    {
        [XmlElement("RCON")]
        public RemoteConsole RCON = new RemoteConsole();

        [XmlElement("AutomaticShutdown")]
        public AutomaticShutdown AutomaticShutdown = new AutomaticShutdown();

        [XmlElement("WebConfigurations")]
        public WebConfigurations WebConfigurations = new WebConfigurations();

        [XmlElement("WebPermissions")]
        public WebPermissions WebPermissions = new WebPermissions();

        [XmlElement("LanguageCode")]
        public string LanguageCode = "en";

        [XmlElement("MaxFrames")]
        public int MaxFrames = 60;
        
        public void LoadDefaults()
        {
            RCON = new RemoteConsole();
            AutomaticShutdown = new AutomaticShutdown();
            WebConfigurations = new WebConfigurations();
            WebPermissions = new WebPermissions();
            LanguageCode = "en";
            MaxFrames = 60;
        }
    }
}
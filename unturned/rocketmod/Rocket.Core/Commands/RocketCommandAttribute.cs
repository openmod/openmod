using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RocketCommandPermissionAttribute : System.Attribute
    {
        public RocketCommandPermissionAttribute(string Name)
        {
            _name = Name;
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _name;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RocketCommandAliasAttribute : System.Attribute
    {
        public RocketCommandAliasAttribute(string Name)
        {
            _name = Name;
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _name;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RocketCommandAttribute : System.Attribute
    {
        readonly string name;
        readonly string help;
        readonly string syntax;
        readonly AllowedCaller allowedCaller;

        public RocketCommandAttribute(string Name,string Help,string Syntax = "", AllowedCaller AllowedCaller = AllowedCaller.Both)
        {
            name = Name;
            help = Help;
            syntax = Syntax;
            allowedCaller = AllowedCaller;
        }

        public string Name { get { return name; } }
        public string Help { get { return help; } }
        public string Syntax { get { return syntax; } }
        public AllowedCaller AllowedCaller { get { return allowedCaller; } }
    }
}

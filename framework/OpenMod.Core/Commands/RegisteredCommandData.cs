﻿using System;
using System.Collections.Generic;
using OpenMod.API.Prioritization;
using VYaml.Annotations;

namespace OpenMod.Core.Commands
{
    [Serializable, YamlObject]
    public sealed partial class RegisteredCommandData
    {
        public string? Id { get; set; } = null!;

        public string? ParentId { get; set; }

        public string? Name { get; set; } = null!;

        public List<string>? Aliases { get; set; }

        public bool? Enabled { get; set; }

        public Dictionary<string, object?>? Data { get; set; }

        public Priority? Priority { get; set; }
    }
}
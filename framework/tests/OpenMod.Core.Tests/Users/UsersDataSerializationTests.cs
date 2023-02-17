using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq.Extensions;
using OpenMod.Core.Persistence.Yaml;
using OpenMod.Core.Users;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.NodeTypeResolvers;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace OpenMod.Core.Tests.Users;

[TestClass]
public class UsersDataSerializationTests
{
    private ISerializer m_DefaultSerializer;
    private ISerializer m_CustomSerializer;

    private UsersData m_UsersData;

    private IDeserializer m_DefaultDeserializer;
    private IDeserializer m_CustomDeserializer;

    [TestInitialize]
    public void Init()
    {
        m_UsersData = new()
        {
            Users = new()
            {
                new()
                {
                    Id = "7656111",
                    Type = KnownActorTypes.Player,
                    LastDisplayName = "Diffoz",
                    FirstSeen = DateTime.Now.Add(TimeSpan.FromDays(-3)),
                    LastSeen = DateTime.Now,
                    BanInfo = new("breaking rules", null, DateTime.Now.AddDays(15)),
                    Roles = new()
                    {
                        "vip",
                        "boss"
                    }
                },
                new()
                {
                    Id = "4512121",
                    Type = KnownActorTypes.Player,
                    LastDisplayName = "Trojaner",
                    FirstSeen = DateTime.Now.Add(TimeSpan.FromDays(-5)),
                    LastSeen = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    Permissions = new()
                    {
                        "SomePlugin:commands.test",
                        "AnotherPlugin:vaults.big"
                    },
                    Data = new()
                    {
                        { "data", 15 },
                        { "CamelData", -99 },
                    }
                },
                new()
                {
                    Id = "437774",
                    Type = KnownActorTypes.Player,
                    LastDisplayName = "$%^@^*/'",
                    FirstSeen = DateTime.Now.Add(TimeSpan.FromDays(-50)),
                    LastSeen = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    Permissions = new()
                    {
                        "SomePlugin:commands.test",
                        "AnotherPlugin:vaults.big"
                    },
                    BanInfo = new()
                    {
                        ExpireDate = DateTime.Now,
                        InstigatorId = "4512121",
                        InstigatorType = KnownActorTypes.Console,
                        Reason = "Bad"
                    },
                    Data = new()
                    {
                        { "data", 15 },
                        { "CamelData", -99 },
                    }
                },
                new()
                {
                    Id = null,
                    Type = default,
                    LastDisplayName = default,
                    FirstSeen = null,
                    LastSeen = default,
                    Permissions = null,
                    BanInfo = new(),
                    Data = null
                },
            }
        };

        m_DefaultSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .DisableAliases()
            .WithoutEmissionPhaseObjectGraphVisitor<CustomSerializationObjectGraphVisitor>()
            .Build();

        m_CustomSerializer = Serializer.FromValueSerializer(
            new ValueSerializerEx(CamelCaseNamingConvention.Instance, false, false),
            new());

        m_DefaultDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .WithoutNodeDeserializer<YamlConvertibleNodeDeserializer>()
            .Build();

        m_CustomDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    [TestMethod]
    public void Serialize_ShouldEquals()
    {
        var s1 = m_DefaultSerializer.Serialize(m_UsersData);
        var s2 = m_CustomSerializer.Serialize(m_UsersData);

        Assert.AreEqual(s1, s2);
    }

    [TestMethod]
    public void Deserialize_ShouldEquals()
    {
        const string value = @"users:
- id: 7656111
  type: player
  lastDisplayName: Diffoz
  firstSeen: 2023-02-15T05:32:59.6446564+07:00
  lastSeen: 2023-02-18T05:32:59.6447328+07:00
  banInfo:
    expireDate: 2023-03-05T05:32:59.6447732+07:00
    instigatorType: 
    instigatorId: 
    reason: breaking rules
  permissions: []
  roles:
  - vip
  - boss
  data: {}
- id: 4512121
  type: player
  lastDisplayName: Trojaner
  firstSeen: 2023-02-13T05:32:59.6451535+07:00
  lastSeen: 2023-02-15T05:32:59.6451545+07:00
  banInfo: 
  permissions:
  - SomePlugin:commands.test
  - AnotherPlugin:vaults.big
  roles: []
  data:
    data: 15
    CamelData: -99
- id: 437774
  type: player
  lastDisplayName: $%^@^*/'
  firstSeen: 2022-12-30T05:32:59.6451616+07:00
  lastSeen: 2023-02-15T05:32:59.6451617+07:00
  banInfo:
    expireDate: 2023-02-18T05:32:59.6451989+07:00
    instigatorType: console
    instigatorId: 4512121
    reason: Bad
  permissions:
  - SomePlugin:commands.test
  - AnotherPlugin:vaults.big
  roles: []
  data:
    data: 15
    CamelData: -99
- id: 
  type: 
  lastDisplayName: 
  firstSeen: 
  lastSeen: 
  banInfo:
    expireDate: 
    instigatorType: 
    instigatorId: 
    reason: 
  permissions: 
  roles: []
  data: 
";

        var s1 = m_DefaultDeserializer.Deserialize<UsersData>(value);
        var s2 = m_CustomDeserializer.Deserialize<UsersData>(value);

        Assert.IsNotNull(s1);
        Assert.IsNotNull(s2);

        Assert.IsNotNull(s1.Users);
        Assert.IsNotNull(s2.Users);

        Assert.IsTrue(s1.Users.Count == s2.Users.Count);
        for (var i = 0; i < s1.Users.Count; i++)
        {
            var data1 = s1.Users[i];
            var data2 = s2.Users[i];

            Assert.AreEqual(data1.Id, data2.Id);
            Assert.AreEqual(data1.Type, data2.Type);
            Assert.AreEqual(data1.LastDisplayName, data2.LastDisplayName);
            Assert.AreEqual(data1.FirstSeen, data2.FirstSeen);
            Assert.AreEqual(data1.LastSeen, data2.LastSeen);
            CollectionAssert.AreEqual(data1.Permissions?.ToArray(), data2.Permissions?.ToArray());
            CollectionAssert.AreEqual(data1.Roles?.ToArray(), data2.Roles?.ToArray());

            Assert.AreEqual(data1.BanInfo?.ExpireDate, data2.BanInfo?.ExpireDate);
            Assert.AreEqual(data1.BanInfo?.InstigatorType, data2.BanInfo?.InstigatorType);
            Assert.AreEqual(data1.BanInfo?.InstigatorId, data2.BanInfo?.InstigatorId);
            Assert.AreEqual(data1.BanInfo?.Reason, data2.BanInfo?.Reason);

            CollectionAssert.AreEqual(data1.Data, data2.Data);
        }
    }
}

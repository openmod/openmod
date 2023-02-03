using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;
using OpenMod.Core.Persistence.Yaml;
using OpenMod.Core.Users;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace OpenMod.Core.Benchmark.Yaml;

[SimpleJob]
[MemoryDiagnoser(displayGenColumns: false)]
public class UsersDataSerialization
{
    [Params(10000, 50000)]
    public int Count { get; set; }

    private UsersData m_UsersData = null!;

    private ISerializer m_DefaultSerializer = null!;
    private ISerializer m_CustomSerializer = null!;

    [GlobalSetup]
    public void Init()
    {
        var rng = new Random(1337);

        m_UsersData = new()
        {
            Users = new(Count)
        };

        Span<byte> buf = stackalloc byte[16];
        for (var i = 0; i < Count; i++)
        {
            rng.NextBytes(buf);

            m_UsersData.Users.Add(new()
            {
                Id = new Guid(buf).ToString(),
                Type = KnownActorTypes.Player,
                LastDisplayName = rng.Next().ToString(),
                FirstSeen = DateTime.Now,
                LastSeen = DateTime.Now
            });
        }

        m_DefaultSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .DisableAliases()
            .WithoutEmissionPhaseObjectGraphVisitor<CustomSerializationObjectGraphVisitor>()
            .Build();

        m_CustomSerializer = Serializer.FromValueSerializer(
            new ValueSerializerEx(CamelCaseNamingConvention.Instance, false, false),
            new());
    }

    [Benchmark(Baseline = true)]
    public string YamlDotNetSerialize()
    {
        using var output = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
        m_DefaultSerializer.Serialize(output, m_UsersData);

        return output.ToString();
    }

    [Benchmark]
    public string YamlDotNetConvertibleSerialize()
    {
        using var output = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
        m_CustomSerializer.Serialize(new EmitterEx(output), m_UsersData);

        return output.ToString();
    }
}

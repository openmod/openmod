using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoreLinq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using ITypeResolver = YamlDotNet.Serialization.ITypeResolver;

[assembly: InternalsVisibleTo("OpenMod.Core.Benchmark")]

namespace OpenMod.Core.Persistence.Yaml
{
    internal sealed class ValueSerializerEx : IValueSerializer
    {
        private static readonly ITypeResolver s_TypeResolver = new DynamicTypeResolver();

        private readonly IObjectGraphTraversalStrategy m_TraversalStrategy;
        private readonly IEventEmitter m_EventEmitter;
        private readonly IYamlTypeConverter[] m_TypeConverters;
        private readonly Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>[] m_EmissionPhaseObjectGraphVisitorFactory;
        private readonly IObjectGraphVisitor<IEmitter> m_InnerObjectGraphVisitor;

        public ValueSerializerEx(INamingConvention namingConvention, bool ignoreFields, bool includeNonPublicProperties)
        {
            var typeInspector = BuildTypeInspector(namingConvention, ignoreFields, includeNonPublicProperties);
            m_TypeConverters = BuildTypeConverters();
            m_EventEmitter = BuildEventEmitter();
            m_EmissionPhaseObjectGraphVisitorFactory = BuildEmissionPhaseObjectGraphVisitorFactory();
            m_TraversalStrategy = new FullObjectGraphTraversalStrategy(typeInspector, s_TypeResolver, 50, namingConvention);
            m_InnerObjectGraphVisitor = new EmittingObjectGraphVisitor(m_EventEmitter);
        }

        private ITypeInspector BuildTypeInspector(INamingConvention namingConvention, bool ignoreFields, bool includeNonPublicProperties)
        {
            ITypeInspector innerInspector = new ReadablePropertiesTypeInspector(s_TypeResolver, includeNonPublicProperties);
            if (!ignoreFields)
            {
                innerInspector = new CompositeTypeInspector(new ReadableFieldsTypeInspector(s_TypeResolver), innerInspector);
            }

            // chain (reversed)
            return new Func<ITypeInspector, ITypeInspector>[]
            {
                (inner) => new YamlAttributeOverridesInspector(inner, new()),
                (inner) => new YamlAttributesTypeInspector(inner),
                (inner) => namingConvention is NullNamingConvention ? inner : new NamingConventionTypeInspector(inner, namingConvention),
                (inner) => new CachedTypeInspector(inner),
            }.Aggregate(innerInspector, (inner, factory) => factory(inner));
        }

        private IYamlTypeConverter[] BuildTypeConverters()
        {
            return new IYamlTypeConverter[]
            {
                new GuidConverter(jsonCompatible: false),
                new SystemTypeConverter(),
            };
        }

        private IEventEmitter BuildEventEmitter()
        {
            IEventEmitter innerEmitter = new WriterEventEmitter();
            return new Func<IEventEmitter, IEventEmitter>[]
            {
                (inner) => new TypeAssigningEventEmitter(inner, false, new Dictionary<Type, TagName>()),
            }.Aggregate(innerEmitter, (inner, factory) => factory(inner));
        }

        private Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>[] BuildEmissionPhaseObjectGraphVisitorFactory()
        {
            return new Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>[]
            {
                (args) => new CommentsObjectGraphVisitor(args.innerVisitor),
                (args) => new DefaultValuesObjectGraphVisitor(DefaultValuesHandling.Preserve, args.innerVisitor),
                (args) => new CustomSerializationObjectGraphVisitor(args.innerVisitor, args.typeConverters, args.objectSerializer),
            };
        }

        public void SerializeValue(IEmitter emitter, object? value, Type? type)
        {
            var type2 = type ?? ((value != null) ? value.GetType() : typeof(object));
            var staticType = type ?? typeof(object);
            var graph = new ObjectDescriptor(value, type2, staticType);

            void NestedObjectSerializer(object? v, Type? t)
            {
                SerializeValue(emitter, v, t);
            }
            ObjectSerializer objectSerializer = NestedObjectSerializer;

            var visitor = m_EmissionPhaseObjectGraphVisitorFactory
                .Aggregate(m_InnerObjectGraphVisitor, (inner, factory)
                    => factory(new EmissionPhaseObjectGraphVisitorArgs(inner, m_TypeConverters, objectSerializer)));

            m_TraversalStrategy.Traverse(graph, visitor, emitter);
        }

        private readonly struct EmissionPhaseObjectGraphVisitorArgs
        {
            public readonly IObjectGraphVisitor<IEmitter> innerVisitor;
            public readonly IYamlTypeConverter[] typeConverters;
            public readonly ObjectSerializer objectSerializer;

            public EmissionPhaseObjectGraphVisitorArgs(IObjectGraphVisitor<IEmitter> innerVisitor,
                IYamlTypeConverter[] typeConverters, ObjectSerializer objectSerializer)
            {
                this.innerVisitor = innerVisitor;
                this.typeConverters = typeConverters;
                this.objectSerializer = objectSerializer;
            }
        }
    }
}

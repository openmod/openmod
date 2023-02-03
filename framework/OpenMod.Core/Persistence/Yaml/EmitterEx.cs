using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace OpenMod.Core.Persistence.Yaml
{
    /// <summary>
    /// Emits YAML streams.
    /// Ex: Caches scalar data for optimization
    /// </summary>
    internal sealed class EmitterEx : IEmitter
    {
        private delegate void AnalyzeScalar(Emitter emitter, Scalar scalar);
        private delegate void AnalyzeTag(Emitter emitter, TagName tagName);
        private delegate void StateMachine(Emitter emitter, ParsingEvent parsingEvent);
        private delegate bool NeedsMoreEvents(Emitter emitter);

        private static readonly ConcurrentDictionary<string, ScalarData> s_CachedScalarDatas;

        private static readonly AccessTools.FieldRef<object, object> s_ScalarDataFieldRef;
        private static readonly AccessTools.FieldRef<object, bool>[] s_ScalarDataBooleanFieldRefs;
        private static readonly AccessTools.FieldRef<object, string> s_ScalarDataValueFieldRef;

        private static readonly AccessTools.FieldRef<object, object> s_TagDataFieldRef;
        private static readonly AccessTools.FieldRef<object, string>[] s_TagDataStringFieldRefs;

        private static readonly AccessTools.FieldRef<object, Queue<ParsingEvent>> s_EventsFieldRef;

        private static readonly StateMachine s_StateMachineMethod;
        private static readonly AnalyzeTag s_AnalyzeTagMethod;
        private static readonly NeedsMoreEvents s_NeedsMoreEventsMethod;
        private static readonly AnalyzeScalar s_AnalyzeScalarMethod;

        static EmitterEx()
        {
            s_CachedScalarDatas = new();

            var emitterType = typeof(Emitter);
            const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;

            var scalarDataField = emitterType.GetField("scalarData", bindingAttr);
            s_ScalarDataFieldRef = AccessTools.FieldRefAccess<object>(emitterType, scalarDataField.Name);

            var fields = scalarDataField.FieldType.GetFields();
            s_ScalarDataBooleanFieldRefs = fields
                .Where(x => x.FieldType == typeof(bool))
                .Select(x => AccessTools.FieldRefAccess<bool>(scalarDataField.FieldType, x.Name))
                .ToArray();

            s_ScalarDataValueFieldRef
                = AccessTools.FieldRefAccess<string>(scalarDataField.FieldType, fields.Single(x => x.FieldType == typeof(string)).Name);

            var tagDataField = emitterType.GetField("tagData", bindingAttr);
            s_TagDataFieldRef = AccessTools.FieldRefAccess<object>(emitterType, tagDataField.Name);

            s_TagDataStringFieldRefs = tagDataField.FieldType.GetFields()
                .Select(x => AccessTools.FieldRefAccess<string>(tagDataField.FieldType, x.Name))
                .ToArray();

            s_EventsFieldRef = AccessTools.FieldRefAccess<Queue<ParsingEvent>>(emitterType, "events");

            s_StateMachineMethod = AccessTools.MethodDelegate<StateMachine>(
            emitterType.GetMethod("StateMachine", bindingAttr), null, false);

            s_AnalyzeTagMethod = AccessTools.MethodDelegate<AnalyzeTag>(
            emitterType.GetMethod("AnalyzeTag", bindingAttr), null, false);

            s_NeedsMoreEventsMethod = AccessTools.MethodDelegate<NeedsMoreEvents>(
            emitterType.GetMethod(name: "NeedMoreEvents", bindingAttr), null, false);

            s_AnalyzeScalarMethod = AccessTools.MethodDelegate<AnalyzeScalar>(
            emitterType.GetMethod(name: "AnalyzeScalar", bindingAttr), null, false);
        }

        private readonly Emitter m_Emitter;
        private readonly Queue<ParsingEvent> m_Events;
        private readonly object m_TagDataValue;
        private readonly object m_ScalarDataValue;

        public EmitterEx(TextWriter output)
        {
            m_Emitter = new Emitter(output);

            m_ScalarDataValue = s_ScalarDataFieldRef(m_Emitter);
            m_TagDataValue = s_TagDataFieldRef(m_Emitter);
            m_Events = s_EventsFieldRef(m_Emitter);
        }

        public void Emit(ParsingEvent @event)
        {
            m_Events.Enqueue(@event);

            while (!s_NeedsMoreEventsMethod(m_Emitter))
            {
                var current = m_Events.Peek();
                try
                {
                    ClearTagData();

                    if (current is NodeEvent nodeEvent)
                    {
                        if (current is Scalar scalar)
                        {
                            AnalyzeScalarEx(scalar);
                        }

                        if (!nodeEvent.Tag.IsEmpty && nodeEvent.IsCanonical)
                        {
                            s_AnalyzeTagMethod(m_Emitter, nodeEvent.Tag);
                        }
                    }

                    s_StateMachineMethod(m_Emitter, current);
                }
                finally
                {
                    m_Events.Dequeue();
                }
            }
        }

        private void ClearTagData()
        {
            foreach (var field in s_TagDataStringFieldRefs)
            {
                ref var data = ref field(m_TagDataValue);
                data = null;
            }
        }

        private void AnalyzeScalarEx(Scalar scalar)
        {
            if (scalar.Value.Length == 0)
            {
                s_AnalyzeScalarMethod(m_Emitter, scalar);
                return;
            }

            if (s_CachedScalarDatas.TryGetValue(scalar.Value, out var data))
            {
                SetScalarData(m_ScalarDataValue, data);
                return;
            }

            s_AnalyzeScalarMethod(m_Emitter, scalar);
            s_CachedScalarDatas.TryAdd(scalar.Value, CopyScalarData(m_ScalarDataValue));
        }

        private static ScalarData CopyScalarData(object scalarData)
        {
            var booleans = new bool[6];
            var value = s_ScalarDataValueFieldRef(scalarData);
            for (var i = 0; i < s_ScalarDataBooleanFieldRefs.Length; i++)
            {
                var field = s_ScalarDataBooleanFieldRefs[i];
                booleans[i] = field(scalarData);
            }

            return new(value, booleans);
        }

        private static void SetScalarData(object oldData, ScalarData newData)
        {
            ref var oldValue = ref s_ScalarDataValueFieldRef(oldData);
            oldValue = newData.Value;

            for (var i = 0; i < s_ScalarDataBooleanFieldRefs.Length; i++)
            {
                var field = s_ScalarDataBooleanFieldRefs[i];

                ref var oldBool = ref field(oldData);
                oldBool = newData.Booleans[i];
            }
        }

        private sealed class ScalarData
        {
            public readonly string Value;
            public readonly bool[] Booleans;

            public ScalarData(string value, bool[] booleans)
            {
                Value = value;
                Booleans = booleans;
            }
        }
    }
}

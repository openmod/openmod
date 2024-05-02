using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Hosting;

namespace OpenMod.UnityEngine.Helpers
{
    /// <summary>
    /// Fixes <c>TlsException: Handshake failed - error code: UNITYTLS_INTERNAL_ERROR,
    /// verify result: UNITYTLS_X509VERIFY_NOT_DONE</c> error.
    /// <br/><br/>
    /// See https://github.com/openmod/openmod/issues/816
    /// </summary>

    // To test, start ssl server with:
    // openssl req -x509 -nodes -days 365 -sha256 -newkey rsa:2048 -keyout mykey.pem -out mycert.pem
    // openssl pkcs12 -inkey mykey.pem -in mycert.pem -export -out mycert.pfx -legacy
    // openssl s_server -accept 4443 -cert mycert.pem -key mykey.pem -WWW
    //
    // Then connect with:
    // using var client = new TcpClient();
    // using var stream = new SslStream(client.GetStream(), false, (_, _, _, _) => true);
    // var certificate = new X509Certificate2("path/to/mycert.pfx", "<enter pfx password here>");
    // stream.AuthenticateAsClient("localhost:4443",
    //     new X509CertificateCollection(new X509Certificate[] { certificate }),
    //     SslProtocols.Tls12, false);
    internal sealed class TlsHandshakeWorkaround : IHostedService
    {
        private Harmony? m_Harmony;

        private static readonly MethodInfo? s_ProcessHandshakeMethod;
        private static readonly HarmonyMethod? s_ProcessHandshakeTranspilerMethod;
        private static readonly MethodInfo? s_IsServerGetter;

        private const string c_HarmonyId = "OpenMod.UnityEngine.TlsHandshakeWorkaround";

        static TlsHandshakeWorkaround()
        {
            var type = Type.GetType("Mono.Unity.UnityTlsContext, System", throwOnError: false);

            if (type is null)
            {
                return;
            }

            s_ProcessHandshakeMethod = AccessTools.Method(type, "ProcessHandshake");

            s_ProcessHandshakeTranspilerMethod = new HarmonyMethod(
                typeof(TlsHandshakeWorkaround), nameof(ProcessHandshakeTranspiler));

            s_IsServerGetter = AccessTools.PropertyGetter(type, "IsServer");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (m_Harmony is not null || s_ProcessHandshakeMethod is null)
            {
                return Task.CompletedTask;
            }

            m_Harmony = new Harmony(c_HarmonyId);
            m_Harmony.Patch(s_ProcessHandshakeMethod, transpiler: s_ProcessHandshakeTranspilerMethod!);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (m_Harmony is null)
            {
                return Task.CompletedTask;
            }

            m_Harmony.UnpatchAll(m_Harmony.Id);
            m_Harmony = null;

            return Task.CompletedTask;
        }

        // Transpiles this method:
        // https://github.com/Unity-Technologies/mono/blob/2021.3.29f1/mcs/class/System/Mono.UnityTls/UnityTlsContext.cs#L328..L351
        private static IEnumerable<CodeInstruction> ProcessHandshakeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = instructions.ToList();
            var isServerCheckVar = (CodeInstruction?) null;

            var matched = instructionsList is
            [
                _,
                { opcode.Value: (short) OpCodeValue.Ldarg_0 }, // this
                { opcode.Value: (short) OpCodeValue.Call } possibleIsServerCall, // get_IsServer()
                { opcode.Value: (short) OpCodeValue.Brfalse_S } isServerCheck,

                { opcode.Value: (short) OpCodeValue.Ldloc_1 }, // verifyResult
                { operand: 0x80000000 }, // UnityTls.unitytls_x509verify_result.UNITYTLS_X509VERIFY_NOT_DONE
                { opcode.Value: (short) OpCodeValue.Bne_Un_S },

                { opcode.Value: (short) OpCodeValue.Ldloc_0 }, // errorState
                { opcode.Value: (short) OpCodeValue.Ldstr, operand: "Handshake failed" },
                { opcode.Value: (short) OpCodeValue.Ldc_I4_S, operand: 0x28 }, // AlertDescription.HandshakeFailure
                { opcode.Value: (short) OpCodeValue.Call },
                _
            ] && possibleIsServerCall.Calls(s_IsServerGetter!) && (isServerCheckVar = isServerCheck) is not null;

            // If it doesn't math it's possible that it was fixed in the source code or patched by something else.
            if (matched)
            {
                // Changes `if(IsServer)` to `if(!IsServer)`
                isServerCheckVar!.opcode = OpCodes.Brtrue_S;
            }

            return instructionsList;
        }
    }

    file enum OpCodeValue : short
    {
        // ReSharper disable InconsistentNaming
        Ldarg_0 = 2,
        Ldloc_0 = 6,
        Ldloc_1 = 7,
        Ldc_I4_S = 31,
        Call = 40,
        Brfalse_S = 44,
        Bne_Un_S = 51,
        Ldstr = 114,
        // ReSharper restore InconsistentNaming
    }
}
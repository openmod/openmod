using System;
using System.Net.Http;
using HarmonyLib;
using NuGet.Protocol;

namespace OpenMod.Unturned.Module.Shared
{
    [HarmonyPatch(typeof(HttpRequestMessageFactory))]
    [HarmonyPatch(nameof(HttpRequestMessageFactory.Create))]
    [HarmonyPatch(new[] { typeof(HttpMethod), typeof(Uri), typeof(HttpRequestMessageConfiguration) })]
    public class HttpRequestMessageFactoryCreatePatchUri
    {
        [HarmonyPrefix]
        public static void Create(HttpMethod method, ref Uri requestUri, HttpRequestMessageConfiguration configuration)
        {
#if DEBUG
            System.Console.WriteLine("HttpRequestMessageFactory: Creating from URI: " + requestUri);
#endif
            requestUri = new Uri(requestUri.ToString().Replace("https://", "http://"));
        }
    }
}
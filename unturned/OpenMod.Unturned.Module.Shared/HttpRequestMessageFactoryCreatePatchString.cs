using System.Net.Http;
using HarmonyLib;
using NuGet.Protocol;

namespace OpenMod.Unturned.Module.Shared
{
    [HarmonyPatch(typeof(HttpRequestMessageFactory))]
    [HarmonyPatch(nameof(HttpRequestMessageFactory.Create))]
    [HarmonyPatch(new[] { typeof(HttpMethod), typeof(string), typeof(HttpRequestMessageConfiguration) })]
    public class HttpRequestMessageFactoryCreatePatchString
    {
        [HarmonyPrefix]
        public static void Create(HttpMethod method, ref string requestUri, HttpRequestMessageConfiguration configuration)
        {
#if DEBUG
            System.Console.WriteLine("HttpRequestMessageFactory: Creating from string: " + requestUri);
#endif
            requestUri = requestUri.Replace("https://", "http://");
        }
    }
}
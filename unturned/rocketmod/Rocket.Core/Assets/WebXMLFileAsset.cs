using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Rocket.Core.Assets
{
    public class WebXMLFileAsset<T> : Asset<T> where T : class
    {
        private Uri url;
        private RocketWebClient webclient = new RocketWebClient();
        private System.Net.DownloadStringCompletedEventHandler handler = new System.Net.DownloadStringCompletedEventHandler((object sender, System.Net.DownloadStringCompletedEventArgs e) => { });
        private XmlRootAttribute attr;
        private bool waiting = false;

        public WebXMLFileAsset(Uri url = null, XmlRootAttribute attr = null, AssetLoaded<T> callback = null)
        {
            this.url = url;
            this.attr = attr;
            Load(callback);
        }

        public override void Load(AssetLoaded<T> callback = null)
        {
            try
            {
                if (!waiting)
                {
                    Logger.Log(String.Format("Updating WebXMLFileAsset {0} from {1}", typeof(T).Name, url));
                    waiting = true;

                    webclient.DownloadStringCompleted -= handler;
                    handler = (object sender, System.Net.DownloadStringCompletedEventArgs e) =>
                    {
                        if (e.Error != null)
                        {
                            Logger.Log(String.Format("Error retrieving WebXMLFileAsset {0} from {1}: {2}", typeof(T).Name, url, e.Error.Message));
                        }
                        else
                        {
                            try
                            {
                                using (StringReader reader = new StringReader(e.Result))
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(T), attr);
                                    T result = (T)serializer.Deserialize(reader);
                                    if (result != null)
                                        TaskDispatcher.QueueOnMainThread(() =>
                                        {
                                            instance = result;
                                            Logger.Log(String.Format("Successfully updated WebXMLFileAsset {0} from {1}", typeof(T).Name, url));
                                        });
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(String.Format("Error retrieving WebXMLFileAsset {0} from {1}: {2}", typeof(T).Name, url, ex.Message));
                            }
                        }

                        TaskDispatcher.QueueOnMainThread(() =>
                        {
                            callback?.Invoke(this);
                            waiting = false;
                        });
                    };
                    webclient.DownloadStringCompleted += handler;
                    webclient.DownloadStringAsync(url);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(String.Format("Error retrieving WebXMLFileAsset {0} from {1}: {2}", typeof(T).Name, url, ex.Message));
            }
        }
    }
}
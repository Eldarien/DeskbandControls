using Deskband.Settings;
using DeskbandBridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Deskband.AutoUpdater
{
    public class Checker
    {
        public void TimerTick()
        {
            var lastUpdateHours = (DateTime.Now - SettingsManager.Instance.State.LastUpdateCheck).TotalHours;
            if (lastUpdateHours < 24)
                return;

            //System.Windows.Forms.MessageBox.Show("Autoupdate Check");
            SettingsManager.Instance.State.LastUpdateCheck = DateTime.Now;
            SettingsManager.Instance.SaveSettings();

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.RunWorkerAsync();
        }

        private static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var url = "https://dl.dropboxusercontent.com/u/374593/Foobar2000DeskbandControlsNET.xml";

            Version installedVersion = new Version(FB2KConstants.DeskbandControlsVersion);
            Version newVersion = null;

            var webRequest = WebRequest.Create(url);
            webRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

            WebResponse webResponse;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (Exception)
            {
                return;
            }

            var metadataStream = webResponse.GetResponseStream();
            if (metadataStream == null)
            {
                return;
            }

            var doc = XDocument.Load(metadataStream);
            var updateInfo = GetUpdateInfo(doc);
            if (updateInfo.Version == null)
            {
                return;
            }

            newVersion = new Version(updateInfo.Version);
            if (newVersion > installedVersion)
            {
                var thread = new Thread(new ParameterizedThreadStart(ShowUI));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(updateInfo);
            }
        }

        private static UpdateInfo GetUpdateInfo(XDocument doc)
        {
            var info = new UpdateInfo();
            info.Version = doc.Descendants("version").Select(x => x.Value).FirstOrDefault();
            info.Title = doc.Descendants("title").Select(x => x.Value).FirstOrDefault();
            info.Url = doc.Descendants("url").Select(x => x.Value).FirstOrDefault();
            info.ChangelogUrl = doc.Descendants("changelog").Select(x => x.Value).FirstOrDefault();
            return info;
        }

        private static void ShowUI(object infoObj)
        {
            var info = (UpdateInfo)infoObj;

            var notificationForm = new NotificationForm();
            notificationForm.VersionTitle = info.Title;
            notificationForm.ShowDialog();
        }
    }
}
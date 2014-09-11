using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace DeploymentUpdate.DTO
{
    public class WCFServiceSettings : INotifyPropertyChanged
    {
        static Regex serviceRegex = new Regex(@"Service=\""([^""]*)\""", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex languageRegex = new Regex(@"Language=\""([^""]*)\""", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex factoryRegex = new Regex(@"factory=\""([^""]*)\""", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Deployment manifest
        #region Fields
        private String _ServiceName;
        private String _Language;
        private String _Factory;
        #endregion
        #region Properties
        public String ServiceName
        {
            get { return _ServiceName; }
        }
        public String Language
        {
            get { return _Language; }
        }
        public String Factory
        {
            get { return _Factory; }
        }
        #endregion
        #region Methods
        private void LoadServiceDetails(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi == null)
                return;
            Stream fs = null;
            try
            {
                fs = File.OpenRead(fi.FullName);
                var sr = new StreamReader(fs);

                var content = sr.ReadToEnd();

                if (serviceRegex.IsMatch(content))
                {
                    Match match = serviceRegex.Match(content);
                    _ServiceName = match.Groups[1].Value;
                }
                if (languageRegex.IsMatch(content))
                {
                    Match match = languageRegex.Match(content);
                    _Language = match.Groups[1].Value;
                }
                if (factoryRegex.IsMatch(content))
                {
                    Match match = factoryRegex.Match(content);
                    _Factory = match.Groups[1].Value;
                }
            }
            catch //(Exception ex)
            {

            }
            finally
            {
                if (fs != null)
                    fs.Close();
                OnPropertyChanged("ServiceName");
                OnPropertyChanged("Language");
                OnPropertyChanged("Factory");
            }
        }
        #endregion
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static WCFServiceSettings Load(string service)
        {
            WCFServiceSettings newSettings = new WCFServiceSettings();
            newSettings.LoadServiceDetails(service);
            if (newSettings.ServiceName != null)
                return newSettings;
            else
                return null;
        }
    }
}

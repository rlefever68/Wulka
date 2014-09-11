using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace DeploymentUpdate.DTO
{
    public class DeploymentSettings : INotifyPropertyChanged
    {
        private bool _ApplicationConfigurationDirty;
        private bool _ApplicationManifestDirty;
        private bool _DeploymentManifestDirty;

        private bool _SigningInfoDirty;

        public static List<String> DirtyIndicatorColumns { get { return new List<string> { "ApplicationConfigurationDirty", "ApplicationManifestDirty", "DeploymentManifestDirty", "SigningInfoDirty" }; } }

        public bool ApplicationConfigurationDirty
        {
            get { return _ApplicationConfigurationDirty; }
            set
            {
                if (value == _ApplicationConfigurationDirty) return;
                _ApplicationConfigurationDirty = value;
                if (value)
                    ApplicationManifestDirty = true;
                OnPropertyChanged("ApplicationConfigurationDirty");
            }
        }
        public bool ApplicationManifestDirty
        {
            get { return _ApplicationManifestDirty; }
            set
            {
                if (value == _ApplicationManifestDirty) return;
                _ApplicationManifestDirty = value;
                if (value)
                    DeploymentManifestDirty = true;
                OnPropertyChanged("ApplicationManifestDirty");
            }
        }
        public bool DeploymentManifestDirty
        {
            get { return _DeploymentManifestDirty; }
            set
            {
                if (value == _DeploymentManifestDirty) return;
                _DeploymentManifestDirty = value;

                OnPropertyChanged("DeploymentManifestDirty");
            }
        }
        public bool SigningInfoDirty
        {
            get { return _SigningInfoDirty; }
            set
            {
                if (value == _SigningInfoDirty) return;
                _SigningInfoDirty = value;
                if (value)
                    ApplicationManifestDirty = true;
                OnPropertyChanged("SigningInfoDirty");
            }
        }

        #region Deployment manifest
        #region Fields
        private FileInfo _DeploymentManifestFile;
        private String _SuiteName;
        private String _ProductName;
        private String _Version;
        private String _DeploymentProvider;
        #endregion
        #region Properties
        public FileInfo DeploymentManifestFile
        {
            get
            {
                if (_DeploymentManifestFile != null)
                    _DeploymentManifestFile.Refresh();
                return _DeploymentManifestFile;
            }
            set
            {
                if (value == _DeploymentManifestFile) return;
                _DeploymentManifestFile = value;
                DeploymentManifestDirty = true;

                OnPropertyChanged("DeploymentManifestFile");
            }
        }
        public String ProductName
        {
            get { return _ProductName; }
            set
            {
                if (value == _ProductName) return;
                _ProductName = value;
                DeploymentManifestDirty = true;
                OnPropertyChanged("ProductName");
            }
        }
        public String SuiteName
        {
            get { return _SuiteName; }
            set
            {
                if (value == _SuiteName) return;
                _SuiteName = value;
                DeploymentManifestDirty = true;

                OnPropertyChanged("SuiteName");
            }
        }
        public String Version
        {
            get { return _Version; }
            set
            {
                if (value == _Version) return;
                _Version = value;
                DeploymentManifestDirty = true;

                OnPropertyChanged("Version");
            }
        }
        public String DeploymentProvider
        {
            get { return _DeploymentProvider; }
            set
            {
                if (value == _DeploymentProvider) return;
                _DeploymentProvider = value;
                DeploymentManifestDirty = true;

                OnPropertyChanged("DeploymentProvider");
            }
        }
        #endregion
        #region Methods
        private void LoadDeploymentManifestDetails(string filename)
        {
            DeploymentManifestFile = new FileInfo(filename);
            if (DeploymentManifestFile == null)
                return;

            try
            {
                XmlDocument document;
                XmlNamespaceManager namespaceManager;
                namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
                namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");
                namespaceManager.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
                namespaceManager.AddNamespace("co.v1", "urn:schemas-microsoft-com:clickonce.v1");
                namespaceManager.AddNamespace("co.v2", "urn:schemas-microsoft-com:clickonce.v2");
                namespaceManager.AddNamespace("xrml", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
                namespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                document = new XmlDocument();
                document.Load(DeploymentManifestFile.FullName);

                string deploymentProviderXPath = "/asmv1:assembly/asmv2:deployment/asmv2:deploymentProvider";
                XmlNode deploymentProviderNode = document.SelectSingleNode(deploymentProviderXPath, namespaceManager);
                if (deploymentProviderNode != null)
                {
                    var deploymentProvider = deploymentProviderNode.Attributes["codebase"];
                    DeploymentProvider = deploymentProvider != null ? deploymentProvider.Value : String.Empty;
                }
                string descriptionXPath = "/asmv1:assembly/asmv1:description";
                XmlNode descriptionNode = document.SelectSingleNode(descriptionXPath, namespaceManager);
                if (descriptionNode != null)
                {
                    var suiteName = descriptionNode.Attributes["co.v1:suiteName"];
                    SuiteName = suiteName != null ? suiteName.Value : String.Empty;
                    var productName = descriptionNode.Attributes["asmv2:product"];
                    ProductName = productName != null ? productName.Value : String.Empty;
                }


                string assemblyIdentityXPath = "/asmv1:assembly/asmv1:assemblyIdentity";
                XmlNode assemblyIdentityNode = document.SelectSingleNode(assemblyIdentityXPath, namespaceManager);
                if (assemblyIdentityNode != null)
                {
                    var version = assemblyIdentityNode.Attributes["version"];
                    Version = version != null ? version.Value : String.Empty;
                }
                string dependentAssemblyXPath = "/asmv1:assembly/asmv2:dependency/asmv2:dependentAssembly";
                XmlNode dependentAssemblyNode = document.SelectSingleNode(dependentAssemblyXPath, namespaceManager);
                if (dependentAssemblyNode != null)
                {
                    string applicationManifestPath = Path.Combine(DeploymentManifestFile.Directory.FullName, dependentAssemblyNode.Attributes["codebase"].Value);
                    if (File.Exists(applicationManifestPath))
                        LoadApplicationManifestDetails(applicationManifestPath);
                }
            }
            catch //(Exception ex)
            {

            }
            finally
            {
                DeploymentManifestDirty = false;
            }
        }
        private void SaveDeploymentManifestDetails()
        {
            SaveDeploymentManifestDetails(DeploymentManifestFile.FullName);
        }
        private void SaveDeploymentManifestDetails(String filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi == null)
                return;

            XmlDocument document;
            XmlNamespaceManager namespaceManager;
            namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
            namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");

            document = new XmlDocument();
            document.Load(fi.FullName);

            if (DeploymentProvider != null)
            {
                string deploymentProviderXPath = "/asmv1:assembly/asmv2:deployment/asmv2:deploymentProvider";
                XmlNode deploymentProviderNode = document.SelectSingleNode(deploymentProviderXPath, namespaceManager);
                deploymentProviderNode.Attributes["codebase"].Value = DeploymentProvider;
            }
            if (ApplicationManifestFile != null)
            {
                string dependentAssemblyXPath = "/asmv1:assembly/asmv2:dependency/asmv2:dependentAssembly";
                XmlNode dependentAssemblyNode = document.SelectSingleNode(dependentAssemblyXPath, namespaceManager);
                dependentAssemblyNode.Attributes["size"].Value = ApplicationManifestFile.Length.ToString();
            }
            document.Save(filename);
            DeploymentManifestDirty = false;

            fi.Refresh();
        }
        #endregion
        #endregion

        #region Application manifest
        #region Fields
        private FileInfo _ApplicationManifestFile;
        private String _ApplicationName;
        #endregion
        #region Properties
        public FileInfo ApplicationManifestFile
        {
            get
            {
                if (_ApplicationManifestFile != null)
                    _ApplicationManifestFile.Refresh();
                return _ApplicationManifestFile;
            }
            set
            {
                if (value == _ApplicationManifestFile) return;
                _ApplicationManifestFile = value;
                ApplicationManifestDirty = true;
                OnPropertyChanged("ApplicationManifestFile");
            }
        }
        public String ApplicationName
        {
            get { return _ApplicationName; }
            set
            {
                if (value == _ApplicationName) return;
                _ApplicationName = value;
                ApplicationManifestDirty = true;
                OnPropertyChanged("ApplicationName");
            }
        }
        #endregion
        #region Methods
        private void LoadApplicationManifestDetails(string filename)
        {
            ApplicationManifestFile = new FileInfo(filename);
            if (ApplicationManifestFile == null)
                return;

            try
            {
                XmlDocument document;
                XmlNamespaceManager namespaceManager;
                namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
                namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");

                document = new XmlDocument();
                document.Load(ApplicationManifestFile.FullName);

                string assemblyIdentityXPath = "/asmv1:assembly/asmv1:assemblyIdentity";
                XmlNode assemblyIdentityNode = document.SelectSingleNode(assemblyIdentityXPath, namespaceManager);
                if (assemblyIdentityNode != null)
                {
                    ApplicationName = assemblyIdentityNode.Attributes["name"].Value;

                    var configPath = Path.Combine(ApplicationManifestFile.Directory.FullName, String.Format("{0}.config.deploy", ApplicationName));
                    if (File.Exists(configPath))
                        LoadApplicationConfigurationDetails(configPath);
                }
            }
            catch //(Exception ex)
            {
            }
            finally
            {
                ApplicationManifestDirty = false;
            }
        }
        private void SaveApplicationManifestDetails()
        {
            SaveApplicationManifestDetails(ApplicationManifestFile.FullName);
        }
        private void SaveApplicationManifestDetails(String filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi == null)
                return;

            XmlDocument document;
            XmlNamespaceManager namespaceManager;
            namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
            namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");

            document = new XmlDocument();
            document.Load(fi.FullName);

            document.Save(fi.FullName);
            ApplicationManifestDirty = false;

            ApplicationManifestFile.Refresh();
        }
        #endregion
        #endregion

        #region Application configuration
        #region Fields
        private FileInfo _ApplicationConfigurationFile;
        private bool _AppSettingsValue;
        private String _EndpointAddress;
        #endregion
        #region Properties
        public FileInfo ApplicationConfigurationFile
        {
            get
            {
                if (_ApplicationConfigurationFile != null)
                    _ApplicationConfigurationFile.Refresh();
                return _ApplicationConfigurationFile;
            }
            set
            {
                if (value == _ApplicationConfigurationFile) return;
                _ApplicationConfigurationFile = value;
                ApplicationConfigurationDirty = true;
                OnPropertyChanged("ApplicationConfigurationFile");
            }
        }
        public bool AppSettingsValue
        {
            get { return _AppSettingsValue; }
            set
            {
                if (value == _AppSettingsValue) return;
                _AppSettingsValue = value;
                ApplicationConfigurationDirty = true;
                OnPropertyChanged("AppSettingsValue");
            }
        }
        public String EndpointAddress
        {
            get { return _EndpointAddress; }
            set
            {
                if (value == _EndpointAddress) return;
                _EndpointAddress = value;
                ApplicationConfigurationDirty = true;
                OnPropertyChanged("EndpointAddress");
            }
        }
        #endregion
        #region Methods
        private void LoadApplicationConfigurationDetails(string filename)
        {
            ApplicationConfigurationFile = new FileInfo(filename);
            if (ApplicationConfigurationFile == null)
                return;

            try
            {
                XmlDocument document;
                XmlNamespaceManager namespaceManager;
                namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
                namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");

                document = new XmlDocument();
                document.Load(ApplicationConfigurationFile.FullName);

                string appSettingsXpath = "/configuration/appSettings";
                string endpointXPath = "/configuration/system.serviceModel/client/endpoint";

                XmlNode appSettings = document.SelectSingleNode(appSettingsXpath, namespaceManager);
                if (appSettings != null)
                {
                    for (int i = 0; i < appSettings.ChildNodes.Count; i++)
                    {
                        if (appSettings.ChildNodes[i].Attributes["key"].Value == "Disco.Endpoint")
                        {
                            var endpointAddressValue = appSettings.ChildNodes[i].Attributes["value"].Value;

                            if (endpointAddressValue.Contains("://"))
                            {
                                EndpointAddress = endpointAddressValue;
                                AppSettingsValue = true;
                            }
                            else
                            {
                                XmlNodeList endpointNodeList = document.SelectNodes(endpointXPath, namespaceManager);
                                if (endpointNodeList != null)
                                {
                                    for (int j = 0; j < endpointNodeList.Count; j++)
                                    {
                                        if (endpointNodeList.Item(j).Attributes["name"].Value == endpointAddressValue)
                                        {
                                            var endpointAddress = endpointNodeList.Item(j).Attributes["address"].Value;
                                            EndpointAddress = endpointAddress;
                                            AppSettingsValue = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



            }
            catch// (Exception ex)
            {
            }
            finally
            {
                ApplicationConfigurationDirty = false;
            }
        }
        public void SaveApplicationConfigurationDetails()
        {
            SaveApplicationConfigurationDetails(ApplicationConfigurationFile.FullName);
        }
        public void SaveApplicationConfigurationDetails(String filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi == null)
                return;

            XmlDocument document;
            XmlNamespaceManager namespaceManager;
            namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
            namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");

            document = new XmlDocument();
            document.Load(fi.FullName);

            string appSettingsXpath = "/configuration/appSettings";
            string endpointXPath = "/configuration/system.serviceModel/client/endpoint";

            if (EndpointAddress != null)
            {
                XmlNode appSettings = document.SelectSingleNode(appSettingsXpath, namespaceManager);
                if (appSettings != null)
                {
                    for (int i = 0; i < appSettings.ChildNodes.Count; i++)
                    {
                        if (appSettings.ChildNodes[i].Attributes != null && appSettings.ChildNodes[i].Attributes["key"].Value == "Disco.Endpoint")
                        {
                            var endpointAddressValue = appSettings.ChildNodes[i].Attributes["value"].Value;

                            if (endpointAddressValue.Contains("://"))
                            {
                                appSettings.ChildNodes[i].Attributes["value"].Value = EndpointAddress;
                            }
                            else
                            {
                                XmlNodeList endpointNodeList = document.SelectNodes(endpointXPath, namespaceManager);
                                if (endpointNodeList != null)
                                {
                                    for (int j = 0; j < endpointNodeList.Count; j++)
                                    {
                                        if (endpointNodeList.Item(j).Attributes["name"].Value == endpointAddressValue)
                                        {
                                            endpointNodeList.Item(j).Attributes["address"].Value = EndpointAddress;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            document.Save(fi.FullName);
            ApplicationConfigurationDirty = false;

            fi.Refresh();
        }
        #endregion
        #endregion

        #region Signing
        #region Fields
        private bool _SignDeployment;
        private String _SigningKeyLocation;
        #endregion
        #region Properties
        public bool SignDeployment
        {
            get { return _SignDeployment; }
            set
            {
                if (value == _SignDeployment) return;
                _SignDeployment = value;
                OnPropertyChanged("SignDeployment");
            }
        }
        public String SigningKeyLocation
        {
            get { return _SigningKeyLocation; }
            set
            {
                if (value == _SigningKeyLocation) return;
                _SigningKeyLocation = value;
                SigningInfoDirty = true;
                OnPropertyChanged("SigningKeyLocation");
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

        public static DeploymentSettings Load(string deployment)
        {
            DeploymentSettings newSettings = new DeploymentSettings();
            newSettings.LoadDeploymentManifestDetails(deployment);
            if (newSettings.ApplicationManifestFile != null)
                return newSettings;
            else
                return null;
        }

        public void Save(string foldername, String keyPassword = null)
        {

            if (!Directory.Exists(foldername))
                return;

            String application = ApplicationName.Replace(".exe", String.Empty);
            String version = Version;
            String tempDir = String.Format(@"{2}\Application Files\{0}_{1}", application, version.Replace(".", "_"), foldername);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            while (Directory.Exists(tempDir))
            { }
            Directory.CreateDirectory(tempDir);

            File.Copy(DeploymentManifestFile.FullName, Path.Combine(foldername, DeploymentManifestFile.Name), true);

            foreach (var file in ApplicationConfigurationFile.Directory.GetFiles())
            {
                if (file.Extension != ".application")
                    file.CopyTo(Path.Combine(tempDir, file.Name.Replace(".deploy", String.Empty)), true);
                else
                    file.CopyTo(Path.Combine(foldername, file.Name.Replace(".deploy", String.Empty)), true);
            }

            Process x = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "mage.exe";


            if (ApplicationConfigurationDirty)
            {
                SaveApplicationConfigurationDetails(Path.Combine(tempDir, ApplicationConfigurationFile.Name.Replace(".deploy", String.Empty)));
                startInfo.Arguments = String.Format(@"-u ""{1}\{0}.exe.manifest"" -fd ""{1}""", application, tempDir);
                x.StartInfo = startInfo;
                x.Start();
                x.WaitForExit();
            }
            if (ApplicationManifestDirty)
            {
                SaveApplicationManifestDetails(String.Format(@"{1}\{0}.exe.manifest", application, tempDir));
                if (keyPassword != null)
                {
                    startInfo.Arguments = String.Format(@"-s ""{1}\{0}.exe.manifest"" -cf ""{2}"" -pwd {3}", application, tempDir, SigningKeyLocation, keyPassword);
                    x.StartInfo = startInfo;
                    x.Start();
                    x.WaitForExit();
                }
                startInfo.Arguments = String.Format(@"-u ""{2}\{0}.application"" -appm ""{1}\{0}.exe.manifest""", application, tempDir, foldername);
                x.StartInfo = startInfo;
                x.Start();
                x.WaitForExit();
            }

            if (DeploymentManifestDirty)
            {
                SaveDeploymentManifestDetails(String.Format(@"{1}\{0}.application", application, foldername));
                if (keyPassword != null)
                {
                    startInfo.Arguments = String.Format(@"-s ""{1}\{0}.application"" -cf ""{2}"" -pwd {3}", application, foldername, SigningKeyLocation, keyPassword);
                    x.StartInfo = startInfo;
                    x.Start();
                    x.WaitForExit();
                }
            }
            SigningInfoDirty = false;
            DirectoryInfo tempDirFI = new DirectoryInfo(tempDir);
            foreach (var file in tempDirFI.GetFiles())
            {
                if (file.Extension != ".manifest")
                    file.MoveTo(String.Concat(file.FullName, ".deploy"));
            }
            if (ApplicationConfigurationDirty)
            {
                SaveApplicationConfigurationDetails();
                SaveApplicationManifestDetails();
                SaveDeploymentManifestDetails();
            }
            else if (ApplicationManifestDirty)
            {
                SaveApplicationManifestDetails();
            }
            else if (DeploymentManifestDirty)
            {
                SaveDeploymentManifestDetails();
            }
        }
    }
}

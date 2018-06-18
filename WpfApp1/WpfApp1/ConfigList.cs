using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class ConfigList
    {
        private string configFileName;
        private System.DateTime configFileModificationTime;
        private string configFileMD5Code;
        private bool isVersion;
        private string configFilePath;

        public ConfigList(string configFileName, System.DateTime configFileModificationTime, bool isVersion, string configFilePath)
        {
            this.configFileName = configFileName;
            this.configFileModificationTime = configFileModificationTime;
            //this.configFileHashCode = configFileHashCode;
            this.isVersion = isVersion;
            this.configFilePath = configFilePath;
        }

        public string ConfigFileName { get => configFileName; set => configFileName = value; }
        public System.DateTime ConfigFileModificationTime { get => configFileModificationTime; set => configFileModificationTime = value; }
        public string ConfigFileMD5Code { get => configFileMD5Code; set => configFileMD5Code = value; }
        public bool IsVersion { get => isVersion; set => isVersion = value; }
        public string ConfigFilePath { get => configFilePath; set => configFilePath = value; }

        public override bool Equals(object obj)
        {
            var list = obj as ConfigList;
            return list != null &&
                   configFileName == list.configFileName &&
                   configFileModificationTime == list.configFileModificationTime;
        }

        public string GetConfigFileMD5Code()
        {
            return MDfive.createMd5(configFilePath);
        }

        public override int GetHashCode()
        {
            var hashCode = -321212534;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ConfigFileMD5Code);
            return hashCode;
        }
    }
}

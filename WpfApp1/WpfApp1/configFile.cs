using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class configFile
    {
        private List<File> configFiles;

        public configFile(List<File> configFiles)
        {
            this.ConfigFiles = configFiles;
        }

        internal List<File> ConfigFiles { get => configFiles; set => configFiles = value; }

        public override bool Equals(object obj)
        {
            var file = obj as configFile;
            return file != null &&
                   EqualityComparer<List<File>>.Default.Equals(ConfigFiles, file.ConfigFiles);
        }

        public override int GetHashCode()
        {
            return 2039249806 + EqualityComparer<List<File>>.Default.GetHashCode(ConfigFiles);
        }
    }
}

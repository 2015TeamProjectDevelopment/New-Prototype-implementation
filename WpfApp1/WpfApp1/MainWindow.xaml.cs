using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        NewConfigureFilePage newConfigureFilePage = new NewConfigureFilePage();
        ConfigureFileListPage ConfigureFileListPage = new ConfigureFileListPage();
        UpdateSoftwarePage updateSoftwarePage = new UpdateSoftwarePage();

        public MainWindow()
        {
            InitializeComponent();
            initTestFile();
            setUrltxt();
            MainFrame.Navigate(newConfigureFilePage);
        }
        
        private void setUrltxt()
        {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            string UrlPath = System.IO.Directory.GetCurrentDirectory() + "//url.txt";
            if (!System.IO.File.Exists(UrlPath))
            {
                using (FileStream fs = new FileStream(UrlPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine("file://" + currentPath +"\\Server");
                    }
                }
            }
        }

        public void initTestFile()
        {
            string currentdir = System.IO.Directory.GetCurrentDirectory();
            string PCDir = System.IO.Path.Combine(currentdir, "PC");
            string ServerDir = System.IO.Path.Combine(currentdir, "Server");
            if (!System.IO.Directory.Exists(PCDir))
            {
                System.IO.Directory.CreateDirectory(PCDir);
                new FileStream(PCDir+ "\\deletefile.txt", FileMode.CreateNew);
                new FileStream(PCDir + "\\replacefile.txt", FileMode.CreateNew);
            }
            if (!System.IO.Directory.Exists(ServerDir))
            {
                System.IO.Directory.CreateDirectory(ServerDir);
                new FileStream(ServerDir + "\\newfile.txt", FileMode.CreateNew);
                new FileStream(ServerDir + "\\deletefile.txt", FileMode.CreateNew);
                new FileStream(ServerDir + "\\replacefile.txt", FileMode.CreateNew);
            }
        }

        private void MenuItem_Click_about_us(object sender, RoutedEventArgs e)
        {
            SoftwareSetting SWSetting = new SoftwareSetting();
            //在父窗口中间显示
            SWSetting.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SWSetting.Owner = this;
            SWSetting.Title = "基本设置";
            SWSetting.ShowDialog();
        }

        private void Button_Page_List(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(ConfigureFileListPage);
        }

        private void Button_Page_New(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(newConfigureFilePage);
        }

        private void Button_Page_Update(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(updateSoftwarePage);
        }

        private void MenuItem_Click_exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class IniFiles
    {
        public string path;
        [DllImport("kernel32")] //返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")] //返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 保存ini文件的路径
        /// 调用示例：var ini = IniFiles("C:\file.ini");
        /// </summary>
        /// <param name="INIPath"></param>
        public IniFiles(string iniPath)
        {
            this.path = iniPath;
        }
        /// <summary>
        /// 写Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name","localhost");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <param name="value">值</param>
        public void IniWritevalue(string Section, string Key, string value)
        {
            WritePrivateProfileString(Section, Key, value, this.path);
        }
        /// <summary>
        /// 读Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <returns>值</returns>
        public string IniReadvalue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }

    }

    public class ConfigList
    {
        private string configFileName;
        private System.DateTime configFileModificationTime;
        private int configFileHashCode;
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
        public int ConfigFileHashCode { get => configFileHashCode; set => configFileHashCode = value; }
        public bool IsVersion { get => isVersion; set => isVersion = value; }
        public string ConfigFilePath { get => configFilePath; set => configFilePath = value; }

        public override bool Equals(object obj)
        {
            var list = obj as ConfigList;
            return list != null &&
                   configFileName == list.configFileName &&
                   configFileModificationTime == list.configFileModificationTime;
        }

        public override int GetHashCode()
        {
            var hashCode = -159444910;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(configFileName);
            hashCode = hashCode * -1521134295 + configFileModificationTime.GetHashCode();
            return hashCode;
        }
    }

    //配置文件类
    public class configFile
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

    //文件类
    public class File
    {
        private String fileName;        //文件名
        private long fileSize;          //文件大小
        private int hashcode;           //哈希码
        private String updateMethod;    //更新操作
        private String lastModified;    //最后修改时间
        private String path;            //路径

        public File()
        {

        }

        //构造函数
        public File(string fileName, long fileSize, string updateMethod, string lastModified, string path)
        {
            this.fileName = fileName;
            this.fileSize = fileSize;
            this.hashcode = GetHashCode();
            this.updateMethod = updateMethod;
            this.lastModified = lastModified;
            this.path = path;
        }

        public string FileName { get => fileName; set => fileName = value; }
        public long FileSize { get => fileSize; set => fileSize = value; }
        public int Hashcode { get => hashcode; set => hashcode = value; }
        public string UpdateMethod { get => updateMethod; set => updateMethod = value; }
        public string LastModified { get => lastModified; set => lastModified = value; }
        public string Path { get => path; set => path = value; }

        public override bool Equals(object obj)
        {
            var file = obj as File;
            return file != null &&
                   fileName == file.fileName &&
                   fileSize == file.fileSize &&
                   hashcode == file.hashcode &&
                   updateMethod == file.updateMethod &&
                   lastModified == file.lastModified &&
                   path == file.path;
        }

        public override int GetHashCode()
        {
            var hashCode = 2096143202;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileSize.ToString());
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(updateMethod);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(lastModified);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(path);
            return hashCode;
        }
    }

       
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// UpdateSoftwarePage.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateSoftwarePage : Page
    {
        ConfigList cfPC, cfServer;
        public UpdateSoftwarePage()
        {
            InitializeComponent();
            //CommandBinding bindingNew = new CommandBinding(ApplicationCommands.New);
            //bindingNew.Executed += NewCommand;
            //this.CommandBindings.Add(bindingNew);
        }

        private void Button_Check(object sender, RoutedEventArgs e)
        {
            // 获取PC的 ini 文件
            cfPC = GetPCIni();
            int PCIniHash = 0;
            if (cfPC != null)
                PCIniHash = cfPC.ConfigFileHashCode;
            // 获取Server的 ini 文件
            cfServer = GetServerIni();
            int ServerIniHash = 0;
            if (cfServer != null)
                ServerIniHash = cfServer.ConfigFileHashCode;

            //Console.WriteLine(PCIniHash);
            //Console.WriteLine(ServerIniHash);

            // 如果文件hash码不同，进行更新
            if(PCIniHash != ServerIniHash)
            {
                UpdateUI SWSetting = new UpdateUI(cfPC, cfServer);
                //在父窗口中间显示
                SWSetting.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                SWSetting.Title = "软件更新";
                SWSetting.ShowDialog();
            }
            // 否则 不更新
            else
            {
                MessageBox.Show("当前为最新版本，无须更新");
            }

        }

        //获取PC的 ini 文件
        private ConfigList GetPCIni()
        {
            string fileDir = Environment.CurrentDirectory;
            string PCDir = System.IO.Path.Combine(fileDir, "PC");
            DirectoryInfo fileFold = new DirectoryInfo(PCDir);
            FileInfo[] files = fileFold.GetFiles();
            for (int i = 0; files != null && i < files.Length; i++)  //找出ini
            {
                    if (files[i].Extension == ".ini")   //挑选出符合条件的信息  
                    {
                        ConfigList config1 = new ConfigList(files[i].Name, files[i].LastWriteTime, false, PCDir + "\\" + files[i].Name);
                        config1.ConfigFileHashCode = config1.GetHashCode();
                        return config1;
                    }
                    else
                    {
                        continue;
                    }
            }
            return null;
        }

        // 获取Server的 ini 文件 通过url找
        private ConfigList GetServerIni()
        {
            string current = System.IO.Directory.GetCurrentDirectory();
            StreamReader sr = new StreamReader(current + "//url.txt", Encoding.Default);
            string line;
            string urlPath = "";
            while ((line = sr.ReadLine()) != null)
            {
                Uri uriAddress = new Uri(line);
                urlPath = uriAddress.LocalPath;
            }
            if (Directory.Exists(urlPath))
            {
                string path = urlPath + "\\newestFolder";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                DirectoryInfo fileFold = new DirectoryInfo(path);
                FileInfo[] files = fileFold.GetFiles();
                foreach (FileInfo content in fileFold.GetFiles())
                {
                    if (content.Extension == ".ini")
                    {
                        string tempDir = current + "\\PC\\temp";
                        if (!System.IO.Directory.Exists(tempDir))
                        {
                            System.IO.Directory.CreateDirectory(tempDir);
                        }
                        ConfigureFileListPage.DeleteFolder(tempDir);
                        //存在 下载（复制）下来进行比较
                        System.IO.File.Copy(content.FullName, tempDir + "\\" + content.Name
                                , true);
                        ConfigList config1 = new ConfigList(content.Name, content.LastWriteTime, false, tempDir + "\\" + content.Name);
                        config1.ConfigFileHashCode = config1.GetHashCode();
                        //删除temp文件夹
                        Directory.Delete(tempDir, true);
                        return config1;
                        
                    }
                }
            }
            return null;
        }
    }
}

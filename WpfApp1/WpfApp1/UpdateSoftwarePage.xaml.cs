using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private static readonly log4net.ILog log =
    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ConfigList cfPC, cfServer;
        public UpdateSoftwarePage()
        {
            InitializeComponent();
            log.Info("加载UpdateSofewarePage");
        }

        private void Button_Update(object sender, RoutedEventArgs e)
        {
            log.Info("点击更新");
            // 获取PC的 ini 文件
            cfPC = GetPCIni();
            string PCIniHash = "";
            if (cfPC != null)
                PCIniHash = cfPC.ConfigFileMD5Code;
            // 获取Server的 ini 文件
            cfServer = GetServerIni();
            string ServerIniHash = "";
            if (cfServer != null)
                ServerIniHash = cfServer.ConfigFileMD5Code;

            Console.WriteLine(PCIniHash);
            Console.WriteLine(ServerIniHash);

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
                        config1.ConfigFileMD5Code = config1.GetConfigFileMD5Code();
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
            StreamReader sr = new StreamReader(current + @"\url.txt", Encoding.Default);
            string line;
            string url = "";
            while ((line = sr.ReadLine()) != null)
            {
                url = line;
                //Console.WriteLine(url);
                //Console.WriteLine(line);
            }
            //Uri uri = new Uri(url);
            //if (uri.IsFile)
            //{
            //    string urifilepath = uri.LocalPath;
            //    if(!System.IO.File.Exists(urifilepath))
            //        return null;
            //}

            string tempDir = current + @"\PC\temp";
            if (!System.IO.Directory.Exists(tempDir))
            {
                System.IO.Directory.CreateDirectory(tempDir);
            }
            ConfigureFileListPage.DeleteFolder(tempDir);
            //存在 下载下来进行比较
            WebClient webClient = new WebClient();
            string downloadPath = tempDir + "\\temp.ini";
            webClient.DownloadFile(url, downloadPath);
            System.IO.FileInfo f = new System.IO.FileInfo(downloadPath);
            ConfigList config1 = new ConfigList("temp.ini", f.LastWriteTime, false, downloadPath);
            config1.ConfigFileMD5Code = config1.GetConfigFileMD5Code();
            //删除temp文件夹
            //Directory.Delete(tempDir, true);
            return config1;
        }
    }
}

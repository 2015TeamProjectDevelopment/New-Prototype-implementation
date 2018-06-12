using System;
using System.IO;
using System.Net;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// UpdateUI.xaml 的交互逻辑
    /// 检测到更新时候的弹出界面
    /// </summary>
    public partial class UpdateUI : Window
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConfigList cfPC;
        private ConfigList cfServer;
        private string downloadPath;
        static string current = System.IO.Directory.GetCurrentDirectory();
        
        public UpdateUI(ConfigList cfPC, ConfigList cfServer)
        {
            InitializeComponent();
            log.Info("加载UpdateUI");
            this.cfPC = cfPC;
            this.cfServer = cfServer;
            showDiff();
            doUpdate();
        }

        //显示版本差异
        private void showDiff()
        {
            string sText = "";
            string oldHash = "", newHash = "";

            if(cfPC != null)
            {
                string name = cfPC.ConfigFileName;
                oldHash  = cfPC.ConfigFileMD5Code;
            }
            sText += "哈希值：" + oldHash + "\n";
            if (cfServer != null)
            {
                string name = cfServer.ConfigFileName;
                newHash = cfServer.ConfigFileMD5Code;
            }
            sText += "哈希值：" + newHash + "\n";
            textblock.Text = sText;
        }

        // 执行更新操作
        // 读取已下载的ini 然后一个一个进行下载
        private void download()
        {
            if(cfServer == null)
            {
                return;
            }
            string PcPath = current + @"\PC";
            string destPath = PcPath + @"\temp"; // 目标文件夹
            downloadPath = destPath;
            Console.WriteLine(downloadPath);
            if (!System.IO.Directory.Exists(destPath))
            {
                System.IO.Directory.CreateDirectory(destPath);
            }

            IniFiles ini_file_read = new IniFiles(current + @"\PC\temp\temp.ini");
            WebClient webClient = new WebClient();
            for (int j = 0; j < 10000; j++)
            {
                String temp_session = "session" + j.ToString();
                String temp_file_name = ini_file_read.IniReadvalue(temp_session, "fileName");
                String temp_file_path = ini_file_read.IniReadvalue(temp_session, "path");
                if (temp_file_path == "")
                {
                    break;
                }
                // 每一个都使用url进行下载
                temp_file_path = "file://" + temp_file_path;
                webClient.DownloadFile(temp_file_path, downloadPath + @"\" + temp_file_name);
            }
        }

        //更新操作
        private void doUpdate()
        {
            download();
            //读取配置文件并进行操作
            DirectoryInfo fileFold = new DirectoryInfo(downloadPath);
            
            FileInfo[] files = fileFold.GetFiles();
            string PcPath = current + "\\PC";
            string IniName = "";
            for (int i = 0; files != null && i < files.Length; i++)  //将文件信息添加到List里面  
            {
                if (files[i].Extension == ".ini")   //挑选出符合条件的信息  
                {
                    IniName = files[i].Name;
                    IniFiles ini_file_read = new IniFiles(downloadPath + "\\" + files[i].Name);
                    for (int j = 0; j < 10000; j++)
                    {
                        String tem_path = "session" + j.ToString();
                        String tem_file_name = ini_file_read.IniReadvalue(tem_path, "fileName");
                        String tem_file_updateMethod = ini_file_read.IniReadvalue(tem_path, "updateMethod");
                        if (tem_file_updateMethod == "")
                        {
                            break;
                        }
                        else if(tem_file_updateMethod == "新增")
                        {
                            System.IO.File.Copy(downloadPath + "\\" + tem_file_name,
                                PcPath + "\\" + tem_file_name, true);
                        }
                        else if (tem_file_updateMethod == "删除")
                        {
                            System.IO.File.Delete(PcPath + "\\" + tem_file_name);
                        }
                        else if (tem_file_updateMethod == "替换")
                        {
                            //先删除后复制
                            System.IO.File.Delete(PcPath + "\\" + tem_file_name);
                            System.IO.File.Copy(downloadPath + "\\" + tem_file_name,
                                PcPath + "\\" + tem_file_name, true);
                        }
                        //应该放在底部，否则会导致软件直接退出，其它操作没有进行。
                        else if (tem_file_updateMethod == "更新本软件")
                        {
                            System.IO.File.Copy(downloadPath + "\\" + tem_file_name,
                                PcPath + "\\" + tem_file_name, true);
                            MessageBox.Show("更新完毕，软件需要重启");
                            string targetSoftwarePath = PcPath + " " + tem_file_name + " " + current;
                            System.Diagnostics.Process.Start(@".\update_its.exe", targetSoftwarePath);
                            UpdatePCini(PcPath, IniName);
                            Environment.Exit(0);
                        }
                    }
                    break;
                }
            }

            UpdatePCini(PcPath, IniName);

        }

        public void UpdatePCini(string PcPath, string IniName)
        {
            //更新PC的ini
            DirectoryInfo Fold = new DirectoryInfo(PcPath);
            FileInfo[] fs = Fold.GetFiles();
            for (int i = 0; fs != null && i < fs.Length; i++)  //将文件信息添加到List里面  
            {
                if (fs[i].Extension == ".ini")   //挑选出符合条件的信息  
                {
                    Console.WriteLine(PcPath + "\\" + fs[i].Name);
                    //删除原来的
                    System.IO.File.Delete(PcPath + "\\" + fs[i].Name);
                    break;
                }
            }
            
            //复制新的进去
            System.IO.File.Copy(downloadPath + @"\temp.ini",
                PcPath + "\\" + IniName, true);
            System.IO.File.Move(PcPath + @"\temp.ini", PcPath + "\\newest.ini");
        }

        //取消更新
        private void Button_Click_cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

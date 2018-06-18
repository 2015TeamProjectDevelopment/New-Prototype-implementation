using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace WpfApp1
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        NewConfigureFilePage newConfigureFilePage = new NewConfigureFilePage();
        ConfigureFileListPage ConfigureFileListPage = new ConfigureFileListPage();
        UpdateSoftwarePage updateSoftwarePage = new UpdateSoftwarePage();

        public MainWindow()
        {
            InitializeComponent();
            log.Info("加载MainWindow");

            InitTestFile();
            log.Info("初始化testfile");

            SetUrltxt();
            log.Info("设置初始url");

            MainFrame.Navigate(newConfigureFilePage);
            log.Info("设置初始页面 newConfigureFilePage ");
        }
        
        // 设置url.txt
        private void SetUrltxt()
        {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            string UrlPath = System.IO.Directory.GetCurrentDirectory() + @"\url.txt";
            if (!System.IO.File.Exists(UrlPath))
            {
                using (FileStream fs = new FileStream(UrlPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine("file:///" + currentPath + @"\Server\newestFolder\newest.ini");
                    }
                }
            }
        }

        //初始化几个测试的txt文件
        public void InitTestFile()
        {
            string currentdir = System.IO.Directory.GetCurrentDirectory();
            string PCDir = System.IO.Path.Combine(currentdir, "PC");
            string ServerDir = System.IO.Path.Combine(currentdir, "Server");
            if (!System.IO.Directory.Exists(PCDir))
            {
                System.IO.Directory.CreateDirectory(PCDir);
                new FileStream(PCDir+ @"\deletefile.txt", FileMode.CreateNew);
                new FileStream(PCDir + @"\replacefile.txt", FileMode.CreateNew);
            }
            if (!System.IO.Directory.Exists(ServerDir))
            {
                System.IO.Directory.CreateDirectory(ServerDir);
                new FileStream(ServerDir + @"\newfile.txt", FileMode.CreateNew);
                new FileStream(ServerDir + @"\deletefile.txt", FileMode.CreateNew);
                new FileStream(ServerDir + @"\replacefile.txt", FileMode.CreateNew);
                string path = System.IO.Path.Combine(ServerDir, "newestFolder");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
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
            log.Info("点击设置-关于本产品，进入基本设置");
        }

        private void Button_Page_List(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(ConfigureFileListPage);
            log.Info("点击配置文件列表，跳转到列表页");
        }

        private void Button_Page_New(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(newConfigureFilePage);
            log.Info("点击新建配置文件，跳转到新建页");
        }

        private void Button_Page_Update(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(updateSoftwarePage);
            log.Info("点击更新软件，跳转到更新页");
        }

        private void MenuItem_Click_exit(object sender, RoutedEventArgs e)
        {
            this.Close();
            log.Info("点击管理-退出，退出程序");
        }
    }
}

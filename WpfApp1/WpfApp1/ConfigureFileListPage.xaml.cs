using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// ConfigureFileListPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigureFileListPage : Page
    {
        public ConfigureFileListPage()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            //initList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            listView.Items.Clear();
            initList();//执行查询
        }

        public void initList()
        {
            string fileDir = Environment.CurrentDirectory;
            string configureListDir = System.IO.Path.Combine(fileDir + "\\Server", "configureList");
            if (!System.IO.Directory.Exists(configureListDir))
            {
                System.IO.Directory.CreateDirectory(configureListDir);
            }
            DirectoryInfo fileFold = new DirectoryInfo(configureListDir);
            FileInfo[] files = fileFold.GetFiles(); //获取指定文件夹下的所有文件
            List<ConfigList> config2 = new List<ConfigList>();
            for (int i = 0; files != null && i < files.Length; i++)  //将文件信息添加到List里面  
            {
                try
                {
                    if (files[i].Extension == ".ini")   //挑选出符合条件的信息  
                    {
                        ConfigList config1 = new ConfigList(files[i].Name, files[i].LastWriteTime, false, configureListDir + "\\"+ files[i].Name);
                        config1.ConfigFileHashCode = config1.GetHashCode();
                        config2.Add(config1);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
            //按修改时间排序
            config2.Sort((x, y) => { return y.ConfigFileModificationTime.CompareTo(x.ConfigFileModificationTime); });
            bool tem_i = true;
            foreach (var tem_config in config2)
            {
                if (tem_i)
                {
                    tem_config.IsVersion = true;
                    listView.Items.Add(tem_config);
                    tem_i = false;
                    continue;
                }
                listView.Items.Add(tem_config);
            }
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var player = btn.DataContext as ConfigList;
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            Write(currentPath + "\\fileName.txt", player.ConfigFileName);
            ModifyProfile SWSetting = new ModifyProfile();
            SWSetting.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SWSetting.Title = "修改配置文件";
            SWSetting.ShowDialog();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            // dir 当前路径 fileName 选中那一列的文件名 
            // versionFloder 保存配置文件夹的路径 
            //subDirName 对应配置文件生成的目录 subVersionPath 该路径，保存配置文件、复制过去的文件
            var btn = sender as Button;
            var player = btn.DataContext as ConfigList;
            string dir = System.IO.Directory.GetCurrentDirectory();
            Write(dir + "\\fileName.txt", player.ConfigFileName);
            string fileName = player.ConfigFileName;

            //判断是否是最新版本，是的话将配置文件移动到 最新版本目录下
            if(player.IsVersion)
            {
                string newestVersion = System.IO.Path.Combine(dir + "\\Server", "newestFolder");
                if (!System.IO.Directory.Exists(newestVersion))
                {
                    System.IO.Directory.CreateDirectory(newestVersion);
                }
                DeleteFolder(newestVersion);//删除
                System.IO.File.Copy(player.ConfigFilePath, newestVersion + "\\" + fileName, true);
            }

            //判断文件夹是否存在，文件夹设置在哪里比较合适呢？
            string versionFloder = System.IO.Path.Combine(dir + "\\Server", "versionFolder");
            if (!System.IO.Directory.Exists(versionFloder))
            {
                System.IO.Directory.CreateDirectory(versionFloder);
            }
            //子文件夹 对应每个配置文件
            string subDirName = fileName.Replace('.', '_');
            string subVersionPath = System.IO.Path.Combine(versionFloder, subDirName);
            System.IO.Directory.CreateDirectory(subVersionPath);

            //复制配置文件到对应的子文件夹下
            System.IO.File.Copy(player.ConfigFilePath, subVersionPath + "\\" + fileName, true);

            //复制选中的文件的内容到该文件下
            ObservableCollection<Info> infos = new ModifyProfile().GetFileMessage();
            for (int i = 0; i < infos.Count; i++)
            {
                if (System.IO.File.Exists(infos[i].path))
                {
                    string[] strArr = infos[i].path.Split('\\');
                    int length = strArr.Length - 1;
                    System.IO.File.Copy(infos[i].path, subVersionPath + "\\" + strArr[length], true);
                }
            }
            MessageBox.Show("生成版本成功");
        }

        //删除指定目录下的文件
        public static void DeleteFolder(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                foreach (string content in Directory.GetFileSystemEntries(dirPath))
                {
                    if (Directory.Exists(content))
                    {
                        Directory.Delete(content, true);
                    }
                    else if (System.IO.File.Exists(content))
                    {
                        System.IO.File.Delete(content);
                    }
                }
            }
        }

        //保存点击的文件名
        public void Write(string path, string fileName)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            //string str = fileName.Substring(0, fileName.Length - 4) + "." + fileName.Substring(fileName.Length - 3);
            sw.Write(fileName);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

    }
}

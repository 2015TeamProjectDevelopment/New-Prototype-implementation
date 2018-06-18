using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

    public class methodList : List<String>
    {
        public methodList()
        {
            this.Add(" ");
            this.Add("新增");
            this.Add("替换");
            this.Add("删除");
            this.Add("更新本软件");
        }
    }

    /// <summary>
    /// NewConfigureFile.xaml 的交互逻辑
    /// </summary>
    public partial class NewConfigureFilePage : Page
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section, string key, string value, string filePath);
        ObservableCollection<Info> infos = new ObservableCollection<Info>
            {
                
            };
        public NewConfigureFilePage()
        {
            InitializeComponent();
            log.Info("加载 NewConfigureFilePage");
            DataGridForNew.ItemsSource = infos;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Server";
            if (ofd.ShowDialog() == true)
            {
                Info i = new Info { path = ofd.FileName, way = "" };
                // 判断是否重复
                //if (!infos.Contains)
                    infos.Add(i);
                //else
                  //  MessageBox.Show("该文件已添加，请勿重复添加");
            }

        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if(DataGridForNew.SelectedItem != null)
            {
                Info DRV = (Info)DataGridForNew.SelectedItem;
                String name = DRV.path;
                
                //删除infos里面的数据
                for(int i = 0; i < infos.Count; i++)
                {
                    if(infos[i].path.CompareTo(name) == 0)
                    {
                        infos.Remove(infos[i]);       
                        break;
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            List<File> configFiles = new List<File>();
            for (int i = 0; i < infos.Count; i++)
            {
                FileInfo fileInfo = new FileInfo(infos[i].path);
                File file = new File(fileInfo.Name, fileInfo.Length, infos[i].way, fileInfo.LastWriteTime.ToString(), fileInfo.FullName);
                configFiles.Add(file);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            string currentPath = System.IO.Directory.GetCurrentDirectory() + @"\Server";
            string configureListDir = System.IO.Path.Combine(currentPath, "configureList");
            if (!System.IO.Directory.Exists(configureListDir))
            {
                System.IO.Directory.CreateDirectory(configureListDir);
            }
            sfd.InitialDirectory = configureListDir;
            //设置保存的文件的类型
            sfd.Filter = "INI配置文件|*.ini";
            if (sfd.ShowDialog() == true)
            {
                for (int i = 0; i < configFiles.Count; i++)
                {
                    File f = configFiles[i];
                    WritePrivateProfileString("session" + i, "fileName" , f.FileName, sfd.FileName);
                    WritePrivateProfileString("session" + i, "fileSize", f.FileSize.ToString(), sfd.FileName);
                    WritePrivateProfileString("session" + i, "hashcode", f.Hashcode.ToString(), sfd.FileName);
                    WritePrivateProfileString("session" + i, "updateMethod", f.UpdateMethod, sfd.FileName);
                    WritePrivateProfileString("session" + i, "lastModified", f.LastModified, sfd.FileName);
                    WritePrivateProfileString("session" + i, "path", f.Path, sfd.FileName);
                }
                MessageBox.Show("保存成功");
                log.Info("另存配置文件: " + sfd.SafeFileName + "成功");
            }
            else
            {
                MessageBox.Show("取消保存");
            }

        }        
    }

    public class Info
    {
        public String path { get; set; }
        public String way { get; set; }
    }
   
}

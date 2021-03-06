﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// ModifyProfile.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class ModifyProfile : Window
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section, string key, string value, string filePath);
        //存储DataGrid中的信息
        ObservableCollection<Info> infos = new ObservableCollection<Info>
            {
            }; 

        public ModifyProfile()
        {
            InitializeComponent();
            log.Info("加载ModifyProfile");
            DataGridForChange.ItemsSource = GetFileMessage();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Server";
            if (ofd.ShowDialog() == true)
            {
                infos.Add(new Info { path = ofd.FileName, way = "" });
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridForChange.SelectedItem != null)
            {
                Info DRV = (Info)DataGridForChange.SelectedItem;
                String name = DRV.path;

                //删除infos里对应的数据
                for (int i = 0; i < infos.Count; i++)
                {
                    if (infos[i].path.CompareTo(name) == 0)
                    {
                        infos.Remove(infos[i]);
                        break;
                    }
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            string configureListDir = System.IO.Path.Combine(currentPath + @"\Server\configureList");
            if (!System.IO.Directory.Exists(configureListDir))
            {
                System.IO.Directory.CreateDirectory(configureListDir);
            }
            sfd.InitialDirectory = configureListDir;
            //设置保存的文件的类型
            sfd.Filter = "INI配置文件|*.ini";
            if (sfd.ShowDialog() == true)
            {
                List<File> configFiles = new List<File>();
                for (int i = 0; i < infos.Count; i++)
                {
                    FileInfo fileInfo = new FileInfo(infos[i].path);
                    File file = new File(fileInfo.Name, fileInfo.Length, infos[i].way, fileInfo.LastWriteTime.ToString(), fileInfo.FullName);
                    configFiles.Add(file);
                }
                //写入ini文件
                for (int i = 0; i < configFiles.Count; i++)
                {
                    File f = configFiles[i];
                    WritePrivateProfileString("session" + i, "fileName", f.FileName, sfd.FileName);
                    WritePrivateProfileString("session" + i, "fileSize", f.FileSize.ToString(), sfd.FileName);
                    WritePrivateProfileString("session" + i, "hashcode", f.Hashcode.ToString(), sfd.FileName);
                    WritePrivateProfileString("session" + i, "updateMethod", f.UpdateMethod, sfd.FileName);
                    WritePrivateProfileString("session" + i, "lastModified", f.LastModified, sfd.FileName);
                    WritePrivateProfileString("session" + i, "path", f.Path, sfd.FileName);
                }
                MessageBox.Show("保存成功");
                log.Info("另存配置文件: "+sfd.SafeFileName + "成功");
            }
            else
            {
                MessageBox.Show("取消保存");
            }
        }

        public ObservableCollection<Info> GetFileMessage()
        {
            infos.Clear();
            string fileDir = Environment.CurrentDirectory;
            //fileName.txt缓存选中的行
            String fileDirResp = Read(fileDir + @"\fileName.txt");
            string configureListDir = System.IO.Path.Combine(fileDir + @"\Server\configureList");
            if (!System.IO.Directory.Exists(configureListDir))
            {
                System.IO.Directory.CreateDirectory(configureListDir);
            }
            DirectoryInfo fileFold = new DirectoryInfo(configureListDir);
            FileInfo[] files = fileFold.GetFiles(); //获取指定文件夹下的所有文件
            this.FileNameText.Text = fileDirResp;
            for (int i = 0; files != null && i < files.Length; i++)  //将文件信息添加到List里面  
            {
                if (files[i].Name == fileDirResp)   //挑选出符合条件的信息  
                {
                    IniFiles ini_file_read = new IniFiles(configureListDir + @"\"+fileDirResp);
                    for(int j = 0; j < 10000; j++)
                    {
                        String tem_path = "session"+j.ToString();
                        String tem_file_path = ini_file_read.IniReadvalue(tem_path, "path");
                        String tem_file_updateMethod = ini_file_read.IniReadvalue(tem_path, "updateMethod");
                        
                        if(tem_file_path == "")
                        {
                            break;
                        }
                        infos.Add(new Info { path = tem_file_path, way = tem_file_updateMethod });
                    }
                    break;
                }  
            }
            return infos;
        }

        public String Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            line = sr.ReadLine();
            return line;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] recArgs;
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string[] args)
        {
            InitializeComponent();
            if(args.Length > 0)
            {
                string name = "";
                Thread.Sleep(3000);
                string current = System.Environment.CurrentDirectory;
                string tem_file_name2 = args[1];
                DirectoryInfo Fold = new DirectoryInfo(current);
                FileInfo[] fs = Fold.GetFiles();
                for (int i = 0; fs != null && i < fs.Length; i++)  //将文件信息添加到List里面  
                {
                    if (fs[i].Extension == ".exe")   //挑选出符合条件的信息  
                    {
                        if (fs[i].Name != "update_its.exe")
                            name = fs[i].Name;
                    }
                }
                string delete_path1 = current + "\\" + name;
                System.IO.File.Delete(delete_path1);
                System.IO.File.Copy(current + @"\temp\" + tem_file_name2, current + @"\" + tem_file_name2, true);
                System.Diagnostics.Process.Start(current + "\\" + tem_file_name2);
            }
            
            Environment.Exit(0);
        }
    }
}

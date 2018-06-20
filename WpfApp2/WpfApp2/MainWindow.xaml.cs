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
                string tem_file_name2 = args[1];
                string current = args[2];
                string delete_path1 = current + "\\test.exe";
                System.IO.File.Delete(delete_path1);
                System.Diagnostics.Process.Start(current + "\\" + tem_file_name2);
            }
            
            Environment.Exit(0);
        }
    }
}

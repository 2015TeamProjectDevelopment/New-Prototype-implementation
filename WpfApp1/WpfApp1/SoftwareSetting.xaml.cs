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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// softwareSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SoftwareSetting : Window
    {
        string currentPath = System.IO.Directory.GetCurrentDirectory();
        string UrlPath = System.IO.Directory.GetCurrentDirectory() + "//url.txt";
        public SoftwareSetting()
        {
            InitializeComponent();
            InitUrl();
        }

        private void InitUrl()
        {
            if (!System.IO.File.Exists(UrlPath))
            {
                this.URLText.Text = currentPath;
                using (FileStream fs = new FileStream(UrlPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(this.URLText.Text.Trim());
                    }
                }
            }
            else
            {
                StreamReader sr = new StreamReader(UrlPath, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    this.URLText.Text = line;
                }
            }
            
        }

        public static bool CheckUri(string strUri)
        {
            try
            {
                Uri uriAddress = new Uri(strUri);
                string localPath = uriAddress.LocalPath;
                Console.WriteLine(localPath);
                if (!System.IO.Directory.Exists(localPath))
                {
                    return false;
                }
                else if (strUri.StartsWith("http"))
                {
                    System.Net.HttpWebRequest.Create(strUri).GetResponse();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void sureButton_Click(object sender, RoutedEventArgs e)
        {
            //检查一下是否合法 
            if (!CheckUri(this.URLText.Text.Trim()))
            {
                System.Windows.MessageBox.Show("URL不合法", "ERROR");
            }
            else
            {
                //把URL存入本地文件？
                using (FileStream fs = new FileStream(UrlPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(this.URLText.Text.Trim());
                    }
                }
                this.Close();
            }
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}

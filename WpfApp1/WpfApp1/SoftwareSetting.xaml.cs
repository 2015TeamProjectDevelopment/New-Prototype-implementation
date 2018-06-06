using System;
using System.IO;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// softwareSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SoftwareSetting : Window
    {
        private static readonly log4net.ILog log =
    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string currentPath = System.IO.Directory.GetCurrentDirectory();
        string UrlPath = System.IO.Directory.GetCurrentDirectory() + @"/url.txt";
        public SoftwareSetting()
        {
            InitializeComponent();
            log.Info("加载SoftwareSetting");
            InitUrl();
        }

        private void InitUrl()
        {

            StreamReader sr = new StreamReader(UrlPath, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                this.URLText.Text = line;
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

        private void SureButton_Click(object sender, RoutedEventArgs e)
        {
            //检查一下是否合法 
            if (!CheckUri(this.URLText.Text.Trim()))
            {
                System.Windows.MessageBox.Show("URL不合法", "ERROR");
                log.Error("URL不合法");
            }
            else
            {
                //把URL存入本地文件
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}

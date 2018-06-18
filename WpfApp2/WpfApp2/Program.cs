using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CustomApplication app = new CustomApplication();
            //MainWindow window = new MainWindow(args);  
            //app.MainWindow = window;  
            app.Run();
        }
    }

    class CustomApplication : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow(e.Args);
            window.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Update
    {
        static string current = System.IO.Directory.GetCurrentDirectory();
        private string sourceUrl = "";
        private string localUrl = current + @"\temp";
        private string fileName = "newest.ini";
        
        public string SourceUrl { get => sourceUrl; set => sourceUrl = value; }
        public string LocalUrl { get => localUrl; set => localUrl = value; }
        public string FileName { get => fileName; set => fileName = value; }

        public bool Upadte(String url)
        {
            sourceUrl = url;
            WebClient webClient = new WebClient();
            Check check = new Check(sourceUrl, localUrl+@"\"+fileName);
            if (!System.IO.Directory.Exists(localUrl))
            {
                System.IO.Directory.CreateDirectory(localUrl);
            }
            else
            {
                Directory.Delete(localUrl, true);
                System.IO.Directory.CreateDirectory(localUrl);
            }
            if (check.Checking())
            {
                string fileDir = localUrl;
                string filePath = localUrl + @"\" + fileName;
                IniFiles ini_file_read = new IniFiles(filePath);
                for (int j = 0; j < 10000; j++)
                {
                    String temp_session = "session" + j.ToString();
                    String temp_file_name = ini_file_read.IniReadvalue(temp_session, "fileName");
                    String temp_file_path = ini_file_read.IniReadvalue(temp_session, "path");
                    String tem_file_updateMethod = ini_file_read.IniReadvalue(temp_session, "updateMethod");
                    if (temp_file_path == "")
                    {
                        break;
                    }
                    // 每一个都使用url进行下载
                    temp_file_path = "file:///" + temp_file_path;
                    Console.WriteLine(temp_file_path);
                    webClient.DownloadFile(temp_file_path, fileDir + @"\" + temp_file_name);
                    UpdateFile(fileDir, current, temp_file_name, tem_file_updateMethod); 
                }
                UpdateIni(current);
                return true;
            }
            return false;
        }

        private void UpdateFile(string downloadPath, string targetPath, string name, string updateMethod )
        {
            if (updateMethod == "新增")
            {
                System.IO.File.Copy(downloadPath + "\\" + name,
                    targetPath + "\\" + name, true);
            }
            else if (updateMethod == "删除")
            {
                System.IO.File.Delete(targetPath + "\\" + name);
            }
            else if (updateMethod == "替换")
            {
                //先删除后复制
                System.IO.File.Delete(targetPath + "\\" + name);
                System.IO.File.Copy(downloadPath + "\\" + name,
                    targetPath + "\\" + name, true);
            }
            //应该放在底部，否则会导致软件直接退出，其它操作没有进行。
            else if (updateMethod == "更新本软件")
            {
                string targetSoftwarePath = name;
                System.Diagnostics.Process.Start(@".\update_its.exe", targetSoftwarePath);
                UpdateIni(current);
                Environment.Exit(0);
            }
        }

        private void UpdateIni(string PcPath)
        {
            //更新PC的ini
            DirectoryInfo Fold = new DirectoryInfo(PcPath);
            FileInfo[] fs = Fold.GetFiles();
            for (int i = 0; fs != null && i < fs.Length; i++)  //将文件信息添加到List里面  
            {
                if (fs[i].Extension == ".ini")   //挑选出符合条件的信息  
                {
                    //删除原来的
                    System.IO.File.Delete(PcPath + "\\" + fs[i].Name);
                    break;
                }
            }

            //复制新的进去
            System.IO.File.Copy(localUrl + @"\" + fileName,
                PcPath + "\\" + fileName, true);
        }

    }

    public class Check
    {
        private string localIniPath = "";
        private string downloadIniUrl = "";
        public Check(string downloadIniUrl, string localIniPath)
        {
            this.downloadIniUrl = downloadIniUrl;
            this.localIniPath = localIniPath;
        }

        public bool Checking()
        {
            string LocalFileMD5 = null;
            string fileDir = Environment.CurrentDirectory;
            DirectoryInfo fileFold = new DirectoryInfo(fileDir);
            FileInfo[] files = fileFold.GetFiles();
            for (int i = 0; files != null && i < files.Length; i++)  //找出ini
            {
                if (files[i].Extension == ".ini")   //挑选出符合条件的信息  
                {
                    LocalFileMD5 = CreateMd5(files[i].FullName);
                    break;
                }
                else
                {
                    continue;
                }
            }
            WebClient webClient = new WebClient();
            webClient.DownloadFile(downloadIniUrl, localIniPath);
            string UrlFileMD5 = CreateMd5(localIniPath);
            
            if (UrlFileMD5 != LocalFileMD5)
            {
                return true;
            }
            return false;
        }

        private static string CreateMd5(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));
            int bufferSize = 1024 * 16;//自定义缓冲区大小16K  
            byte[] buffer = new byte[bufferSize];
            Stream inputStream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
            int readLength = 0;//每次读取长度  
            var output = new byte[bufferSize];
            while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                //计算MD5  
                hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
            }
            //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            string md5 = BitConverter.ToString(hashAlgorithm.Hash);
            hashAlgorithm.Clear();
            inputStream.Close();
            md5 = md5.Replace("-", "");
            return md5;
        }
    }

    class IniFiles
    {
        public string path;
        [DllImport("kernel32")] //返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")] //返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 保存ini文件的路径
        /// 调用示例：var ini = IniFiles("C:\file.ini");
        /// </summary>
        /// <param name="INIPath"></param>
        public IniFiles(string iniPath)
        {
            this.path = iniPath;
        }
        /// <summary>
        /// 写Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name","localhost");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <param name="value">值</param>
        public void IniWritevalue(string Section, string Key, string value)
        {
            WritePrivateProfileString(Section, Key, value, this.path);
        }
        /// <summary>
        /// 读Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <returns>值</returns>
        public string IniReadvalue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }
}

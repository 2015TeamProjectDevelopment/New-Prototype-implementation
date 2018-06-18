using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class File
    {
        private String fileName;        //文件名
        private long fileSize;          //文件大小
        private int hashcode;           //哈希码
        private String updateMethod;    //更新操作
        private String lastModified;    //最后修改时间
        private String path;            //路径

        public File()
        {

        }

        //构造函数
        public File(string fileName, long fileSize, string updateMethod, string lastModified, string path)
        {
            this.fileName = fileName;
            this.fileSize = fileSize;
            this.hashcode = GetHashCode();
            this.updateMethod = updateMethod;
            this.lastModified = lastModified;
            this.path = path;
        }

        public string FileName { get => fileName; set => fileName = value; }
        public long FileSize { get => fileSize; set => fileSize = value; }
        public int Hashcode { get => hashcode; set => hashcode = value; }
        public string UpdateMethod { get => updateMethod; set => updateMethod = value; }
        public string LastModified { get => lastModified; set => lastModified = value; }
        public string Path { get => path; set => path = value; }

        public override bool Equals(object obj)
        {
            var file = obj as File;
            return file != null &&
                   fileName == file.fileName &&
                   fileSize == file.fileSize &&
                   hashcode == file.hashcode &&
                   updateMethod == file.updateMethod &&
                   lastModified == file.lastModified &&
                   path == file.path;
        }

        public override int GetHashCode()
        {
            var hashCode = 2096143202;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileSize.ToString());
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(updateMethod);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(lastModified);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(path);
            return hashCode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MD5CryptoServiceProvider = System.Security.Cryptography.MD5CryptoServiceProvider;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Appacitive.Sdk.Tests
{
    public static class FileHelper
    {
        public static async Task<string> UploadTestFileAsync()
        {
            // Upload file
            var filePath = TestFilePath;
            var upload = new FileUpload("image/png", Unique.String + ".png");
            return await upload.UploadFileAsync(filePath);
        }

        private static string _filePath = string.Empty;
        public static string TestFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_filePath) == true)
                {
                    FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    _filePath = info.Directory.FullName.EndsWith(@"\") ? info.Directory.FullName + "logo.png" :
                        info.Directory.FullName + @"\logo.png";
                }
                return _filePath;
            }
        }
        private static readonly string ReferenceMd5Hash = Md5.CalculateHash(FileHelper.TestFilePath);
        
        public static string GenerateNewDownloadFilePath()
        {

            FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var path = info.Directory.FullName.EndsWith(@"\") ? info.Directory.FullName + Unique.String + ".png" :
                info.Directory.FullName + @"\" + Unique.String + ".png";
            return path;

        }

        public static bool Md5ChecksumMatch(string file)
        {
            return Md5ChecksumMatch(File.ReadAllBytes(file));
        }

        public static bool Md5ChecksumMatch(byte[] bytes)
        {
            var hash = Md5.CalculateHash(bytes);
            return string.Compare(hash, ReferenceMd5Hash, true) == 0;
        }
    }

    internal class Md5
    {
        public static string CalculateHash(string filepath)
        {
            return CalculateHash(File.ReadAllBytes(filepath));
        }

        public static string CalculateHash( byte[] bytes)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(bytes);
                return Encoding.Unicode.GetString(hash);
            }
        }
    }
}

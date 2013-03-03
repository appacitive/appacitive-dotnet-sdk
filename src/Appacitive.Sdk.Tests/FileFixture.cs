using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class FileFixture
    {
        [TestMethod]
        public async Task GetUploadUrlWithFilenameTest()
        {
            var filename = Unique.String + ".png";
            var upload = new FileUpload("image/png", filename);
            var fileUrl = await upload.GetUploadUrlAsync(2);
            Assert.IsNotNull(fileUrl);
            Assert.IsFalse( string.IsNullOrWhiteSpace(fileUrl.FileName));
            Console.WriteLine("Filename: {0}", fileUrl.FileName);
            Assert.IsTrue(string.Compare(filename, fileUrl.FileName, true) == 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(fileUrl.Url));
            Console.WriteLine("Upload url: {0}", fileUrl.Url);
        }

        [TestMethod]
        public async Task GetUploadUrlWithoutFilenameTest()
        {
            var upload = new FileUpload("image/png");
            var fileUrl = await upload.GetUploadUrlAsync(2);
            Assert.IsNotNull(fileUrl);
            Assert.IsFalse(string.IsNullOrWhiteSpace(fileUrl.FileName));
            Console.WriteLine("Filename: {0}", fileUrl.FileName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(fileUrl.Url));
            Console.WriteLine("Upload url: {0}", fileUrl.Url);
        }

        [TestMethod]
        public async Task DataUploadAsyncTest()
        {
            var bytes = File.ReadAllBytes(FileHelper.TestFilePath);
            var filename = Unique.String + ".png";
            Console.WriteLine("Generated file name: {0}", filename);
            var handler = new FileUpload("image/png", filename);
            var uploadedFilename = await handler.UploadAsync(bytes);
            Console.WriteLine("Uploaded to file {0}", uploadedFilename);
        }

        [TestMethod]
        public async Task FileUploadAsyncTest()
        {
            var file = FileHelper.TestFilePath;
            var filename = Unique.String + ".png";
            Console.WriteLine("Generated file name: {0}", filename);
            var handler = new FileUpload("image/png", filename);
            var uploadedFilename = await handler.UploadFileAsync(file);
            Console.WriteLine("Uploaded to file {0}", uploadedFilename);
        }

        [TestMethod] 
        public async Task GetDownloadUrlAsyncTest()
        {
            // Upload file
            var filePath = FileHelper.TestFilePath;
            var upload = new FileUpload("image/png", Unique.String + ".png");
            var filename = await upload.UploadFileAsync(filePath);

            // Get download url
            var download = new FileDownload(filename);
            var url = await download.GetDownloadUrl();
            Assert.IsFalse(string.IsNullOrWhiteSpace(url));
            Console.WriteLine("Download url: {0}", url);
            
        }

        [TestMethod]
        public async Task DownloadAsyncTest()
        {
            // Upload a new file
            var filename = await FileHelper.UploadTestFileAsync();

            // Download the file
            var bytes = await new FileDownload(filename).DownloadAsync();
            Assert.IsTrue(FileHelper.Md5ChecksumMatch(bytes));
        }

        [TestMethod]
        public async Task DownloadFileAsyncTest()
        {
            // Upload a new file
            var filename = await FileHelper.UploadTestFileAsync();
            var downloadTo = FileHelper.GenerateNewDownloadFilePath();
            // Download the file
            await new FileDownload(filename).DownloadFileAsync(downloadTo);
            Assert.IsTrue(FileHelper.Md5ChecksumMatch(downloadTo));
        }


        [TestMethod]
        [ExpectedException( typeof(AppacitiveException))]
        public async Task DownloadNonExistingFileTest()
        {
            // Upload file
            var filename = Unique.String + ".png";
            
            // Get download url
            var download = new FileDownload(filename);
            var content = await download.DownloadAsync();
        }


        

        
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MONO
using NUnit.Framework;
using MonoTouch.Foundation;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Appacitive.Sdk.Tests
{
	#if MONO
	[TestFixture]
	#else
	[TestClass]
	#endif
    public class FileFixture
    {
		#if MONO
		[TestFixtureSetUp]
		public void Setup()
		{
			OneTimeSetup.Run ();
		}
		#endif

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
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

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
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

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task DataUploadAsyncTest()
        {
            var bytes = File.ReadAllBytes(FileHelper.TestFilePath);
            var filename = Unique.String + ".png";
            Console.WriteLine("Generated file name: {0}", filename);
            var handler = new FileUpload("image/png", filename);
            handler.UploadProgressChanged += (s, e) =>
                {
                    Console.WriteLine("Uploading bytes {0} out of {1}.",
                        e.BytesSent, e.TotalBytesToSend);
                };
            handler.UploadCompleted += (s, e) =>
                {
                    Console.WriteLine("Upload completed.");
                };
            var uploadedFilename = await handler.UploadAsync(bytes);
            Console.WriteLine("Uploaded to file {0}", uploadedFilename);
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task FileUploadAsyncTest()
        {
            var file = FileHelper.TestFilePath;
            var filename = Unique.String + ".png";
            Console.WriteLine("Generated file name: {0}", filename);
            var handler = new FileUpload("image/png", filename);
            handler.UploadProgressChanged += (s, e) =>
            {
                Console.WriteLine("Uploading bytes {0} out of {1}.",
                    e.BytesSent, e.TotalBytesToSend);
            };
            handler.UploadCompleted +=  (s, e) =>
            {
                Console.WriteLine("Upload completed.");
            };
            var uploadedFilename = await handler.UploadFileAsync(file);
            Console.WriteLine("Uploaded to file {0}", uploadedFilename);
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif 
        public async Task GetDownloadUrlAsyncTest()
        {
            // Upload file
            var filePath = FileHelper.TestFilePath;
            var upload = new FileUpload("image/png", Unique.String + ".png");
            var filename = await upload.UploadFileAsync(filePath);
            upload.UploadCompleted += (s, e) =>
                {
                    Console.WriteLine("Upload completed.");
                };
            // Get download url
            var download = new FileDownload(filename);
            var url = await download.GetDownloadUrlAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(url));
            Console.WriteLine("Download url: {0}", url);
            
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task DownloadAsyncTest()
        {
            // Upload a new file
            var filename = await FileHelper.UploadTestFileAsync();

            // Download the file
            var handler = new FileDownload(filename);
            handler.DownloadCompleted += (s, e) =>
                {
                    Console.WriteLine("Download completed. Downloaded {0} bytes.", e.Result.Length);
                };
            handler.DownloadProgressChanged += (s, e) =>
                {
                    Console.WriteLine("Downloading {0} bytes out of {1}.", e.BytesReceived, e.TotalBytesToReceive);
                };
            var bytes = await handler.DownloadAsync();
            Assert.IsTrue(FileHelper.Md5ChecksumMatch(bytes));
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task DownloadFileAsyncTest()
        {
            // Upload a new file
            var filename = await FileHelper.UploadTestFileAsync();
            var downloadTo = FileHelper.GenerateNewDownloadFilePath();
            // Download the file
            var handler = new FileDownload(filename);
            handler.DownloadCompleted += (s, e) =>
            {
                Console.WriteLine("Download completed. Downloaded {0} bytes.", e.Result.Length);
            };
            handler.DownloadProgressChanged += (s, e) =>
            {
                Console.WriteLine("Downloading {0} bytes out of {1}.", e.BytesReceived, e.TotalBytesToReceive);
            };
            await handler.DownloadFileAsync(downloadTo);
            Assert.IsTrue(FileHelper.Md5ChecksumMatch(downloadTo));
        }


        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        [ExpectedException( typeof(AppacitiveApiException))]
        public async Task DownloadNonExistingFileTest()
        {
            // Upload file
            var filename = Unique.String + ".png";
            
            // Get download url
            var download = new FileDownload(filename);
            await download.DownloadAsync();
        }
    }
}

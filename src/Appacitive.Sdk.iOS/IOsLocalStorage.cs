using System;
using Appacitive.Sdk.Internal;
using MonoTouch.Foundation;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appacitive.Sdk.iOS
{
	public class IOsLocalStorage : ILocalStorage
	{
		public void SetValue (string key, string value)
		{
			if (value != null) 
			{
				WriteToFileAsync (key, value);
			} 
			else 
			{
				Remove (key);
			}
		}

		public string GetValue (string key, string defaultValue = null)
		{
			var filename = GetFileName (key);
			if (File.Exists (filename) == false)
				return defaultValue;
			else
				return ReadFile (filename);
		}

		private string ReadFile (string filename)
		{
			return File.ReadAllText (filename);
		}

		public void Remove (string key)
		{
			DeleteFileAsync (key);
		}

		void DeleteFileAsync (string key)
		{
			var filename = GetFileName (key);
			Task.Factory.StartNew (() => DeleteFile (filename));
		}

		private void DeleteFile(string fileName )
		{
			try 
			{
				if( File.Exists(fileName) == true )
					File.Delete(fileName);
			}
			catch{}
		}

		private void WriteToFileAsync(string key, string value)
		{
			var fileName = GetFileName (key);
			using (var stream = new FileStream (fileName, FileMode.OpenOrCreate)) 
			{
				var bytes = Encoding.UTF8.GetBytes (value);
				stream.BeginWrite (bytes, 0, bytes.Length, 
					ar => 
					{
						try 
						{
							stream.EndWrite(ar);
						}
						catch{}
					}, null);
			}
		}

		private string GetFileName(string key )
		{
			return "Library/Caches/" + key.ToLower() + ".cache";
		}
	}
}


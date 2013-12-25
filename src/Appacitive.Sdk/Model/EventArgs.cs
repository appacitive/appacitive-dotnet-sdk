using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class DownloadCompletedEventArgs : AsyncCompletedEventArgs
    {
        public DownloadCompletedEventArgs(byte[] results, Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            this.Result = results;
        }

        public byte[] Result { get; private set; }
    }

    public class UploadCompletedEventArgs : AsyncCompletedEventArgs
    {
        public UploadCompletedEventArgs(Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {   
        }
    }
    
    public class DownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, int progressPercentage, object userState)
            : base(progressPercentage, userState)
        {
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
        }

        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }
    }

    public class UploadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public UploadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, long bytesSent, long totalBytesToSend,
            int progressPercentage, object userState)
            : base(progressPercentage, userState)
        {
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
            this.BytesSent = bytesSent;
            this.TotalBytesToSend = totalBytesToSend;
        }

        public long BytesReceived { get; private set; }
        public long BytesSent { get; private set; }
        public long TotalBytesToReceive { get; private set; }
        public long TotalBytesToSend { get; private set; }
    }
}

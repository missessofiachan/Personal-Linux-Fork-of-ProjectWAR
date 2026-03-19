using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Launcher
{
    /// <summary>
    /// Provides HTTP helper methods for manifest requests and streamed patch downloads.
    /// </summary>
    public static class HttpUtil
    {
        /// <summary>
        /// Reports streamed download progress.
        /// </summary>
        /// <param name="current">The number of bytes downloaded so far.</param>
        /// <param name="total">The total number of bytes expected.</param>
        /// <param name="chunkSize">The size of the most recent chunk.</param>
        public delegate void StreamProgress(long current, long total, int chunkSize);

        /// <summary>
        /// Sends a POST request to the patch server and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The response type to deserialize.</typeparam>
        /// <param name="address">The patch server address.</param>
        /// <param name="endpoint">The endpoint to request.</param>
        /// <param name="retry">The number of retry attempts before failing.</param>
        /// <returns>The deserialized response payload.</returns>
        public static async Task<T> RequestAsync<T>(string address, string endpoint, int retry = 5)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt < retry; attempt++)
            {
                try
                {
                    ServicePointManager.DefaultConnectionLimit = 1600;
                    string path = $@"http://{address}/{endpoint}";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
                    request.Proxy = null;
                    request.Method = "POST";
                    request.ContentLength = 0;

                    using (WebResponse response = await request.GetResponseAsync())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return (T)new JavaScriptSerializer().Deserialize(reader.ReadToEnd(), typeof(T));
                    }
                }
                catch (Exception exception)
                {
                    lastException = exception;
                }

                await Task.Delay(2000);
            }

            throw lastException;
        }

        /// <summary>
        /// Streams a file download from the patch server into the provided output stream.
        /// </summary>
        /// <param name="address">The patch server address.</param>
        /// <param name="endpoint">The endpoint to request.</param>
        /// <param name="output">The destination stream for the downloaded bytes.</param>
        /// <param name="progress">The callback that receives progress updates.</param>
        /// <param name="blockSize">The size of the transfer buffer.</param>
        public static void RequestStream(string address, string endpoint, Stream output, StreamProgress progress, int blockSize = 0x8FFFF)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($@"http://{address}/{endpoint}");
                request.ContentType = "application/json";
                request.Method = "POST";

                using (request.GetRequestStream())
                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    long current = 0;
                    long total = response.ContentLength;
                    byte[] block = new byte[blockSize];

                    while (current < total)
                    {
                        int read = stream.Read(block, 0, blockSize);
                        if (read <= 0)
                        {
                            progress(current, total, read);
                            break;
                        }

                        output.Write(block, 0, read);
                        current += read;
                        progress(current, total, read);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}

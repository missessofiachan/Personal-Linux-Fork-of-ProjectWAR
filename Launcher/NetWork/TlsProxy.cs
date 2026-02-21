using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Launcher
{
    public class TlsProxy
    {
        private TcpListener _localListener;
        private string _remoteHost;
        private int _remotePort;

        public TlsProxy(int localPort, string remoteHost, int remotePort)
        {
            _localListener = new TcpListener(IPAddress.Loopback, localPort);
            _remoteHost = remoteHost;
            _remotePort = remotePort;
        }

        public void Start()
        {
            _localListener.Start();
            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        public void Stop()
        {
            _localListener.Stop();
        }

        private void AcceptClients()
        {
            try
            {
                while (true)
                {
                    TcpClient client = _localListener.AcceptTcpClient();
                    Thread proxyThread = new Thread(() => HandleClient(client));
                    proxyThread.IsBackground = true;
                    proxyThread.Start();
                }
            }
            catch (Exception)
            {
                // Listener stopped
            }
        }

        private void HandleClient(TcpClient localClient)
        {
            try
            {
                using (TcpClient remoteClient = new TcpClient(_remoteHost, _remotePort))
                using (SslStream remoteStream = new SslStream(remoteClient.GetStream(), false, ValidateServerCertificate, null))
                {
                    remoteStream.AuthenticateAsClient("ProjectWAR");

                    using (NetworkStream localStream = localClient.GetStream())
                    {
                        Thread uploadThread = new Thread(() => ShuttleData(localStream, remoteStream));
                        uploadThread.IsBackground = true;
                        uploadThread.Start();

                        ShuttleData(remoteStream, localStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Proxy Error: " + ex.Message);
            }
            finally
            {
                localClient.Close();
            }
        }

        private void ShuttleData(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            try
            {
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                    outputStream.Flush();
                }
            }
            catch (Exception)
            {
                // Connection closed or error
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // We use a self-signed cert, so unconditionally accept
        }
    }
}


 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace FrameWork
{
    [RpcAttribute(false, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 0)]
    public class ClientMgr : RpcObject
    {
        public override void OnClientConnected(RpcClientInfo Info)
        {
            Log.Info("ClientMgr", Info.Description() + " | Connected");
        }

        public override void OnClientDisconnected(RpcClientInfo Info)
        {
            Log.Info("ClientMgr", Info.Description() + " | Disconnected");
        }

        public override void OnServerConnected()
        {
            Log.Info("ClientMgr", "Server connected !");
        }

        public override void OnServerDisconnected()
        {
            Log.Info("ClientMgr", "Server disconnected !");

        }

        public void Ping()
        {

        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}

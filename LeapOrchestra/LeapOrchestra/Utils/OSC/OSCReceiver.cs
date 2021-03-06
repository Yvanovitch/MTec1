#region licence/info
// OSC.NET - Open Sound Control for .NET
// http://luvtechno.net/
//
// Copyright (c) 2006, Yoshinori Kawasaki 
// All rights reserved.
//
// Changes and improvements:
// Copyright (c) 2005-2008 Martin Kaltenbrunner <mkalten@iua.upf.edu>
// As included with    
// http://reactivision.sourceforge.net/
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// * Neither the name of "luvtechno.net" nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY 
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion licence/info

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Utils.OSC
{
	/// <summary>
	/// OSCReceiver
	/// </summary>
	public class OSCReceiver
	{
		protected UdpClient udpClient;
		protected int localPort;
        private Thread OSCManagement;
        public event Action SendBang;
        public event Action<float> SendOrientation;
        public event Action<int> EvolvePartCursor;

		public OSCReceiver(int localPort)
		{
			this.localPort = localPort;
			Connect();
            OSCManagement = new Thread(this.Process);
            //Lancement du thread de managament
            OSCManagement.Start();
            Console.WriteLine("OSC Ready on port 8000.");
            Console.WriteLine("--> Use /bang or /beatNumber {1, 2, 3, 4} or /orientation [-1; 1]"
                +Environment.NewLine);
		}

		public void Connect()
		{
			if(this.udpClient != null) Close();
			this.udpClient = new UdpClient(this.localPort);
		}

		public void Close()
		{
			if (this.udpClient!=null) this.udpClient.Close();
			this.udpClient = null;
            OSCManagement.Abort();
		}

        public void Process()
        {
            OSCPacket OSCpacket;
            while (true)
            {
                OSCpacket = this.Receive();
                //Console.Write("OSC Packet Received: ");
                if (OSCpacket.Address == "/orientation")
                {
                    foreach (var elem in OSCPacket.Unpack(OSCpacket.BinaryData).Values)
                    {
                        if (elem is float)
                        {
                            SendOrientation((float)elem);
                        }
                    }
                }
                else if (OSCpacket.Address == "/beatNumber")
                {
                    foreach (var elem in OSCPacket.Unpack(OSCpacket.BinaryData).Values)
                    {
                        if (elem is int)
                        {
                            //Console.WriteLine("elem : " + elem);
                            EvolvePartCursor((int)elem);
                        }
                    }
                }
                else if (OSCpacket.Address == "/bang")
                {
                    SendBang(); 
                }
                else
                    Console.WriteLine("Use Adresse : \"/orientation\"");
            }
        }

		public OSCPacket Receive()
		{
            try
            {
                IPEndPoint ip = null;
                byte[] bytes = this.udpClient.Receive(ref ip);
                if (bytes != null && bytes.Length > 0)
                    return OSCPacket.Unpack(bytes);

            } catch (Exception e) { 
                Console.WriteLine(e.Message);
                return null;
            }

			return null;
		}
	}
}

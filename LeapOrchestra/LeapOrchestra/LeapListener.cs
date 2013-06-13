using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace LeapOrchestra
{
    class LeapListener : Listener
    {
        public override void OnConnect(Controller arg0)
        {
            base.OnConnect(arg0);
            Console.WriteLine("Leap Motion connecté");
        }

        public override void OnExit(Controller arg0)
        {
            base.OnExit(arg0);
        }

        public override void OnDisconnect(Controller arg0)
        {
            base.OnDisconnect(arg0);
        }

        public override void OnInit(Controller arg0)
        {
            base.OnInit(arg0);
        }

        public override void OnFrame(Controller arg0)
        {
            base.OnFrame(arg0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra.Sensor
{
    class LeapController : Controller
    {
        public void AddListenerThreadable(object data)
        {
            if (data is LeapListener)
            {
                LeapListener listener = (LeapListener)data;
                base.AddListener(listener);
            }
            else
            {
                Console.WriteLine("Erreur LeapController : pas un listener");
            }
        }
    }
}

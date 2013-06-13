﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra
{
    class LeapModel
    {
        private int previousSpeed1;
        private int previousSpeed2;
        private int top;
        private long temp;
        private long tempo;

        public event Action sendBang;

        public LeapModel()
        {
            previousSpeed1 = 0;
            previousSpeed2 = 0;
            top = 0;
            temp = 0;
            tempo = 0;
        }

        public void OnFrameRegistered(Frame frame)
        {
            OnFingersRegistered(frame.Fingers, frame);
        }

        public void OnFingersRegistered(FingerList fingers, Frame frame)
        {
            Finger finger = frame.Fingers[0];
            var tipVelocity = (int)finger.TipVelocity.Magnitude;

            if (previousSpeed1 < tipVelocity && previousSpeed1 < previousSpeed2 && tipVelocity < 500 && tempo > 100000)
            {
                top = 0;
            }
            else
            {
                top = 1;
            }

            tempo = frame.Timestamp - temp;

            if (top == 0)
            {
                Console.WriteLine(tempo);
                
                temp = frame.Timestamp;

                sendBang(); //On joue la note midi
            }

            previousSpeed2 = previousSpeed1;
            previousSpeed1 = tipVelocity;
        }
    }
}

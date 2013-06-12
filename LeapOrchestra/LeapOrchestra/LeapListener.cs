using System;
using Leap;

namespace ConsoleLeapMouse
{
    class LeapListener: Listener
    {
        private long currentTime;
        private long previousTime;
        private long timeChange;

        public override void OnFrame(Controller cntrlr)
        {
            // Get the current frame.
            Frame currentFrame = cntrlr.Frame();

            currentTime = currentFrame.Timestamp;
            timeChange = currentTime - previousTime;
           
            if (timeChange > 10000)
            {
                if (!currentFrame.Hands.Empty)
                {
                    // Get the first finger in the list of fingers
                    Finger finger = currentFrame.Fingers[0];
                    // Get the closest screen intercepting a ray projecting from the finger
                    Screen screen = cntrlr.CalibratedScreens.ClosestScreenHit(finger);

                    if (screen != null && screen.IsValid)
                    {
                        // Get the velocity of the finger tip
                        var tipVelocity = (int)finger.TipVelocity.Magnitude;

                        // Use tipVelocity to reduce jitters when attempting to hold
                        // the cursor steady
                        if (tipVelocity > 25)
                        {
                            var xScreenIntersect = screen.Intersect(finger, true).x;
                            var yScreenIntersect = screen.Intersect(finger, true).y;
                            
                            if (xScreenIntersect.ToString() != "NaN")
                            {
                                var x = (int)(xScreenIntersect * screen.WidthPixels);
                                var y = (int)(screen.HeightPixels - (yScreenIntersect * screen.HeightPixels));

                                Console.WriteLine("Screen intersect X: " + xScreenIntersect.ToString());
                                Console.WriteLine("Screen intersect Y: " + yScreenIntersect.ToString());
                                Console.WriteLine("Width pixels: " + screen.WidthPixels.ToString());
                                Console.WriteLine("Height pixels: " + screen.HeightPixels.ToString());

                                Console.WriteLine("\n");

                                Console.WriteLine("x: " + x.ToString());
                                Console.WriteLine("y: " + y.ToString());

                                Console.WriteLine("\n");

                                Console.WriteLine("Tip velocity: " + tipVelocity.ToString());

                                // Move the cursor
                                MouseCursor.MoveCursor(x, y);

                                Console.WriteLine("\n" + new String('=', 40) + "\n");
                            }

                        }
                    }

                }

                previousTime = currentTime;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace LeapOrchestra.Kinect
{
    class KinectController
    {
        private KinectSensor sensor;
        
        public KinectController()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                Console.WriteLine("Kinect connected");

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                    sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                Console.WriteLine("No sensor");
            }
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length != 0)
            {
                foreach (Skeleton skel in skeletons)
                {
                    printJoints(skel);
                }
            }
        }

        private void printJoints(Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.JointType == JointType.HandRight)
                {
                    if (joint.TrackingState == JointTrackingState.Tracked)
                    {
                        Console.WriteLine("Join  x : " + joint.Position.X * 100);
                    }
                    else if (joint.TrackingState == JointTrackingState.Inferred)
                    {
                        Console.WriteLine("Déduit Join x : " + joint.Position.X * 100);
                    }
                }
            }
        }

        public void Close()
        {
            sensor.Stop();
        }
    }
}

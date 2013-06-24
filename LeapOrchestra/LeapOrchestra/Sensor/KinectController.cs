using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace LeapOrchestra.Sensor
{
    class KinectController
    {
        private KinectSensor sensor;
        public event Action<SENSOR_TYPE, float, float, float> OnFrameEvent;
        private bool hadInformation;
        private JointType choosedJoint;
        
        public KinectController()
        {
            hadInformation = false;
            ChooseJoint();

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

                // Start the sensor!
                try
                {
                    sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    Console.WriteLine("Kinect Mode :" + sensor.SkeletonStream.TrackingMode);
                    Console.WriteLine("Kinect Status : " + sensor.Status.ToString());
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                Console.WriteLine("No Kinect sensor Detected");
            }
        }

        public void ChooseJoint()
        {
            string entry;
            Boolean choosed = false;
            int result;
            Console.WriteLine("Choose your hand : handRight -> 0, handLeft -> 1");
            while (!choosed)
            {
                entry = Console.ReadLine();
                if (int.TryParse(entry, out result))
                {
                    choosed = true; 
                    if (result == 1)
                        choosedJoint = JointType.HandLeft;
                    else
                        choosedJoint = JointType.HandRight;
                }
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
                    if (!hasInformation(skel))
                    {
                        if (hadInformation)
                        {
                            Console.WriteLine("Kinect arrested to send !");
                            hadInformation = false;
                        }
                        break;
                    }
                    
                    if (!hadInformation)
                    {
                        Console.WriteLine("Kinect start to send");
                        hadInformation = true;
                    }
                    //printJoints(skel);
                    Joint hand = getOneHand(skel);
                    OnFrameEvent(SENSOR_TYPE.KINECT, hand.Position.X*1000, hand.Position.Y*1000, hand.Position.Z*1000);
                    return; //On arrète au premier Skeleton
                }
            }
            else
            {
                Console.WriteLine("No skeleton");
            }
            
        }

        private Boolean hasInformation(Skeleton skel)
        {
            Boolean info = false;
            foreach (Joint joint in skel.Joints)
            {
                if (joint.Position.X != 0 || joint.Position.Y != 0 || joint.Position.Z != 0)
                {
                    info = true;
                }
            }
            return info;
        }
       
        private void printJoints(Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if(joint.Position.X != 0)
                    Console.WriteLine("Type : " + joint.JointType + " x :" + joint.Position.X);
            }
        }

        private Joint getOneHand(Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.JointType == choosedJoint)
                {
                    return joint;
                }
            }
            return new Joint();
        }

        public void Close()
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }
    }
}

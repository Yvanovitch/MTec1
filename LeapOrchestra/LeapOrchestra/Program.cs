using System;
using System.Threading;
using Leap;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LeapOrchestra.SongPlayer;
using Midi;
using Utils.OSC;
using LeapOrchestra.Sensor;

namespace LeapOrchestra
{
    class LeapOrchestra
    {
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("----- OrchestraHero Project -----");
            Console.WriteLine("by ISEN Student : Yvan RICHER, Gautier CARREE, Antoine DENOYELLE," +
                " Alexandre FALTOT, Lena DELVAL, Thomas JUSTER and Grégoire DESSIN");
            Console.WriteLine("Project Leader : Yvan RICHER" + Environment.NewLine);

        //Sound Management
            SoundManager soundManager = new SoundManager();
            Thread soundManagement = new Thread(soundManager.Process);
            //Lancement du thread de managament
            soundManagement.Start();

        //Sensor -> Kinect
            SensorManager sensorManager = new SensorManager();
            sensorManager.sensorModel.evolvePartCursor += soundManager.evolvePartCursor;
            sensorManager.sensorModel.SendOrientation += soundManager.SetCurrentOrientation;
            sensorManager.sensorModel.SendBang += soundManager.throwBang;
            soundManager.sendAnalysisBeatsNumber += sensorManager.sensorModel.setAnalysisBeatsNumber;

            Console.WriteLine("Choisir Fichier Midi dans l'interface");

        //Application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            form1.sendBang += soundManager.throwBang;
            form1.sendPath += soundManager.readMidiFile;
            form1.sendReady += soundManager.setInterfaceReady;
            form1.sendReady += sensorManager.sensorModel.setInterfaceReady;
            form1.sendReady += sensorManager.kinectController.setInterfaceReady;
            form1.sendOutputDevice += soundManager.GetOutputDevice;
            form1.setActiveSensor += sensorManager.sensorModel.useActiveSensor;
            soundManager.sendMeasureInfo += form1.GetMeasureInfo;
            sensorManager.sensorModel.sendPosition += form1.GetVector;
            soundManager.sendTempo += form1.GetTempo;
            //form1.prefosc.sendPort += receiver.GetPort;
            Application.Run(form1);

        // Fermeture de tout les threads
            soundManager.Close();
            soundManagement.Abort();
            sensorManager.Close();
        }
    }
}

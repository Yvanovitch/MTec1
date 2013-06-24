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
            Console.WriteLine("by ISEN Student : Yvan RICHER, Gauthier CARRE, Antoine DENOYELLE," +
                " Alexandre FALTOT, Thomas JUSTER and Grégoire DESSAIN");
            Console.WriteLine("Project Leader : Yvan RICHER" + Environment.NewLine);

        //Sound Management
            SoundManager soundManager = new SoundManager();
            //soundManager.readMidiFile(@"D:\Documents\Cours\Orchestra\Midi\In\link.mid");
            Thread soundManagement = new Thread(soundManager.Process);
            //Lancement du thread de managament
            soundManagement.Start();

        //Sensor -> Kinect
            SensorManager sensorManager = new SensorManager();
            sensorManager.sensorModel.evolvePartCursor += soundManager.evolvePartCursor;
            sensorManager.sensorModel.SendOrientation += soundManager.SetCurrentOrientation;

        //Application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            form1.sendBang += soundManager.throwBang;
            form1.sendPath += soundManager.readMidiFile;
            form1.sendOutputDevice += soundManager.GetOutputDevice;
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

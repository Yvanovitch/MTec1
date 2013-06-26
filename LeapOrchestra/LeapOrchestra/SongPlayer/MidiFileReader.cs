/*
 * Permet de lire un fichier midi
 * 
 * On enregistre toute les notes dans un tableau
 * On viendra à chaque fois calculer où on en est et envoyer les notes qu'il fallait lire depuis
 * 
 * Orientation :
 * On gère l'orientation du mec en allant de -1 (tourné d'un quart de tour vers la gauche) à 1 (droite)
 * 
 * TO DO :
 * Certaines notes restent parfois bloquée en ON. 
 * Je n'arrive pas à déterminer d'où vient le problème.
 * 
 * Solution possible :
 * Enregistrer toutes les notes qui sont ON et vérifier qu'elles ne durent pas trop longtemps...
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Midi;
using System.ComponentModel.Composition;
using System.IO;

namespace LeapOrchestra.SongPlayer
{
    class MidiFileReader
    {
        string filePath;
        long midiTimeCursor;
        long previousMidiTimeCursor;
        List<int> lastPlayedNoteIndex;
        DateTime previousBang;
        DateTime previousBarTime;
        private double millisecondsPerQuarterNote;
        private long QuarterNoteTimeCursor;
        private Queue<long> tempoList;

        private int DeltaTicksPerQuarterNote;
        public TimeSignature timeSignature;
        private int barNumber;
        private int beatNumber;

        public event Action<NoteEvent> SendNote;
        public event Action<int, int> SendProgramChange;
        public event Action SendEnd;
        MidiFile mf;

        private float[] channelOrientation;
        private float currentOrientation;
        
        public MidiFileReader(string path)
        {
            filePath = path;
            tempoList = new Queue<long>();
            
            midiTimeCursor = 0;

            previousBang = DateTime.Now;
            previousBarTime = DateTime.Now;
            millisecondsPerQuarterNote = 480000;

            DeltaTicksPerQuarterNote = 192;
            timeSignature.Numerator = 4;
            timeSignature.Denominator = 4;
            barNumber = 1;
            beatNumber = 1;

            channelOrientation = new float[16];
            channelOrientation.Initialize();
            currentOrientation = 0;

            //Enregistre tout le fichier midi dans la collection mf.Events
            if (!File.Exists(path))
            {
                Console.WriteLine("Fichier inexistant");
                return;
            }
            mf = new MidiFile(filePath, false);
            

            lastPlayedNoteIndex = new List<int>();

            for (int t = 0; t < mf.Tracks; t++)
            {
                lastPlayedNoteIndex.Add(-1);
            }
            
            DeltaTicksPerQuarterNote = mf.DeltaTicksPerQuarterNote;
            //Numerator
            int tempTimeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault().Numerator;
            timeSignature.Numerator = tempTimeSignature == 0 ? 4 : tempTimeSignature;
            //Denominator
            tempTimeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault().Denominator;
            if (tempTimeSignature == 4 || tempTimeSignature == 8)
                timeSignature.Denominator = tempTimeSignature;
            else
                timeSignature.Denominator = 4;

            Console.WriteLine("Midi File Loaded. "+ Environment.NewLine+"--> Signature : "+
                timeSignature.Numerator+"/"+timeSignature.Denominator);
        }

        public struct TimeSignature
        {
            public int Numerator;
            public int Denominator;
        }

        public void analyzeProgramChange()
        {
            if (SendProgramChange == null)
            {
                Console.WriteLine("SendProgramChange Undefined");
                return;
            }

            PatchChangeEvent instrument; 
            for (int t = 0; t < mf.Tracks; t++)
            {
                instrument = mf.Events[t].OfType<PatchChangeEvent>().LastOrDefault();
                if (instrument != null)
                {
                    Console.WriteLine("Instrument : "+instrument);
                    SendProgramChange(t, instrument.Patch);
                }
            }
        }

        public void throwBang()
        {
            if (beatNumber >= timeSignature.Numerator)
                beatNumber = 1;
            else
                beatNumber++;
            evolvePartCursor(beatNumber);
        }

        public void evolvePartCursor(int beatNumber)
        {
            if (beatNumber < 1 || beatNumber > timeSignature.Numerator)
            {
                Console.WriteLine("wrong barState");
                return;
            }

            if (beatNumber == 1)
            {
                barNumber++;
                QuarterNoteTimeCursor = barNumber * timeSignature.Numerator * DeltaTicksPerQuarterNote;
                previousBarTime = DateTime.Now;
            }
                
            this.beatNumber = beatNumber;
            //On calcul où on en est dans la lecture
            //QuarterNoteTimeCursor = barNumber * timeSignature.Numerator * DeltaTicksPerQuarterNote +
            //    (beatNumber -1) * DeltaTicksPerQuarterNote;
            
            computeTempo();
            Console.WriteLine("beatNumber "+beatNumber+" tempo : " + (int)Tempo +
                " Mesure : " + ToMBT(QuarterNoteTimeCursor + (beatNumber -1) * DeltaTicksPerQuarterNote,
                this.DeltaTicksPerQuarterNote, timeSignature));
            //playNote();
        }

        private void computeTempo()
        {
            //On calcul le nouveau tempo :
            TimeSpan timeDifference = DateTime.Now - previousBang;
            long timeDiff = (long)timeDifference.TotalMilliseconds;
            if (timeDiff < 100)
                return;
            else if (timeDiff < 930)
            {
                tempoList.Enqueue(timeDiff);
                millisecondsPerQuarterNote = tempoList.Average();

                if (tempoList.Count() > 8)
                {
                    tempoList.Dequeue();
                }
            }
            //Si > 2000, on garde l'ancience valeur

            previousBang = DateTime.Now;
        }

        public void playNote()
        {
            if (SendNote == null)
            {
                Console.WriteLine("Undefined fonction SendNote");
                return;
            }
            
            TimeSpan timeDifference = DateTime.Now - previousBarTime;
            midiTimeCursor = QuarterNoteTimeCursor +
                (long)(DeltaTicksPerQuarterNote * timeDifference.TotalMilliseconds / millisecondsPerQuarterNote);

            //Si on a dépassé la mesure suivante, on attend
            if (midiTimeCursor > QuarterNoteTimeCursor + DeltaTicksPerQuarterNote * timeSignature.Numerator)
                return;

            int i;
            IList<NoteEvent> track;
            Boolean end = false;

            for (int t = 1; t < mf.Tracks; t++)
            {
                track = mf.Events[t].OfType<NoteEvent>().ToList();
                if (track == null)
                {
                    Console.WriteLine("Error empty track");
                    break ;
                }
                //On récupère la première note après la dernière jouée
                i = lastPlayedNoteIndex[t] + 1;
                if (i >= track.Count)
                    break;
                NoteEvent note = track[i];

                //On lit les notes qui doivent être lues
                while ( note != null && note.AbsoluteTime < midiTimeCursor && i < track.Count)
                {
                    if (note.AbsoluteTime >= previousMidiTimeCursor)
                    {
                        note.Velocity = (int) (note.Velocity * GetOrientationCoef(note.Channel));
                        SendNote(note);
                    }
                    i = i + 1;
                    if (i >= track.Count)
                    {
                        end = true;
                        break;
                    }
                    note = track[i] as NoteEvent;
                    end = false;
                }
                lastPlayedNoteIndex[t] = i - 1;
            }
            previousMidiTimeCursor = midiTimeCursor;
            if (end)
            {
                Console.WriteLine("Fin de lecture");
                SendEnd();
            }
        }

        public double Tempo
        {
            get
            {
                return (60000.0 / millisecondsPerQuarterNote);
            }
        }

        public double MillisecondsPerQuarterNote
        {
            get
            {
                return millisecondsPerQuarterNote;
            }
        }

        /* Inutile
         * public void ImportFileInList()
        {
            MidiFile mf = new MidiFile(filePath, false);
            midiEvents = new MidiEventCollection(mf.FileFormat, mf.DeltaTicksPerQuarterNote);

            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("Format {0}, Tracks {1}, Delta Ticks Per Quarter Note {2}\r\n",
            //    mf.FileFormat, mf.Tracks, mf.DeltaTicksPerQuarterNote);
            DeltaTicksPerQuarterNote = mf.DeltaTicksPerQuarterNote;
            timeSignature.Numerator = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault().Numerator;
            timeSignature.Denominator = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault().Denominator;

            for (int n = 0; n < mf.Tracks; n++)
            {
                
                foreach (MidiEvent midiEvent in mf.Events[n])
                {
                    if(!MidiEvent.IsNoteOff(midiEvent))
                    {
                        //sb.AppendFormat("{0} {1}\r\n", ToMBT(midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                        if (midiEvent is NoteOnEvent)
                        {
                            NoteOnEvent note = (NoteOnEvent)midiEvent;
                            //Console.WriteLine("note : " + note.NoteName+ " ch "+note.Channel+" time : "+midiEvent.AbsoluteTime/1000);
                            midiEvents.Add(midiEvent);
                        }
                        else if (midiEvent is TempoEvent)
                        {
                            TempoEvent tempoEvent = (TempoEvent)midiEvent;
                            tempoFile = (long)tempoEvent.Tempo;
                            tempoList.Enqueue(tempoFile );
                            //Console.WriteLine("tempo : " + tempoFile + " microsec : " + tempoEvent.MicrosecondsPerQuarterNote);
                        }

                        //Fin des notes
                        if (MidiEvent.IsEndTrack(midiEvent) && midiEvent.AbsoluteTime > 10)
                        {
                            return;
                        }
                    }
                }
            }
        }*/

        //Retourne l'emplacement de la note dans la mesure.
        private string ToMBT(long eventTime, int ticksPerQuarterNote, TimeSignature timeSignature)
        {
            int beatsPerBar = timeSignature.Numerator;
            int ticksPerBar = timeSignature.Denominator == 0 ? ticksPerQuarterNote * 4 : 
                (timeSignature.Numerator * ticksPerQuarterNote * 4) / timeSignature.Denominator;
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            long bar = 1 + (eventTime / ticksPerBar);
            long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
            long tick = eventTime % ticksPerBeat;
            return String.Format("{0}:{1}:{2}", bar, beat, tick);
        }

        public string currentMBT()
        {
            return ToMBT(QuarterNoteTimeCursor + (beatNumber - 1) * DeltaTicksPerQuarterNote, 
                this.DeltaTicksPerQuarterNote, this.timeSignature);
        }

        /// <summary>
        /// Find the number of beats per measure
        /// (for now assume just one TimeSignature per MIDI track)
        /// </summary>
        private int FindBeatsPerMeasure(IEnumerable<MidiEvent> midiEvents)
        {
            int beatsPerMeasure = 4;
            foreach (MidiEvent midiEvent in midiEvents)
            {
                TimeSignatureEvent tse = midiEvent as TimeSignatureEvent;
                if (tse != null)
                {
                    beatsPerMeasure = tse.Numerator;
                }
            }
            return beatsPerMeasure;
        }
    
        public void ChooseChannelOrientation()
        {
            PatchChangeEvent instrument;
            Console.WriteLine("Choose an orientation between -10 and 10 to these channels :");
            float value = 0;
            Boolean validValue = false;
            String entry;
            for (int t = 0; t < mf.Tracks; t++)
            {
                instrument = mf.Events[t].OfType<PatchChangeEvent>().LastOrDefault();
                if (instrument != null)
                {
                    validValue = false;
                    while (!validValue)
                    {
                        Console.Write(instrument + " : ...");
                        entry = Console.ReadLine();
                        if(float.TryParse(entry, out value))
                        {
                            if (value <= 10 && value >= -10)
                            {
                                validValue = true;
                                channelOrientation[instrument.Channel] = value/10;
                                break;
                            }
                        }
                        validValue = false;
                        Console.WriteLine("La valeur entrée est incorrect");
                    }
                }
            }
        }

        public void SetCurrentOrientation(float angle)
        {
            if (angle < -1)
                angle = -1;
            else if (angle > 1)
                angle = 1;

            this.currentOrientation = angle;
        }

        private float GetOrientationCoef(int Channel)
        {
            float coef = 1 - Math.Abs(currentOrientation - channelOrientation[Channel]) / 2;
            coef += 0.1F;
            if (coef > 1)
                coef = 1;
            else if (coef < 0)
                coef = 0;
            return coef;
        }

    }
}
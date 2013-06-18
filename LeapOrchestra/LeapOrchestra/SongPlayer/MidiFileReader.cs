/*
 * Permet de lire un fichier midi
 * Sol 1
 * On enregistre toutes les nouvelles notes dans un tableau trié par temps
 * On viendra lire les notes aux bons moments
 * 
 * Sol 2
 * On enregistre toute les notes dans un tableau
 * On viendra à chaque fois calculer où on en est et envoyer les notes qu'il fallait lire depuis
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Midi;
using System.ComponentModel.Composition;

namespace LeapOrchestra.SongPlayer
{
    class MidiFileReader
    {
        string filePath;
        long midiTimeCursor;
        long previousMidiTimeCursor;
        long tempoFile;
        List<int> lastPlayedNoteIndex;
        DateTime previousBang;
        private double millisecondsPerQuarterNote;
        private long QuarterNoteTimeCursor;
        private Queue<long> tempoList;

        private int DeltaTicksPerQuarterNote;
        public TimeSignature timeSignature;

        public event Action<NoteOnEvent> SendNote;
        MidiFile mf;
        
        public MidiFileReader(string path)
        {
            filePath = path;
            tempoList = new Queue<long>();
            
            midiTimeCursor = 0;
            
            previousMidiTimeCursor = 0;
            QuarterNoteTimeCursor = 0;
            tempoFile = 120;

            previousBang = DateTime.Now;
            millisecondsPerQuarterNote = 480000;

            DeltaTicksPerQuarterNote = 192;
            timeSignature.Numerator = 4;
            timeSignature.Denominator = 4;

            //Enregistre tout le fichier midi dans la collection mf.Events
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
            timeSignature.Numerator = tempTimeSignature == 0 ? 4 : tempTimeSignature;
        }

        public struct TimeSignature
        {
            public int Numerator;
            public int Denominator;
        }

        public void throwBang()
        {
            //On calcul le nouveau tempo :
            TimeSpan timeDifference = DateTime.Now - previousBang;
            long timeDiff = (long)timeDifference.TotalMilliseconds;
            if (timeDiff < 100)
                return;
            else if (timeDiff < 2000)
            {
                tempoList.Enqueue(timeDiff);
                millisecondsPerQuarterNote = tempoList.Average();

                if (tempoList.Count() > 8)
                {
                    tempoList.Dequeue();
                }
            }
            //Si > 2000, on garde l'ancience valeur

            //Console.WriteLine("millisec : " + (int)millisecondsPerQuarterNote+" tempo : "+(int)Tempo);
            previousBang = DateTime.Now;

            //On calcul où on en est dans la lecture
            QuarterNoteTimeCursor = QuarterNoteTimeCursor + DeltaTicksPerQuarterNote;

            playNote();
        }

        public void playNote()
        {
            TimeSpan timeDifference = DateTime.Now - previousBang;
            midiTimeCursor = QuarterNoteTimeCursor +
                (long)(DeltaTicksPerQuarterNote * timeDifference.TotalMilliseconds / millisecondsPerQuarterNote);

            //Si on a dépassé le temps suivant, on attend
            if (midiTimeCursor > QuarterNoteTimeCursor + DeltaTicksPerQuarterNote)
                return;


            int i;
            IList<MidiEvent> track;

            for (int t = 0; t < mf.Tracks; t++)
            {
                track = mf.Events[t];
                if (track == null)
                {
                    Console.WriteLine("Error track vide");
                    return;
                }

                i = lastPlayedNoteIndex[t] + 1;//On récupère la première note après la dernière jouée
                
                NoteOnEvent note = track[i] as NoteOnEvent;
                //On récupère la première note
                while (note == null && i < track.Count -1 )
                {
                    i = i + 1;
                    note = track[i] as NoteOnEvent;
                }

                //On lit les notes qui doivent être lues
                while ( note != null && note.AbsoluteTime <= midiTimeCursor && i < track.Count-1)
                {
                    if (note.AbsoluteTime > previousMidiTimeCursor)
                    {
                        SendNote(note);
                        Console.WriteLine("piste : " + t + " note index : " + i);
                    }
                    i = i + 1;
                    note = track[i] as NoteOnEvent;
                }
                lastPlayedNoteIndex[t] = i - 1;
            }
            previousMidiTimeCursor = midiTimeCursor;
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
                (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            int ticksPerBeat = ticksPerBar / beatsPerBar;
            long bar = 1 + (eventTime / ticksPerBar);
            long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
            long tick = eventTime % ticksPerBeat;
            return String.Format("{0}:{1}:{2}", bar, beat, tick);
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
    }
}
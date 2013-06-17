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
        
        public MidiFileReader(string path)
        {
            filePath = path;
        }

        public string FileExtension
        {
            get { return ".mid"; }
        }

        public string FileTypeDescription
        {
            get { return "Standard MIDI File"; }
        }

        public string Describe()
        {
            MidiFile mf = new MidiFile(filePath, false);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Format {0}, Tracks {1}, Delta Ticks Per Quarter Note {2}\r\n",
                mf.FileFormat, mf.Tracks, mf.DeltaTicksPerQuarterNote);
            var timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
            for (int n = 0; n < mf.Tracks; n++)
            {
                foreach (MidiEvent midiEvent in mf.Events[n])
                {
                    if(!MidiEvent.IsNoteOff(midiEvent))
                    {
                        sb.AppendFormat("{0} {1}\r\n", ToMBT(midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                        if (midiEvent is NoteOnEvent)
                        {
                            NoteOnEvent note = (NoteOnEvent)midiEvent;
                            Console.WriteLine("note : " + note.NoteName+ " time : "+midiEvent.AbsoluteTime/1000);
                        }

                        if (MidiEvent.IsEndTrack(midiEvent) && midiEvent.AbsoluteTime > 10)
                        {
                            return "ha";
                        }
                    }
                }
            }
            return sb.ToString();
        }

        //Retourne l'emplacement de la note dans la mesure.
        private string ToMBT(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
        {
            int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
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
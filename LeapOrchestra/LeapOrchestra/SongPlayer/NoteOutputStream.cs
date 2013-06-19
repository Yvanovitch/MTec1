using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Midi;

namespace LeapOrchestra.SongPlayer
{
    abstract class NoteOutputStream
    {
        public virtual void Init()
        { }
        
        public virtual void SendBang()
        { }

        public virtual void SendNote(NoteEvent note)
        { }

        public virtual void SendProgramChange(int track, int ref_instrument)
        { }

        public virtual void Close()
        { }
    }
}

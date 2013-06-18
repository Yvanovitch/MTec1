﻿using System;
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

        public virtual void SendNote(NoteOnEvent note)
        { }

        public virtual void Close()
        { }
    }
}
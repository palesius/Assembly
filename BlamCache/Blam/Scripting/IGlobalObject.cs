﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtryzeDLL.Blam.Scripting
{
    public interface IGlobalObject
    {
        string Name { get; }
        short Class { get; }
        short PlacementIndex { get; }
    }
}

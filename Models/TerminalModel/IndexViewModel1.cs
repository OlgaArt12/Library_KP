﻿using Library_KP.Models.TerminalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Models.TerminalModel
{
    public class IndexViewModel1
    {
        public IEnumerable<Terminal> Terminals { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortViewModel1 SortViewModel { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Models.TerminalModel
{
    public class SortViewModel1
    {
        public SortState1 NameBookSort { get; private set; } 
        public SortState1 FioSort { get; private set; }
        public SortState1 DISort { get; private set; }
        public SortState1 RDSort { get; private set; }  
        public SortState1 Current { get; private set; }     

        public SortViewModel1(SortState1 sortOrder)
        {
            NameBookSort = sortOrder == SortState1.NameBookAsc ? SortState1.NameBookDesc : SortState1.NameBookAsc;
            FioSort = sortOrder == SortState1.FioAsc ? SortState1.FioDesc : SortState1.FioAsc;
            DISort = sortOrder == SortState1.DIAsc ? SortState1.DIDesc : SortState1.DIAsc;
            RDSort = sortOrder == SortState1.RDAsc ? SortState1.RDDesc : SortState1.RDAsc;
            Current = sortOrder;
        }
    }
}

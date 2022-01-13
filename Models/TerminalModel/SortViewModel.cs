using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Models.TerminalModel
{
    public class SortViewModel
    {
        public SortState NameBookSort { get; private set; } 
        public SortState FioSort { get; private set; }
        public SortState DISort { get; private set; }
        public SortState RDSort { get; private set; }  
        public SortState Current { get; private set; }     

        public SortViewModel(SortState sortOrder)
        {
            NameBookSort = sortOrder == SortState.NameBookAsc ? SortState.NameBookDesc : SortState.NameBookAsc;
            FioSort = sortOrder == SortState.FioAsc ? SortState.FioDesc : SortState.FioAsc;
            DISort = sortOrder == SortState.DIAsc ? SortState.DIDesc : SortState.DIAsc;
            RDSort = sortOrder == SortState.RDAsc ? SortState.RDDesc : SortState.RDAsc;
            Current = sortOrder;
        }
    }
}

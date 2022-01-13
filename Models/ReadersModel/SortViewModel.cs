using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Models.ReadersModel
{
    public class SortViewModel
    {
        public SortState NameSort { get; private set; } 
        public SortState PassSort { get; private set; }
        public SortState HomeSort { get; private set; }
        public SortState Current { get; private set; }

        public SortViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            PassSort = sortOrder == SortState.PassAsc ? SortState.PassDesc : SortState.PassAsc;
            HomeSort = sortOrder == SortState.HomeAsc ? SortState.HomeDesc : SortState.HomeAsc;
            Current = sortOrder;
        }
    }
}

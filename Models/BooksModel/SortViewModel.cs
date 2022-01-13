using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Models.BooksModel
{
    public class SortViewModel
    {
        public SortState NameBookSort { get; private set; } 
        public SortState YearSort { get; private set; }
        public SortState numbSort { get; private set; }
        public SortState authSort { get; private set; }
        public SortState PartSort { get; private set; }   
        public SortState Current { get; private set; }     

        public SortViewModel(SortState sortOrder)
        {
            NameBookSort = sortOrder == SortState.NameBookAsc ? SortState.NameBookDesc : SortState.NameBookAsc;
            YearSort = sortOrder == SortState.YearAsc ? SortState.YearDesc : SortState.YearAsc;
            numbSort = sortOrder == SortState.NumbAsc ? SortState.NumbDesc : SortState.NumbAsc;
            authSort = sortOrder == SortState.AuthAsc ? SortState.AuthDesc : SortState.AuthAsc;
            PartSort = sortOrder == SortState.PartAsc ? SortState.PartDesc : SortState.YearAsc;
            Current = sortOrder;
        }
    }
}

using Library_KP.Models.BooksModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_KP.Models.ReadersModel
{
    public class IndexViewModel
    {
        public IEnumerable<Reader> Readers { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}

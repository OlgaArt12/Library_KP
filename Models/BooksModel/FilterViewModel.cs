using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library_KP.Models.BooksModel
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Partition> partitions, int? partition, string nameBook)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            partitions.Insert(0, new Partition { NamePartition = "Все", PartitionId = 0 });
            Partitions = new SelectList(partitions, "PartitionId", "NamePartition", partition);
            SelectedPartition = partition;
            SelectedName = nameBook;
        }
        public SelectList Partitions { get; private set; } 
        public int? SelectedPartition { get; private set; }   
        public string SelectedName { get; private set; } 
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Library_KP.Models.BooksModel;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Library_KP.Models
{
    [Table("Partition")]
    [Index(nameof(NamePartition), Name = "C_namePart", IsUnique = true)]
    public partial class Partition
    {
        public Partition()
        {
            Books = new HashSet<Book>();
        }

        [Key]
        [Column("PartitionID")]
        public int PartitionId { get; set; }

        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Наименование раздела:")]
        [Column("Name_Partition")]
        [StringLength(50)]
        public string NamePartition { get; set; }

        [InverseProperty(nameof(Book.PartitionNameNavigation))]
        public virtual ICollection<Book> Books { get; set; }
    }
}

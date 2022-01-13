using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Library_KP.Models.BooksModel
{
    [Table("Book")]
    public partial class Book
    {
        public Book()
        {
            Terminals = new HashSet<Terminal>();
        }

        [Key]
        [Column("RegistrationID")]
        public int RegistrationId { get; set; }
        public int? PartitionName { get; set; }
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Название:")]
        [Column("Name_book")]
        [StringLength(100)]
        public string NameBook { get; set; }

        [DisplayName("Кол-во страниц:")]
        [Range(6, 2000, ErrorMessage = "Вы вышли за диапозон!")]
        [Column("Number_of_page")]
        public int NumberOfPage { get; set; }

        [DisplayName("Год издания:")]
        [Range(1900, 2022, ErrorMessage = "Вы вышли за диапозон!")]
        [Column("Year_of_publication")]
        public int? YearOfPublication { get; set; }

        [DisplayName("Автор:")]
        [StringLength(100)]
        public string Author { get; set; }

        [ForeignKey(nameof(PartitionName))]
        [InverseProperty(nameof(Partition.Books))]
        [DisplayName("Жанр:")]
        public virtual Partition PartitionNameNavigation { get; set; }

        [InverseProperty(nameof(Terminal.RegistrationBook))]
        public virtual ICollection<Terminal> Terminals { get; set; }
    }
}

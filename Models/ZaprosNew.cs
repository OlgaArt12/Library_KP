using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Library_KP.Models
{
    [Keyless]
    public partial class ZaprosNew
    {
        [DisplayName("№ читательского бтлета:")]
        public int NumberTickets { get; set; }

        [DisplayName("Дата выдачи книги:")]
        [DataType(DataType.Date, ErrorMessage = "Некорректный ввод")]
        [Column("Date_issue", TypeName = "date")]
        public DateTime DateIssue { get; set; }

        [DisplayName("Дата возврата:")]
        [DataType(DataType.Date, ErrorMessage = "Некорректный ввод")]
        [Column("Return_date", TypeName = "date")]
        public DateTime? ReturnDate { get; set; }

        [Required]
        [DisplayName("ФИО:")]
        [Column("FCS")]
        [StringLength(50)]
        public string Fcs { get; set; }

        [DisplayName("Название книги:")]
        [Column("Name_book")]
        [StringLength(100)]
        public string NameBook { get; set; }

        [DisplayName("Автор книги:")]
        [StringLength(100)]
        public string Author { get; set; }

        [DisplayName("Кол-во не сданных вовремя книг:")]
        [Column("countBookNeedReturn")]
        public int? CountBookNeedReturn { get; set; }
    }
}

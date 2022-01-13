using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Library_KP.Models.ReadersModel;
using Library_KP.Models.BooksModel;
using Library_KP.Models.TerminalModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

#nullable disable

namespace Library_KP.Models
{
    [Table("Terminal")]
    [Index(nameof(DateIssue), Name = "ix_ter_dateiss")]
    [Index(nameof(NumberTickets), Name = "ix_ter_numtick")]
    public partial class Terminal
    {
        [Key]
        [Column("TerminalID")]
        public int TerminalId { get; set; }

        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Название книги: ")]
        [Column("Registration_Book_ID")]
        public int RegistrationBookId { get; set; }

        [DisplayName("ФИО читателя:")]
        public int NumberTickets { get; set; }

        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Дата выдачи: ")]
        [Column("Date_issue", TypeName = "date")]
        public DateTime DateIssue { get; set; }

        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Дата возврата: ")]
        [Column("Return_date", TypeName = "date")]
        public DateTime ReturnDate { get; set; }

        [ForeignKey(nameof(NumberTickets))]
        [InverseProperty(nameof(Reader.Terminals))]
        [DisplayName("ФИО читателя:")]
        public virtual Reader NumberTicketsNavigation { get; set; }

        [ForeignKey(nameof(RegistrationBookId))]
        [InverseProperty(nameof(Book.Terminals))]
        [DisplayName("Название книги:")]
        public virtual Book RegistrationBook { get; set; }
    }
}

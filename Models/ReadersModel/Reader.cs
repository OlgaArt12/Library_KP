using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Library_KP.Models.ReadersModel
{
    [Table("Reader")]
    [Index(nameof(PassportData), Name = "AK_Passport_data", IsUnique = true)]
    [Index(nameof(PassportData), Name = "C_Reader_PassportOne", IsUnique = true)]
    [Index(nameof(Fcs), Name = "ix_red_fcs")]
    [Index(nameof(NumberTicket), Name = "ix_red_numtick", IsUnique = true)]
    [Index(nameof(PassportData), Name = "ix_red_pd_fcs")]
    public partial class Reader
    {
        public Reader()
        {
            Terminals = new HashSet<Terminal>();
        }

        [Key]
        public int NumberTicket { get; set; }

        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("ФИО:")]
        [Column("FCS")]
        [StringLength(50)]
        public string Fcs { get; set; }

        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Паспортные данные:")]
        [Range(2515000000, 2580000000, ErrorMessage = "В паспорте 10 цифр")]
        [Column("Passport_data")]
        public long PassportData { get; set; }

        [DisplayName("Домашний адрес:")]
        [Column("Home_address")]
        [StringLength(100)]
        public string HomeAddress { get; set; }

        [InverseProperty(nameof(Terminal.NumberTicketsNavigation))]
        public virtual ICollection<Terminal> Terminals { get; set; }
    }
}

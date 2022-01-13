using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Library_KP.Models
{
    [Keyless]
    public partial class ZaprosNew
    {
        public int NumberTickets { get; set; }
        [Column("Date_issue", TypeName = "date")]
        public DateTime DateIssue { get; set; }
        [Column("Return_date", TypeName = "date")]
        public DateTime? ReturnDate { get; set; }
        [Required]
        [Column("FCS")]
        [StringLength(50)]
        public string Fcs { get; set; }
        [Column("Name_book")]
        [StringLength(100)]
        public string NameBook { get; set; }
        [StringLength(100)]
        public string Author { get; set; }
        [Column("countBookNeedReturn")]
        public int? CountBookNeedReturn { get; set; }
    }
}

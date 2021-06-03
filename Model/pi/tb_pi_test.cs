using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Model.pi
{
    [Table("tb_pi_test")]
    public partial class tb_pi_test
    {
        [Key]
        public Guid id { get; set; }
        [StringLength(36)]
        public string create_user { get; set; }
        [Column(TypeName = "datetime(3)")]
        public DateTime? create_time { get; set; }
        [StringLength(36)]
        public string update_user { get; set; }
        [Column(TypeName = "datetime(3)")]
        public DateTime? update_time { get; set; }
        [Column(TypeName = "int(11)")]
        public int? order_number { get; set; }
        [Column(TypeName = "bit(1)")]
        public ulong? is_delete { get; set; }
        [Column(TypeName = "bit(1)")]
        public ulong? is_success { get; set; }
    }
}

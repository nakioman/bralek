using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("AlertInfos")]
    public class AlertInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual string Key { get; set; }

        public virtual DateTime ViewedOnUTC { get; set; }

        public virtual User ViewedBy { get; set; }
    }
}
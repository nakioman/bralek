using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("Bookmarks")]
    public class Bookmark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime AddedOnUTC { get; set; }

        public virtual SubscriptionPost SubscriptionPost { get; set; }
    }
}
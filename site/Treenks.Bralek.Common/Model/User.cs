using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual string Email { get; set; }

        [InverseProperty("ViewedBy")]
        public virtual ICollection<AlertInfo> AlertsSeen { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserSubscription> Subscriptions { get; set; }

        public virtual bool OrderByOldest { get; set; }

        public virtual bool ShowAllItems { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Bookmark> Bookmarks { get; set; }
    }
}
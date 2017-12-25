//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Comment : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            this.Comments1 = new HashSet<Comment>();
        }

        public Nullable<int> QuestionId { get; set; }
        public Nullable<int> ParentCommentId { get; set; }
        public string CommentText { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public System.DateTime DateCommented { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments1 { get; set; }
        public virtual Comment Comment1 { get; set; }
        public virtual Question Question { get; set; }
        public virtual User User { get; set; }
    }
}

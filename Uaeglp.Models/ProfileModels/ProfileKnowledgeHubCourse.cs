using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models.ProfileModels
{
    [Table("Profile_KnowledgeHubCourse", Schema = "app")]
    public class ProfileKnowledgeHubCourse
    {
        
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("CourseID")]
        public int CourseId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }

        [ForeignKey(nameof(CourseId))]
        //[InverseProperty("ProfileKnowledgeHubCourse")]
        public virtual KnowledgeHubCourse Course { get; set; }

        [ForeignKey(nameof(ProfileId))]
       // [InverseProperty("ProfileKnowledgeHubCourse")]
        public virtual Profile Profile { get; set; }

      
    }
}

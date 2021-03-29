using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models.ProfileModels
{
    [Table("Profile_Languages" , Schema = "app")]
    public partial class ProfileLanguage
    {

        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ProfileID")]
        public int ProfileId { get; set; }

        public int LanguageItemId { get; set; }

        public int ProficiencyItemId { get; set; }

        [ForeignKey(nameof(LanguageItemId))]
        [InverseProperty(nameof(LookupItem.ProfileLanguageItems))]
        public virtual LookupItem LookupLanguage { get; set; }

        [ForeignKey(nameof(ProficiencyItemId))]
        [InverseProperty(nameof(LookupItem.ProficiencyItems))]
        public virtual LookupItem LookupProficiency { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }

    }
}

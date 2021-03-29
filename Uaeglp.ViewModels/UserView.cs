using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uaeglp.ViewModels
{
	public class UserView
	{
		public int Id { get; set; }

		[Required]
		[StringLength(256)]
		public string Username { get; set; }

		[Required]
		[StringLength(75)]
		public string Password { get; set; }

		[StringLength(100)]
		public string NameEn { get; set; }

		[StringLength(100)]
		public string NameAr { get; set; }

		[Required]
		[StringLength(256)]
		public string Email { get; set; }

		public IList<int> FollowersIDs { get; set; }
		public IList<int> FollowingIDs { get; set; }
		public IList<int> MyFavouriteProfilesIDs { get; set; }
	}
}

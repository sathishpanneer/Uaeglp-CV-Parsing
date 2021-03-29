using System;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Models;

namespace Uaeglp.Services
{
	public class FolderService : IFolderService
	{
		private Repositories.AppDbContext _appContext;

		public FolderService(Repositories.AppDbContext appContext)
		{
			_appContext = appContext;
		}
		public async Task<Folder> SaveFileAsync(Folder file)
		{
			_appContext.Folders.Add(file);
			await _appContext.SaveChangesAsync();
			return file;
		}
	}
}

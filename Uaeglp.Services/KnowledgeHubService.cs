using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Models.ProfileModels;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.KnowledgeHub;

namespace Uaeglp.Services
{
	public class KnowledgeHubService : IKnowledgeHubService
	{
		private readonly AppDbContext _appDbContext;
		private readonly IEncryptionManager _encryptor;
		private readonly IFileService _fileService;

		public KnowledgeHubService(AppDbContext appContext, IEncryptionManager encryption, IFileService fileService)
		{
			_appDbContext = appContext;
			_encryptor = encryption;
			_fileService = fileService;
		}

		public async Task<IKnowledgeHubResponse> GetKnowledgeHubs(LanguageType language = LanguageType.AR)
		{
			try
			{
				var list = await _appDbContext.KnowledgeHubs.OrderBy(g => g.Id).Select(t => new KnowledgeHubView()
				{
					ID = t.Id,
					KnowledgeHubTitle = language == LanguageType.EN? t.ArticleTitleEn: t.ArticleTitleAr,
					ArticleTitleAr = t.ArticleTitleAr,
					AuthorTitleAr = t.AuthorTitleAr,
					ArticleTitleEn = t.ArticleTitleEn,
					AuthorTitleEn = t.AuthorTitleEn,
					ArticleTitle = language == LanguageType.EN ? t.ArticleTitleEn: t.ArticleTitleAr,
					AuthorTitle = language == LanguageType.EN ? t.AuthorTitleEn: t.AuthorTitleAr,
					ImageID = t.ImageId,
					Category = language == LanguageType.EN ? t.Category.NameEn: t.Category.NameAr
				}).ToListAsync();

				foreach (var knowledgeHub in list)
				{
					if (knowledgeHub.ImageID.HasValue)
					{
						var file = _fileService.GetFile(knowledgeHub.ImageID.Value, false);
						knowledgeHub.Image = file;
					}
				}

				return new KnowledgeHubResponse(list);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> GetCategories()
		{
			try
			{
				var list = await _appDbContext.KnowledgeHubCategories.Select(e => new KnowledgeHubCategoryView()
				{
					ID = e.Id,
					IDEncyrpted = this._encryptor.Encrypt(e.Id.ToString()),
					TitleEN = e.TitleEn,
					TitleAR = e.TitleAr,
					LogoID = e.LogoId
				}).ToListAsync();

				return new KnowledgeHubResponse(list);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> GetALLCoursesbyCategories(int profileid, int skip = 0, int take = 5)
		{
			try
			{
				List<KnowledgeHubCategoryCourseView> _views = new List<KnowledgeHubCategoryCourseView>();
				var categorieslist = _appDbContext.KnowledgeHubCategories.Include(a => a.KnowledgeHubCourses).ToList();
				foreach (var item in categorieslist)
				{
					KnowledgeHubCategoryCourseView _view = new KnowledgeHubCategoryCourseView();
					_view.ID = item.Id;
					_view.IDEncyrpted = this._encryptor.Encrypt(item.Id.ToString());
					_view.TitleEN = item.TitleEn;
					_view.TitleAR = item.TitleAr;
					_view.LogoID = item.LogoId;
					List<KnowledgeHubCourseView> _courses = new List<KnowledgeHubCourseView>();
					var cours = _appDbContext.KnowledgeHubCourses.Include(a=>a.Category).Include(a => a.ProviderType).Include(a => a.CourseType).Where(a => a.CategoryId == item.Id && !a.IsHide).Skip(skip).Take(take).ToList();
					foreach (var course in cours)
					{
						KnowledgeHubCourseView _course = new KnowledgeHubCourseView();
						_course.ID = course.Id;
						_course.IDEncyrpted = this._encryptor.Encrypt(course.Id.ToString());
						_course.NameEN = course.NameEn; //new EnglishArabicView(e.NameEn, e.NameAr),
						_course.NameAR = course.NameAr;
						_course.AboutEN = course.AboutEn != null ? course.AboutEn : "";
						_course.AboutAR = course.AboutAr != null ? course.AboutAr : "";
						_course.CategoryID = course.CategoryId;
						_course.CategoryNameEn = course.Category != null ? course.Category.TitleEn : "";// e.Category != null ? language == LanguageType.EN? e.Category.TitleEn: e.Category.TitleAr : "",
						_course.CategoryNameAr = course.Category != null ? course.Category.TitleAr : "";
						_course.ProviderTypeEn = course.ProviderType != null ? course.ProviderType.NameEn : "";// language == LanguageType.EN ? e.ProviderType.NameEn: e.ProviderType.NameAr,
						_course.ProviderTypeAr = course.ProviderType != null ? course.ProviderType.NameAr : "";
						_course.IsHide = course.IsHide;
						_course.IframeUrlEN = course.IframeUrlEn != null ? course.IframeUrlEn : "";// new EnglishArabicView(e.IframeUrlEn, e.IframeUrlAr),
						_course.IframeUrlAR = course.IframeUrlAr != null ? course.IframeUrlAr : "";
						_course.To = course.To ?? null;
						_course.From = course.From ?? null;
						_course.Language = course.Language;
						_course.ShowProviderIcon = course.ShowProviderIcon;
						_course.DescriptionEN = course.DescriptionEn != null ? course.DescriptionEn : "";// new EnglishArabicView(e.DescriptionEn, e.DescriptionAr),
						_course.DescriptionAR = course.DescriptionAr != null ? course.DescriptionAr : "";
						_course.CourseTypeEn = course.CourseType != null ? course.CourseType.NameEn : "";// language == LanguageType.EN ? e.CourseType.NameEn: e.CourseType.NameAr,
						_course.CourseTypeAr = course.CourseType != null ? course.CourseType.NameAr : "";
						_course.ImageURL = course.ImageUrl;
						_course.CourseTypeID = course.CourseTypeId;
						_course.ProviderTypeID = course.ProviderTypeId;
						var profilecourse = _appDbContext.ProfileKnowledgeHubCourses.FirstOrDefault(a => a.CourseId == course.Id && a.ProfileId == profileid);
						if (profilecourse != null)
						{
							_course.IsFavourite = true;
						}
						else
						{
							_course.IsFavourite = false;
						}

					    _courses.Add(_course);

					}

					_view.Courses = _courses;

					_views.Add(_view);
				}
				return new KnowledgeHubResponse(_views);
			}
			catch (Exception e)
			{
				//return null;
				//throw;
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> GetCoursesbyCategories(int categoryId,int profileid, int skip = 0, int take = 5)
		{
			try
			{
				var courses = await _appDbContext.KnowledgeHubCourses.Include(a => a.Category).Include(a => a.ProviderType).Include(a => a.CourseType).Where(e => e.CategoryId == categoryId && !e.IsHide).Skip(skip).Take(take)
					.ToListAsync();
				List<KnowledgeHubCourseView> _courses = new List<KnowledgeHubCourseView>();
				//var cours = _appDbContext.KnowledgeHubCourses.Include(a => a.ProviderType).Include(a => a.CourseType).Where(a => a.CategoryId == item.Id).ToList();
				foreach (var course in courses)
				{
					KnowledgeHubCourseView _course = new KnowledgeHubCourseView();
					_course.ID = course.Id;
					_course.IDEncyrpted = this._encryptor.Encrypt(course.Id.ToString());
					_course.NameEN = course.NameEn; //new EnglishArabicView(e.NameEn, e.NameAr),
					_course.NameAR = course.NameAr;
					_course.AboutEN = course.AboutEn != null ? course.AboutEn : "";
					_course.AboutAR = course.AboutAr != null ? course.AboutAr : "";
					_course.CategoryID = course.CategoryId;
					_course.CategoryNameEn = course.Category != null ? course.Category.TitleEn : "";// e.Category != null ? language == LanguageType.EN? e.Category.TitleEn: e.Category.TitleAr : "",
					_course.CategoryNameAr = course.Category != null ? course.Category.TitleAr : "";
					_course.ProviderTypeEn = course.ProviderType != null ? course.ProviderType.NameEn : "";// language == LanguageType.EN ? e.ProviderType.NameEn: e.ProviderType.NameAr,
					_course.ProviderTypeAr = course.ProviderType != null ? course.ProviderType.NameAr : "";
					_course.IsHide = course.IsHide;
					_course.IframeUrlEN = course.IframeUrlEn != null ? course.IframeUrlEn : "";// new EnglishArabicView(e.IframeUrlEn, e.IframeUrlAr),
					_course.IframeUrlAR = course.IframeUrlAr != null ? course.IframeUrlAr : "";
					_course.To = course.To ?? null;
					_course.From = course.From ?? null;
					_course.Language = course.Language;
					_course.ShowProviderIcon = course.ShowProviderIcon;
					_course.DescriptionEN = course.DescriptionEn != null ? course.DescriptionEn : "";// new EnglishArabicView(e.DescriptionEn, e.DescriptionAr),
					_course.DescriptionAR = course.DescriptionAr != null ? course.DescriptionAr : "";
					_course.CourseTypeEn = course.CourseType != null ? course.CourseType.NameEn : "";// language == LanguageType.EN ? e.CourseType.NameEn: e.CourseType.NameAr,
					_course.CourseTypeAr = course.CourseType != null ? course.CourseType.NameAr : "";
					_course.ImageURL = course.ImageUrl;
					_course.CourseTypeID = course.CourseTypeId;
					_course.ProviderTypeID = course.ProviderTypeId;
					var profilecourse = _appDbContext.ProfileKnowledgeHubCourses.FirstOrDefault(a => a.CourseId == course.Id && a.ProfileId == profileid);
					if (profilecourse != null)
					{
						_course.IsFavourite = true;
					}
					else
					{
						_course.IsFavourite = false;
					}
					_courses.Add(_course);

				}
				return new KnowledgeHubResponse(_courses);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> GetDetalis(int id, LanguageType language = LanguageType.AR)
		{
			try
			{
				KnowledgeHub knowledgeHub = await _appDbContext.KnowledgeHubs.FirstOrDefaultAsync(g => g.Id == id);
				var knowledgeHubView = new KnowledgeHubView()
				{
					ID = knowledgeHub.Id,
					KnowledgeHubTitle = language == LanguageType.EN? knowledgeHub.ArticleTitleEn: knowledgeHub.ArticleTitleAr,
					ArticleTitleAr = knowledgeHub.ArticleTitleAr,
					AuthorTitleAr = knowledgeHub.AuthorTitleAr,
					ArticleTitleEn = knowledgeHub.ArticleTitleEn,
					AuthorTitleEn = knowledgeHub.AuthorTitleEn,
					ArticleTitle = language == LanguageType.EN? knowledgeHub.ArticleTitleEn: knowledgeHub.ArticleTitleAr,
					AuthorTitle = language == LanguageType.EN? knowledgeHub.AuthorTitleEn: knowledgeHub.AuthorTitleAr,
					ImageID = knowledgeHub.ImageId,
					HTMLContent = language == LanguageType.EN ? knowledgeHub.HtmlcontentEn: knowledgeHub.HtmlcontentAr
				};
				if (knowledgeHub.ImageId.HasValue)
					knowledgeHubView.Image = _fileService.GetFile(knowledgeHub.ImageId.Value, false);
				return new KnowledgeHubResponse(knowledgeHubView);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> GetKnowledgeHubCourses(string searchText,int profileid, int? providerTypeId, int? categoryId, bool ishide, LanguageType language = LanguageType.AR, int skip = 0, int take = 5)
		{
			try
			{
				KnowledgeHubCourseView hubCourseViewModel = new KnowledgeHubCourseView();
				var source = _appDbContext.KnowledgeHubCourses.AsQueryable<KnowledgeHubCourse>();
				if (!string.IsNullOrEmpty(searchText))
					source = source.Where(e => e.NameEn.Contains(searchText) || e.NameAr.Contains(searchText));
				if (providerTypeId.HasValue)
					source = source.Where(e => e.ProviderTypeId == providerTypeId.Value);
				if (categoryId.HasValue)
					source = source.Where(e => e.CategoryId == categoryId);
				if (ishide)
					source = source.Where(e => !e.IsHide);

				var list = await source.Where(a=>a.CategoryId.HasValue).OrderBy(e => e.CategoryId.HasValue).Select(e => new KnowledgeHubCourseView()
				{
					ID = e.Id,
					IDEncyrpted = this._encryptor.Encrypt(e.Id.ToString()),
					NameEN = e.NameEn, //new EnglishArabicView(e.NameEn, e.NameAr),
					NameAR = e.NameAr,
					AboutEN = e.AboutEn != null ? e.AboutEn : "",
				    AboutAR = e.AboutAr != null ? e.AboutAr : "",
				    CategoryID = e.CategoryId,
					CategoryNameEn = e.Category != null ? e.Category.TitleEn : "",// e.Category != null ? language == LanguageType.EN? e.Category.TitleEn: e.Category.TitleAr : "",
					CategoryNameAr = e.Category != null ? e.Category.TitleAr : "",
					ProviderTypeEn = e.ProviderType != null ? e.ProviderType.NameEn : "",// language == LanguageType.EN ? e.ProviderType.NameEn: e.ProviderType.NameAr,
					ProviderTypeAr = e.ProviderType != null ? e.ProviderType.NameAr : "",
					IsHide = e.IsHide,
					IframeUrlEN = e.IframeUrlEn != null ? e.IframeUrlEn : "",// new EnglishArabicView(e.IframeUrlEn, e.IframeUrlAr),
					IframeUrlAR = e.IframeUrlAr != null ? e.IframeUrlAr : "",
					To = e.To,
					From = e.From,
					Language = e.Language,
					ShowProviderIcon = e.ShowProviderIcon,
					DescriptionEN = e.DescriptionEn != null ? e.DescriptionEn : "",// new EnglishArabicView(e.DescriptionEn, e.DescriptionAr),
					DescriptionAR = e.DescriptionAr != null ? e.DescriptionAr : "",
					CourseTypeEn = e.DescriptionAr != null ? e.CourseType.NameEn : "",// language == LanguageType.EN ? e.CourseType.NameEn: e.CourseType.NameAr,
					CourseTypeAr = e.CourseType != null ? e.CourseType.NameAr : "",
					ImageURL = e.ImageUrl,
					CourseTypeID = e.CourseTypeId,
					ProviderTypeID = e.ProviderTypeId,
					IsFavourite = _appDbContext.ProfileKnowledgeHubCourses.FirstOrDefault(a => a.CourseId == e.Id && a.ProfileId == profileid) != null ? true : false 
				}).ToListAsync();
				return new KnowledgeHubResponse(list.Skip(skip).Take(take).ToList())
				{
					CourseCount = list.Count()
				};
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> GetCourseIframe(int courseId, LanguageType language = LanguageType.AR)
		{
			try
			{
				var knowledgeHubCourse = await _appDbContext.KnowledgeHubCourses.FirstOrDefaultAsync(x => x.Id == courseId);
				if (knowledgeHubCourse == null)
					throw new ArgumentException("Invalid knowledgeHubCourseID: " + (object)courseId);
				var iframe = new KnowledgeHubCourseIframe()
				{
					CourseName = language == LanguageType.EN? knowledgeHubCourse.NameEn: knowledgeHubCourse.NameAr,
					Iframe = language == LanguageType.EN ? knowledgeHubCourse.IframeUrlEn: knowledgeHubCourse.IframeUrlAr
				};
				return new KnowledgeHubResponse(iframe);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}

		public async Task<IKnowledgeHubResponse> SetFavourite(int courseid,int profileid)
		{
			try
			{
				var course = _appDbContext.KnowledgeHubCourses.FirstOrDefault(a => a.Id == courseid);
				if (course == null)
				{
					return new KnowledgeHubResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
				}
				var profilecourse = _appDbContext.ProfileKnowledgeHubCourses.FirstOrDefault(a => a.CourseId == courseid && a.ProfileId == profileid );
				if (profilecourse == null)
				{
					ProfileKnowledgeHubCourse _profilecoure = new ProfileKnowledgeHubCourse();
					_profilecoure.CourseId = courseid;
					_profilecoure.ProfileId = profileid;

					_appDbContext.ProfileKnowledgeHubCourses.Add(_profilecoure);
					_appDbContext.SaveChanges();

				}
				return new KnowledgeHubResponse(true);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
				//throw;
			}
		}
		public async Task<IKnowledgeHubResponse> SetUnFavourite(int courseid, int profileid)
		{
			try
			{
				var course = _appDbContext.KnowledgeHubCourses.FirstOrDefault(a => a.Id == courseid);
				if (course == null)
				{
					return new KnowledgeHubResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
				}
				var profilecourse = _appDbContext.ProfileKnowledgeHubCourses.FirstOrDefault(a => a.CourseId == courseid && a.ProfileId == profileid);
				if (profilecourse != null)
				{
					_appDbContext.Remove(profilecourse);
					_appDbContext.SaveChanges();
				}
				return new KnowledgeHubResponse(false);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
				//throw;
			}
		}
		public async Task<IKnowledgeHubResponse> GetFavouriteCourses(int profileid,int skip = 0,int take = 5)
		{
			try
			{
				var courses = await _appDbContext.KnowledgeHubCourses.Include(a => a.ProviderType).Include(a => a.CourseType).Where(a=>a.IsHide == false).ToListAsync();

				List<KnowledgeHubCourseView> _courses = new List<KnowledgeHubCourseView>();
				//var cours = _appDbContext.KnowledgeHubCourses.Include(a => a.ProviderType).Include(a => a.CourseType).Where(a => a.CategoryId == item.Id).ToList();
				foreach (var course in courses)
				{
					var profilecourse = _appDbContext.ProfileKnowledgeHubCourses.FirstOrDefault(a => a.CourseId == course.Id && a.ProfileId == profileid);
					if (profilecourse != null)
					{
						KnowledgeHubCourseView _course = new KnowledgeHubCourseView();
						_course.ID = course.Id;
						_course.IDEncyrpted = this._encryptor.Encrypt(course.Id.ToString());
						_course.NameEN = course.NameEn; //new EnglishArabicView(e.NameEn, e.NameAr),
						_course.NameAR = course.NameAr;
						_course.AboutEN = course.AboutEn != null ? course.AboutEn : "";
						_course.AboutAR = course.AboutAr != null ? course.AboutAr : "";
						_course.CategoryID = course.CategoryId;
						_course.CategoryNameEn = course.Category != null ? course.Category.TitleEn : "";// e.Category != null ? language == LanguageType.EN? e.Category.TitleEn: e.Category.TitleAr : "",
						_course.CategoryNameAr = course.Category != null ? course.Category.TitleAr : "";
						_course.ProviderTypeEn = course.ProviderType != null ? course.ProviderType.NameEn : "";// language == LanguageType.EN ? e.ProviderType.NameEn: e.ProviderType.NameAr,
						_course.ProviderTypeAr = course.ProviderType != null ? course.ProviderType.NameAr : "";
						_course.IsHide = course.IsHide;
						_course.IframeUrlEN = course.IframeUrlEn != null ? course.IframeUrlEn : "";// new EnglishArabicView(e.IframeUrlEn, e.IframeUrlAr),
						_course.IframeUrlAR = course.IframeUrlAr != null ? course.IframeUrlAr : "";
						_course.To = course.To ?? null;
						_course.From = course.From ?? null;
						_course.Language = course.Language;
						_course.ShowProviderIcon = course.ShowProviderIcon;
						_course.DescriptionEN = course.DescriptionEn != null ? course.DescriptionEn : "";// new EnglishArabicView(e.DescriptionEn, e.DescriptionAr),
						_course.DescriptionAR = course.DescriptionAr != null ? course.DescriptionAr : "";
						_course.CourseTypeEn = course.CourseType != null ? course.CourseType.NameEn : "";// language == LanguageType.EN ? e.CourseType.NameEn: e.CourseType.NameAr,
						_course.CourseTypeAr = course.CourseType != null ? course.CourseType.NameAr : "";
						_course.ImageURL = course.ImageUrl;
						_course.CourseTypeID = course.CourseTypeId;
						_course.ProviderTypeID = course.ProviderTypeId;
						_course.IsFavourite = true;
						_courses.Add(_course);
					}

				}
				return new KnowledgeHubResponse(_courses.Skip(skip).Take(take).ToList()) {
					CourseCount = _courses.Count()
				};
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}
		public async Task<IKnowledgeHubResponse> GetFolderandfiles(int profileid,int parentfolderid)
		{
			try
			{
				
				LibraryView libraryView = new LibraryView();
				List<FolderView> _views = new List<FolderView>();
				List<EsourceView> esourceViews = new List<EsourceView>();
				var folders = await _appDbContext.Folders.Where(a => a.ParentFolderId == parentfolderid).ToListAsync();
				//var cours = _appDbContext.KnowledgeHubCourses.Include(a => a.ProviderType).Include(a => a.CourseType).Where(a => a.CategoryId == item.Id).ToList();

				
				foreach (var folder in folders)
				{
					bool IsAvailable = false;
					var file = await _appDbContext.Eresources.Where(a => a.FolderId == folder.Id).ToListAsync();
					if (file != null && file.Count > 0)
					{
						IsAvailable = true;
					}
					if (IsAvailable)
					{
						FolderView view = new FolderView();
						view.Id = folder.Id;
						view.NameEn = folder.NameEn;
						view.NameAr = folder.NameAr;
						_views.Add(view);
					}

				}

				var source = await _appDbContext.Eresources.Where(a => a.FolderId == parentfolderid).ToListAsync();
				foreach (var item in source)
				{
					EsourceView view = new EsourceView();
					view.Id = item.Id;
					view.NameEn = item.NameEn;
					view.NameAr = item.NameAr;
					view.DescriptionEn = item.DescriptionEn;
					view.DescriptionAr = item.DescriptionAr;
					view.CorrelationId = item.CorrelationId;
					var urls = _appDbContext.EresourceLinks.FirstOrDefault(a => a.EresourceId == item.Id);
					view.EsourceUrl = urls != null ? urls.Url : "";
					var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.CorrelationId == item.CorrelationId);
					if (file != null)
					{
						view.FileName = file.Name;
					}
					else
					{
						view.FileName = "";
					}
					esourceViews.Add(view);
				}
				libraryView.esourceViews = esourceViews;
				libraryView.Folders = _views;
				return new KnowledgeHubResponse(libraryView);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}
		public async Task<IKnowledgeHubResponse> GetFolders(int profileid)
		{
			try
			{
				var folders = await _appDbContext.Folders.Where(a => a.ParentFolderId == null).ToListAsync();


				List<FolderView> _views = new List<FolderView>();
				//var cours = _appDbContext.KnowledgeHubCourses.Include(a => a.ProviderType).Include(a => a.CourseType).Where(a => a.CategoryId == item.Id).ToList();
				foreach (var folder in folders)
				{
					bool IsAvailable = false;
					var data = await _appDbContext.Folders.Include(a=>a.Eresources).Where(a => a.ParentFolderId == folder.Id).ToListAsync();
					foreach (var item in data)
					{
						if (item.Eresources.Count > 0)
						{
							IsAvailable = true;
						}
					}
					var file = await _appDbContext.Eresources.Where(a => a.FolderId == folder.Id).ToListAsync();
					if (file != null && file.Count > 0)
					{
						IsAvailable = true;
					}
					if (IsAvailable)
					{
						FolderView view = new FolderView();
						view.Id = folder.Id;
						view.NameEn = folder.NameEn;
						view.NameAr = folder.NameAr;
						_views.Add(view);
					}

				}
				return new KnowledgeHubResponse(_views);
			}
			catch (Exception e)
			{
				return new KnowledgeHubResponse(e);
			}
		}
	}
}

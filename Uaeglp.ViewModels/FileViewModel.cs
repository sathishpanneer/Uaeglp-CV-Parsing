using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Uaeglp.ViewModels
{
	public class FileViewModel
	{
        private string _nameWithExtension;
        private byte[] _bytes;

        public int ID { get; set; }

        public string IDEncyrpted { get; set; }

        public Guid ID_GUID { get; set; }

        public string MimeType { get; set; }

        public string NameWithExtension
        {
            get
            {
                if (!string.IsNullOrEmpty(this._nameWithExtension))
                    return this._nameWithExtension;
                if (this.postedFiles == null || this.postedFiles.Count<IFormFile>() == 0)
                {
                    this._nameWithExtension = string.Empty;
                    return this._nameWithExtension;
                }
                this._nameWithExtension = this.postedFiles.First<IFormFile>().FileName;
                return this._nameWithExtension;
            }
            set
            {
                this._nameWithExtension = value;
            }
        }

        public byte[] Bytes
        {
            get
            {
                if (this._bytes != null)
                    return this._bytes;
                if (!string.IsNullOrEmpty(this.Base64String))
                {
                    this.Base64String = this.Base64String.Substring(this.Base64String.IndexOf(',') + 1);
                    this.Base64String = this.Base64String.Replace(" ", "+");
                    int num = this.Base64String.Length % 4;
                    if (num > 0)
                        this.Base64String += new string('=', 4 - num);
                    this._bytes = Convert.FromBase64String(this.Base64String);
                    this.Base64String = string.Empty;
                    return this._bytes;
                }
                if (this.postedFiles == null || this.postedFiles.Count<IFormFile>() == 0)
                {
                    this._bytes = new byte[0];
                    return this._bytes;
                }
                var httpPostedFileBase = this.postedFiles.First<IFormFile>();
                if (httpPostedFileBase != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        httpPostedFileBase.CopyTo((Stream)memoryStream);
                        this._bytes = memoryStream.ToArray();
                    }
                }
                return this._bytes;
            }
            set
            {
                this._bytes = value;
            }
        }

        public string Base64String { get; set; }

        public FileViewModel()
        {
        }

        public FileViewModel(Guid? ID_GUID)
        {
            this.ID_GUID = ID_GUID ?? Guid.Empty;
        }

        public bool? IsFileRemoved { get; set; }

        public IEnumerable<IFormFile> postedFiles { get; set; }

        public bool? IsValidationRemoved { get; set; }

        public bool? IsRequired { get; set; }

        public bool HasFile
        {
            get
            {
                Guid idGuid = this.ID_GUID;
                return !(this.ID_GUID == Guid.Empty) || this.Bytes != null && this.Bytes.Length != 0;
            }
        }

        public string FileURL
        {
            get
            {
                return this.ID_GUID == Guid.Empty ? string.Empty : string.Format("/File/Get/{0}", (object)this.ID_GUID);
            }
        }

        public Guid? CorrelationId { get; set; }

        public DateTime Modified { get; set; }

        public string ModifiedBy { get; set; }

        public string FontAwesomeClass { get; set; }

        public Decimal SizeMB { get; set; }

        public string ProviderName { get; set; }

        public string Tags { get; set; }
    }
}

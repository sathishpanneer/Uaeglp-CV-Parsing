using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using FileView = Uaeglp.ViewModels.ProfileViewModels.FileView;

namespace Uaeglp.Contracts.Communication
{
    public interface IFileResponse : IBaseResponse
    {
        List<FileView> DocumentsInfoList { get; set; }

        FileView FileView { get; set; }

        string UploadedFileUrl { get; set; }

        decimal ProfileCompletedPercentage { get; set; }
    }
}

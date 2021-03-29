using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Services.Communication
{
	public class ProgramResponse : BaseResponse, IProgramResponse
    {
		public List<CompletedProgramView> CompletedPrograms { get; set; }
		public List<ProgramView> ProgramViews { get; set; }

        public ProgramView ProgramView { get; set; }
        public string ReferenceNumber { get; set; }
        public ProfileCompletedViewModel ProfileCompletedView { get; set; }
        public ProgramCompletedDetailsViewModel ProgramCompletedDetails { get; set; }
        public List<QuestionViewModel> QuestionList { get; set; }
		

		private ProgramResponse(bool success, string message, List<CompletedProgramView> completedProgramViews) : base(success, message)
        {
            CompletedPrograms = completedProgramViews;
        }

        private ProgramResponse(bool success, string message, List<ProgramView> programViews) : base(success, message)
        {
            ProgramViews = programViews;
        }

        private ProgramResponse(bool success, string message,string reference) : base(success, message)
        {
            ReferenceNumber = reference;
        }

        private ProgramResponse(bool success, string message, ProfileCompletedViewModel programView) : base(success, message)
        {
            ProfileCompletedView = programView;
        }

        private ProgramResponse(bool success, string message, ProgramCompletedDetailsViewModel programView) : base(success, message)
        {
            ProgramCompletedDetails = programView;
        }
        private ProgramResponse(bool success, string message, ProgramView programView) : base(success, message)
        {
            ProgramView = programView;
        }


        private ProgramResponse(bool success, string message, List<QuestionViewModel> questionList) : base(success, message)
        {
            QuestionList = questionList;
        }


        public ProgramResponse(string message, HttpStatusCode status) : base(false, message, status)
		{ }


		public ProgramResponse(List<CompletedProgramView> completedProgramViews) : this(true, string.Empty, completedProgramViews)
		{ }

        public ProgramResponse(ProgramCompletedDetailsViewModel completedProgramViews) : this(true, string.Empty, completedProgramViews)
        { }
        public ProgramResponse(List<ProgramView> programViews) : this(true, string.Empty, programViews)
        { }

        public ProgramResponse(string reference) : this(true, string.Empty, reference)
        { }

        public ProgramResponse(ProgramView programView) : this(true, string.Empty, programView)
        { }

        public ProgramResponse(ProfileCompletedViewModel programView) : this(true, string.Empty, programView)
        { }
        public ProgramResponse(List<QuestionViewModel> questionList) : this(true, string.Empty, questionList)
        { }

        public ProgramResponse() : base()
        { }
    }
}

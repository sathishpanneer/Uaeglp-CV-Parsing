using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.ActivityViewModels;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Services.Communication
{
	public class ActivityAndChallengesResponse : BaseResponse, IActivityAndChallengesResponse
    {
		public List<InitiativeCategoryViewModel> ActivityCategories { get; set; }
        public List<InitiativeViewModel> ActivityList { get; set; }
        public InitiativeViewModel Activity { get; set; }
        public InitiativeProfileViewModel InitiativeProfileView { get; set; }
        public List<QuestionViewModel> QuestionViewModel { get; set; }


        private ActivityAndChallengesResponse(bool success, string message, List<InitiativeCategoryViewModel> activityCategories) : base(success, message)
        {
            ActivityCategories = activityCategories;
        }

        private ActivityAndChallengesResponse(bool success, string message, List<InitiativeViewModel> activityList) : base(success, message)
        {
            ActivityList = activityList;
        }

        private ActivityAndChallengesResponse(bool success, string message, InitiativeViewModel activity) : base(success, message)
        {
            Activity = activity;
        } 
        private ActivityAndChallengesResponse(bool success, string message, InitiativeProfileViewModel profileViewModel) : base(success, message)
        {
            InitiativeProfileView = profileViewModel;
        }

        private ActivityAndChallengesResponse(bool success, string message, List<QuestionViewModel> questionViewModel) : base(success, message)
        {
            QuestionViewModel = questionViewModel;
        }


        public ActivityAndChallengesResponse(List<InitiativeCategoryViewModel> viewModels) : this(true, string.Empty, viewModels)
        { }

        public ActivityAndChallengesResponse(List<InitiativeViewModel> viewModels) : this(true, string.Empty, viewModels)
        { }

        public ActivityAndChallengesResponse(InitiativeViewModel viewModel) : this(true, string.Empty, viewModel)
        { }     
        public ActivityAndChallengesResponse(InitiativeProfileViewModel viewModel) : this(true, string.Empty, viewModel)
        { }

        public ActivityAndChallengesResponse(string message, HttpStatusCode status) : base(false, message, status)
		{ }


        public ActivityAndChallengesResponse(List<QuestionViewModel> viewModels) : this(true, string.Empty, viewModels)
        { }

        public ActivityAndChallengesResponse(Exception e) : base(e)
        { }

        public ActivityAndChallengesResponse() : base()
        { }
    }
}

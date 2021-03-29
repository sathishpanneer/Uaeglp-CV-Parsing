using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Enums
{
    public enum ParentType
    {
        Post = 1,
        User = 2,
        Group = 3,
        KnowledgeHub = 4,
        Challenge = 5,
        EngagementActivities = 6,
        Batch = 7,
        Event = 8,
        Assessment = 9,
        Meetup = 10, // 0x0000000A
        AssessmentGroup = 11, // 0x0000000B
        AssignedAssessment = 12, // 0x0000000C
        AssessmentAdded = 13,
        SendRecommendation = 14,
        AskRecommendation = 15,
        AssessmentCoordinator = 16,
        Messaging = 17,
        Programme = 18,
        ///UserRecommendation = 13,
        //Programme = 14,   
        //Messaging = 15
    }
}

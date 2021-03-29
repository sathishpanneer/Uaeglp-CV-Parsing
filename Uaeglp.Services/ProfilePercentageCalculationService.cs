using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Services
{
    public class ProfilePercentageCalculationService : IProfilePercentageCalculationService
    {
        private readonly AppDbContext _appDbContext;
        public ProfilePercentageCalculationService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<decimal> UpdateProfileCompletedPercentageAsync(int profileId)
        {
            try
            {
                decimal completedPercentage = 0;


                var profile = await _appDbContext.Profiles
                                                .Include(k => k.ProfileEducations)
                                                .Include(k => k.ProfileWorkExperiences)
                                                .Include(k => k.ProfileAchievements)
                                                .Include(k => k.ProfileTrainings)
                                                .Include(k => k.ProfileAssessmentToolScores)
                                                .FirstOrDefaultAsync(k => k.Id == profileId).ConfigureAwait(false);

                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == profileId);

                if (userInfo.User.LargeImageFileId.HasValue && userInfo.User.OriginalImageFileId.HasValue && userInfo.User.SmallImageFileId.HasValue)
                {
                    completedPercentage += (decimal)ProfileCompletionPercentage.ProfileImageCompletion;
                }

                if (profile.FirstNameEn != null && profile.SecondNameEn != null &&
                    (profile.ThirdNameEn != null && profile.LastNameEn != null) &&
                    (profile.FirstNameAr != null && profile.SecondNameAr != null &&
                     (profile.ThirdNameAr != null && profile.LastNameAr != null)) &&
                    (profile.ResidenceCountryId.HasValue && profile.Address != null &&
                     profile.PassportNumber != null) && (profile.PassportIssueEmirateItemId.HasValue &&
                                                         (profile.BirthDate.HasValue && userInfo.Mobile != null &&
                                                          profile.Eid != null)))
                {
                    completedPercentage += (decimal)ProfileCompletionPercentage.BasicInfoCompletion;
                }

                if (profile.ProfileEducations.Any())
                {
                    completedPercentage += (decimal)ProfileCompletionPercentage.EducationCompletion;
                }

                if (profile.ProfileWorkExperiences.Any())
                {
                    completedPercentage += (decimal)ProfileCompletionPercentage.WorkExperienceCompletion;
                }

                if (profile.ProfileAchievements.Any())
                {
                    completedPercentage += (decimal)ProfileCompletionPercentage.AchievementCompletion;
                }

                if (profile.ProfileTrainings.Any())
                {
                    completedPercentage += (decimal)ProfileCompletionPercentage.TrainingCompletion;
                }


                var profileToolScore = profile.ProfileAssessmentToolScores.ToList();

                var personalityAssessment =
                    await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Personality);

                if (personalityAssessment != null)
                {
                    var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                    if (isCompleted && !CheckAssessmentValidity(personalityAssessment.Id, profileId,
                            personalityAssessment.ValidityRangeNumber))
                    {
                        completedPercentage += (decimal)ProfileCompletionPercentage.PersonalityAssessmentCompletion;
                    }

                }

                var eQAssessment =
                    await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.EQ);

                if (eQAssessment != null)
                {
                    var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                    if (isCompleted && !CheckAssessmentValidity(eQAssessment.Id, profileId,
                            eQAssessment.ValidityRangeNumber))
                    {
                        completedPercentage += (decimal)ProfileCompletionPercentage.EQAssessmentCompletion;
                    }

                }


                var wellBeingAssessment =
                    await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing);

                if (wellBeingAssessment != null)
                {
                    var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                    if (isCompleted && !CheckAssessmentValidity(wellBeingAssessment.Id, profileId,
                            wellBeingAssessment.ValidityRangeNumber))
                    {
                        completedPercentage += (decimal)ProfileCompletionPercentage.WellBeingAssessmentCompletion;
                    }

                }

                profile.CompletenessPercentage = completedPercentage;

                await _appDbContext.SaveChangesAsync();

                return completedPercentage;
            }
            catch (Exception e)
            {
                return 0;
            }


        }

        private bool CheckAssessmentValidity(
            int assessmentToolId,
            int profileId,
            int validityRangeNumber)
        {
            var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.Where(p => p.AssessmentToolId == assessmentToolId && p.ProfileId == profileId && p.IsCompleted == true).OrderByDescending(e => e.Order).FirstOrDefault();
            return assessmentToolScore == null || assessmentToolScore.Created.AddMonths(validityRangeNumber) <= DateTime.Now;
        }
    }
}

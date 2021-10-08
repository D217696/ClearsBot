using ClearsBot.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Bungie : IBungie
    {
        public DateTime ReleaseDate { get; set; } = new DateTime(2017, 09, 05, 17, 0, 0);
        readonly IBungieDestiny2RequestHandler _requestHandler;
        public Bungie(IBungieDestiny2RequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public async Task<RequestData> GetRequestDataAsync(string membershipId = "", string membershipType = "")
        {
            const long steamIdLower = 76561190000000000;
            const long steamIdUpper = 76561200000000000;
            int searchMembershipType = -1;

            RequestData requestData = new RequestData();

            if (membershipId == "")
            {
                requestData.DisplayName = "Please enter an ID or register.";
                requestData.Code = 2;

                return requestData;
            }

            if (membershipType != "")
            {
                bool succeed = int.TryParse(membershipType, out searchMembershipType);
                if (!succeed)
                {
                    requestData.DisplayName = "Please enter a valid membership type.";
                    requestData.Code = 3;

                    return requestData;
                }
            }

            bool parsedSucceedId = long.TryParse(membershipId, out long parsedMembershipId);

            if (!parsedSucceedId)
            {
                SearchDestinyPlayer profiles = await _requestHandler.SearchDestinyPlayerAsync(membershipId, searchMembershipType.ToString());
                if (profiles.ErrorCode != 1)
                {
                    requestData.Code = 7;
                    requestData.DisplayName = profiles.Message;
                    return requestData;
                }

                if (profiles.Response == null)
                {
                    requestData.DisplayName = "User not found.";
                    requestData.Code = 4;

                    return requestData;
                }

                if (profiles.Response.Count() > 1)
                {
                    requestData.profiles = profiles.Response;
                    requestData.Code = 8;
                    return requestData;
                }

                if (profiles.Response.Count() < 1)
                {
                    requestData.profiles = null;
                    requestData.Code = 9;
                    return requestData;
                }

                requestData.MembershipId = profiles.Response[0].MembershipId;
                requestData.MembershipType = profiles.Response[0].MembershipType;
            }
            else
            {
                if (membershipType == "")
                {
                    if (parsedMembershipId < steamIdLower || parsedMembershipId > steamIdUpper)
                    {
                        requestData.DisplayName = "Please enter a valid Steam Id.";
                        requestData.MembershipId = 0;
                        requestData.MembershipType = 0;
                        requestData.Code = 5;

                        return requestData;
                    }
                    else
                    {
                        GetMembershipFromHardLinkedCredential steamProfile = await _requestHandler.GetMembershipFromHardLinkedCredentialAsync(parsedMembershipId);
                        if (steamProfile.ErrorCode != 1)
                        {
                            requestData.Code = 7;
                            requestData.DisplayName = steamProfile.Message;
                            return requestData;
                        }
                        requestData.MembershipId = steamProfile.Response.MembershipId;
                        requestData.MembershipType = steamProfile.Response.MembershipType;
                        requestData.SteamID = parsedMembershipId;
                    }
                }
                else
                {
                    bool parsedSucceedType = int.TryParse(membershipType, out int parsedMembershipType);
                    if (!parsedSucceedType || parsedMembershipType > 5 || parsedMembershipType <= 0)
                    {
                        requestData.DisplayName = "Please enter a valid Bungie Id.";
                        requestData.MembershipId = 0;
                        requestData.MembershipType = 0;
                        requestData.Code = 6;

                        return requestData;
                    }
                    requestData.MembershipId = parsedMembershipId;
                    requestData.MembershipType = parsedMembershipType;
                }
            }

            GetProfile profile = await _requestHandler.GetProfileAsync(requestData.MembershipType, requestData.MembershipId, new[] { DestinyComponentType.Profiles });
            if (profile.ErrorCode != 1)
            {
                requestData.Code = 7;
                requestData.DisplayName = profile.Message;
                return requestData;
            }
            else
            {
                requestData.DisplayName = profile.Response.Profile.Data.UserInfo.DisplayName;
                requestData.DateLastPlayed = profile.Response.Profile.Data.DateLastPlayed;
            }

            return requestData;
        }

        public async Task<GetCompletionsResponse> GetCompletionsForUserAsync(User user)
        {
            GetHistoricalStatsForAccount getHistoricalStatsForAccount = await _requestHandler.GetHistoricalStatsForAccount(user.MembershipType, user.MembershipId);
            if (getHistoricalStatsForAccount.ErrorCode != 1)
            {
                //await Users.labsDMs.SendMessageAsync("GetCompletionsResponse returned an error " + getHistoricalStatsForAccount.Message);
                return new GetCompletionsResponse() { Code = getHistoricalStatsForAccount.ErrorCode, ErrorMessage = getHistoricalStatsForAccount.Message };
            }
            List<Task<GetActivityHistoryResponse>> tasks = new List<Task<GetActivityHistoryResponse>>();
            foreach (DestinyHistoricalStatsPerCharacter character in getHistoricalStatsForAccount.Response.Characters)
            {
                Character characterFromUser = user.Characters.Where(x => x.CharacterID == character.CharacterId).FirstOrDefault();
                if (characterFromUser == null)
                {
                    user.Characters.Add(new Character()
                    {
                        CharacterID = character.CharacterId,
                        Deleted = character.Deleted,
                        Handled = false
                    });
                    characterFromUser = user.Characters.Where(x => x.CharacterID == character.CharacterId).FirstOrDefault();
                }

                if (characterFromUser.Handled) continue;

                tasks.Add(Task.Run(() => GetCharacterPagesAsync(user.MembershipType, user.MembershipId, characterFromUser, user.DateLastPlayed)));
            }

            List<Task<GetFreshForCompletionResponse>> TasksToGetPGCR = new List<Task<GetFreshForCompletionResponse>>();

            foreach (var res in await Task.WhenAll(tasks))
            {
                foreach (GetActivityHistory page in res.Pages)
                {
                    if (page.ErrorCode != 1)
                    {
                        //await Users.labsDMs.SendMessageAsync("GetCompletionsResponse returned an error " + page.Message);
                        return new GetCompletionsResponse() { Code = page.ErrorCode, ErrorMessage = page.Message };
                    }

                    foreach (DestinyHistoricalStatsPeriodGroup activity in page.Response.Activities)
                    {
                        if (!(activity.Values["completionReason"].Basic.DisplayValue == "Objective Completed" && activity.Values["completed"].Basic.Value == 1.0)) continue;
                        if (user.Completions.ContainsKey(activity.ActivityDetails.InstanceId)) continue;
                        if (activity.Period > new DateTime(2020, 10, 10, 0, 0, 0))
                        {
                            user.Completions.Add(activity.ActivityDetails.InstanceId, new Completion()
                            {
                                InstanceID = activity.ActivityDetails.InstanceId,
                                Period = activity.Period,
                                RaidHash = activity.ActivityDetails.ReferenceId,
                                Kills = activity.Values["kills"].Basic.Value,
                                Time = TimeSpan.FromSeconds(activity.Values["activityDurationSeconds"].Basic.Value),
                                StartingPhaseIndex = 0
                            });

                            continue;
                        }

                        await Task.Delay(100);
                        TasksToGetPGCR.Add(Task.Run(() => GetFreshForCompletionAsync(new Completion()
                        {
                            InstanceID = activity.ActivityDetails.InstanceId,
                            Period = activity.Period,
                            RaidHash = activity.ActivityDetails.ReferenceId,
                            Kills = activity.Values["kills"].Basic.Value,
                            Time = TimeSpan.FromSeconds(activity.Values["activityDurationSeconds"].Basic.Value)
                        })));
                    }
                }
                if (res.Character.Deleted) user.Characters.Where(x => x.CharacterID == res.Character.CharacterID).FirstOrDefault().Handled = true;
            }

            foreach (var res in await Task.WhenAll(TasksToGetPGCR))
            {
                if (res.Code != 1) return new GetCompletionsResponse() { Code = res.Code, ErrorMessage = res.ErrorMessage };

                user.Completions.Add(res.Completion.InstanceID, res.Completion);
            }

            if (user.Completions.Count > 0)
            {
                user.DateLastPlayed = user.Completions.OrderByDescending(x => x.Value.Period).FirstOrDefault().Value.Period;
            }

            return new GetCompletionsResponse()
            {
                Code = 1,
                User = user,
                ErrorMessage = "Success"
            };
        }

        public async Task<GetFreshForCompletionResponse> GetFreshForCompletionAsync(Completion completion)
        {
            GetFreshForCompletionResponse getFreshForCompletionResponse = new GetFreshForCompletionResponse();
            GetPostGameCarnageReport getPostGameCarnageReport = await _requestHandler.GetPostGameCarnageReportAsync(completion.InstanceID);
            if (getPostGameCarnageReport.ErrorCode != 1)
            {
                getFreshForCompletionResponse.Code = getPostGameCarnageReport.ErrorCode;
                getFreshForCompletionResponse.ErrorMessage = getPostGameCarnageReport.Message;
                //await Users.labsDMs.SendMessageAsync("GetPostGameCarnageReport returned an error. " + getPostGameCarnageReport.Message);
                return getFreshForCompletionResponse;
            }

            completion.StartingPhaseIndex = getPostGameCarnageReport.Response.StartingPhaseIndex;
            getFreshForCompletionResponse.Completion = completion;
            getFreshForCompletionResponse.Code = 1;
            getFreshForCompletionResponse.ErrorMessage = "Success";
            return getFreshForCompletionResponse;
        }
        public async Task<GetActivityHistoryResponse> GetCharacterPagesAsync(int membershipType, long membershipId, Character character, DateTime releaseDate)
        {
            GetActivityHistoryResponse getActivityHistoryResponse = new GetActivityHistoryResponse()
            {
                Character = character
            };
            int page = 0;
            bool done = false;
            while (!done)
            {
                GetActivityHistory activityPage = await _requestHandler.GetActivityHistoryAsync(membershipType, membershipId, character.CharacterID, page);
                if (activityPage.ErrorCode != 1)
                {
                    getActivityHistoryResponse.Code = activityPage.ErrorCode;
                    getActivityHistoryResponse.ErrorMessage = activityPage.Message;
                    //await Users.labsDMs.SendMessageAsync("GetActivityHistory returned an error. " + getActivityHistoryResponse.ErrorMessage);
                    return getActivityHistoryResponse;
                }

                if (activityPage.Response.Activities != null)
                {
                    getActivityHistoryResponse.Pages.Add(activityPage);
                    page++;
                    if (activityPage.Response.Activities.Last().Period.Date < releaseDate.Date)
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }
            }

            getActivityHistoryResponse.Code = 1;
            getActivityHistoryResponse.ErrorMessage = "Success";
            return getActivityHistoryResponse;
        }
        public static string GetPlatformString(int membershipType)
        {
            return membershipType switch
            {
                1 => "xbox",
                2 => "playstation",
                3 => "steam",
                4 => "battle.net",
                5 => "stadia",
                _ => "",
            };
        }
    }
    public class RequestData
    {
        public string DisplayName { get; set; }
        public string Emblem { get; set; }
        public long MembershipId { get; set; }
        public int MembershipType { get; set; }
        public long SteamID { get; set; } = 0;
        public DateTime DateLastPlayed { get; set; }
        public int Code { get; set; } = 1;
        public UserInfoCard[] profiles = null;
    }
    public class GetCompletionsResponse
    {
        public User User { get; set; }
        public int Code { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class GetActivityHistoryResponse
    {
        public Character Character { get; set; }
        public List<GetActivityHistory> Pages { get; set; } = new List<GetActivityHistory>();
        public int Code { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class GetFreshForCompletionResponse
    {
        public Completion Completion { get; set; }
        public int Code { get; set; }
        public string ErrorMessage { get; set; }
    }
}

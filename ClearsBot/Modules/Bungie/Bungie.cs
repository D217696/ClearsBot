using ClearsBot.Modules;
using ClearsBot.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Bungie : IBungie
    {
        public DateTime ReleaseDate { get; set; } = new DateTime(2017, 09, 05, 17, 0, 0);
        readonly IBungieDestiny2RequestHandler _requestHandler;
        readonly Database _database;
        public Bungie(IBungieDestiny2RequestHandler requestHandler, Database database)
        {
            _requestHandler = requestHandler;
            _database = database;
        }

        public async Task<RequestData> GetRequestDataAsync(string membershipId = "", string membershipType = "")
        {
            if (!int.TryParse(membershipType, out int membershipTypeInt))
            {
                membershipTypeInt = -1;
            }

            if (!long.TryParse(membershipId, out long membershipIdLong))
            {
                membershipIdLong = 0;
            }

            if (membershipType == "")
            {
                membershipType = "-1";
            }

            List<Task<GetProfile>> getProfileTasks = new List<Task<GetProfile>>()
            {
                Task.Run(() => _requestHandler.GetProfileAsync(1, membershipIdLong, new DestinyComponentType[] { DestinyComponentType.Profiles })),
                Task.Run(() => _requestHandler.GetProfileAsync(2, membershipIdLong, new DestinyComponentType[] { DestinyComponentType.Profiles })),
                Task.Run(() => _requestHandler.GetProfileAsync(3, membershipIdLong, new DestinyComponentType[] { DestinyComponentType.Profiles })),
                Task.Run(() => _requestHandler.GetProfileAsync(5, membershipIdLong, new DestinyComponentType[] { DestinyComponentType.Profiles }))
            };

            var searchDestinyPlayerTask = Task.Run(() => _requestHandler.SearchDestinyPlayerAsync(membershipId, membershipType));
            var getMembershipFromHardlinkedCredentialTask = Task.Run(() => _requestHandler.GetMembershipFromHardLinkedCredentialAsync(membershipIdLong));

            await Task.WhenAll(searchDestinyPlayerTask);
            await Task.WhenAll(getMembershipFromHardlinkedCredentialTask);

            List<UserInfoCard> infoCards = new List<UserInfoCard>();
            foreach (var res in await Task.WhenAll(getProfileTasks))
            {
                if (res.Response != null) infoCards.Add(res.Response.Profile.Data.UserInfo);
            }

            if (searchDestinyPlayerTask.Result.Response.Count() > 0)
            {
                infoCards.AddRange(searchDestinyPlayerTask.Result.Response);
            }

            if (getMembershipFromHardlinkedCredentialTask.Result.Response != null)
            {
                infoCards.Add(new UserInfoCard()
                {
                    MembershipId = getMembershipFromHardlinkedCredentialTask.Result.Response.MembershipId,
                    MembershipType = getMembershipFromHardlinkedCredentialTask.Result.Response.MembershipType
                });
            }

            if (infoCards.Count == 1)
            {
                return new RequestData()
                {
                    Code = 1,
                    MembershipId = infoCards.FirstOrDefault().MembershipId,
                    MembershipType = infoCards.FirstOrDefault().MembershipType,
                    DisplayName = infoCards.FirstOrDefault().DisplayName,
                    profiles = infoCards.ToArray()
                };
            }

            if (infoCards.Count > 1)
            {
                List<Task<GetLinkedProfiles>> tasks = new List<Task<GetLinkedProfiles>>();
                foreach (UserInfoCard userInfoCard in infoCards)
                {
                    tasks.Add(Task.Run(() => _requestHandler.GetLinkedProfilesAsync(userInfoCard.MembershipType, userInfoCard.MembershipId)));
                }

                List<UserInfoCard> userInfoCards = new List<UserInfoCard>();

                foreach (var res in await Task.WhenAll(tasks))
                {
                    if (res.ErrorCode == 1)
                    {
                        foreach (var profile in res.Response.Profiles)
                        {
                            if (userInfoCards.FirstOrDefault(x => x.MembershipId == profile.MembershipId) != null) continue;
                            userInfoCards.Add(new UserInfoCard()
                            {
                                MembershipId = profile.MembershipId,
                                MembershipType = profile.MembershipType,
                                DisplayName = profile.BungieGlobalDisplayName
                            });
                        }
                    }
                }

                if (userInfoCards.Count > 1)
                {
                    return new RequestData()
                    {
                        Code = 8,
                        profiles = userInfoCards.ToArray()
                    };
                }

                if (userInfoCards.Count > 0)
                {
                    return new RequestData()
                    {
                        Code = 1,
                        MembershipId = userInfoCards.FirstOrDefault().MembershipId,
                        MembershipType = userInfoCards.FirstOrDefault().MembershipType,
                        DisplayName = userInfoCards.FirstOrDefault().DisplayName,
                        profiles = userInfoCards.ToArray()
                    };
                }

            }

            return new RequestData()
            {
                Code = 4
            };
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
                Character characterFromUser = user.Characters.Where(x => x.CharacterId == character.CharacterId).FirstOrDefault();
                if (characterFromUser == null)
                {
                    user.Characters.Add(new Character()
                    {
                        CharacterId = character.CharacterId,
                        Deleted = character.Deleted,
                        Handled = false
                    });
                    characterFromUser = user.Characters.Where(x => x.CharacterId == character.CharacterId).FirstOrDefault();
                }

                if (characterFromUser.Handled) continue;

                tasks.Add(Task.Run(() => GetCharacterPagesAsync(user.MembershipType, user.MembershipId, characterFromUser, user.DateLastPlayed)));
            }

            List<Task<GetFreshForCompletionResponse>> TasksToGetPGCR = new List<Task<GetFreshForCompletionResponse>>();
           //List<Task<GetPostGameCarnageReport>> TasksToGetPgcrs = new List<Task<GetPostGameCarnageReport>>();

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
                        //TasksToGetPgcrs.Add(Task.Run(() => GetPostGameCarnageReport(activity.ActivityDetails.InstanceId)));
                        if (activity.Period > new DateTime(2020, 11, 10, 17, 0, 0))
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
                if (res.Character.Deleted) user.Characters.Where(x => x.CharacterId == res.Character.CharacterId).FirstOrDefault().Handled = true;
            }

            foreach (var res in await Task.WhenAll(TasksToGetPGCR))
            {
                if (res.Code != 1) return new GetCompletionsResponse() { Code = res.Code, ErrorMessage = res.ErrorMessage };

                user.Completions.Add(res.Completion.InstanceID, res.Completion);
            }

            //_database.InsertPgcrsIntoDb(await Task.WhenAll(TasksToGetPgcrs));

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

        public async Task<GetPostGameCarnageReport> GetPostGameCarnageReport(long instanceId)
        {
            return await _requestHandler.GetPostGameCarnageReportAsync(instanceId);
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
                GetActivityHistory activityPage = await _requestHandler.GetActivityHistoryAsync(membershipType, membershipId, character.CharacterId, page);
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

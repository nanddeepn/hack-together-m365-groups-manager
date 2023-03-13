using hack_together_groups_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using System.Diagnostics.Metrics;

namespace hack_together_groups_manager.Pages
{
    public class GroupDetailsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public Models.M365Group M365GroupDetails { get; set; }

        [BindProperty]
        public string? GroupId { get; set; }

        [BindProperty]
        public string? MemberToAdd { get; set; }

        [BindProperty]
        public string? OwnerToAdd { get; set; }

        public GroupDetailsModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        public async Task OnGetAsync()
        {
            GroupId = HttpContext.Request.Query["groupId"];
            M365Group groupInfo = await GetGroupInfo(GroupId);
            M365GroupDetails = groupInfo;
        }

        private async Task<M365Group> GetGroupInfo(string GroupId)
        {
            var groupInfo = new Models.M365Group();

            if (GroupId != null)
            {
                var resultGroupInfo = await this._graphServiceClient.Groups[GroupId].Request().GetAsync();
                if (resultGroupInfo != null)
                {
                    var group = resultGroupInfo as Microsoft.Graph.Group;

                    groupInfo.Id = group.Id;
                    groupInfo.Description = group.Description;
                    groupInfo.DisplayName = group.DisplayName;
                    groupInfo.Visibility = group.Visibility;
                }

                var resultMembersInfo = await this._graphServiceClient.Groups[GroupId].Members.Request().GetAsync();
                if (resultMembersInfo != null)
                {
                    groupInfo.Members = new List<string>();
                    foreach (var member in resultMembersInfo)
                    {
                        groupInfo.Members.Add((member as Microsoft.Graph.User).UserPrincipalName);
                    }
                }

                var me = await this._graphServiceClient.Me.Request().GetAsync();
                var resultOwnersInfo = await this._graphServiceClient.Groups[GroupId].Owners.Request().GetAsync();
                if (resultOwnersInfo != null)
                {
                    groupInfo.Owners = new List<string>();
                    foreach (var owner in resultOwnersInfo)
                    {
                        var upn = (owner as Microsoft.Graph.User).UserPrincipalName;
                        if (me.UserPrincipalName == upn)
                        {
                            groupInfo.UserRole = "Owner";
                        }

                        groupInfo.Owners.Add(upn);
                    }
                }
            }

            return groupInfo;
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var groupIdValue = Request.Form["groupId"];
            await this._graphServiceClient.Groups[groupIdValue.ToString()].Request().DeleteAsync();
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostLeave()
        {
            var groupIdValue = Request.Form["groupId"];
            var me = await this._graphServiceClient.Me.Request().GetAsync();

            M365Group groupInfo = await GetGroupInfo(groupIdValue);
            if (groupInfo.UserRole == "Owner")
            {
                await this._graphServiceClient.Groups[groupIdValue.ToString()].Owners[me.Id].Reference.Request().DeleteAsync();
            }

            await this._graphServiceClient.Groups[groupIdValue.ToString()].Members[me.Id].Reference.Request().DeleteAsync();
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAddOwner()
        {
            var ownerQuery = await this._graphServiceClient.Users.Request().Filter($"userPrincipalName eq '{OwnerToAdd}'").GetAsync();

            var owner = ownerQuery.FirstOrDefault();
            if (owner != null)
            {
                var groupIdValue = Request.Form["groupId"];
                await this._graphServiceClient.Groups[groupIdValue].Owners.References.Request().AddAsync(owner);

                M365Group groupInfo = await GetGroupInfo(groupIdValue);
                M365GroupDetails = groupInfo;
                OwnerToAdd = string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddMember()
        {
            var memberQuery = await this._graphServiceClient.Users.Request().Filter($"userPrincipalName eq '{MemberToAdd}'").GetAsync();

            var member = memberQuery.FirstOrDefault();
            if (member != null)
            {
                var groupIdValue = Request.Form["groupId"];
                await this._graphServiceClient.Groups[groupIdValue].Members.References.Request().AddAsync(member);

                M365Group groupInfo = await GetGroupInfo(groupIdValue);
                M365GroupDetails = groupInfo;
                MemberToAdd = string.Empty;
            }

            return Page();
        }
    }
}

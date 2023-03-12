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

        public GroupDetailsModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        public async Task OnGetAsync()
        {
            string groupId = HttpContext.Request.Query["groupId"];
            var  groupInfo = new Models.M365Group();

            if (groupId != null)
            {
                var resultGroupInfo = await this._graphServiceClient.Groups[groupId].Request().GetAsync();
                if (resultGroupInfo != null)
                {
                    var group = resultGroupInfo as Microsoft.Graph.Group;

                    groupInfo.Description = group.Description;
                    groupInfo.DisplayName = group.DisplayName;
                    groupInfo.Visibility = group.Visibility;
                }

                var resultMembersInfo = await this._graphServiceClient.Groups[groupId].Members.Request().GetAsync();
                if (resultMembersInfo != null)
                {
                    groupInfo.Members = new List<string>();
                    foreach (var member in resultMembersInfo)
                    {
                        groupInfo.Members.Add((member as Microsoft.Graph.User).UserPrincipalName);
                    }
                }

                var resultOwnersInfo = await this._graphServiceClient.Groups[groupId].Owners.Request().GetAsync();
                if (resultOwnersInfo != null)
                {
                    groupInfo.Owners = new List<string>();
                    foreach (var owner in resultOwnersInfo)
                    {
                        groupInfo.Owners.Add((owner as Microsoft.Graph.User).UserPrincipalName);
                    }
                }
            }

            M365GroupDetails = groupInfo;
        }
    }
}

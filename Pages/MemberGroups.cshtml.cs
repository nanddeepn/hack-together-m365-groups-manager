using hack_together_groups_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

namespace hack_together_groups_manager.Pages
{
    public class MemberGroupsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public IEnumerable<Models.M365Group>? M365MemberGroups { get; set; }

        public MemberGroupsModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        public async Task OnGetAsync()
        {
            List<Models.M365Group> m365Groups = new();

            var resultsMemberOf = await this._graphServiceClient.Me.MemberOf.Request().GetAsync();
            foreach (var groupDirectoryObject in resultsMemberOf)
            {
                var group = groupDirectoryObject as Microsoft.Graph.Group;
                if (group != null)
                {
                    var groupInfo = new Models.M365Group
                    {
                        Id = group.Id,
                        DisplayName = group.DisplayName,
                        Description = group.Description,
                        Visibility = group.Visibility
                    };

                    m365Groups.Add(groupInfo);
                }
            }

            M365MemberGroups = m365Groups;
        }
    }
}

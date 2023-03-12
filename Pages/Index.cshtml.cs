using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using System.Reflection;

namespace hack_together_groups_manager.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public IEnumerable<Models.M365Group>? M365Groups { get; set; }

        public IndexModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        public async Task OnGetAsync()
        {
            List<Models.M365Group> m365Groups = new();

            var resultsOwnerOf = await this._graphServiceClient.Me.OwnedObjects.Request().GetAsync();
            foreach (var ownedObject in resultsOwnerOf)
            {
                var group = ownedObject as Microsoft.Graph.Group;
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

            M365Groups = m365Groups;
        }
        public ActionResult GroupDetails(string Id)
        {
            return RedirectToAction("GroupDetails");
        }

        public ActionResult Edit(string Id)
        {
            return RedirectToAction("GroupDetails");
        }
    }
}
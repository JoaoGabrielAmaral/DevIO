using System.Collections.Generic;

namespace DevIO.Api.ViewModels.Auth
{
    public class UserTokenViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimViewModel> Claims { get; set; }
    }
}

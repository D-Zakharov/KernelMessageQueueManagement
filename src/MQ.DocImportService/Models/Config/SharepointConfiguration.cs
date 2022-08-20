using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.DocImportService.Models.Config;

public class SharepointConfiguration
{
    private const string ConfigIsIncorrectMsg = "Sharepoint configuration is incorrect";

    public string? SiteUrl { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }

    public void CheckData()
    {
        if (SiteUrl is null)
            throw new ArgumentNullException(nameof(SiteUrl), ConfigIsIncorrectMsg);
        if (Login is null)
            throw new ArgumentNullException(nameof(Login), ConfigIsIncorrectMsg);
        if (Password is null)
            throw new ArgumentNullException(nameof(Password), ConfigIsIncorrectMsg);
    }
}

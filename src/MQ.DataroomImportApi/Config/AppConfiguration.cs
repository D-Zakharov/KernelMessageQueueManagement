using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.DataroomImportApi.Config;

public class AppConfiguration
{
    public long MaxFileSizeInBytes { get; set; }
    public string? TempFolderForContent { get; set; }
}

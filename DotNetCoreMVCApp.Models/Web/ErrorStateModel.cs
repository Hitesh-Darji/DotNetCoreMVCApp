using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreMVCApp.Models.Web
{
    public class ErrorStateModel
    {
        public bool IsValid { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new();
    }
}

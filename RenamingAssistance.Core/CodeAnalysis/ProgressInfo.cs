using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class ProgressInfo
    {
        public ProgressInfo(string message, int percentPassed)
        {
            Message = message;
            PercentPassed = percentPassed;
        }

        public string Message { get; }

        public int PercentPassed { get; }
    }
}

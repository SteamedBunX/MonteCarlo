using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    class ProgressReportModel
    {
       
        public int MaxAmount { get; set; }
        public int CurrentAmount { get; set; }
        public ProgressReportModel(int currentAmount, int maxAmount)
        {
            CurrentAmount = currentAmount;
            MaxAmount = maxAmount;
        }
    }
}

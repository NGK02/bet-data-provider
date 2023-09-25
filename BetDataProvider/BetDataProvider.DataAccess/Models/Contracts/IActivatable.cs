using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Models.Contracts
{
    public interface IActivatable
    {
        bool IsActive { get; set; }
    }
}

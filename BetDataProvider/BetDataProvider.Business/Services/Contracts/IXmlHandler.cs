using BetDataProvider.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.Business.Services.Contracts
{
    public interface IXmlHandler
    {
        Task<Sport> GetAndParseXmlDataAsync();

        // remove
        //Sport GetAndParseXmlDataAsync();

        public bool SaveSportData(Sport sportData);
    }
}

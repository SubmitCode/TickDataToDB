using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TickDataImporter
{
    interface ITickDate
    {
        String GetSymbol();

        DateTime GetDateTime();

        Double GetPrice();

        long GetVolume();
    }
}
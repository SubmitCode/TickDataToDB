using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TickDataImporter
{
    interface IBaraDate
    {
        DateTime GetDateTime();

        String GetSymbol();

        Double GetOpen();

        Double GetHigh();

        Double GetLow();

        Double GetClose();

        long GetVolume();
    }
}
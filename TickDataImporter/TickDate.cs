using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TickDataImporter
{
    internal class TickDate
    {
        private string InstrumentID;
        private string DateTime;
        private double Price;
        private int Volueme;
        private string MarketFlag;
        private int SalesCondition;
        private string ExcludeFlag;
        private double UnfilteredPrice;

        private TickDate() { }

        public TickDate(string tickentry, string instrumentID)
        {
            string[] values = tickentry.Split(',');
            if (values.Length > 0)
            {
                this.InstrumentID = instrumentID;
                DateTime = values[0] + " " + values[1];
                Price = Convert.ToDouble(values[2]);
                Volueme = Convert.ToInt16(values[3]);
                MarketFlag = values[4];
                if (values.Length == 5)
                {
                    SalesCondition = 0;
                    ExcludeFlag = "";
                    UnfilteredPrice = 0;
                }
                else if (values.Length == 8)
                {
                    SalesCondition = Convert.ToInt16(values[5]);
                    ExcludeFlag = values[6];
                    UnfilteredPrice = Convert.ToDouble(values[7]);
                }
            }
            else
            {
                throw new Exception("No Tickdata entry identified");
            }
        }

        public string[] GetTickDataEntries()
        {
            return new string[8]
            {
                InstrumentID,
                DateTime,
                Price.ToString(),
                Volueme.ToString(),
                MarketFlag,
                SalesCondition.ToString(),
                ExcludeFlag,
                UnfilteredPrice.ToString()
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TickDataImporter
{
    internal class TickDate : ITickDate
    {
        private string DateTime;
        private string ExcludeFlag;
        private string InstrumentID;
        private string MarketFlag;
        private double Price;
        private string SalesCondition;
        private double UnfilteredPrice;
        private int Volueme;

        public TickDate(string tickentry, string instrumentID)
        {
            string[] values = tickentry.Split(',');
            if (values.Length > 0)
            {
                this.InstrumentID = instrumentID;
                DateTime = values[0].Substring(3, 2) + "-" + values[0].Substring(0, 2) + "-" + values[0].Substring(6, 4) + "  " + values[1];
                Price = Convert.ToDouble(values[2]);
                Volueme = Convert.ToInt16(values[3]);
                MarketFlag = values[4];
                if (values.Length == 5)
                {
                    SalesCondition = "";
                    ExcludeFlag = "";
                    UnfilteredPrice = 0;
                }
                else if (values.Length == 8)
                {
                    SalesCondition = values[5];
                    ExcludeFlag = values[6];
                    UnfilteredPrice = Convert.ToDouble(values[7]);
                }
            }
            else
            {
                throw new Exception("No Tickdata entry identified");
            }
        }

        private TickDate() { }

        public DateTime GetDateTime()
        {
            return Convert.ToDateTime(DateTime);
        }

        public double GetPrice()
        {
            return Price;
        }

        public string GetSymbol()
        {
            return InstrumentID;
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

        public long GetVolume()
        {
            return Volueme;
        }

        public override string ToString()
        {
            return InstrumentID + "," +
                DateTime + "," +
                Price.ToString() + "," +
                Volueme.ToString() + "," +
                MarketFlag + "," +
                SalesCondition.ToString() + "," +
                ExcludeFlag + "," +
                UnfilteredPrice.ToString();
        }
    }
}
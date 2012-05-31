using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TickDataImporter
{
    public enum IntradayFrequency
    {
        OneMinute = 1, FiveMinutes = 5, FifteenMinutes = 15, ThirtyMinutes = 30, OneHour = 60
    }

    internal class TickToBarConverter
    {
        private List<TickDate> lstTickData;

        private TickToBarConverter() { }

        public TickToBarConverter(IEnumerable<TickDate> tickData)
        {
            this.lstTickData = (List<TickDate>)tickData;
        }

        public IEnumerable<IBaraData> GetBarData(IntradayFrequency frequency)
        {
            throw new NotImplementedException();
        }

        private List<DateTime> GetDateTimeList(DateTime maxDate, DateTime minDate, IntradayFrequency freq)
        {
            var finalDateList = new List<DateTime>();
            // round maxDate to Frequency for instance if frequency is 15 minutes and mintime is 09:05
            var remainderMinutes = maxDate.Minute % Convert.ToInt16(freq);
            maxDate = maxDate.AddMinutes(Convert.ToDouble(freq) - remainderMinutes);
            maxDate = maxDate.AddSeconds(-maxDate.Second);
            maxDate = maxDate.AddMilliseconds(-maxDate.Millisecond);
            // round minDate to Frequency for instance if frequency is 15 minutes and mintime is 09:05
            remainderMinutes = minDate.Minute % Convert.ToInt16(freq);
            var sDate = minDate.AddMinutes(Convert.ToDouble(freq) - remainderMinutes);
            sDate = sDate.AddSeconds(-sDate.Second);
            sDate = sDate.AddMilliseconds(-sDate.Millisecond);
            while (sDate <= maxDate)
            {
                finalDateList.Add(new DateTime(sDate.Ticks));
                sDate = sDate.AddMinutes(Convert.ToDouble(freq));
            }
            return finalDateList;
        }
    }
}
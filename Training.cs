using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teretana
{
    internal class Training
    {
        public DateTime startTime;
        public DateTime endTime;

        public string GetStartTimeInMySqlFormat()
        {
            return String.Format("{0:yyyy/MM/dd}", startTime).Replace('/','-');
        } 
        public string Date { set; get; }
        public string From { set; get; }
        public string Until { set; get; }
        public Training(DateTime startTime)
        {
            this.startTime = startTime;
            Date = GetStartDate();
            From = GetStartHours();
        }
        public void SetEndTime(DateTime endTime)
        {
            this.endTime = endTime;
            Until = GetEndHours();
        }
        public Training(DateTime startTime, DateTime endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            Date = GetStartDate();
            From = GetStartHours();
            Until = GetEndHours();
        }
        public string GetStartDate()
        {
            return startTime.ToShortDateString();
        }
        public string GetStartHours()
        {
            return startTime.ToString("HH:mm");
        }
        public string GetEndHours()
        {
            try
            {
                return endTime.ToString("HH:mm");
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}

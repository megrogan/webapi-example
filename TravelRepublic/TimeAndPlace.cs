using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TravelRepublic
{
    public class TimeAndPlace
    {
        public DateTime Time { get; set; }
        public Place Place { get; set; }

        public override string ToString()
        {
            var str = "";

            if (Place != null)
                str += "(" + Place.Name + " @ ";

            str += Time.ToString(CultureInfo.InvariantCulture);

            if (Place != null)
                str += ")";

            return str;
        }
    }
}

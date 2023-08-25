using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace It_Supporter.Convert
{
    public class CustomerDateFormat : IsoDateTimeConverter
    {
        public CustomerDateFormat()
        {
            base.DateTimeFormat = "yyyy-mm-dd";
        }
    }
}

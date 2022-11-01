using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hinon_Mro_WebApi.Models
{
    public class ErrorLogModelClass
    {
        public string AppOrderID { get; set; }
        public string ErrorDesc { get; set; }
        public DateTime InsertedDateTime { get; set; }
        public string BrCode { get; set; }

    }
    public class ErrorResponse
    {
        public List<ErrorLogModelClass> Response { get; set; }
    }
}

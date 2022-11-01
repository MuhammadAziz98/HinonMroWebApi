using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Hinon_Mro_WebApi.Models
{
    public class ConStrings
    {
        public string ConDeliveryManagementDB()
        {
            return WebConfigurationManager.ConnectionStrings["ConDeliveryManagementDB"].ConnectionString;
        }
        //public static string constrFn()
        //{
        //    return WebConfigurationManager.ConnectionStrings["ConSalesdb"].ConnectionString;
        //}
        //public static string constrPraxisDB()
        //{
        //    return WebConfigurationManager.ConnectionStrings["PraxisDBConnectionString"].ConnectionString;
        //}

    }
}

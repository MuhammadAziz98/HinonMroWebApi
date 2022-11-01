using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hinon_Mro_WebApi.Models
{

    public class MasterDataList
    {
        public string autoid { get; set; }
        public string apporderid { get; set; }
        public string orderdate { get; set; }
        public string userid { get; set; }
        public string custcode { get; set; }
        public string custname { get; set; }
        public string targetbranch { get; set; }
        public string deliverydate { get; set; }
        public string ordertype { get; set; }
        public string orderstatus { get; set; }
        public string poweruserid { get; set; }
        public string cancelreasonid { get; set; }
        public string cancelreasondesc { get; set; }
        public string DsfName { get; set; }
    }

    public class DetailItemList
    {
        public string detailautoid { get; set; }
        public string autoid { get; set; }
        public string prodmastid { get; set; }
        public string prodname { get; set; }
        public string orderqty { get; set; }
    }


    public class ResponseList
    {
        public List<MasterDataList> MasterDataList { get; set; }
        public List<DetailItemList> MasterDetailDataList { get; set; }
    }
}
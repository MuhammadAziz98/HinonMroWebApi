using Hinon_Mro_WebApi.Filters;
using Hinon_Mro_WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Hinon_Mro_WebApi.Controllers
{
    [BasicAuthentication]
    public class LoadController : ApiController
    {
        
        DataTable dt;
        SqlParameter[] param;
        DBFunction db;
        ResponseList rl;
        ConStrings con = new ConStrings();
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
        string AutoID;
       [HttpPost]
        public IHttpActionResult DataInsert([FromBody] ResponseList DataList)
        {
            try
            {
                var json = JsonConvert.SerializeObject(DataList);
                WriteToFile(json + "\r\n" + "**************************************************************End JSON********************************************************");
                
                db = new DBFunction();
                dt = new DataTable();
                rl = new ResponseList();
                string Status_Code = null;
                string Status_Message = null;
                List<ResponseList> Loadobj = new List<ResponseList>();

                //For Validation
                // if IsSuccess is 0 no data inserted and Load Deleted
                // if IsSuccess is 1 means no error found and Load Inserted Successfully
                int IsSuccess = 1;

                DataSet ds = new DataSet();
                List<MasterDataList> obj_MasterDataList = DataList.MasterDataList.ToList();
                List<DetailItemList> obj_DetailItemList = DataList.MasterDetailDataList.ToList();
                string AppOrderID = DataList.MasterDataList[0].apporderid;
                List<ErrorLogModelClass> errorlog = new List<ErrorLogModelClass>();
                ErrorResponse er = new ErrorResponse();
                //Get max errorcounter id for new error to show on User side(salesflo)

                string ErrorCounter = (db.CrudScalarQuery(con.ConDeliveryManagementDB(), "exec sp_GetErrorCounter '" + AppOrderID + "' ")).ToString();


                //Check LOADId here if its Already Exists then return error, if not exist then Insert In DB
                var CheckAppOrderIDExistance = db.SelectWithQueryDT(con.ConDeliveryManagementDB(),$@"select * from mro_ordermaster where apporderid='{AppOrderID}'");
                if (CheckAppOrderIDExistance.Rows.Count > 0)
                {

                    string errordesc = "App OrderID is already received " + CheckAppOrderIDExistance.Rows[0][1];
                    InserError(AppOrderID, errordesc, null, ErrorCounter);

                    
                    Status_Code = "4002";
                    Status_Message = "Data Not Inserted";
                    errorlog.Add(new ErrorLogModelClass
                    {
                        AppOrderID = AppOrderID,
                        BrCode = null,
                        ErrorDesc = errordesc,
                        InsertedDateTime =DateTime.Now,
                    }); 
                    er.Response = errorlog;

                    return Ok(new { Status_Code, Status_Message, er.Response });

                }
                else
                {

                    //****************Some values  are fix by default while inserting in Database****************************
                    //Userid=991003499
                    //orderstatus=0
                        
                        string query = "exec sp_Mro_ordermaster_Insert '"+ obj_MasterDataList[0].apporderid +"','"+ obj_MasterDataList[0].orderdate +"',"+ obj_MasterDataList[0].userid +","+ obj_MasterDataList[0].custcode +",'"+ obj_MasterDataList[0].custname +"' ";
                        AutoID = (db.CrudScalarQuery(con.ConDeliveryManagementDB(),query)).ToString();
                        foreach (var item in obj_DetailItemList)
                        {
                            string query1 = "exec sp_Mro_orderDetail_Insert " + AutoID + "," + item.prodmastid + ",'" + item.prodname + "'," +item.orderqty+ " ";
                            string AutoID1 = (db.CrudScalarQuery(con.ConDeliveryManagementDB(), query1)).ToString();
                        }

                    


                    Status_Code = "200";
                    Status_Message = "Data Inserted Successfully";

                    return Ok(new { Status_Code, Status_Message, Response = Loadobj });

                }

            }
            catch (Exception ex)
            {
                db.CrudScalarQuery(con.ConDeliveryManagementDB(), "delete from mro_orderdetail where autoid="+AutoID+ ";delete from mro_ordermaster where autoid=" + AutoID + " ");
                InserError(DataList.MasterDataList[0].apporderid, ex.Message.Replace("'", " "), null, "99");
                return Ok(new { Status_Code = "400", Status_Message = "Error", Response = ex.Message.Replace("'", " ") });
            }

        }

        public void InserError(string AppOrderID, String errordesc, string BrCode, string ErrorCounter)
        {
            db.SelectWithQueryDT(con.ConDeliveryManagementDB(),"insert into tblHinon_Log (AppOrderID,ErrorDesc,BrCode,ErrorCounter,LogFrom) values ('" + AppOrderID + "', '" + errordesc + "','" + BrCode + "'," + ErrorCounter + ",'GetApi')");
        }


        [HttpPost]
        public IHttpActionResult DataSelect(DateTime Fromdate,DateTime ToDate)
        {
            string Status_Code = null;
            string Status_Message = null;
            ResponseList Loadobj = new ResponseList();
            DataTable dtMaster = new DataTable();
            DataTable dtMasterdetail = new DataTable();

            List<MasterDataList> masterDataList = new List<MasterDataList>();
            List<DetailItemList> detailItemsList = new List<DetailItemList>();
            db = new DBFunction();
            dtMaster = db.SelectWithQueryDT(con.ConDeliveryManagementDB(), "select * from mro_ordermaster where orderdate between '"+Fromdate+"' and '"+ToDate+"' ");
            dtMasterdetail = db.SelectWithQueryDT(con.ConDeliveryManagementDB(), "select * from mro_orderdetail where autoid in (select autoid from mro_ordermaster where orderdate between '" + Fromdate + "' and '" + ToDate + "')");

            foreach (DataRow row in dtMaster.Rows)
            {
                masterDataList.Add(new MasterDataList
                {
                    apporderid = row["apporderid"].ToString(),
                    cancelreasondesc = row["cancelreasondesc"].ToString(),
                    cancelreasonid = row["cancelreasonid"].ToString(),
                    autoid = row["autoid"].ToString(),
                    custcode = row["custcode"].ToString(),
                    custname=row["custname"].ToString(),
                    deliverydate= row["deliverydate"].ToString(),
                    DsfName= row["DsfName"].ToString(),
                    orderdate= row["orderdate"].ToString(),
                    orderstatus= row["orderstatus"].ToString(),
                    ordertype= row["ordertype"].ToString(),
                    poweruserid= row["poweruserid"].ToString(),
                    targetbranch= row["targetbranch"].ToString(),
                    userid= row["userid"].ToString()

                }) ;
            }

            foreach(DataRow row in dtMasterdetail.Rows)
            {
                detailItemsList.Add(new DetailItemList
                {
                    autoid = row["autoid"].ToString(),
                    detailautoid = row["detailautoid"].ToString(),
                    orderqty = row["orderqty"].ToString(),
                    prodmastid = row["prodmastid"].ToString(),
                    prodname = row["prodname"].ToString()
                });
            }

            Loadobj.MasterDataList = masterDataList;
            Loadobj.MasterDetailDataList = detailItemsList;

            return Ok(new { Status_Code, Status_Message, Response = Loadobj });
        }
    }
}

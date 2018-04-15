using RoleDomain.Common.Import;
using RoleDomain.Common.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class dy_api_api : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";
        

        if (Request.QueryString["act"] != null) {
            switch (Request.QueryString["act"].ToString()) { 
                //输出本cp/sp的节目全量列表
                case "getall":
                    //注意判断您的spcode
                    string path2 = Server.MapPath("~/Documents/getall/list.js");
                    if (File.Exists(path2))
                    {

                        Response.Write(IOHelper.file_readtext(path2));
                    }
                    break;
                //输出本cp/sp的单个节目
                case "getcontent":  
                    if (Request.QueryString["guid"] != null)
                    {
                        //注意判断您的spcode
                        string guid = Request.QueryString["guid"].ToString();
                        string path = Server.MapPath("~/static_api/123001/" + guid + ".json");
                        if (File.Exists(path))
                        {

                            Response.Write(IOHelper.file_readtext(path));
                        }
                    }
                    break;
            }
        
        
        }

    }


    void p1() {
        //动态解析json
        JavaScriptSerializer jss = new JavaScriptSerializer();
        jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
        dynamic dy = jss.Deserialize("["
+ " { "
+ "\"guid\": \"651123651as65a651sd6a651sa\"                "
+ ",\"spcode\":\"123145\", \"title\": \"环太平洋2\"            "
+ "                                                    "
+ " }                                                  "
+ ", {                                                 "
+ "    \"guid\": \"235f12651f6215f65123\"                  "
+ "    , \"spcode\": \"123145\", \"title\": \"环太平洋1\"      "
+ "}                                                   "
+ "]", typeof(object)) as dynamic;


        Response.Write(dy[0].guid);
    }

}
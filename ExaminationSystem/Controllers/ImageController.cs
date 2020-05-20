using ExaminationSystem.Models;
using ExaminationSystem.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class ImageController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        public string UpLoad()
        {
            int code;
            string message;

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase f = Request.Files["img"];
                    string imageStr = "Static/images/";
                    string dir = Server.MapPath("~/" + imageStr);
                    string fileName = f.FileName;
                    // 获取后缀
                    string fileFormat = fileName.Split('.')[fileName.Split('.').Length - 1];
                    // guid命名存储
                    string guid = Guid.NewGuid().ToString();
                    string saveName = guid + '.' + fileFormat;
                    string path = Path.Combine(dir + saveName);
                    f.SaveAs(path);
                    path = "/" + imageStr + saveName;

                    // 存入数据库
                    ES_Image imageData = new ES_Image()
                    {
                        ImgTitle = guid,
                        ImgUrl = path,
                        ImgOther = fileName
                    };
                    db.ES_Image.Add(imageData);
                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        return JsonConvert.SerializeObject(new { code, path });
                    }
                    else
                    {
                        code = 1;
                        message = "服务端出错!图片存储失败";
                        return JsonConvert.SerializeObject(new { code, message });
                    }
                }
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务端出错!" + ex;
                return JsonConvert.SerializeObject(new { code, message });
            }

            code = 1;
            message = "服务端出错";
            return JsonConvert.SerializeObject(new { code, message });
        }
    }
}
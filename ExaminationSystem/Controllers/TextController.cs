using ExaminationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class TextController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string Upload(HttpPostedFileBase file)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
            var filePath = Server.MapPath("~/Static/UserText");
            file.SaveAs(Path.Combine(filePath, fileName));

            return WriteToDatabase(Read(fileName));
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string[] Read(string fileName)
        {
            string filePath = Server.MapPath("~/Static/UserText/" + fileName);

            string fileContent = System.IO.File.ReadAllText(filePath);

            // 根据回车换行将数据分组
            string[] contents = fileContent.Split(new char[2] { '\r', '\n' });
            // 去掉空行
            contents = contents.Where(s => !string.IsNullOrEmpty(s)).ToArray();

            return contents;
        }

        /// <summary>
        /// 将文件内容写入数据库
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public string WriteToDatabase(string[] fileContent)
        {
            int code;
            string message;
            try
            {
                foreach (string item in fileContent)
                {
                    string[] userStr = item.Split(',');

                    ES_User user = new ES_User
                    {
                        UserAccount = userStr[0],
                        UserPassword = userStr[1],
                        UserName = userStr[2],
                        RoleId = 1
                    };
                    db.ES_User.Add(user);
                }

                if (db.SaveChanges() >= fileContent.Length)
                {
                    code = 0;
                    message = "添加成功！";
                    return JsonConvert.SerializeObject(new { code, message });
                }

                code = 1;
                message = "部分数据插入失败！";
                return JsonConvert.SerializeObject(new { code, message });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "添加失败，请检查账号是否重复" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }
    }
}
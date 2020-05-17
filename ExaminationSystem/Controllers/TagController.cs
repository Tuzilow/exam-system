using ExaminationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class TagController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <returns></returns>
        public string GetTags()
        {
            int code;
            string message;

            try
            {
                var tags = from t in db.ES_Tag where t.IsDel == false select t;

                // 格式化
                List<object> tagList = new List<object>();

                foreach (var tag in tags)
                {
                    int id = tag.TagId;
                    string name = tag.TagName;
                    string desc = tag.TagDescribe;

                    tagList.Add(new { id, name, desc });
                }

                // 序列化为JSON 传递到View
                return JsonConvert.SerializeObject(tagList);
            }
            catch (Exception ex)
            {
                code = 1;
                message = "获取标签失败！" + ex.ToString();
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddTag(string tagName, string desc)
        {
            int code;
            string message;

            ES_Tag tag = new ES_Tag()
            {
                TagName = tagName,
                TagDescribe = desc
            };
            try
            {
                db.ES_Tag.Add(tag);
                if (db.SaveChanges() > 0)
                {
                    code = 0;
                    message = "添加成功";

                    return JsonConvert.SerializeObject(new { code, message });
                }
                code = 1;
                message = "添加失败！";
                return JsonConvert.SerializeObject(new { code, message });

            }
            catch (Exception ex)
            {
                code = 1;
                message = "添加失败！" + ex;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 修改tag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagName"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpPost]
        public string EditTag(int id, string tagName, string desc)
        {
            int code;
            string message;

            var tag = db.ES_Tag.Where(t => t.TagId == id && t.IsDel == false).FirstOrDefault();


            if (tag != null)
            {
                try
                {
                    tag.TagName = tagName;
                    tag.TagDescribe = desc;
                    db.Entry(tag).State = System.Data.Entity.EntityState.Modified;

                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        message = "修改成功";

                        return JsonConvert.SerializeObject(new { code, message });
                    }
                }
                catch (Exception ex)
                {
                    code = 1;
                    message = "服务端错误" + ex;

                    return JsonConvert.SerializeObject(new { code, message });
                }
            }
            code = 1;
            message = "未找到该记录";

            return JsonConvert.SerializeObject(new { code, message });

        }

        /// <summary>
        /// 删除tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string RemoveTag(int id)
        {
            int code;
            string message;

            var tag = db.ES_Tag.Where(t => t.TagId == id && t.IsDel == false).FirstOrDefault();


            if (tag != null)
            {
                try
                {
                    tag.IsDel = true;
                    db.Entry(tag).State = System.Data.Entity.EntityState.Modified;

                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        message = "删除成功";

                        return JsonConvert.SerializeObject(new { code, message });
                    }
                }
                catch (Exception ex)
                {
                    code = 1;
                    message = "服务端错误" + ex;

                    return JsonConvert.SerializeObject(new { code, message });
                }
            }
            code = 1;
            message = "未找到该记录";

            return JsonConvert.SerializeObject(new { code, message });
        }
    }
}
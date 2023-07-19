using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 读取所有员工
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string EmpList()
        {
            IM_Emp dot = new IM_Emp();

            ResultMes result = new ResultMes();
            result.Code = 200;

            var emplist = dot.EmpList();
            if (emplist == null)
            {
                result.Code = 400;
            }
            result.Data = emplist;

            var js = JsonConvert.SerializeObject(result);
            return js;
        }

        /// <summary>
        /// 一对一聊天窗口
        /// </summary>
        /// <returns></returns>
        public ActionResult SendMes()
        {
            return View();
        }



        /// <summary>
        /// 查询当前用户与别一用户的聊天记录
        /// </summary>
        /// <param name="To_empid">发消息用户编号【当前用户编号】</param>
        /// <param name="form_empid">收消息用户编号</param>
        /// <returns></returns>
        [HttpGet]
        public string QueryList(int To_empid, int form_empid)
        {

            IM_Content dot = new IM_Content();

            ResultMes result = new ResultMes();
            result.Code = 200;

            var emplist = dot.QueryList(To_empid, form_empid);
            if (emplist == null)
            {
                result.Code = 400;
            }
            result.Data = emplist;

            var js = JsonConvert.SerializeObject(result);
            return js;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="To_empid">发送人ID</param>
        /// <param name="form_empid">接收人ID</param>
        /// <param name="contentmes">内容消息</param>
        /// <returns></returns>
        [HttpGet]
        public string ToSendMes(int To_empid, int form_empid , string contentmes)
        {

            IM_Content entity = new IM_Content();
            entity.To_empid = To_empid;
            entity.Form_empid = form_empid;
            entity.Mes_content = contentmes;
            entity.Addtime = DateTime.Now;



            ResultMes result = new ResultMes();
            result.Code = 200;

            object emplist = entity.ToSendMes(entity);///业务 

            if (emplist == null)
            {
                result.Code = 400;
            }
            result.Data = emplist;

            var js = JsonConvert.SerializeObject(result);
            return js;
        }
    }
}
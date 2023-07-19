using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string Lgoin(string ename,string pwd)
        {
            ResultMes result = new ResultMes();
            result.Code = 200;

            IM_Emp dot = new IM_Emp();
            var entity = dot.FirstEnity(ename, pwd);

            if(entity==null)
            {
                result.Code = 400;
            }
            result.Data = entity;

            var js = JsonConvert.SerializeObject(result);

            return js;
        }

    }
}
using SignalRChatMvc.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRChatMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 获取二维码，生成唯一值，并存入库
        /// </summary>
        /// <returns>唯一值</returns>
        public JsonResult GetQrCode()
        {
            string id = Guid.NewGuid().ToString();
           //保存
            return Json(new { Success = true, Id = id });

            //string id = Guid.NewGuid().ToString();
            //qrCodeLoginService.Add(new SYS_QrCodeLogin() { Id = id, CreateTime = DateTime.Now, UserId = -1 });
            //return Json(new { Success = true, Id = id });
        }

         
         
        /// <summary>
        /// 扫码登录，   测试http://localhost:55030/Home/Login  http://localhost:55030/Home/ScanQrCode?id=1f5be1fc-4226-40cc-bbda-4a121e4087f3&userid=5
        /// </summary>
        /// <param name="id">二维码Id</param>
        /// <param name="userid">用户Id（前台发token,TokenValidate过滤解析得到userid）</param>
        /// <returns></returns>
        public JsonResult ScanQrCode(string id, int userid)
        {
            var signalRModel = new SignalRModel() { Id = id, Success = true, Message = "扫码登录成功", Url = "http://www.baidu.com" };
            new QrCodeLoginHub().SendToGroup(id, signalRModel);
            return Json(signalRModel);

            //  var signalRModel = new SignalRModel() { Id = id ,Success=false,Message="登录失败",Url=""};
            ////基本逻辑判断，
            //if (string.IsNullOrEmpty(id))
            //{
            //    signalRModel.Code = SignalRMessageCode.二维码不能为空;
            //    signalRModel.Message = "二维码不能为空";
            //}
            //else if (userid <0)
            //{
            //    signalRModel.Code = SignalRMessageCode.用户没有登录;
            //    signalRModel.Message = "用户没有登录";
            //}
            //var model = qrCodeLoginService.GetList(q => q.Id == id).FirstOrDefault();
            //if (model == null)
            //{
            //    signalRModel.Code = SignalRMessageCode.二维码不存在;
            //    signalRModel.Message = "二维码不存在";
            //}
            //else if(model.LoginTime== null)
            //{
            //    TimeSpan span = (TimeSpan)(DateTime.Now-model.CreateTime);
            //    if(span.TotalSeconds>60)
            //    {
            //        signalRModel.Code = SignalRMessageCode.二维码已过期;
            //        signalRModel.Message = "二维码已过期";
            //    }
            //    else
            //    {
            //        model.UserId = userid;
            //        model.LoginTime = DateTime.Now;
            //        qrCodeLoginService.UpdateEntityFields(model, new List<string>() { "UserId", "LoginTime" });

            //        signalRModel.Message = "登录成功";
            //        signalRModel.Success = true;
            //        signalRModel.Url = "/html/index.html#!home";
            //        signalRModel.Code = SignalRMessageCode.扫码登录成功;
            //    }
            //}
            //else 
            //{
            //    signalRModel.Message = "此二维码已被其他用户登录，请刷新页面获取新二维码";
            //    signalRModel.Code = SignalRMessageCode.此二维码已被其他用户登录;
            //}
            //new QrCodeLoginHub().SendToGroup(id,signalRModel);
            //return Json(signalRModel);
        
        }


    }
}
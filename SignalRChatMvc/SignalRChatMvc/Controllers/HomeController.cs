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
        }



        public JsonResult ScanQrCode(string id, int userid)
        {
            var signalRModel = new SignalRModel() { Id = id, Success = true, Message = "扫码登录成功", Url = "http://www.baidu.com" };
            new QrCodeLoginHub().SendToGroup(id, signalRModel);
            return Json(signalRModel);
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRChatMvc.Hubs
{
    
    /// <summary>
    /// 消息模型
    /// </summary>
    public class SignalRModel
    {
        /// <summary>
        /// 二维码唯一值
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// 状态码
        /// </summary>
        public SignalRMessageCode Code { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息说明
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 登录成功后跳转的页面路径
        /// </summary>
        public string Url { get; set; }
    }

    public enum SignalRMessageCode
    {
        扫码登录成功 = 200,
        二维码不能为空 = 401,
        用户没有登录 = 402,
        此二维码已被其他用户登录 = 403,
        二维码不存在 = 404,
        二维码已过期 = 405
    }

    /// <summary>
    /// 二维码登录Hub，一般情况下，转发器里的方法是提供给客户端调用的，如AddToGroup
    /// 这里的Send是给服务端调用
    /// </summary>
    public class QrCodeLoginHub : Hub
    {
        /// <summary>
        /// 加到不同分组
        /// </summary>
        /// <param name="groupId">分组Id</param>
        public void AddToGroup(string groupId)
        {
            this.Groups.Add(Context.ConnectionId, groupId);
        }

        /// <summary>
        /// 广播，向所有客户端发消息
        /// </summary>
        /// <param name="signalRMessage">消息模型</param>
        public void SendToAll(SignalRModel signalRMessage)
        {
            Clients.All.sendMessage(signalRMessage);
        }


        /// <summary>
        /// 组播，向指定分组下的客户端发消息
        /// </summary>
        /// <param name="groupId">分组Id</param>
        /// <param name="signalRMessage">消息模型</param>
        public void SendToGroup(string groupId, SignalRModel signalRMessage)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<QrCodeLoginHub>();
            hubContext.Clients.Group(groupId).SendMessage(signalRMessage);
        }



        /// <summary>
        /// 向指定客户端发送消息，考虑到二维码登录，
        /// </summary>
        /// <param name="signalRMessage"></param>
        public static void Send(SignalRModel signalRMessage)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<QrCodeLoginHub>();
            hubContext.Clients.Group(signalRMessage.Id).SendMessage(signalRMessage);
        }
    }
}
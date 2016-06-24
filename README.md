# 二维码登录
一.应用场景
用户在手机APP上登录后，通过扫描登录网页二维码，实现登录：<br>

二.所用技术
SignalR+EF+MVC：<br>

三.先来看看效果吧：<br>
第一步：请求网页，生成一个二维码：http://localhost:55030/Home/Login，生成ID，在控制台输出：
![](https://github.com/Zhujinyong/SignalRQrCodeLogin/raw/master/SignalRChatMvc/SignalRChatMvc/Images/qrCode.jpg)  

第二步：用户扫码后程序中访问：http://localhost:55030/Home/ScanQrCode?id=1f5be1fc-4226-40cc-bbda-4a121e4087f3&userid=5，这里的id是上面生成的二维码，登录成功后跳转到其他页面：
![](https://github.com/Zhujinyong/SignalRQrCodeLogin/raw/master/SignalRChatMvc/SignalRChatMvc/Images/jump.jpg)  

四.代码<br>
1.转换器QrCodeLoginHub包含组播，广播，绑定等操作：<br>
~~~C#
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
~~~

2.前端调用代码：<br>
~~~C#
 <center><div id="qrcode"></div></center>
    <center><div id="status"></div></center>
  
    <script type="text/javascript">
        var secs = 5; //倒计时的秒数
        var URL;
        var chat = $.connection.qrCodeLoginHub;

        $(function () {


            chat.client.SendMessage = qrcode_login;
           $.connection.hub.start().done(function () {
                $.ajax({
                    type: "POST",
                    url: '/Home/GetQrCode',
                    dataType: "json",
                    beforeSend: function () {
                        $("#qrcode").empty();
                    },
                    success: function (data) {
                        if (data.Success) {
                            console.log(data.Id);
                            var str = "{\"code\": \"" + data.Id + "\"}";
                            create_qrcode(str);
                            chat.server.addToGroup(data.Id);
                        }
                        else {
                            alert("获取失败，请刷新重试！");
                        }
                    }
                });
            });




        });

        function create_qrcode(value) {
            if (value != undefined && value != "" && value != null) {
                $("#qrcode").qrcode({ width: 300, height: 300, text: value });
            }
        }

        function Load(url) {
            URL = url;
            for (var i = secs; i >= 0; i--) {
                window.setTimeout('doUpdate(' + i + ')', (secs - i) * 1000);
            }
        }
        function doUpdate(num) {
            $("#status").text('将在' + num + '秒后自动跳转到主页');
            if (num == 0) { window.location = URL; }
        }

        function qrcode_login(data) {
            $("#status").text(data.Message);
            if (data.Success) {
                Load(data.Url);
            }
        }
    </script>
~~~



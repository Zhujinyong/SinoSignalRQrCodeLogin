# SignalR实现二维码登录
#### 一.应用场景<br>
　　用户在手机APP上登录后，通过扫描登录网页二维码，实现登录。如果让你实现这个功能，你会怎么做？可以先思考3分钟，然后再继续看。。。<br>
　　大部分人在用SetTimeOut在定时发请求，看看这个二维码是否被扫，成功就登录跳转，这种方法可以实现功能 ，可是频繁发请求并不是一个好的选择。不过SignalR的出现完美解决轮询的问题，关于它的使用可以去<a href="http://www.asp.net/signalr">SignalR官网</a> 去查<br>

#### 二.原理及技术<br>
##### 1.原理<br>
　　后台生成一个唯一值id，存储在服务端，然后返回给前端，生成二维码。用户扫码，获取到这个id,发送id和登录信息到某个接口，接口根据id查询数据库，如果找到，判断登录时间字段，没有则比较创建时间，1分钟内允许登录，否则二维码失效；如果登录时间有值，则此二维码已被某用户登录。这里最核心的是用组播的运用，即生成二维码的网页可能被打开很多，每个页面都有一个id，彼此独立，应该是扫描任意一个都能登录，且其他网页不受影响，这时就要用分组功能，每生成一个二维码，把它加到一个分组里，用户扫码请求后，服务端转发处理结果给该分组<br>

##### 2.表设计<br>
　　表名：SYS_QrCodeLogin<br>
　　备注：存储二维码登录信息<br>
　　字段：<br>
　　Id           二维码唯一值    <br>
　　IdUserId     登录的用户Id    <br>
　　CreateTime   二维码生成时间  <br>
　　LoginTime    登录时间        <br>

##### 3.使用的技术<br>
　　SignalR+EF+MVC<br>

#### 三.先来看看效果吧<br>
　　第一步：请求网页，生成一个二维码：http://localhost:55030/Home/Login，生成ID，在控制台输出：
![](https://github.com/Zhujinyong/SignalRQrCodeLogin/raw/master/SignalRChatMvc/SignalRChatMvc/Images/qrCode.jpg)  

　　第二步：用户扫码后程序中访问：http://localhost:55030/Home/ScanQrCode?id=1f5be1fc-4226-40cc-bbda-4a121e4087f3&userid=5，这里的id是上面生成的二维码，登录成功后跳转到其他页面：
![](https://github.com/Zhujinyong/SignalRQrCodeLogin/raw/master/SignalRChatMvc/SignalRChatMvc/Images/jump.jpg)  

#### 四.代码<br>
##### 1.转换器QrCodeLoginHub包含组播，广播，绑定等操作：<br>
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

##### 2.前端调用代码：<br>
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



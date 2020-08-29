using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Dynamic;

namespace BOTCH.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {

        const string channelAccessToken = "AIgVhorUwZtTvTVWwoc0mRg/iipO6ztQ31lfZ9RrjumOTuWM0UBkWsA4xpPuFdx0SByfzk2paEXUVZm52nZ5ii1lxVt5TqdK0jIJPCh+DFyZdgzKe8i8XBnFf3P4gj7YF79CHrULoVqXaQNUaHNIcwdB04t89/1O/w1cDnyilFU=";
        const string AdminUserId = "U61738922af721de670886129330fc060";

        MessageCommand MessageCommand = new MessageCommand();

        [Route("api/LineWebHookSample")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            try
            {
                //設定ChannelAccessToken(或抓取Web.Config)
                this.ChannelAccessToken = channelAccessToken;
                //取得Line Event(範例，只取第一個)
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                //回覆訊息
                if (LineEvent.type == "message")
                {
                    if (LineEvent.message.type == "text") //收到文字
                        if (LineEvent.message.text.Contains("RPG-"))
                        {
                            //進到RPG指令區
                            string RetuenMessage = MessageCommand.RPGCommand(LineEvent.message.text);
                            this.ReplyMessage(LineEvent.replyToken, RetuenMessage);
                        }
                        else if(LineEvent.message.text.Contains("圖片問答-"))
                        {
                            isRock.LineBot.Bot bot;
                            bot = new isRock.LineBot.Bot(channelAccessToken);
                            //取得 http Post RawData(should be JSO
                            string postData = Request.Content.ReadAsStringAsync().Result;
                            var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);

                            //建立actions，作為ButtonTemplate的用戶回覆行為
                            var act1 = new isRock.LineBot.MessageAction()
                            { text = "test action1", label = "test action1" };
                            var act2 = new isRock.LineBot.MessageAction()
                            { text = "test action2", label = "test action2" };

                            var tmp = new isRock.LineBot.ButtonsTemplate()
                            {
                                text = "Button Template text",
                                title = "Button Template title",
                                thumbnailImageUrl = new Uri("https://i.imgur.com/wVpGCoP.png"),
                            };

                            tmp.actions.Add(act1);
                            tmp.actions.Add(act2);

                            //var UserID = isRock.LineBot.Utility.Parsing(postData).events[0].source.userId;
                            //bot.PushMessage(UserID, ButtonTemplate);
                            this.ReplyMessage(LineEvent.replyToken, new isRock.LineBot.TemplateMessage(tmp));

                        }
                        else if(LineEvent.message.text.Contains("圖-"))
                        {

                            //isRock.LineBot.ImagemapMessage img = new isRock.LineBot.ImagemapMessage(new Uri("http://"));
                            this.ReplyMessage(LineEvent.replyToken, new Uri("https://i.imgur.com/QqjmONg.png"));
                        }
                        else
                        {
                            this.ReplyMessage(LineEvent.replyToken, "你說了:" + LineEvent.message.text);
                        }
                    if (LineEvent.message.type == "sticker") //收到貼圖
                        this.ReplyMessage(LineEvent.replyToken, 1, 2);
                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                this.ReplyMessage(LineEvent.replyToken, "發生錯誤:\n" + ex.Message);
                //如果發生錯誤，傳訊息給Admin
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }

    }
    

 
}

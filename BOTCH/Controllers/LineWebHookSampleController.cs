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
                            //建立actions，作為ButtonTemplate的用戶回覆行為
                            var actions = new List<isRock.LineBot.TemplateActionBase>();
                            actions.Add(new isRock.LineBot.MessageActon()
                            { label = "點選這邊等同用戶直接輸入某訊息", text = "/例如這樣" });
                            actions.Add(new isRock.LineBot.UriActon()
                            { label = "點這邊開啟網頁", uri = new Uri("http://www.google.com") });
                            actions.Add(new isRock.LineBot.PostbackActon()
                            { label = "點這邊發生postack", data = "abc=aaa&def=111" });

                            //單一Button Template Message
                            var ButtonTemplate = new isRock.LineBot.ButtonsTemplate()
                            {
                                altText = "替代文字(在無法顯示Button Template的時候顯示)",
                                text = "AAA",
                                title = "BBB",
                                //設定圖片
                                thumbnailImageUrl = new Uri("https://scontent-tpe1-1.xx.fbcdn.net/v/t31.0-8/15800635_1324407647598805_917901174271992826_o.jpg?oh=2fe14b080454b33be59cdfea8245406d&oe=591D5C94"),
                                actions = actions //設定回覆動作
                            };

                        }
                        else if(LineEvent.message.text.Contains("圖-"))
                        {

                            //isRock.LineBot.ImagemapMessage img = new isRock.LineBot.ImagemapMessage(new Uri("http://"));
                            this.ReplyMessage(LineEvent.replyToken, new Uri("https://圖片位置/22-124303-d8b2c4de-9a8c-48da-83f1-7c0d36de3ab6.png"));
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
                //如果發生錯誤，傳訊息給Admin
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }

    }
    

 
}

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
                            if(LineEvent.message.text.Contains("TeM"))
                            {
                                List<isRock.LineBot.MessageBase> RetuenMessage = MessageCommand.RPGTeMCommand(LineEvent.message.text, LineEvent.source.roomId, LineEvent.source.userId);
                                this.ReplyMessage(LineEvent.replyToken, RetuenMessage);
                            }
                            else
                            { 
                                string RetuenMessage = MessageCommand.RPGCommand(LineEvent.message.text, LineEvent.source.roomId, LineEvent.source.userId);
                                this.ReplyMessage(LineEvent.replyToken, RetuenMessage);
                            }
                        }
                        else if (LineEvent.message.text.Contains("多圖片問答-"))
                        {
                            isRock.LineBot.Bot bot;
                            bot = new isRock.LineBot.Bot(channelAccessToken);
                            //取得 http Post RawData(should be JSO
                            string postData = Request.Content.ReadAsStringAsync().Result;
                            var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);

                            //建立actions，作為ButtonTemplate的用戶回覆行為
                            var actions = new List<isRock.LineBot.TemplateActionBase>();
                            actions.Add(new isRock.LineBot.MessageActon() { label = "標題-文字回覆", text = "回覆文字" });
                            actions.Add(new isRock.LineBot.UriActon() { label = "標題-Google", uri = new Uri("http://www.google.com") });
                            actions.Add(new isRock.LineBot.PostbackActon() { label = "標題-發生postack", data = "abc=aaa&def=111" });

                            //單一Column 
                            var Column = new isRock.LineBot.Column
                            {
                                text = "ButtonsTemplate文字訊息",
                                title = "ButtonsTemplate標題",
                                //設定圖片
                                thumbnailImageUrl = new Uri("https://arock.blob.core.windows.net/blogdata201706/22-124357-ad3c87d6-b9cc-488a-8150-1c2fe642d237.png"),
                                actions = actions //設定回覆動作
                            };

                            //建立CarouselTemplate
                            var CarouselTemplate = new isRock.LineBot.CarouselTemplate();

                            //這是範例，所以用一組樣板建立三個
                            CarouselTemplate.columns.Add(Column);
                            CarouselTemplate.columns.Add(Column);
                            CarouselTemplate.columns.Add(Column);

                            this.ReplyMessage(LineEvent.replyToken, new isRock.LineBot.TemplateMessage(CarouselTemplate));

                        }
                        else if (LineEvent.message.text.Contains("圖片問答-"))
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
                        else if (LineEvent.message.text.Contains("相關代號"))
                        {
                            string 回覆訊息 = "";

                            回覆訊息 += "房間編號: " + LineEvent.source.roomId + "\n";
                            回覆訊息 += "玩家編號: " + LineEvent.source.userId + "\n";

                            this.ReplyMessage(LineEvent.replyToken, 回覆訊息);
                        }
                        else if (LineEvent.message.text.Contains("測試"))
                        {
                            List<isRock.LineBot.MessageBase> responseMsgs = new List<isRock.LineBot.MessageBase>();
                            isRock.LineBot.MessageBase responseMsg = null;

                            //add text response
                            responseMsg = new isRock.LineBot.TextMessage($"you said : {LineEvent.message.text}");
                            responseMsgs.Add(responseMsg);
                            //add ButtonsTemplate if user say "/Show ButtonsTemplate"
                            if (LineEvent.message.text.ToLower().Contains("show buttonstemplate"))
                            {
                                //define actions
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
                                //add TemplateMessage into responseMsgs
                                responseMsgs.Add(new isRock.LineBot.TemplateMessage(tmp));

                            }

                            this.ReplyMessage(LineEvent.replyToken, responseMsgs);
                        }

                    //else
                    //{
                    //    this.ReplyMessage(LineEvent.replyToken, "你說了:" + LineEvent.message.text);
                    //}
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

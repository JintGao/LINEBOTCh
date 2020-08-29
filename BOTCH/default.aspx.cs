using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BOTCH
{
    public partial class _default : System.Web.UI.Page
    {
        const string channelAccessToken = "AIgVhorUwZtTvTVWwoc0mRg/iipO6ztQ31lfZ9RrjumOTuWM0UBkWsA4xpPuFdx0SByfzk2paEXUVZm52nZ5ii1lxVt5TqdK0jIJPCh+DFyZdgzKe8i8XBnFf3P4gj7YF79CHrULoVqXaQNUaHNIcwdB04t89/1O/w1cDnyilFU=";
        const string AdminUserId= "U61738922af721de670886129330fc060";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var bot = new Bot(channelAccessToken);
            bot.PushMessage(AdminUserId, $"測試 {DateTime.Now.ToString()} ! ");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var bot = new Bot(channelAccessToken);
            bot.PushMessage(AdminUserId, 1,2);
        }
    }
}
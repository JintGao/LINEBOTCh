using System;
using System.Collections.Generic;

namespace BOTCH.Controllers
{

    public static class HC
    {
        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;
        public const int E = 4;
        public const int F = 5;
        public const int G = 6;
        public const int H = 7;
        public const int I = 8;
        public const int J = 9;
        public const int K = 10;
        public const int L = 11;
        public const int M = 12;
        public const int N = 13;
    }


    public class MessageCommand
    {

        BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();
        Fighting 戰鬥 = new Fighting();

        public string RPGCommand(string LineEvent, string 群組代碼, string 玩家代碼)
        {
            //google表單初始化
            GH.SstGooglesHeet();

            if (LineEvent.Contains("建立角色"))
            {
                if (LineEvent.Split('-').Length == 4)
                {
                    Random rnd = new Random(Guid.NewGuid().GetHashCode());
                    string[] TArray = LineEvent.Split('-');

                    //是否人物重複
                    if (GH.ReadRoleEntries(TArray[2]).Count > 0)
                    {
                        return "創建角色錯誤 " + TArray[2] + " 已存在";
                    }
                    else
                    {
                        //是否存在這個職業
                        if (GH.ReadProfessionEntries(TArray[3]).Count > 0)
                        {
                            //角色數值
                            string 姓名 = TArray[2];
                            string 職業 = TArray[3];
                            string 力量 = rnd.Next(5, 10).ToString();
                            string 敏捷 = rnd.Next(5, 10).ToString();
                            string 體質 = rnd.Next(5, 10).ToString();
                            string 智慧 = rnd.Next(5, 10).ToString();
                            string 血量 = (20 + Convert.ToInt16(體質) * 2).ToString();
                            string 技能1 = "";
                            string 技能2 = "";

                            foreach (var row in GH.ReadEntries("職業", "A", "D"))
                            {
                                if (row[0].ToString() == TArray[3])
                                {
                                    技能1 = row[1].ToString();
                                    技能2 = row[2].ToString();

                                    break;
                                }
                            }

                            //Todo: 剩下名稱 職業 血量 傷害，且初始化牌組也要建立

                            List<string> TList = new List<string>() { 姓名, 職業, 血量, 力量, 敏捷, 體質, 智慧, 技能1, 技能2 };

                            GH.CreateEntry("角色", "A", "I", TList);
                            return "創建角色成功," + " \n姓名:" + 姓名 + "\n 職業:" + 職業 + "\n 血量:" + 血量 + "\n 力量:" + 力量 + "\n 敏捷:" + 敏捷 + "\n 體質:" + 體質 + "\n 智慧:" + 智慧 + ",\n 技能1:" + 技能1 + "\n 技能2:" + 技能2;

                        }
                        else
                        {
                            return TArray[3] + "職業不存在 ";
                        }
                    }
                }
                else
                {
                    return "創建角色指令錯誤,EX: RPG-建立角色-托尼-弓箭手";
                }
            }
            else if (LineEvent.Contains("PK"))
            {
                if (LineEvent.Split('-').Length == 4)
                {
                    return 戰鬥.戰鬥計算(LineEvent);
                }
                else
                {
                    return "創建角色指令錯誤,EX: RPG-PK-托尼-涉獵豹貓";
                }
            }
            else if (LineEvent.Contains("指令"))
            {
                string 指令 = "";
                指令 += "查詢目前有的玩家 - EX: RPG-所有玩家 \n";
                指令 += "查詢目前有的職業 - EX: RPG-職業 \n";
                指令 += "查詢目前有的技能 - EX: RPG-技能 \n";
                指令 += "創建角色 - EX: RPG-建立角色-托尼-弓箭手 \n";
                指令 += "玩家戰鬥 - EX: RPG-PK-托尼-涉獵豹貓 \n";

                return 指令;
            }
            else if (LineEvent.Contains("所有玩家"))
            {
                var values = GH.ReadEntries("角色", "A", "I");
                string 回覆訊息 = "玩家列表 \n";
                foreach (var row in values)
                {
                    回覆訊息 += row[0] + " " + row[1] + " " + row[2] + " " + row[3] + " " + row[4] + " " + row[5] + " " + row[6] + " " + row[7] + " " + row[8] + "\n";
                }

                return 回覆訊息;
            }
            else if (LineEvent.Contains("技能"))
            {
                var values = GH.ReadEntries("技能", "A", "D");
                string 回覆訊息 = "技能列表 \n";
                foreach (var row in values)
                {
                    回覆訊息 += row[0] + " " + row[1] + " " + row[2] + " " + row[3] + "\n";
                }

                return 回覆訊息;

            }
            else if (LineEvent.Contains("職業"))
            {
                var values = GH.ReadEntries("職業", "A", "D");
                string 回覆訊息 = "職業列表 \n";
                foreach (var row in values)
                {
                    回覆訊息 += row[0] + " " + row[1] + " " + row[2] + " " + row[3] + "\n";
                }

                return 回覆訊息;
            }


            return "無符合指令";
        }

        public List<isRock.LineBot.MessageBase> RPGTeMCommand(string LineEvent, string 群組代碼, string 玩家代碼)
        {
            List<isRock.LineBot.MessageBase> responseMsgs = new List<isRock.LineBot.MessageBase>();
            var CarouselTemplate = new isRock.LineBot.CarouselTemplate();
            遊戲機制 遊戲機制 = new 遊戲機制();

            if (LineEvent.Contains("遭遇戰-出牌-"))
            {
                string Key = LineEvent.Split('-')[4];

                //Todo 撈出暫存,判斷是否出過牌,出過就不進行判斷,沒出過 戰鬥計算 並更新為出過牌, 判斷狀況,更新暫存 或 結算後清理暫存。
                List<String> 暫存資訊 = 遊戲機制.讀取暫存(Key);


                if (Convert.ToInt32(暫存資訊[HC.N]) == 0 && 暫存資訊.Count != 0) //更新玩家是否已出牌
                {
                    遊戲機制.更新暫存(暫存資訊);

                    //初始化對戰資料
                    怪物 怪物 = 遊戲機制.怪物暫存初始化(暫存資訊);

                    遊戲機制.戰鬥描述(怪物.名稱 + "HP: " + 怪物.血量 + " 出牌: " + 怪物.怪物出牌.卡牌名稱);

                    玩家 玩家 = 遊戲機制.玩家暫存初始化(暫存資訊, LineEvent.Split('-')[5]);

                    遊戲機制.戰鬥描述(玩家.名稱 + "HP:" + 玩家.血量 + " 出牌: " + 玩家.玩家出牌.卡牌名稱);

                    //戰鬥計算
                    戰鬥計算 戰計 = new 戰鬥計算();

                    遊戲機制.戰鬥描述("對戰開始");

                    遊戲機制.戰鬥描述(戰計.對戰(玩家, 怪物, 遊戲機制.取得戰鬥描述()));

                    遊戲機制.戰鬥描述(玩家.名稱 + "剩餘HP:" + 玩家.血量 + "  " + 怪物.名稱 + "剩餘HP: " + 怪物.血量);

                    if (怪物.血量 <= 0 && 玩家.血量 <= 0)
                    {
                        遊戲機制.戰鬥描述("戰鬥失敗");
                    }
                    else if (怪物.血量 <= 0)
                    {
                        遊戲機制.戰鬥描述(玩家.名稱 + " 勝利");
                    }
                    else if (玩家.血量 <= 0)
                    {
                        遊戲機制.戰鬥描述("戰鬥失敗");
                    }

                    responseMsgs.Add(new isRock.LineBot.TextMessage(遊戲機制.取得戰鬥描述()));

                    //刪除原先暫存
                    遊戲機制.刪除暫存(暫存資訊);

                    //不存在勝利或失敗，繼續抽卡 並暫存
                    if (!遊戲機制.取得戰鬥描述().Contains("勝利") && !遊戲機制.取得戰鬥描述().Contains("失敗"))
                    {
                        CarouselTemplate = 遊戲機制.怪物出牌玩家抽卡並暫存(玩家, 怪物, ref responseMsgs);
                        responseMsgs.Add(new isRock.LineBot.TemplateMessage(CarouselTemplate));
                    }
                    
                }
                else
                    responseMsgs.Add(new isRock.LineBot.TextMessage("此戰已戰鬥過 或 暫存無資料"));
            }
            else if (LineEvent.Contains("遭遇戰"))
            {
                responseMsgs.Add(new isRock.LineBot.TextMessage("遭遇戰開始,遇見哥布林"));

                怪物 怪物 = new 怪物("哥布林");
                玩家 玩家 = new 玩家("測試人員");

                //ToDo: 流程 顯示對方出牌 -> 牌庫抽三張牌顯示 -> 玩家選一張牌 -> 戰鬥判定 (迴圈)
                CarouselTemplate = 遊戲機制.怪物出牌玩家抽卡並暫存(玩家, 怪物,ref responseMsgs);

                responseMsgs.Add(new isRock.LineBot.TemplateMessage(CarouselTemplate));
                
            }


            return responseMsgs;
        }

        public class 戰鬥計算
        {
            public string 對戰(玩家 玩家, 怪物 怪物, string 戰鬥描述)
            {

                int 戰鬥判定 = 玩家.玩家出牌.類型 - 怪物.怪物出牌.類型;


                //防禦判定
                if (玩家.玩家出牌.類型 == 4 || 怪物.怪物出牌.類型 == 4)
                {
                    if (玩家.玩家出牌.類型 == 4)
                    {

                        switch (怪物.怪物出牌.類型)
                        {
                            case 1:
                            case 2:
                            case 3:
                                A防禦(玩家, 怪物, 玩家.玩家出牌.數值, ref 戰鬥描述);
                                break;
                            case 4:
                                //都防禦沒事
                                break;
                            case 5:
                                B恢復(怪物, ref 戰鬥描述);
                                break;
                        }
                    }
                    else //怪物出4
                    {
                        switch (玩家.玩家出牌.類型)
                        {
                            case 1:
                            case 2:
                            case 3:
                                B防禦(玩家, 怪物, 怪物.怪物出牌.數值, ref 戰鬥描述);
                                break;
                            case 4:
                                //都防禦沒事
                                break;
                            case 5:
                                A恢復(玩家, ref 戰鬥描述);
                                break;
                        }
                    }
                }

                //恢復判定
                else if (玩家.玩家出牌.類型 == 5 || 怪物.怪物出牌.類型 == 5)
                {
                    if (玩家.玩家出牌.類型 == 5)
                    {
                        switch (怪物.怪物出牌.類型)
                        {
                            case 1:
                            case 2:
                            case 3:
                                A恢復(玩家, ref 戰鬥描述);
                                B克制A(玩家, 怪物, ref 戰鬥描述);
                                break;
                            case 4:
                                A恢復(玩家, ref 戰鬥描述);
                                //怪物防禦沒事發生
                                break;
                            case 5:
                                A恢復(玩家, ref 戰鬥描述);
                                B恢復(怪物, ref 戰鬥描述);
                                break;
                        }
                    }
                    //怪物出5
                    else
                    {
                        switch (玩家.玩家出牌.類型)
                        {
                            case 1:
                            case 2:
                            case 3:
                                B恢復(怪物, ref 戰鬥描述);
                                A克制B(玩家, 怪物, ref 戰鬥描述);
                                break;
                            case 4:
                                B恢復(怪物, ref 戰鬥描述);
                                //玩家防禦沒事發生
                                break;
                            case 5:
                                A恢復(玩家, ref 戰鬥描述);
                                B恢復(怪物, ref 戰鬥描述);
                                break;
                        }
                    }
                }

                //有克制關係
                else if (戰鬥判定 == 2 || 戰鬥判定 == -2 || 戰鬥判定 == 1 || 戰鬥判定 == -1)
                {
                    if (戰鬥判定 > 0)
                    {
                        if(玩家.玩家出牌.類型 == 3 && 怪物.怪物出牌.類型 == 1)
                            A克制B(玩家, 怪物, ref 戰鬥描述);
                        else
                            B克制A(玩家, 怪物, ref 戰鬥描述);
                    }
                    else
                    {
                        A克制B(玩家, 怪物, ref 戰鬥描述);
                    }

                }
                //無克制
                else
                {
                    AB平局(玩家, 怪物, ref 戰鬥描述);
                }


                return 戰鬥描述;
            }

            public void A克制B(玩家 A, 怪物 B, ref string 戰鬥描述)
            {
                int 傷害 = Convert.ToInt32(Math.Round(Convert.ToDouble(A.傷害) * A.玩家出牌.數值));
                B.受到傷害(傷害);

                if (B.怪物出牌.類型 == 5)
                    戰鬥描述 += "= 玩家攻擊怪物 = " + A.名稱 + " 攻擊 " + B.名稱 + " 造成 " + 傷害 + " 點傷害 \n";
                else
                    戰鬥描述 += "= 玩家克制怪物 = " + A.名稱 + " 攻擊 " + B.名稱 + " 造成 " + 傷害 + " 點傷害 \n";

            }


            public void B克制A(玩家 A, 怪物 B, ref string 戰鬥描述)
            {
                int 傷害 = Convert.ToInt32(Math.Round(Convert.ToDouble(B.傷害) * B.怪物出牌.數值));
                A.受到傷害(傷害);

                if (B.怪物出牌.類型 == 5)
                    戰鬥描述 += "= 怪物攻擊玩家 = " + B.名稱 + " 攻擊 " + A.名稱 + " 造成 " + 傷害 + " 點傷害 \n";
                else
                    戰鬥描述 += "= 怪物克制玩家 = " + B.名稱 + " 攻擊 " + A.名稱 + " 造成 " + 傷害 + " 點傷害 \n";

            }


            public void A防禦(玩家 A, 怪物 B, double A減傷, ref string 戰鬥描述)
            {
                int 傷害 = Convert.ToInt32(Math.Round(Convert.ToDouble(B.傷害) * B.怪物出牌.數值 * (A減傷)));
                A.受到傷害(傷害);
                戰鬥描述 += "= 玩家防禦 = " + "減傷為:" + A減傷 + " " + B.名稱 + " 攻擊 " + A.名稱 + " 造成 " + 傷害 + " 點傷害 \n";

            }

            public void B防禦(玩家 A, 怪物 B, double B減傷, ref string 戰鬥描述)
            {
                int 傷害 = Convert.ToInt32(Math.Round(Convert.ToDouble(A.傷害) * A.玩家出牌.數值 * (B減傷)));
                B.受到傷害(傷害);
                戰鬥描述 += "= 怪物防禦 = " + "減傷為:" + B減傷 + " " + A.名稱 + " 攻擊 " + B.名稱 + " 造成 " + 傷害 + " 點傷害 \n";

            }

            public void AB平局(玩家 A, 怪物 B, ref string 戰鬥描述)
            {
                int A傷害 = Convert.ToInt32(Math.Round(Convert.ToDouble(A.傷害) * A.玩家出牌.數值));
                int B傷害 = Convert.ToInt32(Math.Round(Convert.ToDouble(B.傷害) * B.怪物出牌.數值));
                B.受到傷害(A傷害);
                A.受到傷害(B傷害);
                戰鬥描述 += "= 互相傷害 = " + A.名稱 + " 攻擊 " + B.名稱 + " 造成 " + A傷害 + " 點傷害 \n";
                戰鬥描述 += "= 互相傷害 = " + B.名稱 + " 攻擊 " + A.名稱 + " 造成 " + B傷害 + " 點傷害 \n";

            }

            public void A恢復(玩家 A, ref string 戰鬥描述)
            {
                A.恢復血量(Convert.ToInt32(A.傷害 * A.玩家出牌.數值));
                戰鬥描述 += A.名稱 + " 恢復了 " + A.傷害 * A.玩家出牌.數值 + " 點血量 \n";
            }

            public void B恢復(怪物 B, ref string 戰鬥描述)
            {
                B.恢復血量(Convert.ToInt32(B.傷害 * B.怪物出牌.數值));
                戰鬥描述 += B.名稱 + " 恢復了 " + B.傷害 * B.怪物出牌.數值 + " 點血量 \n";
            }

        }

        //玩家 怪物 共用的父類別
        public class 戰鬥角色
        {
            public string 名稱 = "";
            public int 血量 = 0;
            public int 傷害 = 0;
            public List<卡牌> 牌庫 = new List<卡牌>();
            BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();

            public string 取得目前牌庫(string 角色)
            {
                List<int> 目前牌庫 = new List<int>();

                //目前只有玩家會有抽完牌庫的問題
                if(牌庫.Count == 0)
                {
                    List<string> 卡牌清單 = new List<string>();
                    卡牌清單 = GH.CardList(this.名稱, 角色);
                    牌庫初始化 牌庫初始化 = new 牌庫初始化(卡牌清單);
                    this.牌庫 = 牌庫初始化.牌庫;
                }

                foreach (卡牌 卡牌 in 牌庫)
                {
                    目前牌庫.Add(卡牌.順序編號);
                }

                return string.Join(",", 目前牌庫.ToArray());
            }

            public 卡牌 取得卡牌資訊(string 卡牌名)
            {
                卡牌 回傳卡牌資訊 = new 卡牌();
                foreach (卡牌 卡牌 in 牌庫)
                {
                    if (卡牌.卡牌名稱 == 卡牌名)
                    {
                        回傳卡牌資訊 = 卡牌;
                        break;
                    }
                }
                return 回傳卡牌資訊;
            }

            public void 取讀暫存牌庫(List<int> 暫存牌庫)
            {
                List<卡牌> 欲刪除之卡牌 = new List<卡牌>();

                //當剩餘牌庫有三張以上才做過濾，否則用初始化全卡牌就好
                if (暫存牌庫.Count >= 3)
                {
                    foreach (卡牌 卡牌 in 牌庫)
                    {
                        if (!暫存牌庫.Contains(卡牌.順序編號))
                        {
                            欲刪除之卡牌.Add(卡牌);
                        }
                    }

                    foreach(卡牌 卡牌 in 欲刪除之卡牌)
                    {
                        this.牌庫.Remove(卡牌);
                    }
                }
            }

            public void 受到傷害(int 傷害值)
            {
                血量 = 血量 - 傷害值;
            }

            public void 恢復血量(int 回復值)
            {
                血量 = 血量 + 回復值;
            }
        }

        public class 玩家: 戰鬥角色
        {
            public string 職業 = "";
            public int 樓層 = 0;
            public DateTime 動作日期;
            public int 動作次數 = 0;
 
            public 卡牌 玩家出牌 = new 卡牌();

            public 玩家(string 角色名稱)
            {
                BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();
                GH.SstGooglesHeet();
                var values = GH.ReadEntries("角色", "A", "M");

                List<string> 卡牌清單 = new List<string>();

                foreach (var row in values)
                {
                    if (row[0].ToString() == 角色名稱)
                    {
                        this.名稱 = row[HC.A].ToString();
                        this.職業 = row[HC.B].ToString();
                        this.血量 = Convert.ToInt32(row[HC.C]);
                        this.傷害 = Convert.ToInt32(row[HC.D]);
                        this.樓層 = Convert.ToInt32(row[HC.E]);
                        this.動作日期 = Convert.ToDateTime(row[HC.F]);
                        this.動作次數 = Convert.ToInt32(row[HC.G]);

                        break;
                    }
                }

                卡牌清單 = GH.CardList(this.名稱, "玩家");
                牌庫初始化 牌庫初始化 = new 牌庫初始化(卡牌清單);

                this.牌庫 = 牌庫初始化.牌庫;


            }


        }

        public class 怪物 : 戰鬥角色
        {
            public string 階級 = "";
            public 卡牌 怪物出牌 = new 卡牌();
            public string 圖片網址 = "";

            public 怪物(string 怪物名稱)
            {
                BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();
                GH.SstGooglesHeet();
                var values = GH.ReadEntries("怪物", "A", "J");

                List<string> 卡牌清單 = new List<string>();

                foreach (var row in values)
                {
                    if (row[0].ToString() == 怪物名稱)
                    {
                        this.名稱 = row[HC.A].ToString();
                        this.階級 = row[HC.B].ToString();
                        this.血量 = Convert.ToInt32(row[HC.C]);
                        this.傷害 = Convert.ToInt32(row[HC.D]);
                        this.圖片網址 = row[HC.E].ToString();

                        break;
                    }
                }

                卡牌清單 = GH.CardList(this.名稱, "怪物");
                牌庫初始化 牌庫初始化 = new 牌庫初始化(卡牌清單);

                this.牌庫 = 牌庫初始化.牌庫;
            }
        }

        public class 牌庫初始化
        {

            public List<卡牌> 牌庫 = new List<卡牌>();
            BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();

            public 牌庫初始化(List<string> 卡牌清單)
            {
                GH.SstGooglesHeet();
                int 目前編號 = 0;

                var values = GH.ReadEntries("戰鬥卡片", "A", "H");

                foreach (string 卡牌名稱 in 卡牌清單)
                {
                    foreach (var row in values)
                    {
                        if (卡牌名稱 == row[HC.A].ToString())
                        {
                            卡牌 卡牌 = new 卡牌();
                            卡牌.卡牌名稱 = row[HC.A].ToString();
                            卡牌.等級 = Convert.ToInt32(row[HC.B]);
                            卡牌.類型 = Convert.ToInt32(row[HC.C]);
                            卡牌.數值 = Convert.ToDouble(row[HC.D]);
                            卡牌.說明 = row[HC.E].ToString();
                            卡牌.順序編號 = 目前編號;
                            卡牌.圖片網址 = row[HC.H].ToString();
                            牌庫.Add(卡牌);
                            目前編號++;
                            break;
                        }
                    }
                }
            }
        }

        public class 卡牌
        {
            public string 卡牌名稱 = "";
            public int 等級 = 0;
            //輕1特2重3防4	1>2>3>1 ，4看牌義
            public int 類型 = 0;
            public double 數值 = 0.0;
            public string 說明 = "";
            public int 順序編號 = 0;
            public string 圖片網址 = "";

        }

        //暫存 、怪物出牌、 玩家抽卡 、戰鬥描述
        public class 遊戲機制    
        {
            BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();

            private string _戰鬥描述 = "";
            private string _key;

            public void 戰鬥描述(string 戰鬥描述)
            {
                _戰鬥描述 += 戰鬥描述 + "\n";
            }

            public string 取得戰鬥描述()
            {
                return _戰鬥描述;
            }


            public void 儲存暫存(玩家 玩家,怪物 怪物,int 怪物抽到編號)
            {

                List<string> 儲存資料 = new List<string>();

                儲存資料.Add(_key);
                儲存資料.Add(DateTime.Now.ToString());
                儲存資料.Add(玩家.名稱);
                儲存資料.Add(玩家.血量.ToString());
                儲存資料.Add(玩家.傷害.ToString());
                儲存資料.Add("無");
                儲存資料.Add(玩家.取得目前牌庫("玩家"));
                儲存資料.Add(怪物.名稱);
                儲存資料.Add(怪物.血量.ToString());
                儲存資料.Add(怪物.傷害.ToString());
                儲存資料.Add("無");
                儲存資料.Add(怪物.取得目前牌庫("怪物"));
                儲存資料.Add(怪物.牌庫[怪物抽到編號].卡牌名稱);
                儲存資料.Add("0");

                GH.CreateEntry("戰鬥暫存", "A", "L", 儲存資料);
            }

            public List<string>  讀取暫存(string Key)
            {
                return GH.ReadFTempEntries(Key);
            }

            public void 更新暫存(List<string> 暫存資訊)
            {
                GH.UpdateEntry("戰鬥暫存", "N" + 暫存資訊[暫存資訊.Count - 1], "1");
            }

            public void 刪除暫存(List<string> 暫存資訊)
            {
                GH.DeleteEntry("戰鬥暫存", "A" + 暫存資訊[暫存資訊.Count - 1], "N");
            }

            public 玩家 玩家暫存初始化(List<string> 暫存資訊 , string 玩家出牌)
            {
                玩家 玩家 = new 玩家(暫存資訊[HC.C]);
                玩家.血量 = Convert.ToInt32(暫存資訊[HC.D]);
                玩家.傷害 = Convert.ToInt32(暫存資訊[HC.E]);
                玩家.玩家出牌 = 玩家.取得卡牌資訊(玩家出牌);
                玩家.取讀暫存牌庫(new List<int>(Array.ConvertAll<string, int>(暫存資訊[HC.G].Split(','), s => int.Parse(s))));

                return 玩家;
            }

            public 怪物 怪物暫存初始化(List<string> 暫存資訊)
            {
                怪物 怪物 = new 怪物(暫存資訊[HC.H]);
                怪物.血量 = Convert.ToInt32(暫存資訊[HC.I]);
                怪物.傷害 = Convert.ToInt32(暫存資訊[HC.J]);
                怪物.怪物出牌 = 怪物.取得卡牌資訊(暫存資訊[HC.M]);

                return 怪物;
            }

           
            public isRock.LineBot.CarouselTemplate 怪物出牌玩家抽卡並暫存(玩家 玩家 ,怪物 怪物 ,ref List<isRock.LineBot.MessageBase> responseMsgs)
            {
                _key = Guid.NewGuid().ToString();

                _key = _key.Replace('-', '=');

                Random rdm1 = new Random(unchecked((int)DateTime.Now.Ticks));

                //敵人抽牌 (只抽一張)
                int 怪物抽到編號 = rdm1.Next(0, 怪物.牌庫.Count);

                //怪物出牌顯示
                var act1 = new isRock.LineBot.MessageAction()
                { text = " ", label = " " };

                var tmp = new isRock.LineBot.ButtonsTemplate()
                {
                    text = 怪物.名稱 + "階級:" + 怪物.階級 + "HP:" + 怪物.血量,
                    title = "看樣子要使用: " + 怪物.牌庫[怪物抽到編號].卡牌名稱,
                    thumbnailImageUrl = new Uri(怪物.圖片網址),
                };

                tmp.actions.Add(act1);
                responseMsgs.Add(new isRock.LineBot.TemplateMessage(tmp));


                List<卡牌> 玩家抽到 = new List<卡牌>();

                int 牌庫數量;
                int 玩家抽到編號 = 0;
                // 玩家抽牌(抽三張)
                while (玩家抽到.Count < 3)
                {
                    牌庫數量 = 玩家.牌庫.Count - 1;
                    玩家抽到編號 = rdm1.Next(0, 牌庫數量);
                    if (!玩家抽到.Contains(玩家.牌庫[玩家抽到編號]))
                    {
                        玩家抽到.Add(玩家.牌庫[玩家抽到編號]);
                        玩家.牌庫.Remove(玩家.牌庫[玩家抽到編號]);
                    }
                }

                string 抽到卡牌 = "";
                foreach (卡牌 卡牌 in 玩家抽到)
                {
                    if (玩家抽到[玩家抽到.Count - 1] == 卡牌)
                        抽到卡牌 += 卡牌.順序編號;
                    else
                        抽到卡牌 += 卡牌.順序編號 + ",";
                }

                //玩家出牌顯示
                responseMsgs.Add(new isRock.LineBot.TextMessage("請選擇應對方式"));

                var actions1 = new List<isRock.LineBot.TemplateActionBase>();
                actions1.Add(new isRock.LineBot.MessageAction() { label = "使用此張卡牌", text = "RPG-TeM-遭遇戰-出牌-" + _key+"-" + 玩家抽到[0].卡牌名稱 });
                var actions2 = new List<isRock.LineBot.TemplateActionBase>();
                actions2.Add(new isRock.LineBot.MessageAction() { label = "使用此張卡牌", text = "RPG-TeM-遭遇戰-出牌-" + _key+"-" + 玩家抽到[1].卡牌名稱 });
                var actions3 = new List<isRock.LineBot.TemplateActionBase>();
                actions3.Add(new isRock.LineBot.MessageAction() { label = "使用此張卡牌", text = "RPG-TeM-遭遇戰-出牌-" + _key+"-" + 玩家抽到[2].卡牌名稱 });

                var Column1 = new isRock.LineBot.Column
                {
                    text = "等級: " + 玩家抽到[0].等級 + " 說明:" + 玩家抽到[0].說明,
                    title = 玩家抽到[0].卡牌名稱,
                    //設定圖片
                    thumbnailImageUrl = new Uri(玩家抽到[0].圖片網址),
                    actions = actions1 //設定回覆動作
                };

                var Column2 = new isRock.LineBot.Column
                {
                    text = "等級: " + 玩家抽到[1].等級 + " 說明:" + 玩家抽到[1].說明,
                    title = 玩家抽到[1].卡牌名稱,
                    //設定圖片
                    thumbnailImageUrl = new Uri(玩家抽到[1].圖片網址),
                    actions = actions2 //設定回覆動作
                };

                var Column3 = new isRock.LineBot.Column
                {
                    text = "等級: " + 玩家抽到[2].等級 + " 說明:" + 玩家抽到[2].說明,
                    title = 玩家抽到[2].卡牌名稱,
                    //設定圖片
                    thumbnailImageUrl = new Uri(玩家抽到[2].圖片網址),
                    actions = actions3 //設定回覆動作
                };
                //建立CarouselTemplate
                var CarouselTemplate = new isRock.LineBot.CarouselTemplate();

                CarouselTemplate.columns.Add(Column1);
                CarouselTemplate.columns.Add(Column2);
                CarouselTemplate.columns.Add(Column3);

                儲存暫存(玩家, 怪物, 怪物抽到編號);

                return CarouselTemplate;
            }
        }

    }
}
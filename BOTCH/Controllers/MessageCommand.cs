using System;
using System.Collections.Generic;

namespace BOTCH.Controllers
{
    public class MessageCommand
    {
        BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();
        Fighting 戰鬥 = new Fighting();

        public string RPGCommand(string LineEvent)
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
                                else if (TArray[3].Contains("村民")) // 村民系列
                                {
                                    bool 選定職業 = false;

                                    switch (rnd.Next(1, 3))
                                    {
                                        case 1:
                                            職業 = "村民A";
                                            技能1 = "挖土攻擊";
                                            技能2 = "購買鏟子";
                                            選定職業 = true;
                                            break;
                                        case 2:
                                            職業 = "村民B";
                                            技能1 = "採礦攻擊";
                                            技能2 = "購買礦稿";
                                            選定職業 = true;
                                            break;
                                        case 3:
                                            職業 = "村民C";
                                            技能1 = "伐木攻擊";
                                            技能2 = "購買斧頭";
                                            選定職業 = true;
                                            break;
                                    }

                                    if (選定職業) break;
                                }
                            }

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
    }
}
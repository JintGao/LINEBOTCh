using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOTCH.Controllers
{
    public class Fighting
    {
        public string 戰鬥計算(string message)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string[] TArray = message.Split('-');
            string 戰鬥描述 = "對戰開始\n";
            int 計數器 = 1;

            List<戰鬥角色> 戰鬥角色LIST = new List<戰鬥角色>();
            BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();

            if (GH.ReadRoleEntries(TArray[2]).Count == 0)
            {
                戰鬥描述 = TArray[2] + " 指定的玩家不存在";
            }
            else if (GH.ReadRoleEntries(TArray[3]).Count == 0)
            {
                戰鬥描述 = TArray[3] + "指定的玩家不存在";
            }
            else
            {

                List<string> 玩家1 = GH.ReadRoleEntries(TArray[2]);
                List<string> 玩家2 = GH.ReadRoleEntries(TArray[3]);
                戰鬥角色 戰鬥角色1 = new 戰鬥角色(玩家1);
                戰鬥角色 戰鬥角色2 = new 戰鬥角色(玩家2);

                while (戰鬥角色1.HP > 0 && 戰鬥角色2.HP > 0)
                {
                    戰鬥描述 += "=====第 " + 計數器 + " 回合===== \n";

                    //先手判斷
                    if (戰鬥角色1.敏捷 > 戰鬥角色2.敏捷)
                    {

                        戰鬥描述 = 戰鬥過程(戰鬥角色1, 戰鬥角色2, 戰鬥描述);

                        if (戰鬥角色2.HP > 0)
                            戰鬥描述 = 戰鬥過程(戰鬥角色2, 戰鬥角色1, 戰鬥描述);
                        else
                            break;
                    }
                    else
                    {
                        戰鬥描述 = 戰鬥過程(戰鬥角色2, 戰鬥角色1, 戰鬥描述);

                        if (戰鬥角色1.HP > 0)
                            戰鬥描述 = 戰鬥過程(戰鬥角色1, 戰鬥角色2, 戰鬥描述);
                        else
                            break;
                    }

                    計數器++;

                }

                戰鬥描述 += "對戰結束\n";

                if (戰鬥角色1.HP < 0)
                    戰鬥描述 += 戰鬥角色2.姓名 + " 勝利  血量剩餘:" + 戰鬥角色2.HP + "\n";
                else
                    戰鬥描述 += 戰鬥角色1.姓名 + " 勝利  血量剩餘:" + 戰鬥角色1.HP + "\n";
            }


            return 戰鬥描述;
        }

        //戰鬥過程抽出來算
        public string 戰鬥過程(戰鬥角色 戰鬥角色1, 戰鬥角色 戰鬥角色2, string 戰鬥描述)
        {
            Random rnd;

            rnd = new Random(Guid.NewGuid().GetHashCode());
            int 玩家1_是否使用技能 = rnd.Next(0, 2);

            double 傷害 = 0;

            //是否使用技能
            if (玩家1_是否使用技能 == 1)
            {
                rnd = new Random(Guid.NewGuid().GetHashCode());
                int 技能抽取 = rnd.Next(1, 3);

                switch (技能抽取)
                {
                    case 1:
                        if (戰鬥角色1.技能1.類型 == "攻擊")
                        {
                            傷害 = 戰鬥角色1.力量 * 戰鬥角色1.技能1.傷害;
                            戰鬥描述 += 戰鬥角色1.姓名 + "使用:" + 戰鬥角色1.技能1.名稱 + "\n";
                        }
                        else
                        {
                            戰鬥描述 += 戰鬥角色1.姓名 + "使用:" + 戰鬥角色1.技能1.名稱 + "--暫無實作 \n";
                        }
                        break;

                    case 2:
                        if (戰鬥角色1.技能2.類型 == "攻擊")
                        {
                            傷害 = 戰鬥角色1.力量 * 戰鬥角色1.技能2.傷害;
                            戰鬥描述 += 戰鬥角色1.姓名 + "使用:" + 戰鬥角色1.技能2.名稱 + "\n";
                        }
                        else
                        {
                            戰鬥描述 += 戰鬥角色1.姓名 + "使用:" + 戰鬥角色1.技能2.名稱 + " --暫無實作 \n";
                        }
                        break;
                }
            }
            else
            {
                傷害 = 戰鬥角色1.力量;
                戰鬥描述 += 戰鬥角色1.姓名 + "使用:普通攻擊 \n";
            }

            //迴避判定
            rnd = new Random(Guid.NewGuid().GetHashCode());
            double 攻擊命中 = Convert.ToDouble(rnd.Next(1, 10)) / 10;

            if (攻擊命中 > 戰鬥角色2.迴避)
            {
                戰鬥角色2.受到傷害(Convert.ToInt32(傷害));
                戰鬥描述 += 戰鬥角色1.姓名 + "攻擊命中 造成 " + Convert.ToInt32(傷害) + "傷害 \n";
                戰鬥描述 += 戰鬥角色2.姓名 + "血量剩餘: " + 戰鬥角色2.HP + "\n";
            }
            else
            {
                戰鬥描述 += 戰鬥角色1.姓名 + " 攻擊被迴避 \n";
            }

            return 戰鬥描述;
        }
    }


    public class 戰鬥角色
    {
        public string 姓名 = "";
        public int HP = 0;
        public int 力量 = 0;
        public int 敏捷 = 0;
        public int 體質 = 0;
        public int 智慧 = 0;
        public double 迴避;

        public 技能 技能1;
        public 技能 技能2;

        public 戰鬥角色(List<string> 玩家資訊)
        {
            姓名 = 玩家資訊[0];
            HP = Convert.ToInt32(玩家資訊[2]);
            力量 = Convert.ToInt32(玩家資訊[3]);
            敏捷 = Convert.ToInt32(玩家資訊[4]);
            體質 = Convert.ToInt32(玩家資訊[5]);
            智慧 = Convert.ToInt32(玩家資訊[6]);
            迴避 = 0.1 + (敏捷 * 0.02);

            技能1 = new 技能(玩家資訊[7]);
            技能2 = new 技能(玩家資訊[8]);
        }


        public void 受到傷害(int 傷害)
        {
            HP = HP - 傷害;
        }

    }

    public class 技能
    {
        public string 名稱 = "";
        public string 類型 = "";
        public double 傷害;

        public 技能(string name)
        {
            BOTCH.Controllers.GooglesHeet GH = new GooglesHeet();
            List<string> 技能資訊 = GH.ReadSkillEntries(name);
            名稱 = 技能資訊[0];
            類型 = 技能資訊[1];
            傷害 = Convert.ToDouble(技能資訊[2]);
        }
    }

}
﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BililiveRecorder.Core
{
    public enum MsgTypeEnum
    {
        /// <summary>
        /// 彈幕
        /// </summary>
        Comment,

        /// <summary>
        /// 禮物
        /// </summary>
        GiftSend,

        /// <summary>
        /// 歡迎老爷
        /// </summary>
        Welcome,

        /// <summary>
        /// 直播開始
        /// </summary>
        LiveStart,

        /// <summary>
        /// 直播結束
        /// </summary>
        LiveEnd,
        /// <summary>
        /// 其他
        /// </summary>
        Unknown,
        /// <summary>
        /// 欢迎船员
        /// </summary>
        WelcomeGuard,
        /// <summary>
        /// 购买船票（上船）
        /// </summary>
        GuardBuy

    }

    public class DanmakuModel
    {
        /// <summary>
        /// 消息類型
        /// </summary>
        public MsgTypeEnum MsgType { get; set; }

        /// <summary>
        /// 彈幕內容
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.Comment"/></item>
        /// </list></para>
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// 彈幕用戶
        /// </summary>
        [Obsolete("请使用 UserName")]
        public string CommentUser
        {
            get { return UserName; }
            set { UserName = value; }
        }

        /// <summary>
        /// 消息触发者用户名
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.Comment"/></item>
        /// <item><see cref="MsgTypeEnum.GiftSend"/></item>
        /// <item><see cref="MsgTypeEnum.Welcome"/></item>
        /// <item><see cref="MsgTypeEnum.WelcomeGuard"/></item>
        /// <item><see cref="MsgTypeEnum.GuardBuy"/></item>
        /// </list></para>
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 消息触发者用户ID
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.Comment"/></item>
        /// <item><see cref="MsgTypeEnum.GiftSend"/></item>
        /// <item><see cref="MsgTypeEnum.Welcome"/></item>
        /// <item><see cref="MsgTypeEnum.WelcomeGuard"/></item>
        /// <item><see cref="MsgTypeEnum.GuardBuy"/></item>
        /// </list></para>
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 用户舰队等级
        /// <para>0 为非船员 1 为总督 2 为提督 3 为舰长</para>
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.Comment"/></item>
        /// <item><see cref="MsgTypeEnum.WelcomeGuard"/></item>
        /// <item><see cref="MsgTypeEnum.GuardBuy"/></item>
        /// </list></para>
        /// </summary>
        public int UserGuardLevel { get; set; }

        /// <summary>
        /// 禮物用戶
        /// </summary>
        [Obsolete("请使用 UserName")]
        public string GiftUser
        {
            get { return UserName; }
            set { UserName = value; }
        }

        /// <summary>
        /// 禮物名稱
        /// </summary>
        public string GiftName { get; set; }

        /// <summary>
        /// 禮物數量
        /// </summary>
        [Obsolete("请使用 GiftCount")]
        public string GiftNum { get { return GiftCount.ToString(); } }

        /// <summary>
        /// 礼物数量
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.GiftSend"/></item>
        /// <item><see cref="MsgTypeEnum.GuardBuy"/></item>
        /// </list></para>
        /// <para>此字段也用于标识上船 <see cref="MsgTypeEnum.GuardBuy"/> 的数量（月数）</para>
        /// </summary>
        public int GiftCount { get; set; }

        /// <summary>
        /// 当前房间的礼物积分（Room Cost）
        /// 因以前出现过不传递rcost的礼物，并且用处不大，所以弃用
        /// </summary>
        [Obsolete("如有需要请自行解析RawData", true)]
        public string Giftrcost { get { return "0"; } set { } }

        /// <summary>
        /// 该用户是否为房管（包括主播）
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.Comment"/></item>
        /// <item><see cref="MsgTypeEnum.GiftSend"/></item>
        /// </list></para>
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否VIP用戶(老爺)
        /// <para>此项有值的消息类型：<list type="bullet">
        /// <item><see cref="MsgTypeEnum.Comment"/></item>
        /// <item><see cref="MsgTypeEnum.Welcome"/></item>
        /// </list></para>
        /// </summary>
        public bool IsVIP { get; set; }

        /// <summary>
        /// <see cref="MsgTypeEnum.LiveStart"/>,<see cref="MsgTypeEnum.LiveEnd"/> 事件对应的房间号
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 原始数据, 高级开发用
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// 内部用, JSON数据版本号 通常应该是2
        /// </summary>
        public int JSON_Version { get; set; }

        public DanmakuModel()
        { }

        public DanmakuModel(string JSON)
        {
            RawData = JSON;
            JSON_Version = 2;
            var obj = new JSONObject(JSON);
            string cmd = obj["cmd"].str?.Decode();
            switch (cmd)
            {
                case "LIVE":
                    MsgType = MsgTypeEnum.LiveStart;
                    RoomID = obj["roomid"].str?.Decode();
                    break;
                case "PREPARING":
                    MsgType = MsgTypeEnum.LiveEnd;
                    RoomID = obj["roomid"].str?.Decode();
                    break;
                case "DANMU_MSG":
                    MsgType = MsgTypeEnum.Comment;
                    CommentText = obj["info"][1].str?.Decode();
                    UserID = (int)obj["info"][2][0].i;
                    UserName = obj["info"][2][1].str?.Decode();
                    IsAdmin = obj["info"][2][2].str?.Decode() == "1";
                    IsVIP = obj["info"][2][3].str?.Decode() == "1";
                    UserGuardLevel = (int)obj["info"][7].i;
                    break;
                case "SEND_GIFT":
                    MsgType = MsgTypeEnum.GiftSend;
                    GiftName = obj["data"]["giftName"].str?.Decode();
                    UserName = obj["data"]["uname"].str?.Decode();
                    UserID = (int)obj["data"]["uid"].i;
                    // Giftrcost = obj["data"]["rcost"].ToString();
                    GiftCount = (int)obj["data"]["num"].i;
                    break;
                case "WELCOME":
                    {
                        MsgType = MsgTypeEnum.Welcome;
                        UserName = obj["data"]["uname"].str?.Decode();
                        UserID = (int)obj["data"]["uid"].i;
                        IsVIP = true;
                        IsAdmin = obj["data"]?["is_admin"]?.b ?? obj["data"]?["isadmin"]?.str == "1";
                        break;

                    }
                case "WELCOME_GUARD":
                    {
                        MsgType = MsgTypeEnum.WelcomeGuard;
                        UserName = obj["data"]["username"].str?.Decode();
                        UserID = (int)obj["data"]["uid"].i;
                        UserGuardLevel = (int)obj["data"]["guard_level"].i;
                        break;
                    }
                case "GUARD_BUY":
                    {
                        MsgType = MsgTypeEnum.GuardBuy;
                        UserID = (int)obj["data"]["uid"].i;
                        UserName = obj["data"]["username"].str?.Decode();
                        UserGuardLevel = (int)obj["data"]["guard_level"].i;
                        GiftName = UserGuardLevel == 3 ? "舰长" : UserGuardLevel == 2 ? "提督" : UserGuardLevel == 1 ? "总督" : "";
                        GiftCount = (int)obj["data"]["num"].i;
                        break;
                    }
                default:
                    {
                        MsgType = MsgTypeEnum.Unknown;
                        break;
                    }
            }
        }

    }
}

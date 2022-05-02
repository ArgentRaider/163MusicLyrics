﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MusicLyricApp.Exception;
using MusicLyricApp.Utils;

namespace MusicLyricApp.Bean
{
    // 双语歌词类型
    public enum ShowLrcTypeEnum
    {
        ONLY_ORIGIN = 0, // 仅显示原文
        ONLY_TRANSLATE = 1, // 仅显示译文
        ORIGIN_PRIOR = 2, // 优先原文
        TRANSLATE_PRIOR = 3, // 优先译文
        MERGE_ORIGIN = 4, // 合并显示，优先原文
        MERGE_TRANSLATE = 5, // 合并显示，优先译文
    }

    // 输出文件名类型
    public enum OutputFilenameTypeEnum
    {
        NAME_SINGER = 0, // 歌曲名 - 歌手
        SINGER_NAME = 1, // 歌手 - 歌曲名
        NAME = 2 // 歌曲名
    }
    
    // 搜索来源
    public enum SearchSourceEnum
    {
        NET_EASE_MUSIC = 0, // 网易云音乐
        QQ_MUSIC = 1 // QQ音乐
    }

    // 搜索类型
    public enum SearchTypeEnum
    {
        SONG_ID = 0, // 歌曲ID
        ALBUM_ID = 1 // 专辑ID
    }

    // 强制两位类型
    public enum DotTypeEnum
    {
        DISABLE = 0, // 不启用
        DOWN = 1, // 截位
        HALF_UP = 2 // 四舍五入
    }

    // 输出文件格式
    public enum OutputEncodingEnum
    {
        UTF_8 = 0,
        UTF_8_BOM = 1,
        GB_2312 = 2,
        GBK = 3,
        UNICODE = 4
    }

    public enum OutputFormatEnum
    {
        [Description("lrc文件(*.lrc)|*.lrc")] LRC = 0,

        [Description("srt文件(*.srt)|*.srt")] SRT = 1
    }

    // 罗马音转换模式
    public enum RomajiModeEnum
    {
        [Description("标准模式")] NORMAL = 0,
        [Description("空格分组")] SPACED = 1,
        [Description("送假名")] OKURIGANA = 2,
        [Description("注音假名")] FURIGANA = 3,
    }
    
    // 罗马音字体系
    public enum RomajiSystemEnum
    {
        [Description("日本式")] NIPPON = 0,
        [Description("护照式")] PASSPORT = 1,
        [Description("平文式")] HEPBURN = 2,
    }

    /**
     * 错误码
     */
    public static class ErrorMsg
    {
        public const string SUCCESS = "成功";
        public const string SEARCH_RESULT_STAGE = "查询成功，结果已暂存";
        public const string MUST_SEARCH_BEFORE_SAVE = "您必须先搜索，才能保存内容";
        public const string MUST_SEARCH_BEFORE_COPY_SONG_URL = "您必须先搜索，才能获取直链";
        public const string INPUT_ID_ILLEGAL = "您输入的ID不合法";
        public const string ALBUM_NOT_EXIST = "专辑信息暂未被收录或查询失败";
        public const string SONG_NOT_EXIST = "歌曲信息暂未被收录或查询失败";
        public const string LRC_NOT_EXIST = "歌词信息暂未被收录或查询失败";
        public const string FUNCTION_NOT_SUPPORT = "该功能暂不可用，请等待后续更新";
        public const string SONG_URL_COPY_SUCCESS = "歌曲直链，已复制到剪切板";
        public const string SONG_URL_GET_FAILED = "歌曲直链，获取失败";
        public const string ROMAJI_DEPENDENCY_LOSS = "罗马音相关依赖缺失，请重新下载";
        public const string SAVE_COMPLETE = "保存完毕，成功 {0} 跳过 {1}";

        public const string GET_LATEST_VERSION_FAILED = "获取最新版本失败";
        public const string THIS_IS_LATEST_VERSION = "当前版本已经是最新版本";
        public const string SYSTEM_ERROR = "系统错误";
        public const string NETWORK_ERROR = "网络错误，请检查网络链接";
        public const string API_RATE_LIMIT = "请求过于频繁，请稍后再试";
    }

    /// <summary>
    /// 封装单首歌曲的持久化信息
    /// </summary>
    public class SaveVo
    {
        public SaveVo(string songId, SongVo songVo, LyricVo lyricVo)
        {
            SongId = songId;
            SongVo = songVo;
            LyricVo = lyricVo;
        }

        public string SongId { get; }

        public SongVo SongVo { get; }

        public LyricVo LyricVo { get; }
    }

    /// <summary>
    /// 歌曲信息
    /// </summary>
    public class SongVo
    {
        /// <summary>
        /// 歌曲名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 歌手名
        /// </summary>
        public string Singer { get; set; }

        /// <summary>
        /// 所属专辑名
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// 歌曲直链 Url
        /// </summary>
        public string Links { get; set; }

        /// <summary>
        /// 歌曲时长 ms
        /// </summary>
        public long Duration { get; set; }
    }

    /// <summary>
    /// 歌词信息
    /// </summary>
    public class LyricVo
    {
        /// <summary>
        /// 歌词内容
        /// </summary>
        public string Lyric;

        /// <summary>
        /// 译文歌词内容
        /// </summary>
        public string TranslateLyric;

        /// <summary>
        /// 歌曲时长 ms
        /// </summary>
        public long Duration { get; set; }

        public void SetLyric(string content)
        {
            Lyric = HttpUtility.HtmlDecode(content);
        }

        public void SetTranslateLyric(string content)
        {
            TranslateLyric = HttpUtility.HtmlDecode(content);
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Lyric) && string.IsNullOrEmpty(TranslateLyric);
        }
    }
    
    /// <summary>
    /// 当行歌词信息
    /// </summary>
    public class LyricLineVo : IComparable
    {
        /// <summary>
        /// 时间戳字符串
        /// </summary>
        public string Timestamp { get; set; }

        /**
         * 时间偏移量
         */
        public long TimeOffset { get; set; }
        
        /// <summary>
        /// 歌词正文
        /// </summary>
        public string Content { get; set; }

        public LyricLineVo(string lyricLine)
        {
            var index = lyricLine.IndexOf("]");
            if (index == -1)
            {
                Timestamp = "";
                TimeOffset = -1;
                Content = lyricLine;
            }
            else
            {
                Timestamp = lyricLine.Substring(0, index + 1);
                Content = lyricLine.Substring(index + 1);
                TimeOffset = GlobalUtils.TimestampStrToLong(Timestamp);
            }
        }

        public LyricLineVo()
        {
        }

        public int CompareTo(object input)
        {
            if (!(input is LyricLineVo obj))
            {
                throw new MusicLyricException(ErrorMsg.SYSTEM_ERROR);
            }
            
            if (TimeOffset == -1 && obj.TimeOffset == -1)
            {
                return 0;
            }
            
            if (TimeOffset == -1)
            {
                return -1;
            }

            if (obj.TimeOffset == -1)
            {
                return 1;
            }

            return (int) (TimeOffset - obj.TimeOffset);
        }

        public override string ToString()
        {
            return Timestamp + Content;
        }
    }

    /// <summary>
    /// 搜索信息
    /// </summary>
    public class SearchInfo
    {
        /// <summary>
        /// 输入 ID 列表
        /// </summary>
        public string[] InputIds { get; set; }

        /// <summary>
        /// 实际处理的歌曲 ID 列表
        /// </summary>
        public readonly HashSet<string> SongIds = new HashSet<string>();

        public SettingBean SettingBeanBackup { get; set; }

        public SettingBean SettingBean { get; set; }
    }

    public static class EnumHelper
    {
        public static string ToDescription(this Enum val)
        {
            var type = val.GetType();
            var memberInfo = type.GetMember(val.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            //如果没有定义描述，就把当前枚举值的对应名称返回
            if (attributes == null || attributes.Length != 1) return val.ToString();

            return (attributes.Single() as DescriptionAttribute)?.Description;
        }
    }
}
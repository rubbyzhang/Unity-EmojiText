using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Celf
{
    public enum AtlasType
    {
        None = -1,
        Common = 0,
        Login = 1,
        Battle = 2,
        Main = 3,
    }

    public static class UIAtlasUtility
    {
        public static string spriteRootPath = "UI";
        /// <summary>
        /// atlas在Resource下的相对目录
        /// </summary>
        public static string atlasResourceRootPath = "atlas/";
        public static char altasNameSeparator = '_';

        public static string atlasRootPath
        {
            get { return "Resources/" + atlasResourceRootPath; }
        }

        //对应 AtlasType定义顺序
        public static readonly string[] ConstAtlasPath = new string[]
        {
            "UI/atlas/common",
        };
    }
}

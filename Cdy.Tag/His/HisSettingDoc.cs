﻿//==============================================================
//  Copyright (C) 2019  Inc. All rights reserved.
//
//==============================================================
//  Create by 种道洋 at 2019/12/27 18:45:02.
//  Version 1.0
//  种道洋
//==============================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Cdy.Tag
{
    /// <summary>
    /// 
    /// </summary>
    public class HisSettingDoc
    {

        #region ... Variables  ...

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...

        #endregion ...Constructor...

        #region ... Properties ...

        /// <summary>
        /// 单个文件保存数据时长
        /// 单位小时
        /// </summary>
        public int FileDataDuration { get; set; } = 4;


        /// <summary>
        /// 单个数据块保存数据的时长
        /// 单位分钟
        /// </summary>
        public int DataBlockDuration { get; set; } = 5;

        /// <summary>
        /// 一个文件中变量的个数
        /// </summary>
        public int TagCountOneFile { get; set; } = 100000;

        /// <summary>
        /// 数据序列化类型
        /// </summary>
        public string DataSeriser { get; set; } = "LocalFile";

        /// <summary>
        /// 主历史记录路径
        /// </summary>
        public string HisDataPathPrimary { get; set; }

        /// <summary>
        /// 备份历史记录路径
        /// </summary>
        public string HisDataPathBack { get; set; }

        #endregion ...Properties...

        #region ... Methods    ...

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }

    /// <summary>
    /// 
    /// </summary>
    public static class HisSettingDocExtend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public  static XElement SaveToXML(this HisSettingDoc doc)
        {
            XElement xe = new XElement("HisSetting");
            xe.SetAttributeValue("FileDataDuration", doc.FileDataDuration);
            xe.SetAttributeValue("DataBlockDuration", doc.DataBlockDuration);
            return xe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static HisSettingDoc LoadHisSettingDocFromXML(this XElement element)
        {
            HisSettingDoc re = new HisSettingDoc();
            if(element.Attribute("FileDataDuration") !=null)
            {
                re.FileDataDuration = int.Parse(element.Attribute("FileDataDuration").Value);
            }

            if (element.Attribute("DataBlockDuration") != null)
            {
                re.DataBlockDuration = int.Parse(element.Attribute("DataBlockDuration").Value);
            }

            return re;
        }
    }

}

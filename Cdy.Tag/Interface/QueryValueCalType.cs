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

namespace Cdy.Tag
{
    /// <summary>
    /// 
    /// </summary>
    public enum QueryValueMatchType
    {
        /// <summary>
        /// 取前一个值
        /// </summary>
        Previous,
        /// <summary>
        /// 取后一个值
        /// </summary>
        After,
        /// <summary>
        /// 取较近的值
        /// </summary>
        Closed,
        /// <summary>
        /// 线性插值
        /// </summary>
        Linear
    }
}

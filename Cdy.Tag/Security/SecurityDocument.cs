﻿//==============================================================
//  Copyright (C) 2020  Inc. All rights reserved.
//
//==============================================================
//  Create by 种道洋 at 2020/2/16 10:53:26.
//  Version 1.0
//  种道洋
//==============================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace Cdy.Tag
{
    public class SecurityDocument
    {

        #region ... Variables  ...

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...

        #endregion ...Constructor...

        #region ... Properties ...

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "local";

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; } = "0.0.1";

        /// <summary>
        /// 
        /// </summary>
        public UserDocument User { get; set; } = new UserDocument();

        /// <summary>
        /// 
        /// </summary>
        public PermissionDocument Permission { get; set; } = new PermissionDocument();

        #endregion ...Properties...

        #region ... Methods    ...

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }
}

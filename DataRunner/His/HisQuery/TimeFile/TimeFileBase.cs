﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cdy.Tag
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeFileBase:Dictionary<int,TimeFileBase>
    {
        #region ... Variables  ...

        private int mTimeKey = 0;

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...

        #endregion ...Constructor...

        #region ... Properties ...

        /// <summary>
            /// 
            /// </summary>
        public int TimeKey
        {
            get
            {
                return mTimeKey;
            }
            set
            {
                if (mTimeKey != value)
                {
                    mTimeKey = value;
                }
            }
        }



        #endregion ...Properties...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TimeFileBase AddTimefile(int key, TimeFileBase value)
        {
            if (this.ContainsKey(key))
            {
                return this[key];
            }
            else
            {
                base.Add(key, value);
                return value;
            }
        }



        #region ... Methods    ...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public virtual MinuteTimeFile GetFile(DateTime dateTime)
        {
            return null;
        }

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }
}
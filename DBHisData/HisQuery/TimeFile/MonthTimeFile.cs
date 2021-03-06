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
    public class MonthTimeFile : TimeFileBase
    {

        #region ... Variables  ...

        private SortedDictionary<DateTime, Tuple<TimeSpan, DataFileInfo>> mFileMaps = new SortedDictionary<DateTime, Tuple<TimeSpan, DataFileInfo>>();

        private DateTime mMaxTime = DateTime.MinValue;

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...

        #endregion ...Constructor...

        #region ... Properties ...

        /// <summary>
        /// 
        /// </summary>
        public YearTimeFile Parent { get; set; }

        #endregion ...Properties...

        #region ... Methods    ...

        /// <summary>
        /// 
        /// </summary>
        public void UpdateLastDatetime()
        {
            if(mFileMaps.ContainsKey(mMaxTime))
            {
                mFileMaps[mMaxTime].Item2.UpdateLastDatetime();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="file"></param>
        public void AddFile(DateTime startTime,TimeSpan duration, DataFileInfo file)
        {
            if(!mFileMaps.ContainsKey(startTime))
            {
                mFileMaps.Add(startTime, new Tuple<TimeSpan, DataFileInfo>(duration, file));

                if(startTime>mMaxTime)
                {
                    mMaxTime = startTime;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DataFileInfo GetDataFile(DateTime dateTime)
        {
            foreach (var vv in mFileMaps)
            {
                if (vv.Key <= dateTime && dateTime < (vv.Key + vv.Value.Item1))
                {
                    return vv.Value.Item2;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<DataFileInfo> GetDataFiles(DateTime startTime,DateTime endTime)
        {
            List<DataFileInfo> infos = new List<DataFileInfo>();

            DateTime stime = startTime;
            foreach (var vv in mFileMaps)
            {
                if (vv.Key >= startTime && vv.Key < endTime)
                {
                    infos.Add(vv.Value.Item2);
                }
            }
            return infos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public List<DataFileInfo> GetDataFiles(DateTime startTime,TimeSpan span)
        {
            return GetDataFiles(startTime, startTime + span);
        }


        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }
}

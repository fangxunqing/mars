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
using System.Threading;
using System.Linq;

namespace Cdy.Tag
{
    /// <summary>
    /// 
    /// </summary>
    public class TimerMemoryCacheProcesser:IDisposable
    {

        #region ... Variables  ...


        /// <summary>
        /// 定时记录对象集合
        /// </summary>
        private Dictionary<long, List<HisRunTag>> mTimerTags = new Dictionary<long, List<HisRunTag>>();

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<long, long> mCount = new Dictionary<long, long>();

        public const int MaxTagCount = 100000;

        private int mCurrentCount = 0;

        private bool mIsClosed = false;

        private ManualResetEvent resetEvent;

        private ManualResetEvent closedEvent;

        private Thread mRecordThread;

        private DateTime mLastUpdateTime;

        private bool mIsBusy = false;

        private int mBusyCount = 0;

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...
        /// <summary>
        /// 
        /// </summary>
        public TimerMemoryCacheProcesser()
        {
            resetEvent = new ManualResetEvent(false);
            closedEvent = new ManualResetEvent(false);
        }
        #endregion ...Constructor...

        #region ... Properties ...

        /// <summary>
        /// 
        /// </summary>
        public Action<HisRunTag>    PreProcess { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action AfterProcess { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        #endregion ...Properties...

        #region ... Methods    ...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void Notify(DateTime time)
        {
            if (mIsBusy)
            {
                mBusyCount++;
                if(Id==0)
                LoggerService.Service.Warn("Record", "TimerMemoryCacheProcesser"+Id+" 出现阻塞:"+mBusyCount);
            }
            else
            {
                mBusyCount = 0;
            }
            mLastUpdateTime = time;
            resetEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            mRecordThread = new Thread(ThreadProcess);
            mRecordThread.IsBackground=true;
            mRecordThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            mIsClosed = true;
            resetEvent.Set();
            closedEvent.WaitOne(1000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool AddTag(HisRunTag tag)
        {
            if (mCurrentCount < MaxTagCount)
            {
                var cc = tag.Circle;
                if (mTimerTags.ContainsKey(cc))
                {
                    mTimerTags[cc].Add(tag);
                }
                else
                {
                    mTimerTags.Add(cc, new List<HisRunTag>() { tag });
                    mCount.Add(cc, 0);
                }
                mCurrentCount++;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            mTimerTags.Clear();
            mCount.Clear();
            mCurrentCount = 0;
        }
        

        /// <summary>
        /// 
        /// </summary>
        private void ThreadProcess()
        {
            closedEvent.Reset();
            var vkeys = mCount.Keys.ToArray();
            while (!mIsClosed)
            {
                resetEvent.WaitOne();
                resetEvent.Reset();
                if (mIsClosed) break;
                try
                {
                    mIsBusy = true;
                    foreach (var vv in vkeys)
                    {
                        mCount[vv] += HisEnginer.MemoryTimeTick;
                        if (mCount[vv] >= vv)
                        {
                            mCount[vv] = 0;
                            ProcessTags(mTimerTags[vv]);
                        }
                    }
                    mIsBusy = false;
                }
                catch
                {

                }
                resetEvent.Reset();
            }
            closedEvent.Set();
        }


        /// <summary>
        /// 记录一组
        /// </summary>
        /// <param name="tags"></param>
        private void ProcessTags(List<HisRunTag> tags)
        {
            int tim = (int)((mLastUpdateTime - HisRunTag.StartTime).TotalMilliseconds / HisEnginer.MemoryTimeTick);
            foreach (var vv in tags)
            {
                vv.UpdateValue(tim);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            resetEvent.Close();
            closedEvent.Close();
        }

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }
}

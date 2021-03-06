﻿//==============================================================
//  Copyright (C) 2020  Inc. All rights reserved.
//
//==============================================================
//  Create by 种道洋 at 2020/1/2 10:04:50.
//  Version 1.0
//  种道洋
//==============================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Input;
using DBDevelopClientApi;
using DBInStudio.Desktop.ViewModel;
using Cdy.Tag;

namespace DBInStudio.Desktop
{
    /// <summary>
    /// 
    /// </summary>
    public class TagViewModel : ViewModelBase
    {

        #region ... Variables  ...
        private Cdy.Tag.Tagbase mRealTagMode;
        private Cdy.Tag.HisTag mHisTagMode;

        private static string[] mTagTypeList;
        private static string[] mRecordTypeList;
        private static string[] mCompressTypeList;

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string[]> Drivers;

        private string mDriverName;
        private string mRegistorName;

        private bool mHasHisTag;

        private CompressParameterModelBase mCompressParameterModel;

        private string[] mRegistorList;

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...
        /// <summary>
        /// 
        /// </summary>
        static TagViewModel()
        {
            InitEnumType();
            mCompressTypeList = new string[] 
            {
                Res.Get("NoneCompress"),
                Res.Get("LosslessCompress"),
                Res.Get("DeadAreaCompress"),
                Res.Get("SlopeCompress")
            };
        }

        public TagViewModel()
        {

        }

        public TagViewModel(Cdy.Tag.Tagbase realTag, Cdy.Tag.HisTag histag)
        {
            this.mRealTagMode = realTag;
            this.HisTagMode = histag;
            CheckLinkAddress();
        }

        #endregion ...Constructor...

        #region ... Properties ...

        /// <summary>
        /// 
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// 是否新建
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
            /// 
            /// </summary>
        public CompressParameterModelBase CompressParameterModel
        {
            get
            {
                return mCompressParameterModel;
            }
            set
            {
                if (mCompressParameterModel != value)
                {
                    mCompressParameterModel = value;
                    OnPropertyChanged("CompressParameterModel");
                }
            }
        }


        /// <summary>
        /// 实时变量配置
        /// </summary>
        public Cdy.Tag.Tagbase RealTagMode
        {
            get
            {
                return mRealTagMode;
            }
            set
            {
                if (mRealTagMode != value)
                {
                    mRealTagMode = value;
                }
            }
        }

        /// <summary>
        /// 历史变量配置
        /// </summary>
        public Cdy.Tag.HisTag HisTagMode
        {
            get
            {
                return mHisTagMode;
            }
            set
            {
                if (mHisTagMode != value)
                {
                    mHisTagMode = value;
                    CheckRecordTypeParameterModel();
                }
                mHasHisTag = mHisTagMode != null;
            }
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
            get
            {
                return mRealTagMode != null ? mRealTagMode.Name : string.Empty;
            }
            set
            {
                if (mRealTagMode != null && mRealTagMode.Name != value)
                {
                    mRealTagMode.Name = value;
                    IsChanged = true;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc
        {
            get
            {
                return mRealTagMode != null ? mRealTagMode.Desc : string.Empty;
            }
            set
            {
                if (mRealTagMode != null && mRealTagMode.Desc != value)
                {
                    mRealTagMode.Desc = value;
                    IsChanged = true;
                    OnPropertyChanged("Desc");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] TagTypeList
        {
            get
            {
                return mTagTypeList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] RecordTypeList
        {
            get
            {
                return mRecordTypeList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] CompressTypeList
        {
            get
            {
                return mCompressTypeList;
            }
        }

        /// <summary>
            /// 
            /// </summary>
        public string[] RegistorList
        {
            get
            {
                return mRegistorList;
            }
            set
            {
                if (mRegistorList != value)
                {
                    mRegistorList = value;
                }
                OnPropertyChanged("RegistorList");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string[] DriverList
        {
            get
            {
                if (Drivers != null)
                {
                    return Drivers.Keys.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }



        /// <summary>
        /// 类型
        /// </summary>
        public int Type
        {
            get
            {
                return mRealTagMode != null ? (int)mRealTagMode.Type : -1;
            }
            set
            {
                if (mRealTagMode != null && (int)mRealTagMode.Type != value)
                {
                    ChangeTagType((Cdy.Tag.TagType)value);
                    IsChanged = true;
                    OnPropertyChanged("Type");
                    OnPropertyChanged("TypeString");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TypeString
        {
            get
            {
                return mRealTagMode.Type.ToString();
            }
        }

        /// <summary>
        /// 组
        /// </summary>
        public string Group
        {
            get
            {
                return mRealTagMode != null ? mRealTagMode.Group : string.Empty;
            }
            set
            {
                if (mRealTagMode != null && mRealTagMode.Group != value)
                {
                    mRealTagMode.Group = value;
                    IsChanged = true;
                    OnPropertyChanged("Group");
                }
            }
        }

        /// <summary>
        /// 关联外部地址
        /// </summary>
        public string LinkAddress
        {
            get
            {
                return mRealTagMode != null ? mRealTagMode.LinkAddress : string.Empty;
            }
            set
            {
                if (mRealTagMode != null && mRealTagMode.LinkAddress != value)
                {
                    mRealTagMode.LinkAddress = value;
                    IsChanged = true;
                    OnPropertyChanged("LinkAddress");
                }
            }
        }

        /// <summary>
            /// 
            /// </summary>
        public string DriverName
        {
            get
            {
                return mDriverName;
            }
            set
            {
                if (mDriverName != value)
                {
                    mDriverName = value;
                    LinkAddress = DriverName + ":" + RegistorName;
                    if(Drivers!=null&&Drivers.ContainsKey(mDriverName))
                    {
                        RegistorList = Drivers[mDriverName];
                    }
                    else
                    {
                        RegistorList = null;
                    }
                    OnPropertyChanged("DriverName");
                }
            }
        }

        /// <summary>
            /// 
            /// </summary>
        public string RegistorName
        {
            get
            {
                return mRegistorName;
            }
            set
            {
                if (mRegistorName != value)
                {
                    mRegistorName = value;
                    LinkAddress = DriverName + ":" + RegistorName;
                    OnPropertyChanged("RegistorName");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool HasHisTag
        {
            get
            {
                return mHasHisTag;
            }
            set
            {
                if (mHasHisTag != value)
                {
                    mHasHisTag = value;
                    if(!value)
                    {
                        mHisTagMode = null;
                    }
                    else
                    {
                        mHisTagMode = new Cdy.Tag.HisTag() { Id = this.mRealTagMode.Id,TagType=mRealTagMode.Type };
                    }
                    CheckRecordTypeParameterModel();
                    IsChanged = true;
                }
                OnPropertyChanged("RecordType");
                OnPropertyChanged("CompressCircle");
                OnPropertyChanged("CompressType");
                OnPropertyChanged("HasHisTag");
                OnPropertyChanged("IsTimerRecord");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int RecordType
        {
            get
            {
                return mHisTagMode != null ? (int)mHisTagMode.Type : -1;
            }
            set
            {
                if (mHisTagMode != null && (int)mHisTagMode.Type != value)
                {
                    mHisTagMode.Type = (Cdy.Tag.RecordType)value;
                    IsChanged = true;
                    OnPropertyChanged("RecordType");
                    OnPropertyChanged("RecordTypeString");
                    OnPropertyChanged("IsTimerRecord");
                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTimerRecord
        {
            get
            {
                return RecordType == (int)(Cdy.Tag.RecordType.Timer);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string RecordTypeString
        {
            get
            {
                return mHisTagMode!=null?mHisTagMode.Type.ToString():string.Empty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int CompressType
        {
            get
            {
                return mHisTagMode != null ? mHisTagMode.CompressType : -1;
            }
            set
            {
                if (mHisTagMode != null && mHisTagMode.CompressType != value)
                {
                    mHisTagMode.CompressType = value;
                    IsChanged = true;
                    CheckRecordTypeParameterModel();

                    OnPropertyChanged("CompressType");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long CompressCircle
        {
            get
            {
                return mHisTagMode != null ? mHisTagMode.Circle : -1;
            }
            set
            {
                if (mHisTagMode != null && mHisTagMode.Circle != value)
                {
                    mHisTagMode.Circle = value;
                    IsChanged = true;
                    OnPropertyChanged("CompressCircle");
                }
            }
        }


        #endregion ...Properties....

        #region ... Methods    ...

        /// <summary>
        /// 
        /// </summary>
        private void CheckLinkAddress()
        {
            if (string.IsNullOrEmpty(LinkAddress))
            {
                return;
            }
            else
            {
                string[] str = LinkAddress.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                mDriverName = str[0];
                if(str.Length>1)
                {
                    mRegistorName = LinkAddress.Substring(mDriverName.Length+1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitEnumType()
        {
            mTagTypeList = Enum.GetNames(typeof(Cdy.Tag.TagType));
            mRecordTypeList = Enum.GetNames(typeof(Cdy.Tag.RecordType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagType"></param>
        private void ChangeTagType(Cdy.Tag.TagType tagType)
        {
            Cdy.Tag.Tagbase ntag = null;
            if (mHisTagMode == null) return;
            switch (tagType)
            {
                case Cdy.Tag.TagType.Bool:
                    ntag = new Cdy.Tag.BoolTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Bool;
                    }
                    break;
                case Cdy.Tag.TagType.Byte:
                    ntag = new Cdy.Tag.ByteTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Byte;
                    }
                    break;
                case Cdy.Tag.TagType.DateTime:
                    ntag = new Cdy.Tag.DateTimeTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.DateTime;
                    }
                    break;
                case Cdy.Tag.TagType.Double:
                    ntag = new Cdy.Tag.DoubleTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Double;
                    }
                    break;
                case Cdy.Tag.TagType.Float:
                    ntag = new Cdy.Tag.FloatTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Float;
                    }
                    break;
                case Cdy.Tag.TagType.Int:
                    ntag = new Cdy.Tag.IntTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Int;
                    }
                    break;
                case Cdy.Tag.TagType.Long:
                    ntag = new Cdy.Tag.LongTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Long;
                    }
                    break;
                case Cdy.Tag.TagType.Short:
                    ntag = new Cdy.Tag.ShortTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.Short;
                    }
                    break;
                case Cdy.Tag.TagType.String:
                    ntag = new Cdy.Tag.StringTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.String;
                    }
                    break;
                case Cdy.Tag.TagType.UInt:
                    ntag = new Cdy.Tag.UIntTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.UInt;
                    }
                    break;
                case Cdy.Tag.TagType.ULong:
                    ntag = new Cdy.Tag.ULongTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.ULong;
                    }
                    break;
                case Cdy.Tag.TagType.UShort:
                    ntag = new Cdy.Tag.UShortTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    if (mHisTagMode != null)
                    {
                        mHisTagMode.TagType = Cdy.Tag.TagType.UShort;
                    }
                    break;
                default:
                    break;
            }
            if (ntag != null)
            {
                RealTagMode = ntag;
            }
            IsChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckRecordTypeParameterModel()
        {
            if(mHisTagMode==null)
            {
                CompressParameterModel = null;
                return;
            }
            switch (CompressType)
            {
                case 0:
                case 1:
                    CompressParameterModel = null;
                    break;
                case 2:
                    CompressParameterModel = new DeadAreaCompressParameterViewModel() { Parameters = HisTagMode.Parameters };
                    break;
                case 3:
                    CompressParameterModel = new SlopeCompressParameterViewModel() { Parameters = HisTagMode.Parameters };
                    break;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TagViewModel Clone()
        {
            Cdy.Tag.Tagbase ntag = null;
            Cdy.Tag.HisTag htag = null;
            if(mHisTagMode != null)
            {
                htag = new Cdy.Tag.HisTag() { Id = mHisTagMode.Id, Circle = mHisTagMode.Circle, CompressType = mHisTagMode.CompressType, TagType = mHisTagMode.TagType, Type = mHisTagMode.Type };

                if (this.mHisTagMode.Parameters != null)
                {
                    htag.Parameters = new Dictionary<string, double>();
                    foreach(var vv in mHisTagMode.Parameters)
                    {
                        htag.Parameters.Add(vv.Key, vv.Value);
                    }
                }
            }

            switch (this.mRealTagMode.Type)
            {
                case Cdy.Tag.TagType.Bool:
                    ntag = new Cdy.Tag.BoolTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };
                    break;
                case Cdy.Tag.TagType.Byte:
                    ntag = new Cdy.Tag.ByteTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.DateTime:
                    ntag = new Cdy.Tag.DateTimeTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.Double:
                    ntag = new Cdy.Tag.DoubleTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.Float:
                    ntag = new Cdy.Tag.FloatTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.Int:
                    ntag = new Cdy.Tag.IntTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.Long:
                    ntag = new Cdy.Tag.LongTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.Short:
                    ntag = new Cdy.Tag.ShortTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.String:
                    ntag = new Cdy.Tag.StringTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.UInt:
                    ntag = new Cdy.Tag.UIntTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.ULong:
                    ntag = new Cdy.Tag.ULongTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                case Cdy.Tag.TagType.UShort:
                    ntag = new Cdy.Tag.UShortTag() { Id = this.mRealTagMode.Id, Name = mRealTagMode.Name, Desc = mRealTagMode.Desc, LinkAddress = mRealTagMode.LinkAddress, Group = mRealTagMode.Group };

                    break;
                default:
                    break;
            }
            return new TagViewModel(ntag,htag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SaveToCSVString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(mRealTagMode.Id+",");
            sb.Append(mRealTagMode.Name + ",");
            sb.Append(mRealTagMode.Desc + ",");
            sb.Append(mRealTagMode.Group + ",");
            sb.Append(mRealTagMode.Type + ",");
            sb.Append(mRealTagMode.LinkAddress + ",");
            if(this.mHisTagMode!=null)
            {
                sb.Append(mHisTagMode.Type + ",");
                sb.Append(mHisTagMode.Circle + ",");
                sb.Append(mHisTagMode.CompressType + ",");
                if(mHisTagMode.Parameters!=null)
                {
                    foreach(var vv in mHisTagMode.Parameters)
                    {
                        sb.Append(vv.Key + ",");
                        sb.Append(vv.Value + ",");
                    }
                }
            }
            sb.Length = sb.Length > 0 ? sb.Length - 1 : sb.Length;
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public static TagViewModel LoadFromCSVString(string val)
        {
            string[] stmp = val.Split(new char[] { ',' });
            Cdy.Tag.TagType tp = (Cdy.Tag.TagType)Enum.Parse(typeof(Cdy.Tag.TagType),stmp[4]);
            var realtag = TagTypeExtends.GetTag(tp);

            realtag.Id = int.Parse(stmp[0]);
            realtag.Name = stmp[1];
            realtag.Desc = stmp[2];
            realtag.Group = stmp[3];
            realtag.LinkAddress = stmp[5];

            if (stmp.Length > 6)
            {
                Cdy.Tag.HisTag histag = new HisTag();
                histag.Type = (Cdy.Tag.RecordType)Enum.Parse(typeof(Cdy.Tag.RecordType), stmp[6]);

                histag.Circle = long.Parse(stmp[7]);
                histag.CompressType = int.Parse(stmp[8]);
                histag.Parameters = new Dictionary<string, double>();
                histag.TagType = realtag.Type;
                histag.Id = realtag.Id;

                for (int i=9;i<stmp.Length;i++)
                {
                    string skey = stmp[i];
                    if(string.IsNullOrEmpty(skey))
                    {
                        break;
                    }
                    double dval = double.Parse(stmp[i + 1]);

                    if(!histag.Parameters.ContainsKey(skey))
                    {
                        histag.Parameters.Add(skey, dval);
                    }

                    i++;
                }
                return new TagViewModel(realtag, histag);
            }

            return new TagViewModel(realtag, null);


        }

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }

    /// <summary>
    /// 
    /// </summary>
    public class TagGroupViewModel : HasChildrenTreeItemViewModel
    {

        #region ... Variables  ...

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...

        #endregion ...Constructor...

        #region ... Properties ...

        public override string FullName => (Parent is TagGroupViewModel)?(Parent as TagGroupViewModel).FullName+"."+Name:Name;


        #endregion ...Properties...

        #region ... Methods    ...

        public override bool CanRemove()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Remove()
        {
            string sname = this.FullName;
            if (DevelopServiceHelper.Helper.RemoveTagGroup(Database, sname))
            {
                if (Parent != null &&  Parent is HasChildrenTreeItemViewModel)
                {
                    (Parent as HasChildrenTreeItemViewModel).Children.Remove(this);
                    Parent = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public override bool OnRename(string oldName, string newName)
        {
            return DevelopServiceHelper.Helper.ReNameTagGroup(Database,this.FullName, newName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        public void InitData(Dictionary<string,string> groups)
        {
            foreach(var vv in groups.Where(e=>e.Value==this.FullName))
            {
                TagGroupViewModel groupViewModel = new TagGroupViewModel() { Name = vv.Key,Database=Database };
                groupViewModel.Parent = this;
                this.Children.Add(groupViewModel);
            }
        }

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }

    public class RootTagGroupViewModel: TagGroupViewModel
    {

        #region ... Variables  ...
        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...
        /// <summary>
        /// 
        /// </summary>
        public RootTagGroupViewModel()
        {
            Name = Res.Get("Tag");
        }
        #endregion ...Constructor...

        #region ... Properties ...
        /// <summary>
        /// 
        /// </summary>
        public override string FullName => string.Empty;
        #endregion ...Properties...

        #region ... Methods    ...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public override bool OnRename(string oldName, string newName)
        {
            return true;
        }
        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }

    public class DatabaseViewModel : HasChildrenTreeItemViewModel
    {

        #region ... Variables  ...

        #endregion ...Variables...

        #region ... Events     ...

        #endregion ...Events...

        #region ... Constructor...

        #endregion ...Constructor...

        #region ... Properties ...
        
        #endregion ...Properties...

        #region ... Methods    ...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public override bool OnRename(string oldName, string newName)
        {
            return base.OnRename(oldName, newName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CanAddChild()
        {
            return false;
        }
        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }


}

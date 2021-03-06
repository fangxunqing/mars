﻿//==============================================================
//  Copyright (C) 2020  Inc. All rights reserved.
//
//==============================================================
//  Create by 种道洋 at 2020/3/28 22:49:54.
//  Version 1.0
//  种道洋
//==============================================================

using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace DBDevelopClientApi
{
    /// <summary>
    /// 
    /// </summary>
    public class DevelopServiceHelper
    {

        #region ... Variables  ...
        
        public static DevelopServiceHelper Helper = new DevelopServiceHelper();

        private DBDevelopService.DevelopServer.DevelopServerClient mCurrentClient;

        private string mLoginId = string.Empty;

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
        /// <param name="ip"></param>
        private DBDevelopService.DevelopServer.DevelopServerClient GetServicClient(string ip)
        {
            try
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var httpClient = new HttpClient(httpClientHandler);

                Grpc.Net.Client.GrpcChannel grpcChannel = Grpc.Net.Client.GrpcChannel.ForAddress(@"https://" + ip + ":5001", new GrpcChannelOptions { HttpClient = httpClient });
                return new DBDevelopService.DevelopServer.DevelopServerClient(grpcChannel);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public bool Save(string database)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.Save(new DBDevelopService.GetRequest() { Database = database, LoginId = mLoginId}).Result;
            }
            return false;
        }

        /// <summary>
        /// 放弃更改
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public bool Cancel(string database)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.Cancel(new DBDevelopService.GetRequest() { Database = database, LoginId = mLoginId }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string Login(string ip,string user,string pass)
        {
            mCurrentClient = GetServicClient(ip);
            if (mCurrentClient != null)
            {
                try
                {
                    var lres = mCurrentClient.Login(new DBDevelopService.LoginRequest() { UserName = user, Password = pass });            //var sid = await client.LoginAsync(new DBDevelopService.LoginRequest() { UserName = "admin", Password = "12345", Database = "local" });
                    if (lres != null)
                    {
                        mLoginId = lres.LoginId;
                        return lres.LoginId;
                    }
                }
                catch
                {
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> ListDatabase()
        {
            var re = new Dictionary<string, string>();
            try
            {
                if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
                {
                    var vv = mCurrentClient.QueryDatabase(new DBDevelopService.QueryDatabaseRequest() { LoginId = mLoginId }).Database.ToList();
                    foreach(var vvv in vv)
                    {
                        re.Add(vvv.Key, vvv.Value);
                    }
                }
            }
            catch
            {

            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public bool NewDatabase(string name,string desc)
        {
            if(mCurrentClient!=null&&!string.IsNullOrEmpty(mLoginId))
            {
               return mCurrentClient.NewDatabase(new DBDevelopService.NewDatabaseRequest() { Database = name, LoginId = mLoginId,Desc=string.IsNullOrEmpty(desc)?"":desc }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> QueryTagGroups(string database)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                foreach(var vv in mCurrentClient.GetTagGroup(new DBDevelopService.GetRequest() { Database = database, LoginId = mLoginId }).Group)
                {
                    re.Add(vv.Name, vv.Parent);
                }
            }
            return re;
        }


        #region Develop User

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool AddUser(string name, string password)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.NewUser(new DBDevelopService.NewUserRequest() {  LoginId = mLoginId, UserName = name, Password = password }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveUser(string name)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RemoveUser(new DBDevelopService.RemoveUserRequest() { LoginId = mLoginId, UserName = name }).Result;
            }
            return false;
        }

        public bool UpdateUser(string name,List<string> permissions)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var req = new DBDevelopService.UpdateUserRequest() { LoginId = mLoginId, UserName = name };
                req.Permission.AddRange(permissions);
                return mCurrentClient.UpdateUser(req).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool SetUserPassword(string name,string password)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.ModifyPassword(new DBDevelopService.ModifyPasswordRequest() { LoginId = mLoginId, UserName = name,Password=password }).Result;
            }
            return false;
        }

        
        #endregion

        #region DatabaseUser

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public Dictionary<string, string> QueryDatabaseUserGroups(string database)
        {
            Dictionary<string, string> re = new Dictionary<string, string>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                foreach (var vv in mCurrentClient.GetDatabaseUserGroup(new DBDevelopService.GetRequest() { Database = database, LoginId = mLoginId }).Group)
                {
                    re.Add(vv.Name, vv.Parent);
                }
            }
            return re;
        }

        public List<string> GetAllUserNames(string database)
        {
            List<string> re = new List<string>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var groups = QueryDatabaseUserGroups(database).Keys.ToList();
                groups.Add("");
                foreach (var vvg in groups)
                {
                    foreach (var vv in mCurrentClient.GetDatabaseUserByGroup(new DBDevelopService.GetDatabaseUserByGroupRequest() { Database = database, LoginId = mLoginId, Group = vvg }).Users)
                    {
                        //Cdy.Tag.UserItem user = new Cdy.Tag.UserItem() { Name = vv.UserName, Group = vv.Group };
                        //user.Permissions.AddRange(vv.Permission.ToArray());
                        re.Add(vv.UserName);
                    }
                }
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<Cdy.Tag.UserItem> GetUsersByGroup(string database,string group)
        {
            List<Cdy.Tag.UserItem> re = new List<Cdy.Tag.UserItem>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                foreach (var vv in mCurrentClient.GetDatabaseUserByGroup(new DBDevelopService.GetDatabaseUserByGroupRequest() { Database = database, LoginId = mLoginId,Group=group }).Users)
                {
                    Cdy.Tag.UserItem user = new Cdy.Tag.UserItem() { Name = vv.UserName, Group = vv.Group };
                    user.Permissions.AddRange(vv.Permission.ToArray());
                    re.Add(user);
                }
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public List<Cdy.Tag.PermissionItem> GetAllDatabasePermission(string database)
        {
            List<Cdy.Tag.PermissionItem> re = new List<Cdy.Tag.PermissionItem>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                foreach (var vv in mCurrentClient.GetAllDatabasePermission(new DBDevelopService.GetAllDatabasePermissionRequest() { Database = database, LoginId = mLoginId }).Permission)
                {
                    Cdy.Tag.PermissionItem user = new Cdy.Tag.PermissionItem() { Name = vv.Name, Group = vv.Group.ToList(),Desc=vv.Desc,EnableWrite=vv.EnableWrite,SuperPermission=vv.SuperPermission };
                    re.Add(user);
                }
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveDatabasePermission(string database,string name)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RemoveDatabasePermission(new DBDevelopService.RemoveDatabasePermissionRequest() { Database = database, LoginId = mLoginId, Permission = name }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool UpdateDatabasePermission(string database,Cdy.Tag.PermissionItem permission)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var req = new DBDevelopService.DatabasePermissionRequest();
                req.Database = database;
                req.LoginId = mLoginId;
                req.Permission = new DBDevelopService.DatabasePermission() { Name = permission.Name, Desc = permission.Desc, EnableWrite = permission.EnableWrite };
                if(permission.Group!=null)
                req.Permission.Group.Add(permission.Group);
                return mCurrentClient.UpdateDatabasePermission(req).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <param name="parentName"></param>
        public bool AddDatabaseUserGroup(string database, string name, string parentName)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.AddDatabaseUserGroup(new DBDevelopService.AddGroupRequest() { Database = database, LoginId = mLoginId, Name = name, ParentName = parentName }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool ReNameDatabaseUserGroup(string database, string oldName, string newName)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RenameDatabaseUserGroup(new DBDevelopService.RenameGroupRequest() { Database = database, LoginId = mLoginId, OldFullName = oldName, NewName = newName }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveDatabaseUserGroup(string database, string name)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RemoveDatabaseUserGroup(new DBDevelopService.RemoveGroupRequest() { Database = database, LoginId = mLoginId, Name = name }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateDatabaseUser(string database,Cdy.Tag.UserItem user)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var req = new DBDevelopService.UpdateDatabaseUserRequest() { Database = database, LoginId = mLoginId, UserName = user.Name, Group = user.Group };
                if(user.Permissions!=null)
                req.Permission.AddRange(user.Permissions);
                return mCurrentClient.UpdateDatabaseUser(req).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UpdateDatabaseUserPassword(String database ,string user,string password)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var req = new DBDevelopService.ModifyDatabaseUserPasswordRequest() { Database = database, LoginId = mLoginId, UserName = user, Password=password };
                return mCurrentClient.ModifyDatabaseUserPassword(req).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveDatabaseUser(string database, string name)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RemoveDatabaseUser(new DBDevelopService.RemoveByNameRequest() { Database = database, LoginId = mLoginId, Name = name }).Result;
            }
            return false;
        }

        #endregion


        #region database permission

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>> QueryAllTag(string database)
        {
            Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>> re = new Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                int idx = 0;
                int count = 0;
                do
                {
                    var result = mCurrentClient.GetAllTag(new DBDevelopService.GetTagByGroupRequest() { Database = database, LoginId = mLoginId });

                    idx = result.Index;
                    count = result.Count;

                    if (!result.Result) break;

                    Dictionary<int, Cdy.Tag.Tagbase> mRealTag = new Dictionary<int, Cdy.Tag.Tagbase>();
                    foreach (var vv in result.RealTag)
                    {
                        var tag = GetTag((int)vv.TagType);
                        tag.Id = (int)vv.Id;
                        tag.LinkAddress = vv.LinkAddress;
                        tag.Name = vv.Name;
                        tag.Desc = vv.Desc;
                        tag.Group = vv.Group;
                        mRealTag.Add(tag.Id, tag);
                    }

                    Dictionary<int, Cdy.Tag.HisTag> mHisTag = new Dictionary<int, Cdy.Tag.HisTag>();
                    foreach (var vv in result.HisTag)
                    {
                        var tag = new Cdy.Tag.HisTag { Id = (int)vv.Id, TagType = (Cdy.Tag.TagType)vv.TagType, Type = (Cdy.Tag.RecordType)vv.Type, CompressType = (int)vv.CompressType };
                        if (vv.Parameter.Count > 0)
                        {
                            tag.Parameters = new Dictionary<string, double>();
                            foreach (var vvv in vv.Parameter)
                            {
                                tag.Parameters.Add(vvv.Name, vvv.Value);
                            }

                        }
                        mHisTag.Add(tag.Id, tag);
                    }

                    foreach (var vv in mRealTag)
                    {
                        if (mHisTag.ContainsKey(vv.Key))
                        {
                            re.Add(vv.Key, new Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>(mRealTag[vv.Key], mHisTag[vv.Key]));
                        }
                        else
                        {
                            re.Add(vv.Key, new Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>(mRealTag[vv.Key], null));
                        }
                    }

                    idx++;
                }
                while (idx < count);
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="group"></param>
        /// <param name="totalCount"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>> QueryTagByGroup(string database, string group, int index, out int totalCount)
        {
            Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>> re = new Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                int idx = index;
                var result = mCurrentClient.GetTagByGroup(new DBDevelopService.GetTagByGroupRequest() { Database = database, LoginId = mLoginId, Group = group, Index = idx });

                totalCount = result.Count;

                if (result.Result)
                {
                    Dictionary<int, Cdy.Tag.Tagbase> mRealTag = new Dictionary<int, Cdy.Tag.Tagbase>();
                    foreach (var vv in result.RealTag)
                    {
                        var tag = GetTag((int)vv.TagType);
                        tag.Id = (int)vv.Id;
                        tag.LinkAddress = vv.LinkAddress;
                        tag.Name = vv.Name;
                        tag.Desc = vv.Desc;
                        tag.Group = vv.Group;
                        mRealTag.Add(tag.Id, tag);
                    }

                    Dictionary<int, Cdy.Tag.HisTag> mHisTag = new Dictionary<int, Cdy.Tag.HisTag>();
                    foreach (var vv in result.HisTag)
                    {
                        var tag = new Cdy.Tag.HisTag { Id = (int)vv.Id, TagType = (Cdy.Tag.TagType)vv.TagType, Type = (Cdy.Tag.RecordType)vv.Type, CompressType = (int)vv.CompressType };
                        if (vv.Parameter.Count > 0)
                        {
                            tag.Parameters = new Dictionary<string, double>();
                            foreach (var vvv in vv.Parameter)
                            {
                                tag.Parameters.Add(vvv.Name, vvv.Value);
                            }

                        }
                        mHisTag.Add(tag.Id, tag);
                    }

                    foreach (var vv in mRealTag)
                    {
                        if (mHisTag.ContainsKey(vv.Key))
                        {
                            re.Add(vv.Key, new Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>(mRealTag[vv.Key], mHisTag[vv.Key]));
                        }
                        else
                        {
                            re.Add(vv.Key, new Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>(mRealTag[vv.Key], null));
                        }
                    }
                }
            }
            else
            {
                totalCount = -1;
            }
            
            return re;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public Dictionary<int,Tuple<Cdy.Tag.Tagbase,Cdy.Tag.HisTag>> QueryTagByGroup(string database,string group)
        {
            Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>> re = new Dictionary<int, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                int idx = 0;
                int count = 0;
                do
                {
                    var result = mCurrentClient.GetTagByGroup(new DBDevelopService.GetTagByGroupRequest() { Database = database, LoginId = mLoginId, Group = group,Index=idx });

                    idx = result.Index;
                    count = result.Count;

                    if (!result.Result) break;

                    Dictionary<int, Cdy.Tag.Tagbase> mRealTag = new Dictionary<int, Cdy.Tag.Tagbase>();
                    foreach (var vv in result.RealTag)
                    {
                        var tag = GetTag((int)vv.TagType);
                        tag.Id = (int)vv.Id;
                        tag.LinkAddress = vv.LinkAddress;
                        tag.Name = vv.Name;
                        tag.Desc = vv.Desc;
                        tag.Group = vv.Group;
                        mRealTag.Add(tag.Id, tag);
                    }

                    Dictionary<int, Cdy.Tag.HisTag> mHisTag = new Dictionary<int, Cdy.Tag.HisTag>();
                    foreach (var vv in result.HisTag)
                    {
                        var tag = new Cdy.Tag.HisTag { Id = (int)vv.Id, TagType = (Cdy.Tag.TagType)vv.TagType, Type = (Cdy.Tag.RecordType)vv.Type, CompressType = (int)vv.CompressType };
                        if (vv.Parameter.Count > 0)
                        {
                            tag.Parameters = new Dictionary<string, double>();
                            foreach (var vvv in vv.Parameter)
                            {
                                tag.Parameters.Add(vvv.Name, vvv.Value);
                            }

                        }
                        mHisTag.Add(tag.Id, tag);
                    }

                    foreach (var vv in mRealTag)
                    {
                        if (mHisTag.ContainsKey(vv.Key))
                        {
                            re.Add(vv.Key, new Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>(mRealTag[vv.Key], mHisTag[vv.Key]));
                        }
                        else
                        {
                            re.Add(vv.Key, new Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag>(mRealTag[vv.Key], null));
                        }
                    }

                    idx++;
                }
                while (idx < count);
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool UpdateTag(string database,Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag> tag)
        {
            bool re = true;
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                re &= mCurrentClient.UpdateRealTag(new DBDevelopService.UpdateRealTagRequestMessage() { Database = database, LoginId = mLoginId, Tag = ConvertToRealTagMessage(tag.Item1) }).Result;
                if (tag.Item2 != null)
                {
                    re &= mCurrentClient.UpdateHisTag(new DBDevelopService.UpdateHisTagRequestMessage() { Database = database, LoginId = mLoginId, Tag = ConvertToHisTagMessage(tag.Item2) }).Result;
                }
                else
                {
                    var msg = new DBDevelopService.RemoveTagMessage() { Database = database, LoginId = mLoginId };
                    msg.TagId.Add(tag.Item1.Id);
                    re &= mCurrentClient.RemoveHisTag(msg).Result;
                }
            }
            else
            {
                re = false;
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool AddTag(string database, Tuple<Cdy.Tag.Tagbase, Cdy.Tag.HisTag> tag,out int id)
        {
            bool re = false;
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var res = mCurrentClient.AddTag(new DBDevelopService.AddTagRequestMessage() { Database = database, LoginId = mLoginId, RealTag = ConvertToRealTagMessage(tag.Item1),HisTag=ConvertToHisTagMessage(tag.Item2) });
                re = res.Result;
                id = res.TagId;
            }
            id = -1;
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <param name="parentName"></param>
        public bool AddTagGroup(string database,string name,string parentName)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.AddTagGroup(new DBDevelopService.AddGroupRequest() { Database = database, LoginId = mLoginId, Name = name, ParentName = parentName }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool ReNameTagGroup(string database,string oldName,string newName)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RenameTagGroup(new DBDevelopService.RenameGroupRequest() { Database = database, LoginId = mLoginId,OldFullName=oldName,NewName=newName }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveTagGroup(string database,string name)
        {
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                return mCurrentClient.RemoveTagGroup(new DBDevelopService.RemoveGroupRequest() { Database = database, LoginId = mLoginId, Name = name }).Result;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(string database, int id)
        {
            var msg = new DBDevelopService.RemoveTagMessage() { Database = database, LoginId = mLoginId };
            msg.TagId.Add(id);
            return mCurrentClient.RemoveTag(msg).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private DBDevelopService.RealTagMessage ConvertToRealTagMessage(Cdy.Tag.Tagbase tag)
        {
            DBDevelopService.RealTagMessage re = new DBDevelopService.RealTagMessage();
            re.Id = (uint)tag.Id;
            re.LinkAddress = tag.LinkAddress;
            re.Name = tag.Name;
            re.TagType = (uint)tag.Type;
            re.Group = tag.Group;
            re.Desc = tag.Desc;
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private DBDevelopService.HisTagMessage ConvertToHisTagMessage(Cdy.Tag.HisTag tag)
        {
            DBDevelopService.HisTagMessage re = new DBDevelopService.HisTagMessage();
            if (tag != null)
            {
                re.Id = (uint)tag.Id;
                re.TagType = (uint)tag.TagType;
                re.Type = (uint)tag.Type;
                re.CompressType = (uint)tag.CompressType;
                re.Circle = (ulong)tag.Circle;
                if (tag.Parameters != null)
                {
                    foreach (var vv in tag.Parameters)
                    {
                        re.Parameter.Add(new DBDevelopService.hisTagParameterItem() { Name = vv.Key, Value = vv.Value });
                    }
                }
            }
            else
            {
                re.Id = uint.MaxValue;
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public Dictionary<string,string[]> GetRegistorDrivers(string database)
        {
            Dictionary<string, string[]> re = new Dictionary<string, string[]>();
            if (mCurrentClient != null && !string.IsNullOrEmpty(mLoginId))
            {
                var vv = mCurrentClient.GetRegisteDrivers(new DBDevelopService.GetRequest() { Database = database, LoginId = mLoginId });
                if(vv.Result&&vv.Drivers.Count>0)
                {
                    foreach(var vvv in vv.Drivers)
                    {
                        re.Add(vvv.Name, vvv.Registors.ToArray());
                    }
                }
            }
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagType"></param>
        /// <returns></returns>
        private Cdy.Tag.Tagbase GetTag(int tagType)
        {
            switch(tagType)
            {
                case (int)Cdy.Tag.TagType.Bool:
                    return new Cdy.Tag.BoolTag();
                case (int)Cdy.Tag.TagType.Byte:
                    return new Cdy.Tag.ByteTag();
                case (int)Cdy.Tag.TagType.DateTime:
                    return new Cdy.Tag.DateTimeTag();
                case (int)Cdy.Tag.TagType.Double:
                    return new Cdy.Tag.DoubleTag();
                case (int)Cdy.Tag.TagType.Float:
                    return new Cdy.Tag.FloatTag();
                case (int)Cdy.Tag.TagType.Int:
                    return new Cdy.Tag.IntTag();
                case (int)Cdy.Tag.TagType.Long:
                    return new Cdy.Tag.LongTag();
                case (int)Cdy.Tag.TagType.Short:
                    return new Cdy.Tag.ShortTag();
                case (int)Cdy.Tag.TagType.String:
                    return new Cdy.Tag.StringTag();
                case (int)Cdy.Tag.TagType.UInt:
                    return new Cdy.Tag.UIntTag();
                case (int)Cdy.Tag.TagType.ULong:
                    return new Cdy.Tag.ULongTag();
                case (int)Cdy.Tag.TagType.UShort:
                    return new Cdy.Tag.UShortTag();
            }
            return null;
        }

        #endregion ...Methods...

        #region ... Interfaces ...

        #endregion ...Interfaces...
    }
}

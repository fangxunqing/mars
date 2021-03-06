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
using System.Linq;
using System.Text;

namespace Cdy.Tag
{

    /// <summary>
    /// 
    /// </summary>
    public class LosslessCompressUnit : CompressUnitbase
    {
        protected MemoryBlock mMarshalMemory;

        protected VarintCodeMemory mVarintMemory;

        /// <summary>
        /// 
        /// </summary>
        public override string Desc => "无损压缩";

        /// <summary>
        /// 
        /// </summary>
        public override int TypeCode => 1;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override CompressUnitbase Clone()
        {
            return new LosslessCompressUnit();
        }

        #region Compress

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="target"></param>
        /// <param name="targetAddr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public override long Compress(MarshalMemoryBlock source, long sourceAddr, MarshalMemoryBlock target, long targetAddr, long size)
        {
            target.WriteDatetime(targetAddr, this.StartTime);
            switch (TagType)
            {
                case TagType.Bool:
                    return Compress<bool>(source, sourceAddr, target, targetAddr+8, size) + 8;
                case TagType.Byte:
                    return Compress<byte>(source, sourceAddr, target, targetAddr+8, size) + 8;
                case TagType.UShort:
                    return Compress<ushort>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.Short:
                    return Compress<short>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.UInt:
                    return Compress<uint>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.Int:
                    return Compress<int>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.ULong:
                    return Compress<ulong>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.Long:
                    return Compress<long>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.Double:
                    return Compress<double>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.Float:
                    return Compress<float>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.String:
                    return Compress<string>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.IntPoint:
                    return Compress<IntPointData>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.UIntPoint:
                    return Compress<UIntPointData>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.LongPoint:
                    return Compress<LongPointData>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.ULongPoint:
                    return Compress<ULongPointData>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.IntPoint3:
                    return Compress<IntPoint3Data>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.UIntPoint3:
                    return Compress<UIntPoint3Data>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.LongPoint3:
                    return Compress<LongPoint3Data>(source, sourceAddr, target, targetAddr + 8, size) + 8;
                case TagType.ULongPoint3:
                    return Compress<ULongPoint3Data>(source, sourceAddr, target, targetAddr + 8, size) + 8;
            }
            return 8;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerVals"></param>
        /// <param name="emptyIds"></param>
        /// <returns></returns>
        protected byte[] CompressTimers(List<ushort> timerVals, Queue<int> emptyIds)
        {
            int preids = 0;
            mVarintMemory.Position = 0;
            bool isFirst = true;
            for (int i = 0; i < timerVals.Count; i++)
            {
                if (timerVals[i] > 0||i==0)
                {
                    var id = timerVals[i];
                    if (isFirst)
                    {
                        mVarintMemory.WriteInt32(id);
                        isFirst = false;
                    }
                    else
                    {
                        mVarintMemory.WriteInt32(id - preids);
                    }
                    preids = id;
                }
                else
                {
                    emptyIds.Enqueue(i);
                }
            }
            return mVarintMemory.Buffer.AsSpan(0, mVarintMemory.Position).ToArray();
        }
                       
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="emptyIds"></param>
        /// <returns></returns>
        protected virtual Memory<byte> CompressValues<T>(MarshalMemoryBlock source,long offset,int count, Queue<int> emptyIds)
        {
            mMarshalMemory.Position = 0;
            mVarintMemory.Position = 0;
            int ig = -1;
            emptyIds.TryDequeue(out ig);
            bool isFirst = true;

            if (typeof(T) == typeof(byte))
            {
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadByte(offset + i);
                        mMarshalMemory.Write(id);
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
                return mMarshalMemory.StartMemory.AsMemory<byte>(0, (int)mMarshalMemory.Position);
            }
            else if (typeof(T) == typeof(short))
            {
                short sval = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadShort(offset + i * 2);
                        if (isFirst)
                        {
                            mVarintMemory.WriteSInt32(id);
                            isFirst = false;
                            sval = id;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32(id - sval);
                            sval = id;
                        }
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
                
            }
            else if (typeof(T) == typeof(ushort))
            {
                ushort sval = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadUShort(offset + i * 2);
                        if (isFirst)
                        {
                            mVarintMemory.WriteSInt32(id);
                            isFirst = false;
                            sval = id;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32(id - sval);
                            sval = id;
                        }
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
               
            }
            else if (typeof(T) == typeof(int))
            {
                int sval = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadInt(offset + i * 4);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt32(id);
                            isFirst = false;
                            sval = id;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32(id - sval);
                            sval = id;
                        }
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
               
            }
            else if (typeof(T) == typeof(uint))
            {
                uint sval = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadUInt(offset + i * 4);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt32(id);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32((int)(id - sval));
                        }
                        sval = id;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
            }
            else if (typeof(T) == typeof(long))
            {
                long sval = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadLong(offset + i * 8);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt64(id);
                            isFirst = false;
                            sval = id;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt64((id - sval));
                            sval = id;
                        }
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
                
            }
            else if (typeof(T) == typeof(ulong))
            {
                ulong sval = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadULong(offset + i * 8);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt64(id);
                            isFirst = false;
                            sval = id;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt64((long)(id - sval));
                            sval = id;
                        }
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
                
            }
            else if (typeof(T) == typeof(double))
            {
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadDouble(offset + i * 8);
                        mMarshalMemory.Write(id);
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
                return mMarshalMemory.StartMemory.AsMemory<byte>(0, (int)mMarshalMemory.Position);
            }
            else if (typeof(T) == typeof(float))
            {
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadFloat(offset + i * 4);
                        mMarshalMemory.Write(id);
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }
                return mMarshalMemory.StartMemory.AsMemory<byte>(0, (int)mMarshalMemory.Position);
            }
            else if (typeof(T) == typeof(IntPointData))
            {
                int sval = 0;
                int sval2 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadInt(offset + i * 8);
                        var id2 = source.ReadInt(offset + i * 8 + 4);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt32(id);
                            mVarintMemory.WriteInt32(id2);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32(id - sval);
                            mVarintMemory.WriteSInt32(id2 - sval2);
                        }
                        sval = id;
                        sval2 = id2;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(UIntPointData))
            {
                uint sval = 0;
                uint sval2 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadUInt(offset + i * 8);
                        var id2 = source.ReadUInt(offset + i * 8 + 4);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt32(id);
                            mVarintMemory.WriteInt32(id2);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32((int)(id - sval));
                            mVarintMemory.WriteSInt32((int)(id2 - sval2));
                        }
                        sval = id;
                        sval2 = id2;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(IntPoint3Data))
            {
                int sval = 0;
                int sval2 = 0;
                int sval3 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadInt(offset + i * 12);
                        var id2 = source.ReadInt(offset + i * 12 + 4);
                        var id3 = source.ReadInt(offset + i * 12 + 8);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt32(id);
                            mVarintMemory.WriteInt32(id2);
                            mVarintMemory.WriteInt32(id3);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32((int)(id - sval));
                            mVarintMemory.WriteSInt32((int)(id2 - sval2));
                            mVarintMemory.WriteSInt32((int)(id3 - sval3));
                        }
                        sval = id;
                        sval2 = id2;
                        sval3 = id3;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(UIntPoint3Data))
            {
                uint sval = 0;
                uint sval2 = 0;
                uint sval3 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadUInt(offset + i * 12);
                        var id2 = source.ReadUInt(offset + i * 12 + 4);
                        var id3 = source.ReadUInt(offset + i * 12 + 8);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt32(id);
                            mVarintMemory.WriteInt32(id2);
                            mVarintMemory.WriteInt32(id3);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt32((int)(id - sval));
                            mVarintMemory.WriteSInt32((int)(id2 - sval2));
                            mVarintMemory.WriteSInt32((int)(id3 - sval3));
                        }
                        sval = id;
                        sval2 = id2;
                        sval3 = id3;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(LongPointData))
            {
                long sval = 0;
                long sval2 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadLong(offset + i * 16);
                        var id2 = source.ReadLong(offset + i * 16 + 8);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt64(id);
                            mVarintMemory.WriteInt64(id2);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt64(id - sval);
                            mVarintMemory.WriteSInt64(id2 - sval2);
                        }
                        sval = id;
                        sval2 = id2;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(ULongPointData))
            {
                ulong sval = 0;
                ulong sval2 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadULong(offset + i * 16);
                        var id2 = source.ReadULong(offset + i * 16 + 8);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt64(id);
                            mVarintMemory.WriteInt64(id2);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt64((long)(id - sval));
                            mVarintMemory.WriteSInt64((long)(id2 - sval2));
                        }
                        sval = id;
                        sval2 = id2;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(LongPoint3Data))
            {
                long sval = 0;
                long sval2 = 0;
                long sval3 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadLong(offset + i * 24);
                        var id2 = source.ReadLong(offset + i * 24 + 8);
                        var id3 = source.ReadLong(offset + i * 24 + 16);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt64(id);
                            mVarintMemory.WriteInt64(id2);
                            mVarintMemory.WriteInt64(id3);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt64(id - sval);
                            mVarintMemory.WriteSInt64(id2 - sval2);
                            mVarintMemory.WriteSInt64(id3 - sval3);
                        }
                        sval = id;
                        sval2 = id2;
                        sval3 = id3;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }
            else if (typeof(T) == typeof(ULongPoint3Data))
            {
                ulong sval = 0;
                ulong sval2 = 0;
                ulong sval3 = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i != ig)
                    {
                        var id = source.ReadULong(offset + i * 24);
                        var id2 = source.ReadULong(offset + i * 24 + 8);
                        var id3 = source.ReadULong(offset + i * 24 + 16);
                        if (isFirst)
                        {
                            mVarintMemory.WriteInt64(id);
                            mVarintMemory.WriteInt64(id2);
                            mVarintMemory.WriteInt64(id3);
                            isFirst = false;
                        }
                        else
                        {
                            mVarintMemory.WriteSInt64((long)(id - sval));
                            mVarintMemory.WriteSInt64((long)(id2 - sval2));
                            mVarintMemory.WriteSInt64((long)(id3 - sval3));
                        }
                        sval = id;
                        sval2 = id2;
                        sval3 = id3;
                    }
                    else
                    {
                        if (emptyIds.Count > 0)
                            emptyIds.TryDequeue(out ig);
                    }
                }

            }

            return mVarintMemory.Buffer.AsMemory<byte>(0, (int)mVarintMemory.Position);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerVals"></param>
        /// <param name="emptyIds"></param>
        /// <returns></returns>
        protected Memory<byte> CompressValues(List<string> timerVals, Queue<int> emptyIds)
        {
            mMarshalMemory.Position = 0;
            int ig = -1;
            emptyIds.TryDequeue(out ig);
            for (int i = 0; i < timerVals.Count; i++)
            {
                if(i != ig)
                {
                    var id = timerVals[i];
                    mMarshalMemory.Write(id);
                }
                else
                {
                    if (emptyIds.Count > 0)
                        emptyIds.TryDequeue(out ig);
                }
            }
            return mMarshalMemory.StartMemory.AsMemory<byte>(0, (int)mMarshalMemory.Position);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="totalcount"></param>
        /// <param name="emptyIds"></param>
        /// <returns></returns>
        protected Memory<byte> CompressQulitys(MarshalMemoryBlock source, long offset, int totalcount, Queue<int> emptyIds)
        {
            int count = 1;
            byte qus = source.ReadByte(offset);
            //using (VarintCodeMemory memory = new VarintCodeMemory(qulitys.Length * 2))
            mVarintMemory.Position = 0;
            int ig = -1;
            emptyIds.TryDequeue(out ig);
            mVarintMemory.WriteInt32(qus);
            for (int i = 1; i < totalcount; i++)
            {
                if (i != ig)
                {
                    byte bval = source.ReadByte(offset + i);
                    if (bval == qus)
                    {
                        count++;
                    }
                    else
                    {
                        mVarintMemory.WriteInt32(count);
                        qus = bval;
                        mVarintMemory.WriteInt32(qus);
                        count = 1;
                    }
                }
                else
                {
                    if (emptyIds.Count > 0)
                        emptyIds.TryDequeue(out ig);
                }
            }
            mVarintMemory.WriteInt32(count);
            return mVarintMemory.Buffer.AsMemory(0, mVarintMemory.Position);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="qulitys"></param>
        /// <returns></returns>
        protected Memory<byte> CompressQulitys(byte[] qulitys, Queue<int> emptyIds)
        {
            int count = 1;
            byte qus = qulitys[0];
            //using (VarintCodeMemory memory = new VarintCodeMemory(qulitys.Length * 2))
            mVarintMemory.Position = 0;
            int ig = -1;
            emptyIds.TryDequeue(out ig);
            mVarintMemory.WriteInt32(qus);
            for (int i = 1; i < qulitys.Length; i++)
            {
                if (i != ig)
                {
                    if (qulitys[i] == qus)
                    {
                        count++;
                    }
                    else
                    {
                        mVarintMemory.WriteInt32(count);
                        qus = qulitys[i];
                        mVarintMemory.WriteInt32(qus);
                        count = 1;
                    }
                }
                else
                {
                    if (emptyIds.Count > 0)
                        emptyIds.TryDequeue(out ig);
                }
            }
            mVarintMemory.WriteInt32(count);
            return mVarintMemory.Buffer.AsMemory(0, mVarintMemory.Position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected Memory<byte> CompressBoolValues(MarshalMemoryBlock source, long offset, int totalcount, Queue<int> emptyIds)
        {
            List<short> re = new List<short>(totalcount);
            byte bval = source.ReadByte(offset);
            short scount = 1;
            int ig = -1;
            emptyIds.TryDequeue(out ig);

            short sval = (short)(bval << 15);
            for(int i=0;i< totalcount; i++)
            {
                if (i != ig)
                {
                    var btmp = source.ReadByte(offset + i);
                    if(btmp == bval)
                    {
                        scount++;
                    }
                    else
                    {
                        sval = (short)(sval | scount);
                        re.Add(sval);
                        scount = 1;
                        bval = btmp;
                        sval = (short)(bval << 15);
                    }
                }
                else
                {
                    if (emptyIds.Count > 0)
                        emptyIds.TryDequeue(out ig);
                }
            }
            sval = (short)(sval | scount);
            re.Add(sval);

            mMarshalMemory.Position = 0;
            foreach (var vv in re)
            {
                mMarshalMemory.Write(vv);
            }
            return mMarshalMemory.StartMemory.AsMemory<byte>(0, (int)mMarshalMemory.Position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="target"></param>
        /// <param name="targetAddr"></param>
        /// <param name="size"></param>
        protected virtual long Compress<T>(MarshalMemoryBlock source, long sourceAddr, MarshalMemoryBlock target, long targetAddr, long size)
        {
            var count = (int)(size - this.QulityOffset);
            var tims = source.ReadUShorts(sourceAddr, (int)count);

            if(mMarshalMemory==null)
            {
                mMarshalMemory = new MemoryBlock(count * 10);
            }

            if(mVarintMemory==null)
            {
                mVarintMemory = new VarintCodeMemory(count * 10);
            }

            Queue<int> emptys = new Queue<int>();
            var datas = CompressTimers(tims, emptys);

            var emptys2 = new Queue<int>(emptys);

            long rsize = 0;
            //byte[] qus = null;
            int rcount = count - emptys.Count;

            target.WriteUShort(targetAddr,(ushort)rcount);
            rsize += 2;
            target.Write((int)datas.Length);
            target.Write(datas);
            rsize += 4;
            rsize += datas.Length;

            if (typeof(T) == typeof(bool))
            {
                //var vals = source.ReadBytes(count * 2 + sourceAddr, (int)count);
                var cval = CompressBoolValues(source, count * 2 + sourceAddr, count, emptys);
                target.Write(cval.Length);
                target.Write(cval);
                rsize += 4;
                rsize += cval.Length;

                var cqus = CompressQulitys(source, count * 3 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(byte))
            {
                //var vals = source.ReadBytes(count * 2 + sourceAddr, (int)count);
               // qus = source.ReadBytes(count * 3 + sourceAddr, (int)count);

                var cval = CompressValues<byte>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(cval.Length);
                target.Write(cval);
                rsize += 4;
                rsize += cval.Length;

                var cqus = CompressQulitys(source, count * 3 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(short))
            {
                //var vals = source.ReadShorts(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 4 + sourceAddr, (int)count);

                var res = CompressValues<short>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 4 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(ushort))
            {
                //var vals = source.ReadUShorts(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 4 + sourceAddr, (int)count);

                var res = CompressValues<ushort>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 4 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(int))
            {
                //var vals = source.ReadInts(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 6 + sourceAddr, (int)count);

                var res = CompressValues<int>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 6 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(uint))
            {
                //var vals = source.ReadUInts(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 6 + sourceAddr, (int)count);

                var res = CompressValues<uint>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 6 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(long))
            {
                //var vals = source.ReadLongs(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 10 + sourceAddr, (int)count);

                var res = CompressValues<long>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 10 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(ulong))
            {
                //var vals = source.ReadULongs(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 10 + sourceAddr, (int)count);

                var res = CompressValues<ulong>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 10 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                //var vals = source.ReadULongs(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 10 + sourceAddr, (int)count);

                var res = CompressValues<ulong>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 10 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(double))
            {
                //var vals = source.ReadDoubles(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 10 + sourceAddr, (int)count);

                var res = CompressValues<double>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 10 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(float))
            {
                //var vals = source.ReadFloats(count * 2 + sourceAddr, (int)count);
                //qus = source.ReadBytes(count * 6 + sourceAddr, (int)count);

                var res = CompressValues<float>(source, count * 2 + sourceAddr, count, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(source, count * 6 + sourceAddr, count, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            else if (typeof(T) == typeof(string))
            {

                var vals = source.ReadStrings(count * 2 + sourceAddr, count);
                var qus = source.ReadBytes(count);
                var res = CompressValues(vals, emptys);
                target.Write(res.Length);
                target.Write(res);
                rsize += 4;
                rsize += res.Length;

                var cqus = CompressQulitys(qus, emptys2);
                target.Write(cqus.Length);
                target.Write(cqus);
                rsize += 4;
                rsize += cqus.Length;
            }
            return rsize;
        }
        #endregion

        #region Decompress

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerVals"></param>
        /// <param name="emptyIds"></param>
        /// <returns></returns>
        private List<ushort> DeCompressTimers(byte[] timerVals, int count)
        {
            List<ushort> re = new List<ushort>();
            using (VarintCodeMemory memory = new VarintCodeMemory(timerVals))
            {
                ushort sval = (ushort)memory.ReadInt32();
                re.Add(sval);
                ushort preval = sval;
                for (int i = 1; i < count; i++)
                {
                    var ss = (ushort)memory.ReadInt32();
                    var val = (ushort)(preval + ss);
                    re.Add(val);
                    preval = val;
                }
                return re;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        private List<byte> DeCompressQulity(byte[] values)
        {
            List<byte> re = new List<byte>();
            using (VarintCodeMemory memory = new VarintCodeMemory(values))
            {
                while(memory.Position<values.Length)
                {
                    byte sval = (byte)memory.ReadInt32(); //读取质量戳
                    int ival = memory.ReadInt32(); //读取质量戳重复次数
                    for(int i=0;i<ival;i++)
                    {
                        re.Add(sval);
                    }
                }
                return re;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memory"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private List<T> DeCompressValue<T>(byte[] value, int count)
        {
            if (typeof(T) == typeof(byte))
            {
                return value.ToList() as List<T>;
            }
            else if (typeof(T) == typeof(short))
            {
                List<short> re = new List<short>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (short)memory.ReadInt32();
                    re.Add(vv);
                    for (int i = 1; i < count; i++)
                    {
                        var vss = (short)memory.ReadSInt32();
                        re.Add((short)(vv + vss));
                        vv = vss;
                    }
                }
                return re as List<T>;

            }
            else if (typeof(T) == typeof(ushort))
            {
                List<ushort> re = new List<ushort>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (short)memory.ReadInt32();
                    re.Add((ushort)vv);
                    for (int i = 1; i < count; i++)
                    {
                        var vss = (short)memory.ReadSInt32();
                        re.Add((ushort)(vv + vss));
                        vv = vss;
                    }
                }
                return re as List<T>;

            }
            else if (typeof(T) == typeof(int))
            {
                List<int> re = new List<int>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (int)memory.ReadInt32();
                    re.Add(vv);
                    for (int i = 1; i < count; i++)
                    {
                        var vss = (int)memory.ReadSInt32();
                        re.Add((int)(vv + vss));
                        vv = vss;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(uint))
            {
                List<uint> re = new List<uint>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = memory.ReadInt32();
                    re.Add((uint)vv);
                    for (int i = 1; i < count; i++)
                    {
                        var vss = memory.ReadSInt32();
                        re.Add((uint)((uint)vv + vss));
                        vv = vss;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(long))
            {
                List<long> re = new List<long>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (long)memory.ReadInt64();
                    re.Add(vv);
                    for (int i = 1; i < count; i++)
                    {
                        var vss = (long)memory.ReadSInt64();
                        re.Add((long)(vv + vss));
                        vv = vss;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(ulong))
            {
                List<ulong> re = new List<ulong>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = memory.ReadInt64();
                    re.Add((ulong)vv);
                    for (int i = 1; i < count; i++)
                    {
                        var vss = memory.ReadSInt64();
                        re.Add((ulong)(vv + vss));
                        vv = vss;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                List<DateTime> re = new List<DateTime>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (ulong)memory.ReadInt64();
                    re.Add(MemoryHelper.ReadDateTime(BitConverter.GetBytes(vv)));
                    for (int i = 1; i < count; i++)
                    {
                        var vss = (ulong)memory.ReadSInt64();
                        re.Add(MemoryHelper.ReadDateTime(BitConverter.GetBytes((ulong)(vv + vss))));
                        vv = vss;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(double))
            {
                using (MemorySpan block = new MemorySpan(value))
                {
                    return block.ToDoubleList() as List<T>;
                }
            }
            else if (typeof(T) == typeof(float))
            {
                using (MemorySpan block = new MemorySpan(value))
                {
                    return block.ToFloatList() as List<T>;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                using (MemorySpan block = new MemorySpan(value))
                {
                    return block.ToStringList(Encoding.Unicode) as List<T>;
                }
            }
            else if (typeof(T) == typeof(bool))
            {
                using (MemorySpan block = new MemorySpan(value))
                {
                    List<bool> re = new List<bool>();
                    var rtmp = block.ToShortList();

                    foreach (var vv in rtmp)
                    {
                        bool bval = (vv >> 15) > 0;
                        short bcount = (short)(vv & 0x7FFF);
                        for (int i = 0; i < bcount; i++)
                        {
                            re.Add(bval);
                        }
                    }
                    return re as List<T>;
                }
            }
            else if (typeof(T) == typeof(IntPointData))
            {
                List<IntPointData> re = new List<IntPointData>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (int)memory.ReadInt32();
                    var vv2 = (int)memory.ReadInt32();
                    re.Add(new IntPointData(vv,vv2));
                    for (int i = 2; i < count-1; i=i+2)
                    {
                        var vss = (int)memory.ReadSInt32();
                        var vss2 = (int)memory.ReadSInt32();
                        re.Add(new IntPointData((int)(vv + vss), (int)(vv2 + vss2)));
                        vv = vss;
                        vv2 = vss2;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(UIntPointData))
            {
                List<UIntPointData> re = new List<UIntPointData>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (int)memory.ReadInt32();
                    var vv2 = (int)memory.ReadInt32();
                    re.Add(new UIntPointData((uint)vv, (uint)vv2));
                    for (int i = 2; i < count - 1; i = i + 2)
                    {
                        var vss = (int)memory.ReadSInt32();
                        var vss2 = (int)memory.ReadSInt32();
                        re.Add(new UIntPointData((uint)(vv + vss), (uint)(vv2 + vss2)));
                        vv = vss;
                        vv2 = vss2;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(LongPointData))
            {
                List<LongPointData> re = new List<LongPointData>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (long)memory.ReadInt64();
                    var vv2 = (long)memory.ReadInt64();
                    re.Add(new LongPointData(vv, vv2));
                    for (int i = 2; i < count - 1; i = i + 2)
                    {
                        var vss = memory.ReadSInt64();
                        var vss2 = memory.ReadSInt64();
                        re.Add(new LongPointData((vv + vss), (vv2 + vss2)));
                        vv = vss;
                        vv2 = vss2;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(ULongPointData))
            {
                List<ULongPointData> re = new List<ULongPointData>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = memory.ReadInt64();
                    var vv2 = memory.ReadInt64();
                    re.Add(new ULongPointData((ulong)vv, (ulong)vv2));
                    for (int i = 2; i < count - 1; i = i + 2)
                    {
                        var vss = memory.ReadSInt64();
                        var vss2 = memory.ReadSInt64();
                        re.Add(new ULongPointData((ulong)(vv + vss), (ulong)(vv2 + vss2)));
                        vv = vss;
                        vv2 = vss2;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(IntPoint3Data))
            {
                List<IntPoint3Data> re = new List<IntPoint3Data>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (int)memory.ReadInt32();
                    var vv2 = (int)memory.ReadInt32();
                    var vv3 = (int)memory.ReadInt32();
                    re.Add(new IntPoint3Data(vv, vv2,vv3));
                    for (int i = 3; i < count - 2; i = i + 3)
                    {
                        var vss = (int)memory.ReadSInt32();
                        var vss2 = (int)memory.ReadSInt32();
                        var vss3 = (int)memory.ReadSInt32();
                        re.Add(new IntPoint3Data((int)(vv + vss), (int)(vv2 + vss2), (int)(vv3 + vss3)));
                        vv = vss;
                        vv2 = vss2;
                        vv3 = vss3;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(UIntPoint3Data))
            {
                List<UIntPoint3Data> re = new List<UIntPoint3Data>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (int)memory.ReadInt32();
                    var vv2 = (int)memory.ReadInt32();
                    var vv3 = (int)memory.ReadInt32();
                    re.Add(new UIntPoint3Data((uint)vv, (uint)vv2, (uint)vv3));
                    for (int i = 3; i < count - 2; i = i + 3)
                    {
                        var vss = (int)memory.ReadSInt32();
                        var vss2 = (int)memory.ReadSInt32();
                        var vss3 = (int)memory.ReadSInt32();
                        re.Add(new UIntPoint3Data((uint)(vv + vss), (uint)(vv2 + vss2), (uint)(vv3 + vss3)));
                        vv = vss;
                        vv2 = vss2;
                        vv3 = vss3;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(LongPoint3Data))
            {
                List<LongPoint3Data> re = new List<LongPoint3Data>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (long)memory.ReadInt64();
                    var vv2 = (long)memory.ReadInt64();
                    var vv3 = (long)memory.ReadInt64();
                    re.Add(new LongPoint3Data((long)vv, (long)vv2, (long)vv3));
                    for (int i = 3; i < count - 2; i = i + 3)
                    {
                        var vss = memory.ReadInt64();
                        var vss2 = memory.ReadInt64();
                        var vss3 = memory.ReadInt64();
                        re.Add(new LongPoint3Data((long)(vv + vss), (long)(vv2 + vss2), (long)(vv3 + vss3)));
                        vv = vss;
                        vv2 = vss2;
                        vv3 = vss3;
                    }
                }
                return re as List<T>;
            }
            else if (typeof(T) == typeof(ULongPoint3Data))
            {
                List<ULongPoint3Data> re = new List<ULongPoint3Data>();
                using (VarintCodeMemory memory = new VarintCodeMemory(value))
                {
                    var vv = (long)memory.ReadInt64();
                    var vv2 = (long)memory.ReadInt64();
                    var vv3 = (long)memory.ReadInt64();
                    re.Add(new ULongPoint3Data((ulong)vv, (ulong)vv2, (ulong)vv3));
                    for (int i = 3; i < count - 2; i = i + 3)
                    {
                        var vss = memory.ReadInt64();
                        var vss2 = memory.ReadInt64();
                        var vss3 = memory.ReadInt64();
                        re.Add(new ULongPoint3Data((ulong)(vv + vss), (ulong)(vv2 + vss2), (ulong)(vv3 + vss3)));
                        vv = vss;
                        vv2 = vss2;
                        vv3 = vss3;
                    }
                }
                return re as List<T>;
            }
            return null;
        }

        #endregion

        protected Dictionary<int,DateTime> GetTimers(MarshalMemoryBlock source,int sourceAddr,DateTime startTime,DateTime endTime, int timeTick,out int valueCount)
        {
            DateTime sTime = source.ReadDateTime(sourceAddr);
            Dictionary<int, DateTime> re = new Dictionary<int, DateTime>();
            ushort count = source.ReadUShort();
            var datasize = source.ReadInt();
            byte[] datas = source.ReadBytes(datasize);
            var timers = DeCompressTimers(datas, count);

            for (int i = 0; i < timers.Count; i++)
            {
                var vtime = sTime.AddMilliseconds(timers[i] * timeTick);
                if (vtime >= startTime && vtime < endTime)
                    re.Add(i, vtime);
            }
            valueCount = count;
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="timeTick"></param>
        /// <param name="valueCount"></param>
        /// <returns></returns>
        protected Dictionary<int, DateTime> GetTimers(MarshalMemoryBlock source, int sourceAddr, int timeTick, out int valueCount)
        {
            DateTime sTime = source.ReadDateTime(sourceAddr);
            Dictionary<int, DateTime> re = new Dictionary<int, DateTime>();
            ushort count = source.ReadUShort();
            var datasize = source.ReadInt();
            byte[] datas = source.ReadBytes(datasize);
            var timers = DeCompressTimers(datas, count);

            for (int i = 0; i < timers.Count; i++)
            {
                re.Add(i, sTime.AddMilliseconds(timers[i] * timeTick));
            }
            valueCount = count;
            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckTypeIsPointData(Type type)
        {
            return type == typeof(IntPointData) || type == typeof(UIntPointData) || type == typeof(LongPointData) || type == typeof(ULongPointData) || type == typeof(IntPoint3Data) || type == typeof(UIntPoint3Data) || type == typeof(LongPoint3Data) || type == typeof(ULongPoint3Data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeTick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override int DeCompressAllValue<T>(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<T> result)
        {
            int count = 0;
            var timers = GetTimers(source, sourceAddr, startTime, endTime, timeTick, out count);

            var valuesize = source.ReadInt();
            var value = DeCompressValue<T>(source.ReadBytes(valuesize), count);

            var qusize = source.ReadInt();

            var qulityes = DeCompressQulity(source.ReadBytes(qusize));
            int resultCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (qulityes[i] < 100)
                {
                    result.Add<T>(value[i], timers[i], qulityes[i]);
                    resultCount++;
                }
            }
            return resultCount;

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="sourceAddr"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <param name="timeTick"></param>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<bool> result)
        //{
        //   return DeCompressAllValue<bool>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="sourceAddr"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <param name="timeTick"></param>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<byte> result)
        //{
        //    return DeCompressAllValue<byte>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<short> result)
        //{
        //    return DeCompressAllValue<short>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<ushort> result)
        //{
        //    return DeCompressAllValue<ushort>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<int> result)
        //{
        //    return DeCompressAllValue<int>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<uint> result)
        //{
        //    return DeCompressAllValue<uint>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<long> result)
        //{
        //    return DeCompressAllValue<long>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<ulong> result)
        //{
        //    return DeCompressAllValue<ulong>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<float> result)
        //{
        //    return DeCompressAllValue<float>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<double> result)
        //{
        //    return DeCompressAllValue<double>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<DateTime> result)
        //{
        //    return DeCompressAllValue<DateTime>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        //public override int DeCompressAllValue(MarshalMemoryBlock source, int sourceAddr, DateTime startTime, DateTime endTime, int timeTick, HisQueryResult<string> result)
        //{
        //    return DeCompressAllValue<string>(source, sourceAddr, startTime, endTime, timeTick, result);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="time"></param>
        /// <param name="timeTick"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override int DeCompressValue<T>(MarshalMemoryBlock source, int sourceAddr, List<DateTime> time, int timeTick, QueryValueMatchType type, HisQueryResult<T> result)
        {
            if (CheckTypeIsPointData(typeof(T)))
            {
                return DeCompressPointValue<T>(source, sourceAddr, time, timeTick, type, result);
            }

            int count = 0;
            var timers = GetTimers(source, sourceAddr + 8,timeTick, out count);

            var valuesize = source.ReadInt();
            var value = DeCompressValue<T>(source.ReadBytes(valuesize), count);

            var qusize = source.ReadInt();

            var qulityes = DeCompressQulity(source.ReadBytes(qusize));
            int resultCount = 0;

            int j = 0;

            foreach (var time1 in time)
            {
                for (int i = j; i < timers.Count - 1; i++)
                {
                    var skey = timers[i];

                    var snext = timers[i + 1];

                    if (time1 == skey)
                    {
                        var val = value[i];
                        result.Add(val, time1, qulityes[i]);
                        resultCount++;

                        break;
                    }
                    else if (time1 > skey && time1 < snext)
                    {
                        switch (type)
                        {
                            case QueryValueMatchType.Previous:
                                var val = value[i];
                                result.Add(val, time1, qulityes[i]);
                                resultCount++;
                                break;
                            case QueryValueMatchType.After:
                                val = value[i + 1];
                                result.Add(val, time1, qulityes[i+1]);
                                resultCount++;
                                break;
                            case QueryValueMatchType.Linear:
                                if (typeof(T) == typeof(bool)|| typeof(T) == typeof(string)|| typeof(T) == typeof(DateTime))
                                {
                                    var ppval = (time1 - skey).TotalMilliseconds;
                                    var ffval = (snext - time1).TotalMilliseconds;

                                    if (ppval < ffval)
                                    {
                                        val = value[i];
                                        result.Add(val, time1, qulityes[i]);
                                    }
                                    else
                                    {
                                        val = value[i + 1];
                                        result.Add(val, time1, qulityes[i + 1]);
                                    }
                                    resultCount++;
                                }
                                else
                                {
                                    if (qulityes[i] < 20 && qulityes[i + 1] < 20)
                                    {
                                        var pval1 = (time1 - skey).TotalMilliseconds;
                                        var tval1 = (snext - skey).TotalMilliseconds;
                                        var sval1 = value[i];
                                        var sval2 = value[i + 1];

                                        var val1 = pval1 / tval1 * (Convert.ToDouble(sval2) - Convert.ToDouble(sval1)) + Convert.ToDouble(sval1);
                                        
                                        result.Add((object)val1, time1, 0);
                                    }
                                    else if (qulityes[i] < 20)
                                    {
                                        val = value[i];
                                        result.Add(val, time1, qulityes[i]);
                                    }
                                    else if (qulityes[i + 1] < 20)
                                    {
                                        val = value[i + 1];
                                        result.Add(val, time1, qulityes[i + 1]);
                                    }
                                    else
                                    {
                                        result.Add(default(T), time1, (byte)QualityConst.Null);
                                    }
                                    resultCount++;
                                }
                                break;
                            case QueryValueMatchType.Closed:
                                var pval = (time1 - skey).TotalMilliseconds;
                                var fval = (snext - time1).TotalMilliseconds;

                                if (pval < fval)
                                {
                                    val = value[i];
                                    result.Add(val, time1, qulityes[i]);
                                }
                                else
                                {
                                    val = value[i+1];
                                    result.Add(val, time1, qulityes[i + 1]);
                                }
                                resultCount++;
                                break;
                        }
                        break;
                    }
                    else if (time1 == snext)
                    {
                        var val =value[i + 1];
                        result.Add(val, time1, qulityes[i+1]);
                        resultCount++;
                        break;
                    }

                }
            }


            return resultCount;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="time"></param>
        /// <param name="timeTick"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object DeCompressValue<T>(MarshalMemoryBlock source, int sourceAddr, DateTime time, int timeTick, QueryValueMatchType type)
        {
            if (CheckTypeIsPointData(typeof(T)))
            {
                return DeCompressPointValue<T>(source, sourceAddr, time, timeTick, type);
            }

            int count = 0;
            var timers = GetTimers(source, sourceAddr + 8, timeTick, out count);

            var valuesize = source.ReadInt();
            var value = DeCompressValue<T>(source.ReadBytes(valuesize), count);

            var qusize = source.ReadInt();

            var qulityes = DeCompressQulity(source.ReadBytes(qusize));

            int j = 0;

            for (int i = j; i < timers.Count - 1; i++)
            {
                var skey = timers[i];

                var snext = timers[i + 1];

                if (time == skey)
                {
                    return value[i];
                }
                else if (time > skey && time < snext)
                {
                    switch (type)
                    {
                        case QueryValueMatchType.Previous:
                            return value[i];
                        case QueryValueMatchType.After:
                            return value[i + 1];
                        case QueryValueMatchType.Linear:
                            if (typeof(T) == typeof(bool) || typeof(T) == typeof(string) || typeof(T) == typeof(DateTime))
                            {
                                var ppval = (time - skey).TotalMilliseconds;
                                var ffval = (snext - time).TotalMilliseconds;

                                if (ppval < ffval)
                                {
                                    return value[i];
                                }
                                else
                                {
                                    return value[i + 1];
                                }
                            }
                            else
                            {
                                if (qulityes[i] < 20 && qulityes[i + 1] < 20)
                                {
                                    var pval1 = (time - skey).TotalMilliseconds;
                                    var tval1 = (snext - skey).TotalMilliseconds;
                                    var sval1 = value[i];
                                    var sval2 = value[i + 1];

                                    var val1 = pval1 / tval1 * (Convert.ToDouble(sval2) - Convert.ToDouble(sval1)) + Convert.ToDouble(sval1);

                                    if (typeof(T) == typeof(double))
                                    {
                                        return val1;
                                    }
                                    else if (typeof(T) == typeof(float))
                                    {
                                        return (float)val1;
                                    }
                                    else if (typeof(T) == typeof(short))
                                    {
                                        return (short)val1;
                                    }
                                    else if (typeof(T) == typeof(ushort))
                                    {
                                        return (ushort)val1;
                                    }
                                    else if (typeof(T) == typeof(int))
                                    {
                                        return (int)val1;
                                    }
                                    else if (typeof(T) == typeof(uint))
                                    {
                                        return (uint)val1;
                                    }
                                    else if (typeof(T) == typeof(long))
                                    {
                                        return (long)val1;
                                    }
                                    else if (typeof(T) == typeof(ulong))
                                    {
                                        return (ulong)val1;
                                    }
                                    else if (typeof(T) == typeof(byte))
                                    {
                                        return (byte)val1;
                                    }
                                }
                                else if (qulityes[i] < 20)
                                {
                                    return value[i];
                                }
                                else if (qulityes[i + 1] < 20)
                                {
                                    return value[i + 1];
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            break;
                        case QueryValueMatchType.Closed:
                            var pval = (time - skey).TotalMilliseconds;
                            var fval = (snext - time).TotalMilliseconds;

                            if (pval < fval)
                            {
                                return value[i];
                            }
                            else
                            {
                                return value[i + 1];
                            }
                    }
                    break;
                }
                else if (time == snext)
                {
                    return value[i + 1];
                }

            }

            return null;
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="time"></param>
        /// <param name="timeTick"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public  object DeCompressPointValue<T>(MarshalMemoryBlock source, int sourceAddr, DateTime time1, int timeTick, QueryValueMatchType type)
        {
            int count = 0;
            var timers = GetTimers(source, sourceAddr + 8, timeTick, out count);

            var valuesize = source.ReadInt();
            var value = DeCompressValue<T>(source.ReadBytes(valuesize), count);

            var qusize = source.ReadInt();

            var qulityes = DeCompressQulity(source.ReadBytes(qusize));
            
            for (int i = 0; i < timers.Count - 1; i++)
            {
                var skey = timers[i];

                var snext = timers[i + 1];

                if (time1 == skey)
                {
                    return value[i];

                }
                else if (time1 > skey && time1 < snext)
                {
                    switch (type)
                    {
                        case QueryValueMatchType.Previous:
                            return value[i];
                        case QueryValueMatchType.After:
                            return value[i+1];
                        case QueryValueMatchType.Linear:
                            if (qulityes[i] < 20 && qulityes[i + 1] < 20)
                            {
                                return (T)LinerValue(skey, snext, time1, value[i], value[i + 1]);
                            }
                            else if (qulityes[i] < 20)
                            {
                                return value[i];
                            }
                            else if (qulityes[i + 1] < 20)
                            {
                                return value[i+1];
                            }
                            return null;
                        case QueryValueMatchType.Closed:
                            var pval = (time1 - skey).TotalMilliseconds;
                            var fval = (snext - time1).TotalMilliseconds;

                            if (pval < fval)
                            {
                                return value[i];
                            }
                            else
                            {
                                return value[i + 1];
                            }
                            
                    }
                    break;
                }
                else if (time1 == snext)
                {
                    return value[i + 1];
                }

            }

            return null;
            
        }


        #region

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="time"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private object LinerValue<T>(DateTime startTime,DateTime endTime,DateTime time,T value1,T value2)
        {
            var pval1 = (time - startTime).TotalMilliseconds;
            var tval1 = (endTime - startTime).TotalMilliseconds;

            if (typeof(T) == typeof(IntPointData))
            {
                var sval1 = (IntPointData)((object)value1);
                var sval2 = (IntPointData)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                return new IntPointData((int)val1, (int)val2);
            }
            else if (typeof(T) == typeof(UIntPointData))
            {
                var sval1 = (UIntPointData)((object)value1);
                var sval2 = (UIntPointData)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                return new UIntPointData((uint)val1, (uint)val2);
            }
            else if (typeof(T) == typeof(LongPointData))
            {
                var sval1 = (LongPointData)((object)value1);
                var sval2 = (LongPointData)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                return new LongPointData((long)val1, (long)val2);
            }
            else if (typeof(T) == typeof(ULongPointData))
            {
                var sval1 = (ULongPointData)((object)value1);
                var sval2 = (ULongPointData)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                return new ULongPointData((ulong)val1, (ulong)val2);
            }
            else if (typeof(T) == typeof(IntPoint3Data))
            {
                var sval1 = (IntPoint3Data)((object)value1);
                var sval2 = (IntPoint3Data)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                var val3 = pval1 / tval1 * (Convert.ToDouble(sval2.Z) - Convert.ToDouble(sval1.Z)) + Convert.ToDouble(sval1.Z);
                return new IntPoint3Data((int)val1, (int)val2, (int)val3);
            }
            else if (typeof(T) == typeof(UIntPoint3Data))
            {
                var sval1 = (UIntPoint3Data)((object)value1);
                var sval2 = (UIntPoint3Data)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                var val3 = pval1 / tval1 * (Convert.ToDouble(sval2.Z) - Convert.ToDouble(sval1.Z)) + Convert.ToDouble(sval1.Z);
                return new UIntPoint3Data((uint)val1, (uint)val2, (uint)val3);
            }
            else if (typeof(T) == typeof(LongPoint3Data))
            {
                var sval1 = (LongPoint3Data)((object)value1);
                var sval2 = (LongPoint3Data)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                var val3 = pval1 / tval1 * (Convert.ToDouble(sval2.Z) - Convert.ToDouble(sval1.Z)) + Convert.ToDouble(sval1.Z);
                return new LongPoint3Data((long)val1, (long)val2, (long)val3);
            }
            else if (typeof(T) == typeof(ULongPoint3Data))
            {
                var sval1 = (ULongPoint3Data)((object)value1);
                var sval2 = (ULongPoint3Data)((object)value2);
                var val1 = pval1 / tval1 * (Convert.ToDouble(sval2.X) - Convert.ToDouble(sval1.X)) + Convert.ToDouble(sval1.X);
                var val2 = pval1 / tval1 * (Convert.ToDouble(sval2.Y) - Convert.ToDouble(sval1.Y)) + Convert.ToDouble(sval1.Y);
                var val3 = pval1 / tval1 * (Convert.ToDouble(sval2.Z) - Convert.ToDouble(sval1.Z)) + Convert.ToDouble(sval1.Z);
                return new ULongPoint3Data((ulong)val1, (ulong)val2, (ulong)val3);
            }

            return default(T);
        }

        
       
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceAddr"></param>
        /// <param name="time"></param>
        /// <param name="timeTick"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public  int DeCompressPointValue<T>(MarshalMemoryBlock source, int sourceAddr, List<DateTime> time, int timeTick, QueryValueMatchType type, HisQueryResult<T> result)
        {
            int count = 0;
            var timers = GetTimers(source, sourceAddr + 8, timeTick, out count);

            var valuesize = source.ReadInt();
            var value = DeCompressValue<T>(source.ReadBytes(valuesize), count);

            var qusize = source.ReadInt();

            var qulityes = DeCompressQulity(source.ReadBytes(qusize));
            int resultCount = 0;

            int j = 0;

            foreach (var time1 in time)
            {
                for (int i = j; i < timers.Count - 1; i++)
                {
                    var skey = timers[i];

                    var snext = timers[i + 1];

                    if (time1 == skey)
                    {
                        var val = value[i];
                        result.Add(val, time1, qulityes[i]);
                        resultCount++;

                        break;
                    }
                    else if (time1 > skey && time1 < snext)
                    {
                        switch (type)
                        {
                            case QueryValueMatchType.Previous:
                                var val = value[i];
                                result.Add(val, time1, qulityes[i]);
                                resultCount++;
                                break;
                            case QueryValueMatchType.After:
                                val = value[i + 1];
                                result.Add(val, time1, qulityes[i + 1]);
                                resultCount++;
                                break;
                            case QueryValueMatchType.Linear:
                                if (qulityes[i] < 20 && qulityes[i + 1] < 20)
                                {
                                    result.Add(LinerValue(skey, snext, time1, value[i], value[i + 1]), time1, 0);
                                }
                                else if (qulityes[i] < 20)
                                {
                                    val = value[i];
                                    result.Add(val, time1, qulityes[i]);
                                }
                                else if (qulityes[i + 1] < 20)
                                {
                                    val = value[i + 1];
                                    result.Add(val, time1, qulityes[i + 1]);
                                }
                                else
                                {
                                    result.Add(0, time1, (byte)QualityConst.Null);
                                }
                                resultCount++;
                                break;
                            case QueryValueMatchType.Closed:
                                var pval = (time1 - skey).TotalMilliseconds;
                                var fval = (snext - time1).TotalMilliseconds;

                                if (pval < fval)
                                {
                                    val = value[i];
                                    result.Add(val, time1, qulityes[i]);
                                }
                                else
                                {
                                    val = value[i + 1];
                                    result.Add(val, time1, qulityes[i + 1]);
                                }
                                resultCount++;
                                break;
                        }
                        break;
                    }
                    else if (time1 == snext)
                    {
                        var val = value[i + 1];
                        result.Add(val, time1, qulityes[i + 1]);
                        resultCount++;
                        break;
                    }

                }
            }

            return resultCount;
        }
        
        #endregion


    }
}

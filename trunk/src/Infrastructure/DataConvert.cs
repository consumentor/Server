using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Consumentor.ShopGun
{
    public static class DataConvert
    {
        public static float ToSingle(byte value0, byte value1, byte value2, byte value3)
        {
            var newByte = new byte[4];
            newByte[0] = value3;
            newByte[1] = value2;
            newByte[2] = value1;
            newByte[3] = value0;
            return BitConverter.ToSingle(newByte, 0);
        }

        internal static UInt32 ToDWord(byte value0, byte value1, byte value2, byte value3)
        {
            var newByte = new byte[4];
            newByte[0] = value3;
            newByte[1] = value2;
            newByte[2] = value1;
            newByte[3] = value0;
            return BitConverter.ToUInt32(newByte, 0);
        }

        public static int ToDWordNoSwitch(byte value0, byte value1, byte value2, byte value3)
        {
            var newByte = new byte[4];
            newByte[0] = value0;
            newByte[1] = value1;
            newByte[2] = value2;
            newByte[3] = value3;
            return BitConverter.ToInt32(newByte, 0);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720", Justification = "This method is language dependent, so it makes sense to keep the term int")]
        public static int ToInt(byte value0, byte value1, byte value2, byte value3)
        {
            var newByte = new byte[4];
            newByte[0] = value3;
            newByte[1] = value2;
            newByte[2] = value1;
            newByte[3] = value0;
            return BitConverter.ToInt32(newByte, 0);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720", Justification = "This method is language dependent, so it makes sense to keep the term int")]
        public static int ToInt(byte value0, byte value1)
        {
            var newByte = new byte[2];
            newByte[0] = value1;
            newByte[1] = value0;
            return BitConverter.ToInt16(newByte, 0);
        }

        public static int ToWord(byte value0, byte value1)
        {
            var newByte = new byte[2];
            newByte[0] = value1;
            newByte[1] = value0;
            return BitConverter.ToUInt16(newByte, 0);
        }

        public static int ToWordNoSwitch(byte value0, byte value1)
        {
            var newByte = new byte[2];
            newByte[0] = value0;
            newByte[1] = value1;
            return BitConverter.ToUInt16(newByte, 0);
        }

        private static byte[] ByteSwitchBytes(byte[] values)
        {
            var newByte = new byte[values.Length];
            for (int i = 0; i < values.Length; i++)
                newByte[i] = values[values.Length - 1 - i];
            return newByte;
        }

        public static byte[] ToBytesFromInt(int value)
        {
            return ByteSwitchBytes(BitConverter.GetBytes(value));
        }

        public static byte[] ToBytesFromDWordNoSwitch(int value)
        {
            return BitConverter.GetBytes(value);
        }

        //only the lowest 16 bytes will be used!
        public static byte[] ToBytesFromUnsignedShort(int value)
        {
            var shortValue = (UInt16)value;
            return ByteSwitchBytes(BitConverter.GetBytes(shortValue));
        }

        public static byte[] ToBytesFromFloat(float value)
        {
            return ByteSwitchBytes(BitConverter.GetBytes(value));
        }

        public static byte[] ToBytesFromGuid(Guid value)
        {
            //TODO, wait for HMI method then choose method

            //Send as byte representation
            //var newByte = new byte[22];
            //newByte[0] = 22;
            //newByte[1] = 22;

            ////Copy first 4 bytes
            //var CopyBytes=new byte[4];
            //Buffer.BlockCopy(value.ToByteArray(), 0, CopyBytes, 0, 4);
            //CopyBytes = ByteSwitchBytes(CopyBytes);
            //Buffer.BlockCopy(CopyBytes, 0, newByte, 2, 4);
            //newByte[6] = 45; //'-' sign

            //and so on

            //Send as text
            var newByte = new byte[38];
            //S7 format

            //Length of string
            newByte[0] = 38;
            //Number of char used
            newByte[1] = 38;

            var encoding = new ASCIIEncoding();
            Buffer.BlockCopy(encoding.GetBytes(value.ToString()), 0, newByte, 2, 36);

            return newByte;
        }

        public static byte SetBit(byte target, int bitIndex, bool value)
        {
            if (value)
                target |= (byte) Math.Pow(2, bitIndex);
            else
                target &= (byte) (255 - (byte) Math.Pow(2, bitIndex));
            return target;
        }

        public static bool GetBit(byte bitSet, int index)
        {
            return GetBits(bitSet, index, 1) == 1;
        }

        public static int GetBits(byte biteSet, int offset, int count)
        {
            return (biteSet >> offset) & ((1 << count) - 1);
        }
    }
}
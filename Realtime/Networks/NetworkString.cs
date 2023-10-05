// using System.Collections;
// using System.Diagnostics;
// using System.Drawing;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
//
// namespace Realtime.Networks;
//
// [Serializable]
// [StructLayout(LayoutKind.Sequential, Pack = 4)]
// public struct NetworkString<TSize> where TSize : unmanaged, INetworkSize
// {
//     public int Length { get; private set; }
//     private TSize _data;
//     public unsafe int Capacity => sizeof(TSize) / 4;
//
//     private int SafeLength
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get
//         {
//             if (Length < 0 || Length > Capacity)
//                 throw new InvalidOperationException($"Invalid Length: {Length}");
//             return Length;
//         }
//     }
//
//     private int SafeIndex(int index)
//     {
//         var safeLength = this.SafeLength;
//         return index >= 0 && index < safeLength ? index : throw new ArgumentOutOfRangeException(nameof(index));
//     }
//
//     public unsafe ref uint this[int index]
//     {
//         get
//         {
//             fixed (Size* sizePtr = &_data)
//                 return ((IntPtr)sizePtr + (IntPtr)SafeIndex(index) * 4);
//         }
//     }
//
//     public unsafe bool Set(string value)
//     {
//         value ??= string.Empty;
//         var str = value;
//         ref readonly var chPtr1 = ref str.GetPinnableReference();
//         if (chPtr1 != IntPtr.Zero)
//         {
//             char* chPtr2 = (char*)((IntPtr)chPtr1 + RuntimeHelpers.OffsetToStringData);
//         }
//
//         fixed (Size* dst = &this._data)
//         {
//             UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)dst, this.Capacity);
//             this._length = conversionResult.CodePointCount;
//             return conversionResult.CharacterCount == value.Length;
//         }
//     }
// }
//
// public interface INetworkSize
// {
//     public const int BaseSize = sizeof(uint);
// }
//
// [Serializable]
// [StructLayout(LayoutKind.Explicit, Size = 128, Pack = 4)]
// public unsafe struct Size32 : INetworkSize
// {
//     public const int Fields = 32;
//     public const int Size = INetworkSize.BaseSize * Fields;
//     [FieldOffset(0)] public fixed uint Data[Size];
//     [FieldOffset(0)] [NonSerialized] private uint _data0;
//     [FieldOffset(4)] [NonSerialized] private uint _data1;
//     [FieldOffset(8)] [NonSerialized] private uint _data2;
//     [FieldOffset(12)] [NonSerialized] private uint _data3;
//     [FieldOffset(16)] [NonSerialized] private uint _data4;
//     [FieldOffset(20)] [NonSerialized] private uint _data5;
//     [FieldOffset(24)] [NonSerialized] private uint _data6;
//     [FieldOffset(28)] [NonSerialized] private uint _data7;
//     [FieldOffset(32)] [NonSerialized] private uint _data8;
//     [FieldOffset(36)] [NonSerialized] private uint _data9;
//     [FieldOffset(40)] [NonSerialized] private uint _data10;
//     [FieldOffset(44)] [NonSerialized] private uint _data11;
//     [FieldOffset(48)] [NonSerialized] private uint _data12;
//     [FieldOffset(52)] [NonSerialized] private uint _data13;
//     [FieldOffset(56)] [NonSerialized] private uint _data14;
//     [FieldOffset(60)] [NonSerialized] private uint _data15;
//     [FieldOffset(64)] [NonSerialized] private uint _data16;
//     [FieldOffset(68)] [NonSerialized] private uint _data17;
//     [FieldOffset(72)] [NonSerialized] private uint _data18;
//     [FieldOffset(76)] [NonSerialized] private uint _data19;
//     [FieldOffset(80)] [NonSerialized] private uint _data20;
//     [FieldOffset(84)] [NonSerialized] private uint _data21;
//     [FieldOffset(88)] [NonSerialized] private uint _data22;
//     [FieldOffset(92)] [NonSerialized] private uint _data23;
//     [FieldOffset(96)] [NonSerialized] private uint _data24;
//     [FieldOffset(100)] [NonSerialized] private uint _data25;
//     [FieldOffset(104)] [NonSerialized] private uint _data26;
//     [FieldOffset(108)] [NonSerialized] private uint _data27;
//     [FieldOffset(112)] [NonSerialized] private uint _data28;
//     [FieldOffset(116)] [NonSerialized] private uint _data29;
//     [FieldOffset(120)] [NonSerialized] private uint _data30;
//     [FieldOffset(124)] [NonSerialized] private uint _data31;
// }
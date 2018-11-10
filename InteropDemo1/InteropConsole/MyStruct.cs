using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace InteropConsole
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MyStruct
    {
        public MyStruct(int n)
        {
            Id = n;
            Flag = (byte)(n % 256);
            Unique = Guid.NewGuid();
            Date = DateTimeOffset.Now;
            Number = n / 256;
        }


        // offset 0, length 8
        public int Id { get; set; }

        // offset 8, length 8
        public byte Flag { get; set; }

        // offset 16, length 16
        public Guid Unique { get; set; }

        // offset 32, length 16
        public DateTimeOffset Date { get; set; }

        // offset 48, length 8
        public decimal Number { get; set; }
    }
}

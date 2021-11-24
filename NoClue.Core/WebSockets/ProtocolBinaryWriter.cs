using System;
using System.IO;

namespace NoClue.Core.WebSockets {
    internal class ProtocolBinaryWriter : IDisposable {
        private readonly MemoryStream Stream;
        private readonly byte[] Buffer = new byte[sizeof(long)];

        public ProtocolBinaryWriter(MemoryStream stream) {
            Stream = stream;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public byte[] ToArray() {
            return Stream.ToArray();
        }

        public void WriteBoolean(bool value) {
            if (value) {
                WriteByte(1);
            } else {
                WriteByte(0);
            }
        }

        public void WriteByte(sbyte value) {
            Buffer[0] = (byte)value;
            WriteBuffer(1);
        }

        public void WriteUnsignedByte(byte value) {
            WriteByte((sbyte)value);
        }

        public void WriteInt(int value) {
            for (int i = 0; i < 4; i++) {
                Buffer[i] = (byte)(value >> ((3 - i) * 8) & 0xFF);
            }
            WriteBuffer(4);
        }

        public void WriteUnsignedInt(uint value) {
            WriteInt((int)value);
        }

        public void WriteIntArray(int[] values) {
            WriteInt(values.Length);
            for (int i = 0; i < values.Length; i++) {
                WriteInt(values[i]);
            }
        }

        private void WriteBuffer(int length) {
            Stream.Write(Buffer, 0, length);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                Stream.Dispose();
            }
        }
    }
}

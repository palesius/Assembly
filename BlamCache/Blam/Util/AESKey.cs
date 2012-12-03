﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtryzeDLL.Blam.Util
{
    /// <summary>
    /// Provides a way to generate AES keys from a password string.
    /// </summary>
    public class AESKey
    {
        private byte[] _key;
        private byte[] _iv;

        public AESKey(string password)
        {
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);

            _key = new byte[16];
            for (int i = 0; i < passwordBytes.Length; i++)
                _key[i] = (byte)(passwordBytes[i] ^  0xA5);

            _iv = new byte[16];
            for (int i = 0; i < _key.Length; i++)
                _iv[i] = (byte)(_key[i] ^ 0x3C);
        }

        public byte[] Key
        {
            get { return _key; }
        }

        public byte[] IV
        {
            get { return _iv; }
        }
    }
}

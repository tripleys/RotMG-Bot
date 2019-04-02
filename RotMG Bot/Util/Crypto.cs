using System;
using System.Collections.Generic;
using System.Text;

namespace RotMG_Bot.Util
{
    public class Crypto
    {
        private byte[] _key;

        private int _stateEnc;

        private int _stateDec;

        public Crypto(byte[] key, int offset = 0)
        {
            _key = key;
            _stateEnc = offset;
            _stateDec = offset;
        }

        public byte[] Encrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] + NextKeyEnc());
            }
            return data;
        }

        public byte[] Decrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] - NextKeyDec());
            }
            return data;
        }

        private byte NextKeyEnc() => _key[_stateEnc >= _key.Length ? _stateEnc = 0 : _stateEnc++];
        private byte NextKeyDec() => _key[_stateDec >= _key.Length ? _stateDec = 0 : _stateDec++];

    }
}

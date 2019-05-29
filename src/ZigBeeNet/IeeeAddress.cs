﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ZigBeeNet.Util;

namespace ZigBeeNet
{
    public class IeeeAddress : IComparable<IeeeAddress>
    {
        private byte[] _address;

        public ulong Value
        {
            get
            {
                return BitConverter.ToUInt64(_address, 0);
            }
            set
            {
                _address = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// Default constructor. Creates an address 0
        /// </summary>
        public IeeeAddress()
        {
            this._address = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        /// <summary>
        /// Create an <see cref="IeeeAddress"> from a <see cref="BigInteger">
        ///
        /// <param name="address">the address as a <see cref="BigInteger"></param>
        /// </summary>
        public IeeeAddress(BigInteger address) : this()
        {
            _address = BitConverter.GetBytes((ulong)address);
        }

        /// <summary>
        /// Create an <see cref="IeeeAddress"> from a <see cref="String">
        ///
        /// <param name="address">the address as a <see cref="String"></param>
        /// </summary>
        public IeeeAddress(string address) : this()
        {
            try
            {
              _address = BitConverter.GetBytes(Convert.ToUInt64(address, 16));
            }
            catch (FormatException e)
            {
                throw new ArgumentException("IeeeAddress string must contain valid hexadecimal value");
            }
        }

        /// <summary>
        /// Create an <see cref="IeeeAddress"> from an int array
        ///
        /// <param name="address">the address as an int array. Array length must be 8.</param>
        /// @throws IllegalArgumentException
        /// </summary>
        public IeeeAddress(byte[] address)
        {
            if (address.Length != 8)
            {
                throw new ArgumentNullException("IeeeAddress array length must be 8");
            }
            _address = address;//Arrays.copyOf(address, 8);
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <returns></returns>
        public byte[] GetAddress()
        {
            return _address;
        }

        public override int GetHashCode()
        {
            return Hash.CalcHashCode(_address);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!typeof(IeeeAddress).IsAssignableFrom(obj.GetType()))
            {
                return false;
            }

            IeeeAddress other = (IeeeAddress)obj;
            for (int cnt = 0; cnt < 8; cnt++)
            {
                if (other._address[cnt] != _address[cnt])
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public int CompareTo(IeeeAddress other)
        {
            if (other == null)
            {
                return -1;
            }

            if (!typeof(IeeeAddress).IsAssignableFrom(other.GetType()))
            {
                return -1;
            }

            for (int cnt = 0; cnt < 8; cnt++)
            {
                if (other._address[cnt] == _address[cnt])
                {
                    continue;
                }

                return other._address[cnt] < _address[cnt] ? 1 : -1;
            }
            return 0;
        }
    }
}

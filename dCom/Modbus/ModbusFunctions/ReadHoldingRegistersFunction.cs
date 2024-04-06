using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            byte[] PackReq = new byte[12];

            PackReq[0] = BitConverter.GetBytes(CommandParameters.TransactionId)[1];
            PackReq[1] = BitConverter.GetBytes(CommandParameters.TransactionId)[0];
            PackReq[2] = BitConverter.GetBytes(CommandParameters.ProtocolId)[1];
            PackReq[3] = BitConverter.GetBytes(CommandParameters.ProtocolId)[0];
            PackReq[4] = BitConverter.GetBytes(CommandParameters.Length)[1];
            PackReq[5] = BitConverter.GetBytes(CommandParameters.Length)[0];
            PackReq[6] = CommandParameters.UnitId;
            PackReq[7] = CommandParameters.FunctionCode;
            PackReq[8] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).StartAddress)[1];
            PackReq[9] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).StartAddress)[0];
            PackReq[10] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).Quantity)[1];
            PackReq[11] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).Quantity)[0];

            return PackReq;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> resp = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCount = response[8];
            ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;

            if (response[7] == CommandParameters.FunctionCode + 0x80)
            {
                HandeException(response[8]);
            }
            else
            {
                for (int i = 0; i < byteCount; i += 2)
                {
                    ushort value = BitConverter.ToUInt16(response, 9 + i);
                    value = (ushort)IPAddress.NetworkToHostOrder((short)value);
                    Tuple<PointType, ushort> tuple = new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, startAddress++);
                    resp.Add(tuple, value);
                }
            }

            return resp;
        }
    }
}
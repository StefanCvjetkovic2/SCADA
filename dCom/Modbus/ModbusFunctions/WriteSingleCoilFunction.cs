using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
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
            PackReq[8] = BitConverter.GetBytes(((ModbusWriteCommandParameters)CommandParameters).OutputAddress)[1];
            PackReq[9] = BitConverter.GetBytes(((ModbusWriteCommandParameters)CommandParameters).OutputAddress)[0];
            PackReq[10] = BitConverter.GetBytes(((ModbusWriteCommandParameters)CommandParameters).Value)[1];
            PackReq[11] = BitConverter.GetBytes(((ModbusWriteCommandParameters)CommandParameters).Value)[0];

            return PackReq;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> resp = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort outputAddress = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 8));
            ushort value = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 10));

            Tuple<PointType, ushort> tuple = new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, outputAddress);
            resp.Add(tuple, value);

            return resp;
        }
    }
}
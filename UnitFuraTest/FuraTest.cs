﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Neo;
using System.Linq;
using Neo.SmartContract;
using System.Numerics;
using System.Text;
using Neo.Plugins.VM;
using Neo.Plugins;
using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace UnitFuraTest
{
    [TestClass]
    public class FuraTest
    {
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void TestScript2Executions()
        {
            var base64String = "MG3uriu8UCtAWwkG4FRDCxac590P3HrlnbF4l27+6QU=";
            //var base64String = "DCEC13y+vWO9KxAxFwg0SF0rjAJoq/n2N89uNwqrDwi+WHsMFIU5Il4pKR6Kf5xyOLaNS67/1PekEsAfDAR2b3RlDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtS";
            var script = Convert.FromBase64String(base64String);
            var str = Encoding.UTF8.GetString(script);
            var scCalls = Neo.Plugins.VM.Helper.Script2ScCallModels(script, UInt256.Zero, UInt160.Zero, "");
        }

        public static string TryParseByteString(string str)
        {
            try
            {
                str = Encoding.UTF8.GetString(Convert.FromBase64String(str));
            }
            catch
            {

            }

            return str;
        }

        public static void WriteArray(Utf8JsonWriter writer)
        {

        }

        public static void WriteObject(Utf8JsonWriter writer,string key, JsonElement element)
        {
            var type = element.GetProperty("type").GetString();
            string value = "";
            switch (type)
            {
                case "ByteString":
                    value = TryParseByteString(element.GetProperty("value").GetString());
                    writer.WriteString(key, value);
                    break;
                case "Integer":
                    value = element.GetProperty("value").GetString();
                    writer.WriteString(key, value);
                    break;
                case "Map":
                    var values = element.GetProperty("value");
                    foreach (var _e in values.EnumerateArray())
                    {
                        var _key = GetValue(_e.GetProperty("key"));
                        WriteObject(writer, _key, _e.GetProperty("value"));
                    }
                    break;
                case "Array":
                    writer.WriteStartObject(key);
                    foreach (var _e in element.GetProperty("value").EnumerateArray())
                    {
                        JsonElement jsonElement;
                        var suc = _e.TryGetProperty("key", out jsonElement);
                        if (suc)
                        {
                            var _key = GetValue(jsonElement);
                            WriteObject(writer, _key, _e.GetProperty("value"));
                        }
                        else
                        {
                            WriteObject(writer, "", _e);
                        }
                    }
                    writer.WriteEndObject();
                    break;
            }
        }

        public static string GetValue(JsonElement element)
        {
            var type = element.GetProperty("type").GetString();
            string value = "";
            switch (type)
            {
                case "ByteString":
                    value = TryParseByteString(element.GetProperty("value").GetString());
                    break;
                case "Integer":
                    value = element.GetProperty("value").GetString();
                    break;
            }
            return value;
        }

        [TestMethod]
        public void TestConvert()
        {
            var a = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            UInt160 from;
            bool succ = UInt160.TryParse(Convert.FromBase64String("krOcd6pg8ptXwXPO2Rfxf9Mhpus=").Reverse().ToArray().ToHexString(), out from);
            succ = UInt160.TryParse("d79888e16f9186e873b18a2bc80fd3ecf2ba74b3", out from);
        }

        [TestMethod]
        public void TestECPointParse()
        {
            var base64String = "CwwUaUQlwX8eu3xl3jAmyDHrTEnW174SwB8MBHZvdGUMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1I=";
            //var base64String = "DCEC13y+vWO9KxAxFwg0SF0rjAJoq/n2N89uNwqrDwi+WHsMFIU5Il4pKR6Kf5xyOLaNS67/1PekEsAfDAR2b3RlDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtS";
            var script = Convert.FromBase64String(base64String);
            var scCalls = Neo.Plugins.VM.Helper.Script2ScCallModels(script, UInt256.Zero, UInt160.Zero, "");
            UInt160 voter = null;
            bool succ = UInt160.TryParse(scCalls[0].HexStringParams[0].HexToBytes().Reverse().ToArray().ToHexString(), out voter);
            if (scCalls[0].HexStringParams[1] != string.Empty)
            {
                ECPoint ecPoint = null;
                succ = ECPoint.TryParse("", ECCurve.Secp256r1, out ecPoint);
                var candidate = Contract.CreateSignatureContract(ecPoint).ScriptHash;
            }
        }
    }
}

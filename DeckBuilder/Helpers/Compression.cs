﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.IO.Compression;

using DeckBuilder.Models;

namespace DeckBuilder.Helpers
{
    public class Compression
    {
        public static object DecompressGameState(string compressedJSON, DataContractJsonSerializer deserializer)
        {
            string json = "";
            using (var decomStream = new MemoryStream(Encoding.Default.GetBytes(compressedJSON)))
            {
                using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(hgs))
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }
            MemoryStream masterStream = new MemoryStream(Encoding.Default.GetBytes(json));
            return deserializer.ReadObject(masterStream);
        }

        public static string DecompressStringState(string compressedState)
        {
            string state = "";
            using (var decomStream = new MemoryStream(Encoding.Default.GetBytes(compressedState)))
            {
                using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(hgs))
                    {
                        state = reader.ReadToEnd();
                    }
                }
            }
            return state;
        }

        public static string CompressGameState(object fullState, DataContractJsonSerializer serializer)
        {
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, fullState);
            ms.Close();
            MemoryStream compressStream = new MemoryStream(ms.ToArray());
            string minijson = "";
            using (var cmpStream = new MemoryStream())
            {
                using (var hgs = new GZipStream(cmpStream, CompressionMode.Compress))
                {
                    compressStream.CopyTo(hgs);
                }
                minijson = Encoding.Default.GetString(cmpStream.ToArray());
            }
            return minijson;            
        }

        public static string CompressStringState(string fullState)
        {
            MemoryStream compressStream = new MemoryStream(Encoding.Default.GetBytes(fullState));
            string miniState = "";
            using (var cmpStream = new MemoryStream())
            {
                using (var hgs = new GZipStream(cmpStream, CompressionMode.Compress))
                {
                    compressStream.CopyTo(hgs);
                }
                miniState = Encoding.Default.GetString(cmpStream.ToArray());
            }
            return miniState; 
        }

        public static string ConvertToJSON(object fullState, DataContractJsonSerializer serializer)
        {
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, fullState);
            return Encoding.Default.GetString(ms.ToArray());
        }
    }

    public class PickledState
    {
        public string state;
    }

    public class JSONContainer
    {
        public string json;
    }
}
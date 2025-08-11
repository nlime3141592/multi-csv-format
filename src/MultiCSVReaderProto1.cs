using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace nl
{
    public class MultiCSVReaderProto1
    {
        private string _filePath;

        // main alias: not negative number
        // sub alias : negative number
        public Dictionary<string, MultiCSVAlias> _aliasIndex;

        public MultiCSVReaderProto1(string multiCsvFilePath)
        {
            _filePath = multiCsvFilePath;
            _aliasIndex = CreateAliasIndex();
        }

        public List<T> ReadTable<T>(int startRecordIndex = 0, int count = -1)
        {
            List<T> table = new List<T>(0);
            long position = GetIndex(typeof(T).Name);

            if (position < 0)
                return table;

            FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader rd = new StreamReader(fs);

            rd.BaseStream.Position = position;

            while (!rd.EndOfStream)
            {
                string line = rd.ReadLine();
                T data;

                if (line.Length == 0)
                    break;

                if (!TryParseLine<T>(null, line.Split(','), out data))
                    return new List<T>(0);

                table.Add(data);
            }

            rd.Close();
            fs.Close();

            return table;
        }

        private bool TryParseLine<T>(FieldInfo[] fieldInfos, string[] tokens, out T data)
        {
            T instance = Activator.CreateInstance<T>();
            Type type = typeof(T);
            bool parsingSuccess = true;

            BindingFlags flag = BindingFlags.Default;
            flag |= BindingFlags.Public;
            flag |= BindingFlags.Instance;

            int i = 0;
            int j = 0;

            for (i = 0; i < fieldInfos.Length; ++i)
            {
                if (fieldInfos[i] == null)
                {
                    continue;
                }

                parsingSuccess = fieldInfos[i] != null && TrySetField<T>(ref instance, fieldInfos[i], tokens[j]);
                ++j;
            }

            data = instance;
            return parsingSuccess;
        }

        private bool TrySetField<T>(ref T instance, FieldInfo info, string token)
        {
            TypeCode tCode = Type.GetTypeCode(info.FieldType);
            object parsedValue;

            switch (tCode)
            {
                case TypeCode.String:
                    parsedValue = token;
                    break;
                case TypeCode.Boolean:
                    parsedValue = bool.TryParse(token, out var b) ? b : default;
                    break;
                case TypeCode.Char:
                    parsedValue = char.TryParse(token, out var c) ? c : default;
                    break;
                case TypeCode.SByte:
                    parsedValue = sbyte.TryParse(token, out var sby) ? sby : default;
                    break;
                case TypeCode.Byte:
                    parsedValue = byte.TryParse(token, out var by) ? by : default;
                    break;
                case TypeCode.Int16:
                    parsedValue = short.TryParse(token, out var sht) ? sht : default;
                    break;
                case TypeCode.UInt16:
                    parsedValue = ushort.TryParse(token, out var usht) ? usht : default;
                    break;
                case TypeCode.Int32:
                    parsedValue = int.TryParse(token, out var it) ? it : default;
                    break;
                case TypeCode.UInt32:
                    parsedValue = uint.TryParse(token, out var uit) ? uit : default;
                    break;
                case TypeCode.Int64:
                    parsedValue = long.TryParse(token, out var lg) ? lg : default;
                    break;
                case TypeCode.UInt64:
                    parsedValue = ulong.TryParse(token, out var ulg) ? ulg : default;
                    break;
                case TypeCode.Single:
                    parsedValue = float.TryParse(token, out var flt) ? flt : default;
                    break;
                case TypeCode.Double:
                    parsedValue = double.TryParse(token, out var db) ? db : default;
                    break;
                default:
                    parsedValue = default;
                    break;
            }

            bool result = parsedValue != null;
            object boxedInstance = instance;
            info.SetValue(boxedInstance, result ? parsedValue : default);
            instance = (T)boxedInstance;
            return result;
        }

        private Dictionary<string, MultiCSVAlias> CreateAliasIndex()
        {
            Dictionary<string, MultiCSVAlias> dict = new Dictionary<string, MultiCSVAlias>(8);

            FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader rd = new StreamReader(fs);

            while (!rd.EndOfStream)
            {
                string line = rd.ReadLine().Trim();
                
                if (!line.StartsWith('#'))
                    continue;

                line = line.Substring(1, line.Length - 1);

                string[] aliases = line.Split(',');

                MultiCSVAlias parent = new MultiCSVAlias(aliases[0], rd.BaseStream.Position);
                dict.Add(aliases[0], parent);

                for (int i = 1; i < aliases.Length; ++i)
                {
                    MultiCSVAlias child = new MultiCSVAlias(aliases[i], rd.BaseStream.Position, parent);
                    dict.Add(aliases[i], child);
                }
            }

            rd.Close();
            fs.Close();

            return dict;
        }

        private long GetIndex(string alias)
        {
            if (!_aliasIndex.ContainsKey(alias))
                return -1;
            else if (_aliasIndex[alias].index < 0)
                return _aliasIndex[_aliasIndex[alias].parent.alias].index;
            else
                return _aliasIndex[alias].index;
        }
    }
}
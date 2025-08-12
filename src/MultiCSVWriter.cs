using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace nl
{
    public class MultiCSVWriter
    {
        private Stream _stream;
        private StringBuilder _builder;
        private List<string> _writtenTypes;

        public MultiCSVWriter(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }
            
            if (!stream.CanWrite)
            {
                throw new ArgumentException();
            }

            _stream = stream;
            _builder = new StringBuilder(1024);
            _writtenTypes = new List<string>(16);
        }

        public void Close()
        {
            _stream.Close();
        }

        public bool TryWriteTable<T>(List<T> records)
        {
            string[] headers;

            if (_writtenTypes.Contains(typeof(T).Name))
            {
                return false;
            }

            _writtenTypes.Add(typeof(T).Name);

            if (!TryWriteHeaderRecord<T>(out headers))
            {
                return false;
            }

            WriteAliasRecord<T>();
            WriteContentsRecord<T>(records, headers);
            _stream.Write(Encoding.UTF8.GetBytes("\u000D\u000A"));
            return true;
        }

        private void WriteAliasRecord<T>()
        {
            _stream.Write(Encoding.UTF8.GetBytes($"#{typeof(T).Name}\u000D\u000A"));
        }

        private void WriteContentsRecord<T>(List<T> records, string[] headers)
        {
            if (records.Count == 0)
            {
                _stream.Write(Encoding.UTF8.GetBytes("\u000D\u000A"));
                return;
            }

            Type type = typeof(T);

            for (int i = 0; i < records.Count; ++i)
            {
                _builder.Clear();

                for (int j = 0; j < headers.Length; ++j)
                {
                    _builder.Append($"{type.GetField(headers[j]).GetValue(records[i]).ToString()},");
                }

                _builder.Remove(_builder.Length - 1, 1);
                _builder.Append("\u000D\u000A");
                _stream.Write(Encoding.UTF8.GetBytes(_builder.ToString()));
            }
        }

        private bool TryWriteHeaderRecord<T>(out string[] headers)
        {
            FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<string> fields = new List<string>(fieldInfos.Length);

            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                TypeCode tCode = Type.GetTypeCode(fieldInfos[i].FieldType);

                if (IsValidFieldType(tCode))
                {
                    fields.Add(fieldInfos[i].Name);
                }
            }

            if (fields.Count == 0)
            {
                headers = null;
                return false;
            }

            headers = fields.ToArray();
            return true;
        }

        private bool IsValidFieldType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }
    }
}
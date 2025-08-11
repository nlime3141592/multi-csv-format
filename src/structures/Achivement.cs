using System;
using System.Reflection;
using System.Text;

namespace nl
{
    public class Achivement
    {
        public string title;
        public string description;
        public bool isCleared;
        public int difficulty;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            Type type = this.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            builder.Append($"{type.Name}");

            foreach (FieldInfo fieldInfo in type.GetFields(flags))
            {
                builder.Append($" ({fieldInfo.Name} == {fieldInfo.GetValue(this).ToString()})");
            }

            return builder.ToString();
        }
    }
}
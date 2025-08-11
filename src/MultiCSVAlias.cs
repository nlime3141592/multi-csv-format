namespace nl
{
    public class MultiCSVAlias
    {
        public string alias;
        public long index;
        public MultiCSVAlias parent;

        public MultiCSVAlias(string alias, long index, MultiCSVAlias parent)
        {
            System.Diagnostics.Debug.Assert(alias != null);
            this.alias = alias;

            if (parent == null)
            {
                System.Diagnostics.Debug.Assert(index >= 0);
                this.index = index;
            }
            else
            {
                System.Diagnostics.Debug.Assert(index >= 0);
                this.index = -index;
            }

            this.parent = parent;
        }

        public MultiCSVAlias(string alias, long index)
        : this(alias, index, null)
        {

        }

        public override string ToString()
        {
            return $"{alias}:{index}";
        }
    }
}
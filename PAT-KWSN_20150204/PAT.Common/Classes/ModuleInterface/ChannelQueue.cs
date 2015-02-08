using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.LTS
{
    public sealed class ChannelQueue : Queue<ExpressionValue[]>
    {
        public int Size;
        public ChannelQueue(int s)
        {
            this.Size = s;
        }

        public ChannelQueue Clone()
        {
            ChannelQueue queue = new ChannelQueue(Size);
            foreach (ExpressionValue[] var in this)
            {
                queue.Enqueue(var);
            }
            return queue;
        }

        public override string ToString()
        {
            if(this.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (ExpressionValue[] v in this)
                {
                    if (v.Length == 1)
                    {
                        sb.Append(v[0] + ",");
                    }
                    else
                    {
                        sb.Append("["+ Ultility.Ultility.PPStringList(v) + "],");
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }

            return "";
        }

        public string GetID()
        {
            if (this.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (ExpressionValue[] v in this)
                {
                    sb.Append(Ultility.Ultility.PPIDListDot(v) + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            return "";
        }

        public bool IsFull()
        {
            return this.Count == Size;
        }

        public bool IsEmpty()
        {
            return this.Count == 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNet
{
    public abstract class AProtocol
    {
        protected List<object> data = new List<object>();
        public int Count { get { return data.Count; } }
        public abstract void addData(object obj);
        public abstract void deserialize(byte[] buffer, int bytesReceived);
        public abstract byte[] serialize(int i);
        public abstract void parseProtocol(int i);
        public abstract string toString(int i);
    }
}

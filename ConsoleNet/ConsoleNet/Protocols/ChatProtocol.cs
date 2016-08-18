using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNet
{
    public class AddProtocol : AProtocol
    {
        public StringBuilder sb = new StringBuilder();

        public override void addData(object obj)
        {
            data.Add((obj as string));
        }

        public override void deserialize(byte[] buffer, int bytesReceived)
        {
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

            sb.Append(dataReceived);

            int index = sb.ToString().IndexOf("<EOF>");
            while (index > -1)
            {
                data.Add(sb.ToString().Substring(0, index));
                sb.Remove(0, index + "<EOF>".Length);

                index = sb.ToString().IndexOf("<EOF>");
            }
        }

        public override byte[] serialize(int i)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data[i].ToString() + "<EOF>");
            data.RemoveAt(i);

            return byteData;
        }

        public override void parseProtocol(int i)
        {
            //string temp = data[i] as string;
            data.RemoveAt(i);
        }

        public override string toString(int i)
        {
            return data[i].ToString();
        }
    }
}

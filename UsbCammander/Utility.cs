using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace EricWang
{
    class Utility
    {
        public string makeHexTable(byte[] array) {
            return makeHexTable(array, (uint)array.Length);
        }
        public string makeHexTable(byte[] array, uint length) {
            StringBuilder strB = new StringBuilder();
            for(uint i = 0; i < length; i++) {
                strB.Append(array[i].ToString("X2"));
                strB.Append(" ");
                if(((i + 1) % 0x10) == 0) {
                    strB.Append("\n");
                }
            }
            return strB.ToString();
        }

        public string makeHeader(string meg) {
            StringReader sr = new StringReader(meg); 
            StringBuilder strB = new StringBuilder();

            strB.Append("0000| 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F").Append("\n"); 
            strB.Append("====|================================================").Append("\n");

            String line = sr.ReadLine();
            int cnt = 0;
            while(!String.IsNullOrEmpty(line)) {

                strB.Append( cnt.ToString("X04") + "| ");
                strB.Append(line).Append("\n");

                line = sr.ReadLine();
                cnt += 0x10;
            }
            return strB.ToString();
        }



        public string makeAsciiTable(byte[] array) {
            return makeAsciiTable(array, (uint)array.Length);
        }
        public string makeAsciiTable(byte[] array, uint length) {
            StringBuilder strB = new StringBuilder();
            for(uint i = 0; i < length; i++) {
                strB.Append(Convert.ToChar(array[i]));
                strB.Append(" ");
                if(((i + 1) % 0x10) == 0) {
                    strB.Append("\n");
                }
            }
            return strB.ToString();
        }

    }
}

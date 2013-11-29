using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EricWang
{
    class Utility
    {
        public string makeHexTable(byte[] array) {
            StringBuilder strB = new StringBuilder();
            for(byte i = 0; i < array.Length; i++) {
                strB.Append(array[i].ToString("X2"));
                strB.Append(" ");
                if(((i + 1) % 0x10) == 0) {
                    strB.Append("\n");
                }
            }
            return strB.ToString();
        }

        public string makeAsciiTable(byte[] array) {
            StringBuilder strB = new StringBuilder();
            for(byte i = 0; i < array.Length; i++) {
                strB.Append(  Convert.ToChar(array[i]) );
                strB.Append(" ");
                if(((i + 1) % 0x10) == 0) {
                    strB.Append("\n");
                }
            }
            return strB.ToString();
        }

    }
}

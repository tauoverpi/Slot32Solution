using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slot32Solution
{
    //    0   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15
    // 4:                                                            lit
    // 2: add sub mul div cmp and or  xor shl shr
    // 1:                                        neg inc dec not
    // 0:                                                        hole


    public class Interpreter
    {
        private byte[] source;
        private int sp = 0;
        private int counter = 0;
        private long[] stack = new long[64];
 
        Interpreter(byte[] source)
        {
            this.source = source;
        }

        public static Interpreter MakeInterpreter(params byte[] source) =>
            new Interpreter(source);

        // returns -1 in case of failure
        // TOOD: implement compiler to native
        public Int64 Step()
        {
            for (int ip = 0; ip < source.Length; ip++)
            {
                byte n;
                switch ((n = source[ip]))
                {
                    case var x when x < 10 && sp > 1:
                        switch (n)
                        {
                            case 0x0: stack[sp - 1] += stack[sp]; break;
                            case 0x1: stack[sp - 1] -= stack[sp]; break;
                            case 0x2: stack[sp - 1] *= stack[sp]; break;
                            case 0x3: stack[sp - 1] /= stack[sp]; break;
                            case 0x4: stack[sp - 1] = stack[sp - 1] > stack[sp] ? 1 : 0; break;
                            case 0x5: stack[sp - 1] &= stack[sp]; break;
                            case 0x6: stack[sp - 1] |= stack[sp]; break;
                            case 0x7: stack[sp - 1] ^= stack[sp]; break;
                            case 0x8: stack[sp - 1] <<= (int)stack[sp]; break;
                            case 0x9: stack[sp - 1] >>= (int)stack[sp]; break;
                            // unreachable
                        }
                        sp--;
                        break;
                    case var x when x < 14 && sp > 0:
                        switch (n)
                        {
                            case 0xA: stack[sp - 1] ^= (1 << 64); break;
                            case 0xB: stack[sp - 1]++; break;
                            case 0xC: stack[sp - 1]--; break;
                            case 0xD: stack[sp - 1] ^= -1; break;
                            // unreachable
                        }
                        break;
                    case var x when x < 15 && sp < 64:
                        stack[sp] = counter;
                        sp += 1;
                        break;
                    case var x when x == 15 && sp < 64:
                        stack[sp] = (source[ip+1] << 48)
                                  + (source[ip+2] << 40)
                                  + (source[ip+3] << 32)
                                  + (source[ip+4] << 24)
                                  + (source[ip+5] << 16)
                                  + (source[ip+6] << 8)
                                  + source[ip+7];
                        ip += 8;
                        sp += 1;
                        break;
                    default: throw new IndexOutOfRangeException();
                }
            }
            counter++;
            return sp == 0 ? -1 : stack[sp - 1];
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var m = Interpreter.MakeInterpreter(15
                                               , 1, 1, 1
                                               , 1, 1, 1, 1);
            for (int i = 0; i < 16; i++)
            {
                long result = m.Step();
                StringBuilder str = new StringBuilder();
                for (int j = 0; j<35; j++)
                {
                    str.Append((result >> j) & 1);
                }
                Console.WriteLine($"bits: {str.ToString()}");
            }
            Console.ReadLine();
        }
    }
}

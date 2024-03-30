using System.Text;

namespace Enigma {
    internal class Program {
        static void Main(string[] args) {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            byte size_alph = (byte)alphabet.Length;
            Encoder encoder = new Encoder(size_alph);
            encoder.encoder_set_alphabet(alphabet);


            byte num_rotors = 3;
            byte[] reflector = { 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            byte[,] rotors = {
                {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25},
                {20, 21, 22, 23, 24, 25, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                {7, 6, 5, 4, 3, 2, 1, 0, 24, 23, 22, 21, 20, 25, 8, 9, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10}
            };
            
            Enigma enigma = new Enigma(size_alph, num_rotors);
            enigma.enigma_set_reflector(reflector);
            enigma.enigma_set_rotors(rotors);
            char ench_ch, dec_ch;
            int ch;
            bool flag = false;
            while (true) {
                ch = Console.Read();
                if (ch == '~') {
                    Console.WriteLine();
                    break;
                }
                
                ench_ch = (char)encoder.encoder_encode((char)ch, ref flag);
                if (!flag) {
                    Console.Write(ch);
                    continue;
                }

                ench_ch = (char)enigma.enigma_encrypt((byte)ench_ch, ref flag);
                if (!flag) {
                    Console.Write($"\nencoder put to enigma unknown code {ench_ch}\n");
                    break;
                }

                dec_ch = encoder.encoder_decode((byte)ench_ch, ref flag);
                if (!flag) {
                    Console.Write($"\nenigma put to decoder unknown code {ench_ch}\n");
                    break;
                }
                Console.Write(dec_ch);
            }
        }

     
    }

    class Enigma {
        ulong counter;
        byte size_rotor;
        byte num_rotors;
        byte[] reflector;
        byte[,] rotors;

        public Enigma(byte size_rotor, byte num_rotors) {
            
            this.size_rotor = size_rotor;
            this.num_rotors= num_rotors;
            counter = 0;

            reflector = new byte[size_rotor];
            rotors = new byte[num_rotors, size_rotor];

        }

        public byte enigma_encrypt(byte code, ref bool valid) {
            if (code > size_rotor) {
                valid = false;
                return 0;
            }

            ulong rotor_queue;
            byte new_code = code;

            for (int i = 0; i < num_rotors; ++i) { 
                new_code = rotors[i, new_code];
            }

            new_code = reflector[new_code];

            for (int i = num_rotors - 1; i >= 0; --i) {
                new_code = enigma_rotor_find((byte)i, new_code, ref valid);
            }

            rotor_queue = 1;
            counter++;
            for (int i = 0; i < num_rotors; ++i) {
                if (counter % rotor_queue == 0) {
                    enigma_rotor_shift((byte)i);
                }
                rotor_queue *= size_rotor;
            }

            valid = true;
            return new_code;
        }

        public void enigma_rotor_shift(byte num) {
            byte temp = rotors[num, size_rotor - 1];
            for (int i = size_rotor - 1; i > 0; --i) {
                rotors[num, i] = rotors[num, i - 1];
            }
            rotors[num, 0] = temp;
        }

        public byte enigma_rotor_find(byte num, byte code, ref bool valid) {
            for (int i = 0; i < size_rotor; ++i) {
                if (rotors[num, i] == code) {
                    valid = true;
                    return (byte)i;
                }
            }
            valid = false; return 0;
        }

        public void enigma_set_reflector(byte[] reflector) {
            for (int i = 0; i < size_rotor; ++i) {
                this.reflector[i] = reflector[i];
            }
        }

        public void enigma_set_rotors(byte[,] rotor) {
            for (int i = 0; i < num_rotors; ++i) {
                for (int j = 0; j < size_rotor; ++j) {
                    rotors[i, j] = rotor[i, j];
                }
            }
        }
    }

    class Encoder {
        byte size_alph;
        char[] alphabet;

        public Encoder(byte size_alph) { 
            this.size_alph = size_alph;
            alphabet = new char[size_alph];
        }

        public void encoder_set_alphabet(char[] alphabet) {
            for (int i = 0; i < size_alph; ++i) {
                this.alphabet[i] = alphabet[i];
            }
        }

        public byte encoder_encode(char ch, ref bool found) {
            for (int i = 0; i < size_alph; ++i) {
                if (alphabet[i] == ch) {
                    found = true;
                    return (byte)i;
                }
            }
            found = false;
            return 0;
        }

        public char encoder_decode(byte code, ref bool valid) {
            if (code >= size_alph) {
                valid = false;
                return (char)0;
            }
            valid = true;
            return alphabet[code];
        }
    }
}
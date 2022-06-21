
using System.Xml;

namespace PacketGenerator
{
    /*
     * 
     *  처음에는 수작업으로 코드를 작성한 후,
     *  그 다음에 모든 패킷에 공통되는 부분을 남기고, 
     *  변하는 부분만 @"{}"로 뚫어서 코드를 조립.
     *  패킷 같은 경우 미리 패킷을 정의하므로 xml을 통해서
     *  타입과 변수명을 읽어서 사용.
     * 
     */
    class Program
    {
        static string genPackets;
        static ushort packetID;
        static string packetEnums;

        static string clientRegister;
        static string serverRegister;

        static void Main(string[] args)
        {
            string pdlPath = "../PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
           
            };

            if(args.Length > 0)
            {
                pdlPath = args[0];
            }

            using (XmlReader reader = XmlReader.Create(pdlPath, settings))
            {
                reader.MoveToContent();

                while(reader.Read())
                {
                    // 읽기 시작하는 부분이 depth 1, 줄을 읽을때마다 depth가 증가.
                    // XmlNodeType.Element은 xml의 여는 부분(<packet>, <PDL>)
                    // XmlNodeType.EndElement은 xml의 닫는 부분(</packet>, </PDL>)
                    if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                    {
                        // 패킷 하나를 파싱.
                        ParsePacket(reader);
                    }
                }

                string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPackets);
                File.WriteAllText("GenPackets.cs", fileText);
                string clientManagerText = string.Format(PacketFormat.managerFormat, "Client", clientRegister);
                File.WriteAllText("ClientPacketManager.cs", clientManagerText);
                string serverManagerText = string.Format(PacketFormat.managerFormat, "Server", serverRegister);
                File.WriteAllText("ServerPacketManager.cs", serverManagerText);

            }
        }

        public static void ParsePacket(XmlReader reader)
        {
            if(reader.NodeType == XmlNodeType.EndElement)
            {
                return;
            }
            
            // 실수 했을 때 알기 위해서.
            if(reader.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = reader["name"];
            if(string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("packet without name");
                return;
            }

            // 패킷의 내부 멤버를 파싱.
            Tuple<string, string, string> t = ParseMembers(reader);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetID)+Environment.NewLine+"\t";

            // 패킷 이름의 접두를 통해 서버/클라 패킷을 구분.
            if(packetName.StartsWith("S_") || packetName.StartsWith("s_"))
            {
                // 서버가 보내는 패킷
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
            }
            else
            {
                // 클라가 보내는 패킷
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;

            }
        }

        public static Tuple<string, string, string> ParseMembers(XmlReader reader)
        {
            string packetName = reader["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = reader.Depth + 1;

            while(reader.Read())
            {
                if(reader.Depth != depth)
                {
                    break;
                }

                string memberName = reader["name"];

                // 제대로 할당되는데 오류가 걸려서  일단 제외
                //if (string.IsNullOrEmpty(memberName));
                //{
                //    Console.WriteLine("member without name");
                //    return null;
                //}

                // 멤버 변수마다 개행 처리
                if(string.IsNullOrEmpty(memberCode) == false)
                {
                    memberCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(readCode) == false)
                {
                    readCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(writeCode) == false)
                {
                    writeCode += Environment.NewLine;
                }

                string memberType = reader.Name.ToLower();
                switch(memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(reader);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;

                }
            }
            // 개행과 들여쓰기 처리.
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader reader)
        {
            string listName = reader["name"];
            if(string.IsNullOrEmpty(listName))
            {
                return null;
            }

            Tuple<string, string, string> tuple = ParseMembers(reader);

            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName), // 타입명
                FirstCharToLower(listName), // 변수명
                tuple.Item1,
                tuple.Item2,
                tuple.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
using PIoT;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid node1Id = Guid.NewGuid();
            Guid node2Id = Guid.NewGuid();

            var node1Settings = new NodeSettings()
            {
                Id = node1Id,
                Address = "127.0.0.1",
                Port = 42001,
                Name = "Node 1"
            };

            var node2Settings = new NodeSettings()
            {
                Id = node2Id,
                Address = "127.0.0.1",
                Port = 42002,
                Name = "Node 2"
            };

            node1Settings.Links.Add(new NodeLink()
            {
                Address = node2Settings.Address,
                Port = node2Settings.Port,
                Id = node2Settings.Id
            });

            node2Settings.Links.Add(new NodeLink()
            {
                Address = node1Settings.Address,
                Port = node1Settings.Port,
                Id = node1Settings.Id
            });

            var node1 = Node.Create(node1Settings);
            var node2 = Node.Create(node2Settings, false);


            node1.Connect(node2Id);

            node2.SendMessage(node1Id, new PIoT.Messaging.Message("Hello World!"));
           

            while(true)
            {
                node1.SendMessage(node2Id, new PIoT.Messaging.Message(Console.ReadLine()));
            }



            //var guidA = Guid.NewGuid();
            //var guidB = Guid.NewGuid();

            //Console.WriteLine($"Alice: {guidA}");
            //Console.WriteLine($"Bob: {guidB}");

            //var shared = Action(guidA, guidB);

            //// Alice sends session id to bob

            //// Bob returns session id and both parties confirm share session id




            //Console.WriteLine($"Shared: {shared}");



            //Console.WriteLine($"Alice Decrypted: {Action(guidB, shared)}");
            //Console.WriteLine($"Bob Decrypted: {Action(guidA, shared)}");



            Console.ReadLine();
        }

        public static Guid Action(Guid guidA, Guid guidB)
        {
            var ad = new DecomposedGuid(guidA);
            var bd = new DecomposedGuid(guidB);

            ad.Hi ^= bd.Hi;
            ad.Lo ^= bd.Lo;

            return ad.Value;
        }



        [StructLayout(LayoutKind.Explicit)]
        private struct DecomposedGuid
        {
            [FieldOffset(00)] public Guid Value;
            [FieldOffset(00)] public long Hi;
            [FieldOffset(08)] public long Lo;
            public DecomposedGuid(Guid value) : this() => Value = value;
        }
    }
}

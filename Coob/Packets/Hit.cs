﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Coob.CoobEventArgs;
using Coob.Structures;

namespace Coob.Packets
{
    public partial class Packet
    {
        public class Hit : Base
        {
            public ulong EntityID;
            public ulong TargetID;
            public float Damage;
            public byte Critical;
            public uint StunDuration;
            public uint Something8;
            public QVector3 Position;
            public Vector3 HitDirection;
            public byte Skill;
            public byte HitType;
            public byte ShowLight;

            public Entity Attacker;
            public Entity Target;

            public Hit(Client client) : base(client) { }

            public static Base Parse(Client client, Coob coob)
            {
                var hit = new Hit(client);

                hit.EntityID = client.Reader.ReadUInt64();
                hit.TargetID = client.Reader.ReadUInt64();
                hit.Damage = client.Reader.ReadSingle();
                hit.Critical = client.Reader.ReadByte();
                client.Reader.ReadBytes(3);
                hit.StunDuration = client.Reader.ReadUInt32();
                hit.Something8 = client.Reader.ReadUInt32();
                hit.Position = client.Reader.ReadQVector3();
                hit.HitDirection = client.Reader.ReadVector3();
                hit.Skill = client.Reader.ReadByte();
                hit.HitType = client.Reader.ReadByte();
                hit.ShowLight = client.Reader.ReadByte();
                client.Reader.ReadBytes(1);

                coob.World.HitPackets.Add(hit);

                hit.Attacker = coob.World.Entities[hit.EntityID];
                hit.Target = coob.World.Entities[hit.TargetID];

                return hit;
            }

            public override void Write(BinaryWriter bw)
            {
                bw.Write(EntityID);
                bw.Write(TargetID);
                bw.Write(Damage);
                bw.Write(Critical);
                bw.Pad(3);
                bw.Write(StunDuration);
                bw.Write(Something8);
                Position.Write(bw);
                HitDirection.Write(bw);
                bw.Write(Skill);
                bw.Write(HitType);
                bw.Write(ShowLight);
                bw.Pad(1);
            }

            public override bool CallScript()
            {
                return true;
            }

            public override void Process()
            {
                if (EntityID != TargetID)
                {
                    if (Attacker != null && Target != null)
                    {
                        // If target is an npc(probably friendly npc's too) or attacker and target has pvp enabled.
                        if (Target.Client == null || (Attacker.Client != null && Attacker.Client.PVP && Target.Client != null && Target.Client.PVP))
                        {
                            Target.HP -= Damage;
                            Root.ScriptManager.CallEvent("OnEntityAttacked", new EntityAttackedEventArgs(Attacker, Target));
                        }
                    }
                }
            }
        }
    }
}

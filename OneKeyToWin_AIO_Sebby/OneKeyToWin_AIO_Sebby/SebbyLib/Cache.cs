using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SebbyLib
{
    public class Cache
    {
        public static List<MissileClient> MissileList = new List<MissileClient>();
        public static List<Obj_AI_Base> AllMinionsObj = new List<Obj_AI_Base>();
        public static List<Obj_AI_Base> MinionsListEnemy = new List<Obj_AI_Base>();
        public static List<Obj_AI_Base> MinionsListAlly = new List<Obj_AI_Base>();
        public static List<Obj_AI_Base> MinionsListNeutral = new List<Obj_AI_Base>();
        public static List<Obj_AI_Turret> TurretList = ObjectManager.Get<Obj_AI_Turret>().ToList();
        public static List<Obj_HQ> NexusList = ObjectManager.Get<Obj_HQ>().ToList();
        public static List<Obj_BarracksDampener> InhiList = ObjectManager.Get<Obj_BarracksDampener>().ToList();

        static Cache()
        {
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValid))
            {
                AddMinionObject(minion);
                if (!minion.IsAlly)
                    AllMinionsObj.Add(minion);
            }
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            MinionsListEnemy.RemoveAll(minion => !IsValidMinion(minion));
            MinionsListNeutral.RemoveAll(minion => !IsValidMinion(minion));
            MinionsListAlly.RemoveAll(minion => !IsValidMinion(minion));
            AllMinionsObj.RemoveAll(minion => !IsValidMinion(minion));
            MissileList.RemoveAll(missile => !missile.IsValid);
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                AddMinionObject(minion);
                if (!minion.IsAlly )
                    AllMinionsObj.Add(minion);
            }
            var missile = sender as MissileClient;
            if (missile != null)
            { 
                if(missile.Target != null)
                {
                    if(missile.Target is Obj_AI_Hero)
                        MissileList.Add(missile);
                }
                else
                    MissileList.Add(missile); 
            }
        }

        private static void AddMinionObject(Obj_AI_Minion minion)
        {
            if (minion.MaxHealth >= 225)
            {
                if (minion.Team == GameObjectTeam.Neutral)
                {
                    MinionsListNeutral.Add(minion);
                }
                else if (minion.MaxMana == 0 && minion.MaxHealth >= 250)
                {
                    if (minion.Team == GameObjectTeam.Unknown)
                        return;
                    else if (minion.Team != ObjectManager.Player.Team)
                        MinionsListEnemy.Add(minion);
                    else if (minion.Team == ObjectManager.Player.Team)
                        MinionsListAlly.Add(minion);
                }
            }
        }

        public static List<Obj_AI_Base> GetMinions(Vector3 from, float range = float.MaxValue, MinionTeam team = MinionTeam.Enemy)
        {
            if (team == MinionTeam.Enemy)
            {
                
                return MinionsListEnemy.FindAll(minion => CanReturn(minion, from, range));
            }
            else if (team == MinionTeam.Ally)
            {
                
                return MinionsListAlly.FindAll(minion => CanReturn(minion, from, range));
            }
            else if(team == MinionTeam.Neutral)
            {
                
                return MinionsListNeutral.Where(minion => CanReturn(minion, from, range)).OrderByDescending(minion => minion.MaxHealth).ToList();
            }
            else if (team == MinionTeam.NotAlly)
            {
                return AllMinionsObj.FindAll(minion => CanReturn(minion, from, range));
            }
            else
            {
                return AllMinionsObj.FindAll(minion => CanReturn(minion, from, range));
            }
        }

        private static bool IsValidMinion(Obj_AI_Base minion)
        {
            if (minion == null || !minion.IsValid || minion.IsDead)
                return false;
            else
                return true;
        }

        private static bool CanReturn(Obj_AI_Base minion, Vector3 from, float range)
        {
            
            if (minion != null && minion.IsValid && !minion.IsDead && minion.IsVisible && minion.IsTargetable)
            {
                if (range == float.MaxValue)
                    return true;
                else if (range == 0)
                {
                    if (Orbwalking.InAutoAttackRange(minion))
                        return true;
                    else
                        return false;
                }
                else if (Vector2.DistanceSquared((@from).To2D(), minion.Position.To2D()) < range * range)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}

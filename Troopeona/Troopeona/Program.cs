using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Troopeona
{
    using System.ComponentModel.Design;

    class Program
    {
        public const string ChampionName = "Leona";

        public static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static Orbwalking.Orbwalker Orbwalker;

        //Menu
        public static Menu Menu;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell R;

        private static Items.Item cutlass;

        private static Items.Item botrk;

        private static Items.Item hextech;

        private static Obj_AI_Base target;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Leona") return;

            Q = new Spell(SpellSlot.Q, 120f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 875f);
            E.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 1200f);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //SpellMenu
            Menu spellMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("harassQ", "Use Q to harass").SetValue(true));
            harass.AddItem(new MenuItem("harassW", "Use W to harass").SetValue(true));
            //LaneClear Menu
            Menu.AddSubMenu(new Menu("laneclear", "laneclear"));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("jungleclearQ", "use Q to Laneclear").SetValue(true));
            Menu.SubMenu("laneclear").AddItem(new MenuItem("jungleclearW", "use W to Laneclear").SetValue(true));
            ;

            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            hextech = new Items.Item(3146, 700);
            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAa;
            Game.PrintChat("Leona , featuring Troopeona , whitch is like the worst name ever");
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Menu.Item("useR").GetValue<bool>())
            {
                Killsecure();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
            }
        }

        private static void Combo()
        {
            var useE = (Menu.Item("useE").GetValue<bool>());
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var m = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            var useR = (Menu.Item("useR").GetValue<bool>());



            //Itemusage
            if (m != null && Player.Distance(m) <= botrk.Range)
            {
                botrk.Cast(m);
            }
            if (m != null && Player.Distance(m) <= cutlass.Range)
            {
                cutlass.Cast(m);
            }
            if (m != null && Player.Distance(m) <= hextech.Range)
            {
                hextech.Cast(m);
            }
            //combo
            if (useE && (E.IsReady()))
            {
                E.Cast(m);
            }
            if (useQ && Q.IsReady())
            {
                Q.CastOnBestTarget();
            }
            if (useW && W.IsReady())
            {
                W.CastOnBestTarget();       
            }
            if (useR && R.IsReady())
            {
                R.CastOnBestTarget();
            }
        }

        private static void Killsecure()
        {
            var useR = (Menu.Item("useQ").GetValue<bool>());
            var m = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            {
                Q.CastOnUnit(m);
            }
        }

        private static void Harass()
        {
            var o = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("harassW").GetValue<bool>())
            {
                if (Player.Distance(o.Position) > 125 && (Q.IsReady()))
                {
                    W.Cast(o);
                }
            }
            if (Menu.Item("harassQ").GetValue<bool>())
            {
                Q.CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        private static void AfterAa(AttackableUnit unit, AttackableUnit attackableUnit)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var useQ = (Menu.Item("useQ").GetValue<bool>());
                Q.CastOnBestTarget();
            }
        }

        private static void Lane()
        {
            Obj_AI_Base minion = MinionManager.GetMinions(Player.Position, 125).FirstOrDefault();
            if (Menu.Item("jungleclearW").GetValue<bool>())
                W.Cast(minion);
            Obj_AI_Base cs = MinionManager.GetMinions(Player.Position, 125).FirstOrDefault();
            if (Menu.Item("jungleclearQ").GetValue<bool>() && !W.IsReady())
                Q.Cast(cs);
        }
    }
}


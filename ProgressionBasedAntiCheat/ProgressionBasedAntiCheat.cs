using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Terraria.Localization;
using Terraria.ID;
using Newtonsoft.Json;

namespace ProgressionBasedAntiCheat
{
    [ApiVersion(2, 1)]
    public class ProgressionBasedAntiCheat : TerrariaPlugin
    {
        public override string Author => "Ozz5581, Updated by Nuffy";

        public override string Description => "Deletes items based on world progression.";

        public override string Name => "ProgressionLock";

        public override Version Version => new Version(1, 0, 0, 0);

        private ulong UpdateCount = 0;
        //private static string FilePath => Path.Combine(TShock.SavePath, "BossStatus", "BossStatus.json");
        public ProgressionBasedAntiCheat(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdateBoss);
            Commands.ChatCommands.Add(new Command("pbac.resetboss", resetprogress, "resetworldprogress"));
            if (File.Exists(Path.Combine(TShock.SavePath, "BossStatus", "config.json")))
                LoadConfig();
            else
                SaveConfig(new BossStatus
                {
                    
                });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private IEnumerable<TSPlayer> GetLoggedInPlayers()
        {
            return TShock.Players.Where(p => p != null && p.IsLoggedIn);
        }

        private void CheckSlot(TSPlayer player, int slot)
        {
            Item itemToCheck = (
                (slot >= 220) ? player.TPlayer.bank4.item[slot - 220] :
                ((slot >= 180) ? player.TPlayer.bank3.item[slot - 180] :
                ((slot >= 179) ? player.TPlayer.trashItem :
                ((slot >= 139) ? player.TPlayer.bank2.item[slot - 139] :
                ((slot >= 99) ? player.TPlayer.bank.item[slot - 99] :
                ((slot >= 94f) ? player.TPlayer.miscDyes[slot - 94] :
                ((slot >= 89) ? player.TPlayer.miscEquips[slot - 89] :
                ((slot >= 79) ? player.TPlayer.dye[slot - 79] :
                ((!(slot >= 59)) ? player.TPlayer.inventory[slot] : player.TPlayer.armor[slot - 59])))))))));

            if (itemToCheck.type == ItemID.PlatinumCoin &&
                itemToCheck.stack >= itemToCheck.maxStack - 5)
            {
                TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":"+ itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                itemToCheck.SetDefaults(0);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
            }
            if (Main.zenithWorld == true && Main.remixWorld)
            {
                #region ProgressionLock
                BossStatus config = LoadConfig();
                //Here is the general items that will be automatically removed if the boss or event isn't done.
                if (config.downedML == false)
                {
                    if (itemToCheck.type == ItemID.Zenith
                        || itemToCheck.type == ItemID.LastPrism
                        || itemToCheck.type == ItemID.Terrarian
                        || itemToCheck.type == ItemID.Meowmere
                        || itemToCheck.type == ItemID.StarWrath
                        || itemToCheck.type == ItemID.SDMG
                        || itemToCheck.type == ItemID.LunarFlareBook
                        || itemToCheck.type == ItemID.RainbowCrystalStaff
                        || itemToCheck.type == ItemID.MoonlordTurretStaff
                        || itemToCheck.type == ItemID.Celeb2
                        || itemToCheck.type == ItemID.LunarOre
                        || itemToCheck.type == ItemID.LunarBar
                        || itemToCheck.type == ItemID.MoonlordArrow
                        || itemToCheck.type == ItemID.MoonlordBullet
                        || itemToCheck.type == ItemID.MoonLordBossBag
                        || itemToCheck.type == ItemID.PortalGun
                        || itemToCheck.type == ItemID.LongRainbowTrailWings
                        || itemToCheck.type == ItemID.NebulaBreastplate
                        || itemToCheck.type == ItemID.NebulaHelmet
                        || itemToCheck.type == ItemID.NebulaLeggings
                        || itemToCheck.type == ItemID.NebulaPickaxe
                        || itemToCheck.type == ItemID.NebulaDrill
                        || itemToCheck.type == ItemID.LunarHamaxeNebula
                        || itemToCheck.type == ItemID.WingsNebula
                        || itemToCheck.type == ItemID.NebulaAxe
                        || itemToCheck.type == ItemID.NebulaHammer
                        || itemToCheck.type == ItemID.NebulaChainsaw
                        || itemToCheck.type == ItemID.SolarFlareAxe
                        || itemToCheck.type == ItemID.SolarFlareHelmet
                        || itemToCheck.type == ItemID.SolarFlareLeggings
                        || itemToCheck.type == ItemID.SolarFlarePickaxe
                        || itemToCheck.type == ItemID.SolarFlareBreastplate
                        || itemToCheck.type == ItemID.SolarFlareDrill
                        || itemToCheck.type == ItemID.SolarFlareHelmet
                        || itemToCheck.type == ItemID.WingsSolar
                        || itemToCheck.type == ItemID.LunarHamaxeSolar
                        || itemToCheck.type == ItemID.StardustAxe
                        || itemToCheck.type == ItemID.StardustPickaxe
                        || itemToCheck.type == ItemID.StardustDrill
                        || itemToCheck.type == ItemID.StardustHammer
                        || itemToCheck.type == ItemID.LunarHamaxeStardust
                        || itemToCheck.type == ItemID.WingsStardust
                        || itemToCheck.type == ItemID.StardustHelmet
                        || itemToCheck.type == ItemID.StardustBreastplate
                        || itemToCheck.type == ItemID.StardustLeggings
                        || itemToCheck.type == ItemID.StardustChainsaw
                        || itemToCheck.type == ItemID.WingsVortex
                        || itemToCheck.type == ItemID.LunarHamaxeVortex
                        || itemToCheck.type == ItemID.VortexAxe
                        || itemToCheck.type == ItemID.VortexChainsaw
                        || itemToCheck.type == ItemID.VortexDrill
                        || itemToCheck.type == ItemID.VortexPickaxe
                        || itemToCheck.type == ItemID.VortexHelmet
                        || itemToCheck.type == ItemID.VortexBreastplate
                        || itemToCheck.type == ItemID.VortexLeggings
                        || itemToCheck.type == ItemID.DrillContainmentUnit
                        || itemToCheck.type == ItemID.BottomlessShimmerBucket
                        || itemToCheck.type == ItemID.RodOfHarmony
                        || itemToCheck.type == ItemID.Clentaminator2
                        || itemToCheck.type == ItemID.GravityGlobe)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedAllPillars == false)
                {
                    if (itemToCheck.type == ItemID.LunarHook
                        || itemToCheck.type == ItemID.CelestialSigil
                        || itemToCheck.type == ItemID.SuperHealingPotion)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedNebula == false)
                {
                    if (itemToCheck.type == ItemID.NebulaArcanum
                        || itemToCheck.type == ItemID.NebulaBlaze
                        || itemToCheck.type == ItemID.FragmentNebula)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedSolar == false)
                {
                    if (itemToCheck.type == ItemID.FragmentSolar
                        || itemToCheck.type == ItemID.SolarEruption
                        || itemToCheck.type == ItemID.DayBreak)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedStardust == false)
                {
                    if (itemToCheck.type == ItemID.FragmentStardust
                        || itemToCheck.type == ItemID.StardustDragonStaff
                        || itemToCheck.type == ItemID.StardustCellStaff)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedVortex == false)
                {
                    if (itemToCheck.type == ItemID.FragmentVortex
                        || itemToCheck.type == ItemID.VortexBeater
                        || itemToCheck.type == ItemID.Phantasm)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedLC == false)
                {
                    if (itemToCheck.type == ItemID.CultistBossBag)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedGolem == false)
                {
                    if (itemToCheck.type == ItemID.Xenopopper
                        || itemToCheck.type == ItemID.XenoStaff
                        || itemToCheck.type == ItemID.LaserMachinegun
                        || itemToCheck.type == ItemID.ElectrosphereLauncher
                        || itemToCheck.type == ItemID.InfluxWaver
                        || itemToCheck.type == ItemID.CosmicCarKey
                        || itemToCheck.type == ItemID.BrainScrambler
                        || itemToCheck.type == ItemID.LaserDrill
                        || itemToCheck.type == ItemID.AntiGravityHook
                        || itemToCheck.type == ItemID.ChargedBlasterCannon
                        || itemToCheck.type == ItemID.GolemBossBag
                        || itemToCheck.type == ItemID.Picksaw
                        || itemToCheck.type == ItemID.ShinyStone
                        || itemToCheck.type == ItemID.Stynger
                        || itemToCheck.type == ItemID.PossessedHatchet
                        || itemToCheck.type == ItemID.SunStone
                        || itemToCheck.type == ItemID.HeatRay
                        || itemToCheck.type == ItemID.StaffofEarth
                        || itemToCheck.type == ItemID.EyeoftheGolem
                        || itemToCheck.type == ItemID.GolemFist
                        || itemToCheck.type == ItemID.BeetleHusk
                        || itemToCheck.type == ItemID.BeetleHelmet
                        || itemToCheck.type == ItemID.BeetleShell
                        || itemToCheck.type == ItemID.BeetleScaleMail
                        || itemToCheck.type == ItemID.BeetleLeggings
                        || itemToCheck.type == ItemID.BeetleWings
                        || itemToCheck.type == ItemID.CelestialShell
                        || itemToCheck.type == ItemID.CelestialStone
                        || itemToCheck.type == ItemID.DestroyerEmblem
                        || itemToCheck.type == ItemID.SniperScope
                        || itemToCheck.type == ItemID.ReconScope
                        || itemToCheck.type == ItemID.StyngerBolt
                        || itemToCheck.type == ItemID.BetsyWings
                        || itemToCheck.type == ItemID.BossBagBetsy
                        || itemToCheck.type == ItemID.DD2BetsyBow
                        || itemToCheck.type == ItemID.DD2SquireBetsySword
                        || itemToCheck.type == ItemID.MonkStaffT3
                        || itemToCheck.type == ItemID.ApprenticeStaffT3
                        || itemToCheck.type == ItemID.ApprenticeAltHead
                        || itemToCheck.type == ItemID.ApprenticeAltShirt
                        || itemToCheck.type == ItemID.ApprenticeAltPants
                        || itemToCheck.type == ItemID.HuntressAltHead
                        || itemToCheck.type == ItemID.HuntressAltPants
                        || itemToCheck.type == ItemID.HuntressAltShirt
                        || itemToCheck.type == ItemID.MonkAltHead
                        || itemToCheck.type == ItemID.MonkAltPants
                        || itemToCheck.type == ItemID.MonkAltShirt
                        || itemToCheck.type == ItemID.SquireAltHead
                        || itemToCheck.type == ItemID.SquireAltPants
                        || itemToCheck.type == ItemID.SquireAltShirt
                        || itemToCheck.type == ItemID.DD2LightningAuraT3Popper
                        || itemToCheck.type == ItemID.DD2ExplosiveTrapT3Popper
                        || itemToCheck.type == ItemID.DD2BallistraTowerT3Popper
                        || itemToCheck.type == ItemID.DD2FlameburstTowerT3Popper
                        || itemToCheck.type == ItemID.SteampunkWings)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedEOL == false)
                {
                    if (itemToCheck.type == ItemID.EmpressBlade
                        || itemToCheck.type == ItemID.EmpressFlightBooster
                        || itemToCheck.type == ItemID.FairyQueenBossBag
                        || itemToCheck.type == ItemID.FairyQueenRangedItem
                        || itemToCheck.type == ItemID.FairyQueenMagicItem
                        || itemToCheck.type == ItemID.RainbowWhip
                        || itemToCheck.type == ItemID.RainbowWings
                        || itemToCheck.type == ItemID.SparkleGuitar
                        || itemToCheck.type == ItemID.PiercingStarlight)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedPlantera == false)
                {
                    if (itemToCheck.type == ItemID.EmpressButterfly
                        || itemToCheck.type == ItemID.EmpressButterflyJar
                        || itemToCheck.type == ItemID.GrenadeLauncher
                        || itemToCheck.type == ItemID.VenusMagnum
                        || itemToCheck.type == ItemID.NettleBurst
                        || itemToCheck.type == ItemID.LeafBlower
                        || itemToCheck.type == ItemID.FlowerPow
                        || itemToCheck.type == ItemID.WaspGun
                        || itemToCheck.type == ItemID.Seedler
                        || itemToCheck.type == ItemID.PygmyStaff
                        || itemToCheck.type == ItemID.ThornHook
                        || itemToCheck.type == ItemID.TheAxe
                        || itemToCheck.type == ItemID.SporeSac
                        || itemToCheck.type == ItemID.TempleKey
                        || itemToCheck.type == ItemID.RocketI
                        || itemToCheck.type == ItemID.RocketII
                        || itemToCheck.type == ItemID.RocketIII
                        || itemToCheck.type == ItemID.RocketIV
                        || itemToCheck.type == ItemID.Nanites
                        || itemToCheck.type == ItemID.ClusterRocketI
                        || itemToCheck.type == ItemID.ClusterRocketII
                        || itemToCheck.type == ItemID.MiniNukeI
                        || itemToCheck.type == ItemID.MiniNukeII
                        || itemToCheck.type == ItemID.ProximityMineLauncher
                        || itemToCheck.type == ItemID.PiranhaGun
                        || itemToCheck.type == ItemID.ScourgeoftheCorruptor
                        || itemToCheck.type == ItemID.VampireKnives
                        || itemToCheck.type == ItemID.RainbowGun
                        || itemToCheck.type == ItemID.StaffoftheFrostHydra
                        || itemToCheck.type == ItemID.StormTigerStaff
                        || itemToCheck.type == ItemID.BoneFeather
                        || itemToCheck.type == ItemID.Katana
                        || itemToCheck.type == ItemID.MagnetSphere
                        || itemToCheck.type == ItemID.PaladinsShield
                        || itemToCheck.type == ItemID.PaladinsHammer
                        || itemToCheck.type == ItemID.ShadowbeamStaff
                        || itemToCheck.type == ItemID.SpectreStaff
                        || itemToCheck.type == ItemID.InfernoFork
                        || itemToCheck.type == ItemID.RocketLauncher
                        || itemToCheck.type == ItemID.RifleScope
                        || itemToCheck.type == ItemID.SniperRifle
                        || itemToCheck.type == ItemID.TacticalShotgun
                        || itemToCheck.type == ItemID.BlackBelt
                        || itemToCheck.type == ItemID.Tabi
                        || itemToCheck.type == ItemID.Ectoplasm
                        || itemToCheck.type == ItemID.Kraken
                        || itemToCheck.type == ItemID.MaceWhip
                        || itemToCheck.type == ItemID.ShadowJoustingLance
                        || itemToCheck.type == ItemID.MasterNinjaGear
                        || itemToCheck.type == ItemID.SpectreBar
                        || itemToCheck.type == ItemID.SpectreHood
                        || itemToCheck.type == ItemID.SpectreMask
                        || itemToCheck.type == ItemID.SpectrePants
                        || itemToCheck.type == ItemID.SpectreHamaxe
                        || itemToCheck.type == ItemID.SpectrePickaxe
                        || itemToCheck.type == ItemID.SpectreRobe
                        || itemToCheck.type == ItemID.PumpkinMoonMedallion
                        || itemToCheck.type == ItemID.NaughtyPresent
                        || itemToCheck.type == ItemID.Autohammer
                        || itemToCheck.type == ItemID.Hoverboard
                        || itemToCheck.type == ItemID.ShroomiteDiggingClaw
                        || itemToCheck.type == ItemID.ShroomiteHeadgear
                        || itemToCheck.type == ItemID.ShroomiteHelmet
                        || itemToCheck.type == ItemID.ShroomiteMask
                        || itemToCheck.type == ItemID.ShroomiteBreastplate
                        || itemToCheck.type == ItemID.ShroomiteLeggings
                        || itemToCheck.type == ItemID.ShroomiteBar
                        || itemToCheck.type == ItemID.BoneWings
                        || itemToCheck.type == ItemID.FrozenShield
                        || itemToCheck.type == ItemID.HeroShield
                        || itemToCheck.type == ItemID.TikiMask
                        || itemToCheck.type == ItemID.TikiPants
                        || itemToCheck.type == ItemID.TikiShirt
                        || itemToCheck.type == ItemID.StakeLauncher
                        || itemToCheck.type == ItemID.HerculesBeetle
                        || itemToCheck.type == ItemID.StakeLauncher
                        || itemToCheck.type == ItemID.Stake
                        || itemToCheck.type == ItemID.SpookyWood
                        || itemToCheck.type == ItemID.NecromanticScroll
                        || itemToCheck.type == ItemID.PapyrusScarab
                        || itemToCheck.type == ItemID.SpookyHook
                        || itemToCheck.type == ItemID.WitchBroom
                        || itemToCheck.type == ItemID.TheHorsemansBlade
                        || itemToCheck.type == ItemID.RavenStaff
                        || itemToCheck.type == ItemID.BatScepter
                        || itemToCheck.type == ItemID.CandyCornRifle
                        || itemToCheck.type == ItemID.CandyCorn
                        || itemToCheck.type == ItemID.JackOLanternLauncher
                        || itemToCheck.type == ItemID.ExplosiveJackOLantern
                        || itemToCheck.type == ItemID.BlackFairyDust
                        || itemToCheck.type == ItemID.TatteredFairyWings
                        || itemToCheck.type == ItemID.ScytheWhip
                        || itemToCheck.type == ItemID.ChristmasTreeSword
                        || itemToCheck.type == ItemID.Razorpine
                        || itemToCheck.type == ItemID.FestiveWings
                        || itemToCheck.type == ItemID.ChristmasHook
                        || itemToCheck.type == ItemID.BlizzardStaff
                        || itemToCheck.type == ItemID.NorthPole
                        || itemToCheck.type == ItemID.SnowmanCannon
                        || itemToCheck.type == ItemID.ElfMelter
                        || itemToCheck.type == ItemID.ChainGun
                        || itemToCheck.type == ItemID.SpookyBreastplate
                        || itemToCheck.type == ItemID.SpookyHelmet
                        || itemToCheck.type == ItemID.SpookyLeggings
                        || itemToCheck.type == ItemID.TerraBlade
                        || itemToCheck.type == ItemID.TheEyeOfCthulhu
                        || itemToCheck.type == ItemID.MothronWings
                        || itemToCheck.type == ItemID.BrokenHeroSword
                        || itemToCheck.type == ItemID.DeadlySphereStaff
                        || itemToCheck.type == ItemID.ToxicFlask
                        || itemToCheck.type == ItemID.NailGun
                        || itemToCheck.type == ItemID.Nail
                        || itemToCheck.type == ItemID.PsychoKnife
                        || itemToCheck.type == ItemID.LeafWings
                        || itemToCheck.type == ItemID.GhostWings)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedDuke == false)
                {
                    if (itemToCheck.type == ItemID.AquaScepter
                        || itemToCheck.type == ItemID.RazorbladeTyphoon
                        || itemToCheck.type == ItemID.Flairon
                        || itemToCheck.type == ItemID.TempestStaff
                        || itemToCheck.type == ItemID.Tsunami
                        || itemToCheck.type == ItemID.FishronWings
                        || itemToCheck.type == ItemID.FishronBossBag
                        || itemToCheck.type == ItemID.ShrimpyTruffle)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedOneMech == false)
                {
                    if (itemToCheck.type == ItemID.HallowedBar
                        || itemToCheck.type == ItemID.Jetpack
                        || itemToCheck.type == ItemID.Clentaminator
                        || itemToCheck.type == ItemID.Excalibur
                        || itemToCheck.type == ItemID.SwordWhip
                        || itemToCheck.type == ItemID.Gungnir
                        || itemToCheck.type == ItemID.HallowJoustingLance
                        || itemToCheck.type == ItemID.HallowedRepeater
                        || itemToCheck.type == ItemID.SuperStarCannon
                        || itemToCheck.type == ItemID.HallowedPlateMail
                        || itemToCheck.type == ItemID.HallowedGreaves
                        || itemToCheck.type == ItemID.HallowedMask
                        || itemToCheck.type == ItemID.HallowedHood
                        || itemToCheck.type == ItemID.HallowedHeadgear
                        || itemToCheck.type == ItemID.HallowedHelmet
                        || itemToCheck.type == ItemID.AncientHallowedPlateMail
                        || itemToCheck.type == ItemID.AncientHallowedGreaves
                        || itemToCheck.type == ItemID.AncientHallowedMask
                        || itemToCheck.type == ItemID.AncientHallowedHood
                        || itemToCheck.type == ItemID.AncientHallowedHeadgear
                        || itemToCheck.type == ItemID.AncientHallowedHelmet
                        || itemToCheck.type == ItemID.BatWings
                        || itemToCheck.type == ItemID.BrokenBatWing
                        || itemToCheck.type == ItemID.DeathSickle
                        || itemToCheck.type == ItemID.TatteredBeeWing
                        || itemToCheck.type == ItemID.BeeWings
                        || itemToCheck.type == ItemID.ButterflyDust
                        || itemToCheck.type == ItemID.ButterflyWings
                        || itemToCheck.type == ItemID.FireFeather
                        || itemToCheck.type == ItemID.GroxTheGreatWings
                        || itemToCheck.type == ItemID.FoodBarbarianWings
                        || itemToCheck.type == ItemID.SafemanWings
                        || itemToCheck.type == ItemID.GhostarsWings
                        || itemToCheck.type == ItemID.LeinforsWings
                        || itemToCheck.type == ItemID.ArkhalisWings
                        || itemToCheck.type == ItemID.LokisWings
                        || itemToCheck.type == ItemID.LokisWings
                        || itemToCheck.type == ItemID.SkiphsWings
                        || itemToCheck.type == ItemID.JimsWings
                        || itemToCheck.type == ItemID.Yoraiz0rWings
                        || itemToCheck.type == ItemID.BejeweledValkyrieWing
                        || itemToCheck.type == ItemID.CenxsWings
                        || itemToCheck.type == ItemID.CrownosWings
                        || itemToCheck.type == ItemID.WillsWings
                        || itemToCheck.type == ItemID.DTownsWings
                        || itemToCheck.type == ItemID.RedsWings
                        || itemToCheck.type == ItemID.RedsYoyo
                        || itemToCheck.type == ItemID.Arkhalis
                        || itemToCheck.type == ItemID.LifeFruit
                        || itemToCheck.type == ItemID.AegisFruit
                        || itemToCheck.type == ItemID.Hammush
                        || itemToCheck.type == ItemID.MushroomSpear
                        || itemToCheck.type == ItemID.DD2BallistraTowerT2Popper
                        || itemToCheck.type == ItemID.DD2ExplosiveTrapT2Popper
                        || itemToCheck.type == ItemID.DD2FlameburstTowerT2Popper
                        || itemToCheck.type == ItemID.DD2LightningAuraT2Popper
                        || itemToCheck.type == ItemID.ApprenticeHat
                        || itemToCheck.type == ItemID.ApprenticeRobe
                        || itemToCheck.type == ItemID.BookStaff
                        || itemToCheck.type == ItemID.DD2PhoenixBow
                        || itemToCheck.type == ItemID.DD2SquireDemonSword
                        || itemToCheck.type == ItemID.MonkStaffT1
                        || itemToCheck.type == ItemID.MonkStaffT2
                        || itemToCheck.type == ItemID.HuntressBuckler
                        || itemToCheck.type == ItemID.MonkBelt
                        || itemToCheck.type == ItemID.ApprenticeTrousers
                        || itemToCheck.type == ItemID.MonkBrows
                        || itemToCheck.type == ItemID.MonkShirt
                        || itemToCheck.type == ItemID.MonkPants
                        || itemToCheck.type == ItemID.HuntressWig
                        || itemToCheck.type == ItemID.HuntressPants
                        || itemToCheck.type == ItemID.HuntressJerkin
                        || itemToCheck.type == ItemID.SquireGreatHelm
                        || itemToCheck.type == ItemID.SquireGreaves
                        || itemToCheck.type == ItemID.SquirePlating
                        || itemToCheck.type == ItemID.FlowerofFire)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedAllMechs == false)
                {
                    if (itemToCheck.type == ItemID.PickaxeAxe
                        || itemToCheck.type == ItemID.Drax
                        || itemToCheck.type == ItemID.TrueExcalibur
                        || itemToCheck.type == ItemID.TrueNightsEdge
                        || itemToCheck.type == ItemID.AvengerEmblem
                        || itemToCheck.type == ItemID.ChlorophyteOre
                        || itemToCheck.type == ItemID.ChlorophyteBar
                        || itemToCheck.type == ItemID.ChlorophyteArrow
                        || itemToCheck.type == ItemID.ChlorophyteBullet
                        || itemToCheck.type == ItemID.ChlorophyteChainsaw
                        || itemToCheck.type == ItemID.ChlorophyteClaymore
                        || itemToCheck.type == ItemID.ChlorophyteDrill
                        || itemToCheck.type == ItemID.ChlorophyteExtractinator
                        || itemToCheck.type == ItemID.ChlorophyteGreataxe
                        || itemToCheck.type == ItemID.ChlorophyteGreaves
                        || itemToCheck.type == ItemID.ChlorophyteHeadgear
                        || itemToCheck.type == ItemID.ChlorophyteHelmet
                        || itemToCheck.type == ItemID.ChlorophyteJackhammer
                        || itemToCheck.type == ItemID.ChlorophyteMask
                        || itemToCheck.type == ItemID.ChlorophytePartisan
                        || itemToCheck.type == ItemID.ChlorophytePickaxe
                        || itemToCheck.type == ItemID.ChlorophytePlateMail
                        || itemToCheck.type == ItemID.ChlorophyteSaber
                        || itemToCheck.type == ItemID.ChlorophyteShotbow
                        || itemToCheck.type == ItemID.ChlorophyteWarhammer
                        || itemToCheck.type == ItemID.GolfClubChlorophyteDriver
                        || itemToCheck.type == ItemID.TurtleHelmet
                        || itemToCheck.type == ItemID.TurtleLeggings
                        || itemToCheck.type == ItemID.TurtleScaleMail
                        || itemToCheck.type == ItemID.VenomStaff
                        || itemToCheck.type == ItemID.MechanicalGlove
                        || itemToCheck.type == ItemID.FireGauntlet
                        || itemToCheck.type == ItemID.CelestialEmblem)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedDestroyer == false)
                {
                    if (itemToCheck.type == ItemID.SoulofMight
                        || itemToCheck.type == ItemID.LightDisc
                        || itemToCheck.type == ItemID.Megashark)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedTwins == false)
                {
                    if (itemToCheck.type == ItemID.SoulofSight
                        || itemToCheck.type == ItemID.MagicalHarp
                        || itemToCheck.type == ItemID.OpticStaff
                        || itemToCheck.type == ItemID.RainbowRod)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedPrime == false)
                {
                    if (itemToCheck.type == ItemID.SoulofFright
                        || itemToCheck.type == ItemID.Flamethrower)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedQS == false)
                {
                    if (itemToCheck.type == ItemID.QueenSlimeBossBag
                        || itemToCheck.type == ItemID.VolatileGelatin
                        || itemToCheck.type == ItemID.QueenSlimeHook
                        || itemToCheck.type == ItemID.QueenSlimeMountSaddle
                        || itemToCheck.type == ItemID.Smolstar
                        || itemToCheck.type == ItemID.CrystalNinjaHelmet
                        || itemToCheck.type == ItemID.CrystalNinjaChestplate
                        || itemToCheck.type == ItemID.CrystalNinjaLeggings)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedWOF == false)
                {
                    if (itemToCheck.type == ItemID.MechdusaSummon
                        || itemToCheck.type == ItemID.MechanicalEye
                        || itemToCheck.type == ItemID.MechanicalSkull
                        || itemToCheck.type == ItemID.MechanicalWorm
                        || itemToCheck.type == ItemID.TruffleWorm
                        || itemToCheck.type == ItemID.QueenSlimeCrystal
                        || itemToCheck.type == ItemID.PirateMap
                        || itemToCheck.type == ItemID.PearlwoodBow
                        || itemToCheck.type == ItemID.PearlwoodSword
                        || itemToCheck.type == ItemID.PearlwoodHammer
                        || itemToCheck.type == ItemID.CrystalBall
                        || itemToCheck.type == ItemID.SoulofLight
                        || itemToCheck.type == ItemID.LightKey
                        || itemToCheck.type == ItemID.SoulofNight
                        || itemToCheck.type == ItemID.NightKey
                        || itemToCheck.type == ItemID.GreaterHealingPotion
                        || itemToCheck.type == ItemID.GreaterManaPotion
                        || itemToCheck.type == ItemID.DartPistol
                        || itemToCheck.type == ItemID.DartRifle
                        || itemToCheck.type == ItemID.PutridScent
                        || itemToCheck.type == ItemID.FleshKnuckles
                        || itemToCheck.type == ItemID.WormHook
                        || itemToCheck.type == ItemID.TendonHook
                        || itemToCheck.type == ItemID.ChainGuillotines
                        || itemToCheck.type == ItemID.FetidBaghnakhs
                        || itemToCheck.type == ItemID.ClingerStaff
                        || itemToCheck.type == ItemID.SoulDrain
                        || itemToCheck.type == ItemID.DaedalusStormbow
                        || itemToCheck.type == ItemID.IlluminantHook
                        || itemToCheck.type == ItemID.CrystalVileShard
                        || itemToCheck.type == ItemID.BlessedApple
                        || itemToCheck.type == ItemID.FlyingKnife
                        || itemToCheck.type == ItemID.TitanGlove
                        || itemToCheck.type == ItemID.PowerGlove
                        || itemToCheck.type == ItemID.ArcaneFlower
                        || itemToCheck.type == ItemID.CursedFlames
                        || itemToCheck.type == ItemID.GoldenShower
                        || itemToCheck.type == ItemID.DaoofPow
                        || itemToCheck.type == ItemID.CoolWhip
                        || itemToCheck.type == ItemID.DemonWings
                        || itemToCheck.type == ItemID.OnyxBlaster
                        || itemToCheck.type == ItemID.SpiritFlame
                        || itemToCheck.type == ItemID.CrystalStorm
                        || itemToCheck.type == ItemID.AngelWings
                        || itemToCheck.type == ItemID.Chik
                        || itemToCheck.type == ItemID.MeteorStaff
                        || itemToCheck.type == ItemID.SkyFracture
                        || itemToCheck.type == ItemID.CrystalShard
                        || itemToCheck.type == ItemID.CrystalDart
                        || itemToCheck.type == ItemID.CrystalBullet
                        || itemToCheck.type == ItemID.SuperManaPotion
                        || itemToCheck.type == ItemID.HolyArrow
                        || itemToCheck.type == ItemID.FairyWings
                        || itemToCheck.type == ItemID.PearlsandBlock
                        || itemToCheck.type == ItemID.PearlstoneBlock
                        || itemToCheck.type == ItemID.Pearlwood
                        || itemToCheck.type == ItemID.HolyWater
                        || itemToCheck.type == ItemID.HallowedSeeds
                        || itemToCheck.type == ItemID.JoustingLance
                        || itemToCheck.type == ItemID.ZapinatorOrange
                        || itemToCheck.type == ItemID.SanguineStaff
                        || itemToCheck.type == ItemID.PirateStaff
                        || itemToCheck.type == ItemID.CoinGun
                        || itemToCheck.type == ItemID.PirateShipMountItem
                        || itemToCheck.type == ItemID.DripplerFlail
                        || itemToCheck.type == ItemID.ChainKnife
                        || itemToCheck.type == ItemID.SharpTears
                        || itemToCheck.type == ItemID.SlapHand
                        || itemToCheck.type == ItemID.Bananarang
                        || itemToCheck.type == ItemID.BeamSword
                        || itemToCheck.type == ItemID.Frostbrand
                        || itemToCheck.type == ItemID.SnowballCannon
                        || itemToCheck.type == ItemID.FlowerofFrost
                        || itemToCheck.type == ItemID.DualHook
                        || itemToCheck.type == ItemID.WandofFrosting
                        || itemToCheck.type == ItemID.WandofSparking
                        || itemToCheck.type == ItemID.PhilosophersStone
                        || itemToCheck.type == ItemID.CrossNecklace
                        || itemToCheck.type == ItemID.StarCloak
                        || itemToCheck.type == ItemID.ShadowFlameHexDoll
                        || itemToCheck.type == ItemID.ShadowFlameKnife
                        || itemToCheck.type == ItemID.ShadowFlameBow
                        || itemToCheck.type == ItemID.AdamantiteMask
                        || itemToCheck.type == ItemID.AdamantiteLeggings
                        || itemToCheck.type == ItemID.AdamantiteHelmet
                        || itemToCheck.type == ItemID.AdamantiteHeadgear
                        || itemToCheck.type == ItemID.AdamantiteDrill
                        || itemToCheck.type == ItemID.AdamantiteBreastplate
                        || itemToCheck.type == ItemID.AdamantiteBar
                        || itemToCheck.type == ItemID.AdamantiteOre
                        || itemToCheck.type == ItemID.AdamantiteGlaive
                        || itemToCheck.type == ItemID.AdamantitePickaxe
                        || itemToCheck.type == ItemID.AdamantiteRepeater
                        || itemToCheck.type == ItemID.AdamantiteSword
                        || itemToCheck.type == ItemID.AdamantiteWaraxe
                        || itemToCheck.type == ItemID.TitaniumBar
                        || itemToCheck.type == ItemID.TitaniumOre
                        || itemToCheck.type == ItemID.TitaniumLeggings
                        || itemToCheck.type == ItemID.TitaniumBreastplate
                        || itemToCheck.type == ItemID.TitaniumChainsaw
                        || itemToCheck.type == ItemID.TitaniumHeadgear
                        || itemToCheck.type == ItemID.TitaniumHelmet
                        || itemToCheck.type == ItemID.TitaniumMask
                        || itemToCheck.type == ItemID.TitaniumTrident
                        || itemToCheck.type == ItemID.TitaniumRepeater
                        || itemToCheck.type == ItemID.TitaniumSword
                        || itemToCheck.type == ItemID.TitaniumWaraxe
                        || itemToCheck.type == ItemID.TitaniumDrill
                        || itemToCheck.type == ItemID.TitaniumPickaxe
                        || itemToCheck.type == ItemID.AncientBattleArmorHat
                        || itemToCheck.type == ItemID.AncientBattleArmorShirt
                        || itemToCheck.type == ItemID.AncientBattleArmorPants
                        || itemToCheck.type == ItemID.AncientBattleArmorMaterial
                        || itemToCheck.type == ItemID.FrostCore
                        || itemToCheck.type == ItemID.FrostHelmet
                        || itemToCheck.type == ItemID.FrostLeggings
                        || itemToCheck.type == ItemID.FrostBreastplate
                        || itemToCheck.type == ItemID.FrozenTurtleShell
                        || itemToCheck.type == ItemID.FrozenWings
                        || itemToCheck.type == ItemID.FinWings
                        || itemToCheck.type == ItemID.HarpyWings
                        || itemToCheck.type == ItemID.CrystalSerpent
                        || itemToCheck.type == ItemID.OrichalcumBar
                        || itemToCheck.type == ItemID.OrichalcumBreastplate
                        || itemToCheck.type == ItemID.OrichalcumChainsaw
                        || itemToCheck.type == ItemID.OrichalcumDrill
                        || itemToCheck.type == ItemID.OrichalcumHalberd
                        || itemToCheck.type == ItemID.OrichalcumHeadgear
                        || itemToCheck.type == ItemID.OrichalcumHelmet
                        || itemToCheck.type == ItemID.OrichalcumLeggings
                        || itemToCheck.type == ItemID.OrichalcumMask
                        || itemToCheck.type == ItemID.OrichalcumOre
                        || itemToCheck.type == ItemID.OrichalcumPickaxe
                        || itemToCheck.type == ItemID.OrichalcumRepeater
                        || itemToCheck.type == ItemID.OrichalcumSword
                        || itemToCheck.type == ItemID.OrichalcumWaraxe
                        || itemToCheck.type == ItemID.MythrilBar
                        || itemToCheck.type == ItemID.MythrilChainmail
                        || itemToCheck.type == ItemID.MythrilChainsaw
                        || itemToCheck.type == ItemID.MythrilDrill
                        || itemToCheck.type == ItemID.MythrilGreaves
                        || itemToCheck.type == ItemID.MythrilHalberd
                        || itemToCheck.type == ItemID.MythrilHat
                        || itemToCheck.type == ItemID.MythrilHelmet
                        || itemToCheck.type == ItemID.MythrilHood
                        || itemToCheck.type == ItemID.MythrilOre
                        || itemToCheck.type == ItemID.MythrilPickaxe
                        || itemToCheck.type == ItemID.MythrilRepeater
                        || itemToCheck.type == ItemID.MythrilSword
                        || itemToCheck.type == ItemID.MythrilWaraxe
                        || itemToCheck.type == ItemID.CobaltBar
                        || itemToCheck.type == ItemID.CobaltBreastplate
                        || itemToCheck.type == ItemID.CobaltChainsaw
                        || itemToCheck.type == ItemID.CobaltDrill
                        || itemToCheck.type == ItemID.CobaltHat
                        || itemToCheck.type == ItemID.CobaltHelmet
                        || itemToCheck.type == ItemID.CobaltLeggings
                        || itemToCheck.type == ItemID.CobaltMask
                        || itemToCheck.type == ItemID.CobaltNaginata
                        || itemToCheck.type == ItemID.CobaltOre
                        || itemToCheck.type == ItemID.CobaltPickaxe
                        || itemToCheck.type == ItemID.CobaltRepeater
                        || itemToCheck.type == ItemID.CobaltSword
                        || itemToCheck.type == ItemID.CobaltWaraxe
                        || itemToCheck.type == ItemID.PalladiumBar
                        || itemToCheck.type == ItemID.PalladiumBreastplate
                        || itemToCheck.type == ItemID.PalladiumChainsaw
                        || itemToCheck.type == ItemID.PalladiumDrill
                        || itemToCheck.type == ItemID.PalladiumHeadgear
                        || itemToCheck.type == ItemID.PalladiumHelmet
                        || itemToCheck.type == ItemID.PalladiumLeggings
                        || itemToCheck.type == ItemID.PalladiumMask
                        || itemToCheck.type == ItemID.PalladiumOre
                        || itemToCheck.type == ItemID.PalladiumPickaxe
                        || itemToCheck.type == ItemID.PalladiumPike
                        || itemToCheck.type == ItemID.PalladiumRepeater
                        || itemToCheck.type == ItemID.PalladiumSword
                        || itemToCheck.type == ItemID.PalladiumWaraxe
                        || itemToCheck.type == ItemID.CursedArrow
                        || itemToCheck.type == ItemID.CursedFlames
                        || itemToCheck.type == ItemID.CursedFlare
                        || itemToCheck.type == ItemID.CursedDart
                        || itemToCheck.type == ItemID.CursedBullet
                        || itemToCheck.type == ItemID.Ichor
                        || itemToCheck.type == ItemID.IchorBullet
                        || itemToCheck.type == ItemID.IchorDart
                        || itemToCheck.type == ItemID.IchorDart
                        || itemToCheck.type == ItemID.FlaskofIchor
                        || itemToCheck.type == ItemID.FlaskofCursedFlames
                        || itemToCheck.type == ItemID.SpiderFang
                        || itemToCheck.type == ItemID.SpiderMask
                        || itemToCheck.type == ItemID.SpiderBreastplate
                        || itemToCheck.type == ItemID.SpiderGreaves
                        || itemToCheck.type == ItemID.SpiderStaff
                        || itemToCheck.type == ItemID.QueenSpiderStaff
                        || itemToCheck.type == ItemID.FireWhip
                        || itemToCheck.type == ItemID.BreakerBlade
                        || itemToCheck.type == ItemID.ClockworkAssaultRifle
                        || itemToCheck.type == ItemID.LaserRifle
                        || itemToCheck.type == ItemID.MoonCharm
                        || itemToCheck.type == ItemID.MedusaHead
                        || itemToCheck.type == ItemID.PoisonStaff
                        || itemToCheck.type == ItemID.ManaCloak
                        || itemToCheck.type == ItemID.RangerEmblem
                        || itemToCheck.type == ItemID.SorcererEmblem
                        || itemToCheck.type == ItemID.SummonerEmblem
                        || itemToCheck.type == ItemID.WarriorEmblem
                        || itemToCheck.type == ItemID.DemonHeart
                        || itemToCheck.type == ItemID.Gatligator
                        || itemToCheck.type == ItemID.Uzi
                        || itemToCheck.type == ItemID.Shotgun
                        || itemToCheck.type == ItemID.Toxikarp
                        || itemToCheck.type == ItemID.Bladetongue
                        || itemToCheck.type == ItemID.MagicQuiver
                        || itemToCheck.type == ItemID.MoltenQuiver
                        || itemToCheck.type == ItemID.StalkersQuiver
                        || itemToCheck.type == ItemID.BluePhasesaber
                        || itemToCheck.type == ItemID.BluePhasesaberOld
                        || itemToCheck.type == ItemID.GreenPhasesaber
                        || itemToCheck.type == ItemID.GreenPhasesaberOld
                        || itemToCheck.type == ItemID.OrangePhasesaber
                        || itemToCheck.type == ItemID.PurplePhasesaber
                        || itemToCheck.type == ItemID.PurplePhasesaberOld
                        || itemToCheck.type == ItemID.RedPhasesaber
                        || itemToCheck.type == ItemID.RedPhasesaberOld
                        || itemToCheck.type == ItemID.WhitePhasesaber
                        || itemToCheck.type == ItemID.WhitePhasesaberOld
                        || itemToCheck.type == ItemID.YellowPhasesaber
                        || itemToCheck.type == ItemID.YellowPhasesaberOld
                        || itemToCheck.type == ItemID.IceSickle
                        || itemToCheck.type == ItemID.HamBat
                        || itemToCheck.type == ItemID.Cutlass
                        || itemToCheck.type == ItemID.FormatC
                        || itemToCheck.type == ItemID.Gradient
                        || itemToCheck.type == ItemID.HelFire
                        || itemToCheck.type == ItemID.Amarok
                        || itemToCheck.type == ItemID.Yelets
                        || itemToCheck.type == ItemID.Code2
                        || itemToCheck.type == ItemID.ObsidianSwordfish
                        || itemToCheck.type == ItemID.BouncingShield
                        || itemToCheck.type == ItemID.Anchor
                        || itemToCheck.type == ItemID.CorruptFishingCrateHard
                        || itemToCheck.type == ItemID.CrimsonFishingCrateHard
                        || itemToCheck.type == ItemID.DungeonFishingCrateHard
                        || itemToCheck.type == ItemID.FloatingIslandFishingCrateHard
                        || itemToCheck.type == ItemID.FrozenCrateHard
                        || itemToCheck.type == ItemID.GoldenCrateHard
                        || itemToCheck.type == ItemID.HallowedFishingCrateHard
                        || itemToCheck.type == ItemID.IronCrateHard
                        || itemToCheck.type == ItemID.JungleFishingCrateHard
                        || itemToCheck.type == ItemID.LavaCrateHard
                        || itemToCheck.type == ItemID.OasisCrateHard
                        || itemToCheck.type == ItemID.OceanCrateHard
                        || itemToCheck.type == ItemID.WoodenCrateHard
                        || itemToCheck.type == ItemID.FlowerofFire)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedSkeletron == false)
                {
                    if (itemToCheck.type == ItemID.ShadowKey
                        || itemToCheck.type == ItemID.HellwingBow
                        || itemToCheck.type == ItemID.Sunfury
                        || itemToCheck.type == ItemID.FlowerofFire
                        || itemToCheck.type == ItemID.Flamelash
                        || itemToCheck.type == ItemID.DarkLance
                        || itemToCheck.type == ItemID.TreasureMagnet)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedQB == false)
                {
                    if (itemToCheck.type == ItemID.PygmyNecklace
                        || itemToCheck.type == ItemID.HiveBackpack
                        || itemToCheck.type == ItemID.BeeGun
                        || itemToCheck.type == ItemID.BeeKeeper
                        || itemToCheck.type == ItemID.BeesKnees
                        || itemToCheck.type == ItemID.QueenBeeBossBag
                        || itemToCheck.type == ItemID.HoneyComb
                        || itemToCheck.type == ItemID.BeeWax
                        || itemToCheck.type == ItemID.BeeGreaves
                        || itemToCheck.type == ItemID.BeeCloak
                        || itemToCheck.type == ItemID.BeeBreastplate
                        || itemToCheck.type == ItemID.BeeHeadgear
                        || itemToCheck.type == ItemID.Beenade
                        || itemToCheck.type == ItemID.HiveFive
                        || itemToCheck.type == ItemID.HornetStaff
                        || itemToCheck.type == ItemID.HoneyBalloon
                        || itemToCheck.type == ItemID.StingerNecklace
                        || itemToCheck.type == ItemID.SweetheartNecklace)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedEvil == false)
                {
                    if (itemToCheck.type == ItemID.ShadowScale
                        || itemToCheck.type == ItemID.WormScarf
                        || itemToCheck.type == ItemID.EaterOfWorldsBossBag
                        || itemToCheck.type == ItemID.BrainOfConfusion
                        || itemToCheck.type == ItemID.BrainOfCthulhuBossBag
                        || itemToCheck.type == ItemID.TissueSample
                        || itemToCheck.type == ItemID.Hellstone
                        || itemToCheck.type == ItemID.HellstoneBar
                        || itemToCheck.type == ItemID.FireproofBugNet
                        || itemToCheck.type == ItemID.Flamarang
                        || itemToCheck.type == ItemID.ImpStaff
                        || itemToCheck.type == ItemID.MoltenFury
                        || itemToCheck.type == ItemID.MoltenHamaxe
                        || itemToCheck.type == ItemID.PhoenixBlaster
                        || itemToCheck.type == ItemID.FieryGreatsword
                        || itemToCheck.type == ItemID.MoltenPickaxe
                        || itemToCheck.type == ItemID.MoltenHelmet
                        || itemToCheck.type == ItemID.MoltenGreaves
                        || itemToCheck.type == ItemID.MoltenBreastplate)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedDeer == false)
                {
                    if (itemToCheck.type == ItemID.BoneHelm
                        || itemToCheck.type == ItemID.PewMaticHorn
                        || itemToCheck.type == ItemID.WeatherPain
                        || itemToCheck.type == ItemID.HoundiusShootius
                        || itemToCheck.type == ItemID.LucyTheAxe
                        || itemToCheck.type == ItemID.DeerclopsBossBag)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedEoc == false)
                {
                    if (itemToCheck.type == ItemID.EyeOfCthulhuBossBag
                        || itemToCheck.type == ItemID.EoCShield)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedGoblins == false)
                {
                    if (itemToCheck.type == ItemID.TinkerersWorkshop)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedSlimeKing == false)
                {
                    if (itemToCheck.type == ItemID.KingSlimeBossBag)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                #endregion
            };

            if (Main.zenithWorld == false)
            {
                #region ProgressionLock
                BossStatus config = LoadConfig();
                //Here is the general items that will be automatically removed if the boss or event isn't done.
                if (config.downedML == false)
                {
                    if (itemToCheck.type == ItemID.Zenith
                        || itemToCheck.type == ItemID.LastPrism
                        || itemToCheck.type == ItemID.Terrarian
                        || itemToCheck.type == ItemID.Meowmere
                        || itemToCheck.type == ItemID.StarWrath
                        || itemToCheck.type == ItemID.SDMG
                        || itemToCheck.type == ItemID.LunarFlareBook
                        || itemToCheck.type == ItemID.RainbowCrystalStaff
                        || itemToCheck.type == ItemID.MoonlordTurretStaff
                        || itemToCheck.type == ItemID.Celeb2
                        || itemToCheck.type == ItemID.LunarOre
                        || itemToCheck.type == ItemID.LunarBar
                        || itemToCheck.type == ItemID.MoonlordArrow
                        || itemToCheck.type == ItemID.MoonlordBullet
                        || itemToCheck.type == ItemID.MoonLordBossBag
                        || itemToCheck.type == ItemID.PortalGun
                        || itemToCheck.type == ItemID.LongRainbowTrailWings
                        || itemToCheck.type == ItemID.NebulaBreastplate
                        || itemToCheck.type == ItemID.NebulaHelmet
                        || itemToCheck.type == ItemID.NebulaLeggings
                        || itemToCheck.type == ItemID.NebulaPickaxe
                        || itemToCheck.type == ItemID.NebulaDrill
                        || itemToCheck.type == ItemID.LunarHamaxeNebula
                        || itemToCheck.type == ItemID.WingsNebula
                        || itemToCheck.type == ItemID.NebulaAxe
                        || itemToCheck.type == ItemID.NebulaHammer
                        || itemToCheck.type == ItemID.NebulaChainsaw
                        || itemToCheck.type == ItemID.SolarFlareAxe
                        || itemToCheck.type == ItemID.SolarFlareHelmet
                        || itemToCheck.type == ItemID.SolarFlareLeggings
                        || itemToCheck.type == ItemID.SolarFlarePickaxe
                        || itemToCheck.type == ItemID.SolarFlareBreastplate
                        || itemToCheck.type == ItemID.SolarFlareDrill
                        || itemToCheck.type == ItemID.SolarFlareHelmet
                        || itemToCheck.type == ItemID.WingsSolar
                        || itemToCheck.type == ItemID.LunarHamaxeSolar
                        || itemToCheck.type == ItemID.StardustAxe
                        || itemToCheck.type == ItemID.StardustPickaxe
                        || itemToCheck.type == ItemID.StardustDrill
                        || itemToCheck.type == ItemID.StardustHammer
                        || itemToCheck.type == ItemID.LunarHamaxeStardust
                        || itemToCheck.type == ItemID.WingsStardust
                        || itemToCheck.type == ItemID.StardustHelmet
                        || itemToCheck.type == ItemID.StardustBreastplate
                        || itemToCheck.type == ItemID.StardustLeggings
                        || itemToCheck.type == ItemID.StardustChainsaw
                        || itemToCheck.type == ItemID.WingsVortex
                        || itemToCheck.type == ItemID.LunarHamaxeVortex
                        || itemToCheck.type == ItemID.VortexAxe
                        || itemToCheck.type == ItemID.VortexChainsaw
                        || itemToCheck.type == ItemID.VortexDrill
                        || itemToCheck.type == ItemID.VortexPickaxe
                        || itemToCheck.type == ItemID.VortexHelmet
                        || itemToCheck.type == ItemID.VortexBreastplate
                        || itemToCheck.type == ItemID.VortexLeggings
                        || itemToCheck.type == ItemID.DrillContainmentUnit
                        || itemToCheck.type == ItemID.BottomlessShimmerBucket
                        || itemToCheck.type == ItemID.RodOfHarmony
                        || itemToCheck.type == ItemID.Clentaminator2
                        || itemToCheck.type == ItemID.GravityGlobe)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);                      
                    }
                }
                if (config.downedAllPillars == false)
                {
                    if (itemToCheck.type == ItemID.LunarHook
                        || itemToCheck.type == ItemID.CelestialSigil
                        || itemToCheck.type == ItemID.SuperHealingPotion)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedNebula == false)
                {
                    if (itemToCheck.type == ItemID.NebulaArcanum
                        || itemToCheck.type == ItemID.NebulaBlaze
                        || itemToCheck.type == ItemID.FragmentNebula)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedSolar == false)
                {
                    if (itemToCheck.type == ItemID.FragmentSolar
                        || itemToCheck.type == ItemID.SolarEruption
                        || itemToCheck.type == ItemID.DayBreak)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedStardust == false)
                {
                    if (itemToCheck.type == ItemID.FragmentStardust
                        || itemToCheck.type == ItemID.StardustDragonStaff
                        || itemToCheck.type == ItemID.StardustCellStaff)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedVortex == false)
                {
                    if (itemToCheck.type == ItemID.FragmentVortex
                        || itemToCheck.type == ItemID.VortexBeater
                        || itemToCheck.type == ItemID.Phantasm)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedLC == false)
                {
                    if (itemToCheck.type == ItemID.CultistBossBag)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedGolem == false)
                {
                    if (itemToCheck.type == ItemID.Xenopopper
                        || itemToCheck.type == ItemID.XenoStaff
                        || itemToCheck.type == ItemID.LaserMachinegun
                        || itemToCheck.type == ItemID.ElectrosphereLauncher
                        || itemToCheck.type == ItemID.InfluxWaver
                        || itemToCheck.type == ItemID.CosmicCarKey
                        || itemToCheck.type == ItemID.BrainScrambler
                        || itemToCheck.type == ItemID.LaserDrill
                        || itemToCheck.type == ItemID.AntiGravityHook
                        || itemToCheck.type == ItemID.ChargedBlasterCannon
                        || itemToCheck.type == ItemID.GolemBossBag
                        || itemToCheck.type == ItemID.Picksaw
                        || itemToCheck.type == ItemID.ShinyStone
                        || itemToCheck.type == ItemID.Stynger
                        || itemToCheck.type == ItemID.PossessedHatchet
                        || itemToCheck.type == ItemID.SunStone
                        || itemToCheck.type == ItemID.HeatRay
                        || itemToCheck.type == ItemID.StaffofEarth
                        || itemToCheck.type == ItemID.EyeoftheGolem
                        || itemToCheck.type == ItemID.GolemFist
                        || itemToCheck.type == ItemID.BeetleHusk
                        || itemToCheck.type == ItemID.BeetleHelmet
                        || itemToCheck.type == ItemID.BeetleShell
                        || itemToCheck.type == ItemID.BeetleScaleMail
                        || itemToCheck.type == ItemID.BeetleLeggings
                        || itemToCheck.type == ItemID.BeetleWings
                        || itemToCheck.type == ItemID.CelestialShell
                        || itemToCheck.type == ItemID.CelestialStone
                        || itemToCheck.type == ItemID.DestroyerEmblem
                        || itemToCheck.type == ItemID.SniperScope
                        || itemToCheck.type == ItemID.ReconScope
                        || itemToCheck.type == ItemID.StyngerBolt
                        || itemToCheck.type == ItemID.BetsyWings
                        || itemToCheck.type == ItemID.BossBagBetsy
                        || itemToCheck.type == ItemID.DD2BetsyBow
                        || itemToCheck.type == ItemID.DD2SquireBetsySword
                        || itemToCheck.type == ItemID.MonkStaffT3
                        || itemToCheck.type == ItemID.ApprenticeStaffT3
                        || itemToCheck.type == ItemID.ApprenticeAltHead
                        || itemToCheck.type == ItemID.ApprenticeAltShirt
                        || itemToCheck.type == ItemID.ApprenticeAltPants
                        || itemToCheck.type == ItemID.HuntressAltHead
                        || itemToCheck.type == ItemID.HuntressAltPants
                        || itemToCheck.type == ItemID.HuntressAltShirt
                        || itemToCheck.type == ItemID.MonkAltHead
                        || itemToCheck.type == ItemID.MonkAltPants
                        || itemToCheck.type == ItemID.MonkAltShirt
                        || itemToCheck.type == ItemID.SquireAltHead
                        || itemToCheck.type == ItemID.SquireAltPants
                        || itemToCheck.type == ItemID.SquireAltShirt
                        || itemToCheck.type == ItemID.DD2LightningAuraT3Popper
                        || itemToCheck.type == ItemID.DD2ExplosiveTrapT3Popper
                        || itemToCheck.type == ItemID.DD2BallistraTowerT3Popper
                        || itemToCheck.type == ItemID.DD2FlameburstTowerT3Popper
                        || itemToCheck.type == ItemID.SteampunkWings)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedEOL == false)
                {
                    if (itemToCheck.type == ItemID.EmpressBlade
                        || itemToCheck.type == ItemID.EmpressFlightBooster
                        || itemToCheck.type == ItemID.FairyQueenBossBag
                        || itemToCheck.type == ItemID.FairyQueenRangedItem
                        || itemToCheck.type == ItemID.FairyQueenMagicItem
                        || itemToCheck.type == ItemID.RainbowWhip
                        || itemToCheck.type == ItemID.RainbowWings
                        || itemToCheck.type == ItemID.SparkleGuitar
                        || itemToCheck.type == ItemID.PiercingStarlight)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedPlantera == false)
                {
                    if (itemToCheck.type == ItemID.EmpressButterfly
                        || itemToCheck.type == ItemID.EmpressButterflyJar
                        || itemToCheck.type == ItemID.GrenadeLauncher
                        || itemToCheck.type == ItemID.VenusMagnum
                        || itemToCheck.type == ItemID.NettleBurst
                        || itemToCheck.type == ItemID.LeafBlower
                        || itemToCheck.type == ItemID.FlowerPow
                        || itemToCheck.type == ItemID.WaspGun
                        || itemToCheck.type == ItemID.Seedler
                        || itemToCheck.type == ItemID.PygmyStaff
                        || itemToCheck.type == ItemID.ThornHook
                        || itemToCheck.type == ItemID.TheAxe
                        || itemToCheck.type == ItemID.SporeSac
                        || itemToCheck.type == ItemID.TempleKey
                        || itemToCheck.type == ItemID.RocketI
                        || itemToCheck.type == ItemID.RocketII
                        || itemToCheck.type == ItemID.RocketIII
                        || itemToCheck.type == ItemID.RocketIV
                        || itemToCheck.type == ItemID.Nanites
                        || itemToCheck.type == ItemID.ClusterRocketI
                        || itemToCheck.type == ItemID.ClusterRocketII
                        || itemToCheck.type == ItemID.MiniNukeI
                        || itemToCheck.type == ItemID.MiniNukeII
                        || itemToCheck.type == ItemID.ProximityMineLauncher
                        || itemToCheck.type == ItemID.PiranhaGun
                        || itemToCheck.type == ItemID.ScourgeoftheCorruptor
                        || itemToCheck.type == ItemID.VampireKnives
                        || itemToCheck.type == ItemID.RainbowGun
                        || itemToCheck.type == ItemID.StaffoftheFrostHydra
                        || itemToCheck.type == ItemID.StormTigerStaff
                        || itemToCheck.type == ItemID.BoneFeather
                        || itemToCheck.type == ItemID.Keybrand
                        || itemToCheck.type == ItemID.MagnetSphere
                        || itemToCheck.type == ItemID.PaladinsShield
                        || itemToCheck.type == ItemID.PaladinsHammer
                        || itemToCheck.type == ItemID.ShadowbeamStaff
                        || itemToCheck.type == ItemID.SpectreStaff
                        || itemToCheck.type == ItemID.InfernoFork
                        || itemToCheck.type == ItemID.RocketLauncher
                        || itemToCheck.type == ItemID.RifleScope
                        || itemToCheck.type == ItemID.SniperRifle
                        || itemToCheck.type == ItemID.TacticalShotgun
                        || itemToCheck.type == ItemID.BlackBelt
                        || itemToCheck.type == ItemID.Tabi
                        || itemToCheck.type == ItemID.Ectoplasm
                        || itemToCheck.type == ItemID.Kraken
                        || itemToCheck.type == ItemID.MaceWhip
                        || itemToCheck.type == ItemID.ShadowJoustingLance
                        || itemToCheck.type == ItemID.MasterNinjaGear
                        || itemToCheck.type == ItemID.SpectreBar
                        || itemToCheck.type == ItemID.SpectreHood
                        || itemToCheck.type == ItemID.SpectreMask
                        || itemToCheck.type == ItemID.SpectrePants
                        || itemToCheck.type == ItemID.SpectreHamaxe
                        || itemToCheck.type == ItemID.SpectrePickaxe
                        || itemToCheck.type == ItemID.SpectreRobe
                        || itemToCheck.type == ItemID.PumpkinMoonMedallion
                        || itemToCheck.type == ItemID.NaughtyPresent
                        || itemToCheck.type == ItemID.Autohammer
                        || itemToCheck.type == ItemID.Hoverboard
                        || itemToCheck.type == ItemID.ShroomiteDiggingClaw
                        || itemToCheck.type == ItemID.ShroomiteHeadgear
                        || itemToCheck.type == ItemID.ShroomiteHelmet
                        || itemToCheck.type == ItemID.ShroomiteMask
                        || itemToCheck.type == ItemID.ShroomiteBreastplate
                        || itemToCheck.type == ItemID.ShroomiteLeggings
                        || itemToCheck.type == ItemID.ShroomiteBar
                        || itemToCheck.type == ItemID.BoneWings
                        || itemToCheck.type == ItemID.FrozenShield
                        || itemToCheck.type == ItemID.HeroShield
                        || itemToCheck.type == ItemID.TikiMask
                        || itemToCheck.type == ItemID.TikiPants
                        || itemToCheck.type == ItemID.TikiShirt
                        || itemToCheck.type == ItemID.StakeLauncher
                        || itemToCheck.type == ItemID.HerculesBeetle
                        || itemToCheck.type == ItemID.StakeLauncher
                        || itemToCheck.type == ItemID.Stake
                        || itemToCheck.type == ItemID.SpookyWood
                        || itemToCheck.type == ItemID.NecromanticScroll
                        || itemToCheck.type == ItemID.PapyrusScarab
                        || itemToCheck.type == ItemID.SpookyHook
                        || itemToCheck.type == ItemID.WitchBroom
                        || itemToCheck.type == ItemID.TheHorsemansBlade
                        || itemToCheck.type == ItemID.RavenStaff
                        || itemToCheck.type == ItemID.BatScepter
                        || itemToCheck.type == ItemID.CandyCornRifle
                        || itemToCheck.type == ItemID.CandyCorn
                        || itemToCheck.type == ItemID.JackOLanternLauncher
                        || itemToCheck.type == ItemID.ExplosiveJackOLantern
                        || itemToCheck.type == ItemID.BlackFairyDust
                        || itemToCheck.type == ItemID.TatteredFairyWings
                        || itemToCheck.type == ItemID.ScytheWhip
                        || itemToCheck.type == ItemID.ChristmasTreeSword
                        || itemToCheck.type == ItemID.Razorpine
                        || itemToCheck.type == ItemID.FestiveWings
                        || itemToCheck.type == ItemID.ChristmasHook
                        || itemToCheck.type == ItemID.BlizzardStaff
                        || itemToCheck.type == ItemID.NorthPole
                        || itemToCheck.type == ItemID.SnowmanCannon
                        || itemToCheck.type == ItemID.ElfMelter
                        || itemToCheck.type == ItemID.ChainGun
                        || itemToCheck.type == ItemID.SpookyBreastplate
                        || itemToCheck.type == ItemID.SpookyHelmet
                        || itemToCheck.type == ItemID.SpookyLeggings
                        || itemToCheck.type == ItemID.TerraBlade
                        || itemToCheck.type == ItemID.TheEyeOfCthulhu
                        || itemToCheck.type == ItemID.MothronWings
                        || itemToCheck.type == ItemID.BrokenHeroSword
                        || itemToCheck.type == ItemID.DeadlySphereStaff
                        || itemToCheck.type == ItemID.ToxicFlask
                        || itemToCheck.type == ItemID.NailGun
                        || itemToCheck.type == ItemID.Nail
                        || itemToCheck.type == ItemID.PsychoKnife
                        || itemToCheck.type == ItemID.LeafWings
                        || itemToCheck.type == ItemID.GhostWings)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedDuke == false)
                {
                    if (itemToCheck.type == ItemID.BubbleGun
                        || itemToCheck.type == ItemID.RazorbladeTyphoon
                        || itemToCheck.type == ItemID.Flairon
                        || itemToCheck.type == ItemID.TempestStaff
                        || itemToCheck.type == ItemID.Tsunami
                        || itemToCheck.type == ItemID.FishronWings
                        || itemToCheck.type == ItemID.FishronBossBag
                        || itemToCheck.type == ItemID.ShrimpyTruffle)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedOneMech == false)
                {
                    if (itemToCheck.type == ItemID.HallowedBar
                        || itemToCheck.type == ItemID.Jetpack
                        || itemToCheck.type == ItemID.Clentaminator
                        || itemToCheck.type == ItemID.Excalibur
                        || itemToCheck.type == ItemID.SwordWhip
                        || itemToCheck.type == ItemID.Gungnir
                        || itemToCheck.type == ItemID.HallowJoustingLance
                        || itemToCheck.type == ItemID.HallowedRepeater
                        || itemToCheck.type == ItemID.SuperStarCannon
                        || itemToCheck.type == ItemID.HallowedPlateMail
                        || itemToCheck.type == ItemID.HallowedGreaves
                        || itemToCheck.type == ItemID.HallowedMask
                        || itemToCheck.type == ItemID.HallowedHood
                        || itemToCheck.type == ItemID.HallowedHeadgear
                        || itemToCheck.type == ItemID.HallowedHelmet
                        || itemToCheck.type == ItemID.AncientHallowedPlateMail
                        || itemToCheck.type == ItemID.AncientHallowedGreaves
                        || itemToCheck.type == ItemID.AncientHallowedMask
                        || itemToCheck.type == ItemID.AncientHallowedHood
                        || itemToCheck.type == ItemID.AncientHallowedHeadgear
                        || itemToCheck.type == ItemID.AncientHallowedHelmet
                        || itemToCheck.type == ItemID.BatWings
                        || itemToCheck.type == ItemID.BrokenBatWing
                        || itemToCheck.type == ItemID.DeathSickle
                        || itemToCheck.type == ItemID.TatteredBeeWing
                        || itemToCheck.type == ItemID.BeeWings
                        || itemToCheck.type == ItemID.ButterflyDust
                        || itemToCheck.type == ItemID.ButterflyWings
                        || itemToCheck.type == ItemID.FireFeather
                        || itemToCheck.type == ItemID.GroxTheGreatWings
                        || itemToCheck.type == ItemID.FoodBarbarianWings
                        || itemToCheck.type == ItemID.SafemanWings
                        || itemToCheck.type == ItemID.GhostarsWings
                        || itemToCheck.type == ItemID.LeinforsWings
                        || itemToCheck.type == ItemID.ArkhalisWings
                        || itemToCheck.type == ItemID.LokisWings
                        || itemToCheck.type == ItemID.LokisWings
                        || itemToCheck.type == ItemID.SkiphsWings
                        || itemToCheck.type == ItemID.JimsWings
                        || itemToCheck.type == ItemID.Yoraiz0rWings
                        || itemToCheck.type == ItemID.BejeweledValkyrieWing
                        || itemToCheck.type == ItemID.CenxsWings
                        || itemToCheck.type == ItemID.CrownosWings
                        || itemToCheck.type == ItemID.WillsWings
                        || itemToCheck.type == ItemID.DTownsWings
                        || itemToCheck.type == ItemID.RedsWings
                        || itemToCheck.type == ItemID.RedsYoyo
                        || itemToCheck.type == ItemID.Arkhalis
                        || itemToCheck.type == ItemID.LifeFruit
                        || itemToCheck.type == ItemID.AegisFruit
                        || itemToCheck.type == ItemID.Hammush
                        || itemToCheck.type == ItemID.MushroomSpear
                        || itemToCheck.type == ItemID.DD2BallistraTowerT2Popper
                        || itemToCheck.type == ItemID.DD2ExplosiveTrapT2Popper
                        || itemToCheck.type == ItemID.DD2FlameburstTowerT2Popper
                        || itemToCheck.type == ItemID.DD2LightningAuraT2Popper
                        || itemToCheck.type == ItemID.ApprenticeHat
                        || itemToCheck.type == ItemID.ApprenticeRobe
                        || itemToCheck.type == ItemID.BookStaff
                        || itemToCheck.type == ItemID.DD2PhoenixBow
                        || itemToCheck.type == ItemID.DD2SquireDemonSword
                        || itemToCheck.type == ItemID.MonkStaffT1
                        || itemToCheck.type == ItemID.MonkStaffT2
                        || itemToCheck.type == ItemID.HuntressBuckler
                        || itemToCheck.type == ItemID.MonkBelt
                        || itemToCheck.type == ItemID.ApprenticeTrousers
                        || itemToCheck.type == ItemID.MonkBrows
                        || itemToCheck.type == ItemID.MonkShirt
                        || itemToCheck.type == ItemID.MonkPants
                        || itemToCheck.type == ItemID.HuntressWig
                        || itemToCheck.type == ItemID.HuntressPants
                        || itemToCheck.type == ItemID.HuntressJerkin
                        || itemToCheck.type == ItemID.SquireGreatHelm
                        || itemToCheck.type == ItemID.SquireGreaves
                        || itemToCheck.type == ItemID.SquirePlating
                        || itemToCheck.type == ItemID.UnholyTrident)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedAllMechs == false)
                {
                    if (itemToCheck.type == ItemID.PickaxeAxe
                        || itemToCheck.type == ItemID.Drax
                        || itemToCheck.type == ItemID.TrueExcalibur
                        || itemToCheck.type == ItemID.TrueNightsEdge
                        || itemToCheck.type == ItemID.AvengerEmblem
                        || itemToCheck.type == ItemID.ChlorophyteOre
                        || itemToCheck.type == ItemID.ChlorophyteBar
                        || itemToCheck.type == ItemID.ChlorophyteArrow
                        || itemToCheck.type == ItemID.ChlorophyteBullet
                        || itemToCheck.type == ItemID.ChlorophyteChainsaw
                        || itemToCheck.type == ItemID.ChlorophyteClaymore
                        || itemToCheck.type == ItemID.ChlorophyteDrill
                        || itemToCheck.type == ItemID.ChlorophyteExtractinator
                        || itemToCheck.type == ItemID.ChlorophyteGreataxe
                        || itemToCheck.type == ItemID.ChlorophyteGreaves
                        || itemToCheck.type == ItemID.ChlorophyteHeadgear
                        || itemToCheck.type == ItemID.ChlorophyteHelmet
                        || itemToCheck.type == ItemID.ChlorophyteJackhammer
                        || itemToCheck.type == ItemID.ChlorophyteMask
                        || itemToCheck.type == ItemID.ChlorophytePartisan
                        || itemToCheck.type == ItemID.ChlorophytePickaxe
                        || itemToCheck.type == ItemID.ChlorophytePlateMail
                        || itemToCheck.type == ItemID.ChlorophyteSaber
                        || itemToCheck.type == ItemID.ChlorophyteShotbow
                        || itemToCheck.type == ItemID.ChlorophyteWarhammer
                        || itemToCheck.type == ItemID.GolfClubChlorophyteDriver
                        || itemToCheck.type == ItemID.TurtleHelmet
                        || itemToCheck.type == ItemID.TurtleLeggings
                        || itemToCheck.type == ItemID.TurtleScaleMail
                        || itemToCheck.type == ItemID.VenomStaff
                        || itemToCheck.type == ItemID.MechanicalGlove
                        || itemToCheck.type == ItemID.FireGauntlet
                        || itemToCheck.type == ItemID.CelestialEmblem)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedDestroyer == false)
                {
                    if (itemToCheck.type == ItemID.SoulofMight
                        || itemToCheck.type == ItemID.LightDisc
                        || itemToCheck.type == ItemID.Megashark)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedTwins == false)
                {
                    if (itemToCheck.type == ItemID.SoulofSight
                        || itemToCheck.type == ItemID.MagicalHarp
                        || itemToCheck.type == ItemID.OpticStaff
                        || itemToCheck.type == ItemID.RainbowRod)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedPrime == false)
                {
                    if (itemToCheck.type == ItemID.SoulofFright
                        || itemToCheck.type == ItemID.Flamethrower)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedQS == false)
                {
                    if (itemToCheck.type == ItemID.QueenSlimeBossBag
                        || itemToCheck.type == ItemID.VolatileGelatin
                        || itemToCheck.type == ItemID.QueenSlimeHook
                        || itemToCheck.type == ItemID.QueenSlimeMountSaddle
                        || itemToCheck.type == ItemID.Smolstar
                        || itemToCheck.type == ItemID.CrystalNinjaHelmet
                        || itemToCheck.type == ItemID.CrystalNinjaChestplate
                        || itemToCheck.type == ItemID.CrystalNinjaLeggings)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedWOF == false)
                {
                    if (itemToCheck.type == ItemID.MechdusaSummon
                        || itemToCheck.type == ItemID.MechanicalEye
                        || itemToCheck.type == ItemID.MechanicalSkull
                        || itemToCheck.type == ItemID.MechanicalWorm
                        || itemToCheck.type == ItemID.TruffleWorm
                        || itemToCheck.type == ItemID.QueenSlimeCrystal
                        || itemToCheck.type == ItemID.PirateMap
                        || itemToCheck.type == ItemID.PearlwoodBow
                        || itemToCheck.type == ItemID.PearlwoodSword
                        || itemToCheck.type == ItemID.PearlwoodHammer
                        || itemToCheck.type == ItemID.CrystalBall
                        || itemToCheck.type == ItemID.SoulofLight
                        || itemToCheck.type == ItemID.LightKey
                        || itemToCheck.type == ItemID.SoulofNight
                        || itemToCheck.type == ItemID.NightKey
                        || itemToCheck.type == ItemID.GreaterHealingPotion
                        || itemToCheck.type == ItemID.GreaterManaPotion
                        || itemToCheck.type == ItemID.DartPistol
                        || itemToCheck.type == ItemID.DartRifle
                        || itemToCheck.type == ItemID.PutridScent
                        || itemToCheck.type == ItemID.FleshKnuckles
                        || itemToCheck.type == ItemID.WormHook
                        || itemToCheck.type == ItemID.TendonHook
                        || itemToCheck.type == ItemID.ChainGuillotines
                        || itemToCheck.type == ItemID.FetidBaghnakhs
                        || itemToCheck.type == ItemID.ClingerStaff
                        || itemToCheck.type == ItemID.SoulDrain
                        || itemToCheck.type == ItemID.DaedalusStormbow
                        || itemToCheck.type == ItemID.IlluminantHook
                        || itemToCheck.type == ItemID.CrystalVileShard
                        || itemToCheck.type == ItemID.BlessedApple
                        || itemToCheck.type == ItemID.FlyingKnife
                        || itemToCheck.type == ItemID.TitanGlove
                        || itemToCheck.type == ItemID.PowerGlove
                        || itemToCheck.type == ItemID.ArcaneFlower
                        || itemToCheck.type == ItemID.CursedFlames
                        || itemToCheck.type == ItemID.GoldenShower
                        || itemToCheck.type == ItemID.DaoofPow
                        || itemToCheck.type == ItemID.CoolWhip
                        || itemToCheck.type == ItemID.DemonWings
                        || itemToCheck.type == ItemID.OnyxBlaster
                        || itemToCheck.type == ItemID.SpiritFlame
                        || itemToCheck.type == ItemID.CrystalStorm
                        || itemToCheck.type == ItemID.AngelWings
                        || itemToCheck.type == ItemID.Chik
                        || itemToCheck.type == ItemID.MeteorStaff
                        || itemToCheck.type == ItemID.SkyFracture
                        || itemToCheck.type == ItemID.CrystalShard
                        || itemToCheck.type == ItemID.CrystalDart
                        || itemToCheck.type == ItemID.CrystalBullet
                        || itemToCheck.type == ItemID.SuperManaPotion
                        || itemToCheck.type == ItemID.HolyArrow
                        || itemToCheck.type == ItemID.FairyWings
                        || itemToCheck.type == ItemID.PearlsandBlock
                        || itemToCheck.type == ItemID.PearlstoneBlock
                        || itemToCheck.type == ItemID.Pearlwood
                        || itemToCheck.type == ItemID.HolyWater
                        || itemToCheck.type == ItemID.HallowedSeeds
                        || itemToCheck.type == ItemID.JoustingLance
                        || itemToCheck.type == ItemID.ZapinatorOrange
                        || itemToCheck.type == ItemID.SanguineStaff
                        || itemToCheck.type == ItemID.PirateStaff
                        || itemToCheck.type == ItemID.CoinGun
                        || itemToCheck.type == ItemID.PirateShipMountItem
                        || itemToCheck.type == ItemID.DripplerFlail
                        || itemToCheck.type == ItemID.KOCannon
                        || itemToCheck.type == ItemID.SharpTears
                        || itemToCheck.type == ItemID.SlapHand
                        || itemToCheck.type == ItemID.Bananarang
                        || itemToCheck.type == ItemID.BeamSword
                        || itemToCheck.type == ItemID.Frostbrand
                        || itemToCheck.type == ItemID.IceBow
                        || itemToCheck.type == ItemID.FlowerofFrost
                        || itemToCheck.type == ItemID.DualHook
                        || itemToCheck.type == ItemID.MagicDagger
                        || itemToCheck.type == ItemID.PhilosophersStone
                        || itemToCheck.type == ItemID.CrossNecklace
                        || itemToCheck.type == ItemID.StarCloak
                        || itemToCheck.type == ItemID.ShadowFlameHexDoll
                        || itemToCheck.type == ItemID.ShadowFlameKnife
                        || itemToCheck.type == ItemID.ShadowFlameBow
                        || itemToCheck.type == ItemID.AdamantiteMask
                        || itemToCheck.type == ItemID.AdamantiteLeggings
                        || itemToCheck.type == ItemID.AdamantiteHelmet
                        || itemToCheck.type == ItemID.AdamantiteHeadgear
                        || itemToCheck.type == ItemID.AdamantiteDrill
                        || itemToCheck.type == ItemID.AdamantiteBreastplate
                        || itemToCheck.type == ItemID.AdamantiteBar
                        || itemToCheck.type == ItemID.AdamantiteOre
                        || itemToCheck.type == ItemID.AdamantiteGlaive
                        || itemToCheck.type == ItemID.AdamantitePickaxe
                        || itemToCheck.type == ItemID.AdamantiteRepeater
                        || itemToCheck.type == ItemID.AdamantiteSword
                        || itemToCheck.type == ItemID.AdamantiteWaraxe
                        || itemToCheck.type == ItemID.TitaniumBar
                        || itemToCheck.type == ItemID.TitaniumOre
                        || itemToCheck.type == ItemID.TitaniumLeggings
                        || itemToCheck.type == ItemID.TitaniumBreastplate
                        || itemToCheck.type == ItemID.TitaniumChainsaw
                        || itemToCheck.type == ItemID.TitaniumHeadgear
                        || itemToCheck.type == ItemID.TitaniumHelmet
                        || itemToCheck.type == ItemID.TitaniumMask
                        || itemToCheck.type == ItemID.TitaniumTrident
                        || itemToCheck.type == ItemID.TitaniumRepeater
                        || itemToCheck.type == ItemID.TitaniumSword
                        || itemToCheck.type == ItemID.TitaniumWaraxe
                        || itemToCheck.type == ItemID.TitaniumDrill
                        || itemToCheck.type == ItemID.TitaniumPickaxe
                        || itemToCheck.type == ItemID.AncientBattleArmorHat
                        || itemToCheck.type == ItemID.AncientBattleArmorShirt
                        || itemToCheck.type == ItemID.AncientBattleArmorPants
                        || itemToCheck.type == ItemID.AncientBattleArmorMaterial
                        || itemToCheck.type == ItemID.FrostCore
                        || itemToCheck.type == ItemID.FrostHelmet
                        || itemToCheck.type == ItemID.FrostLeggings
                        || itemToCheck.type == ItemID.FrostBreastplate
                        || itemToCheck.type == ItemID.FrozenTurtleShell
                        || itemToCheck.type == ItemID.FrozenWings
                        || itemToCheck.type == ItemID.FinWings
                        || itemToCheck.type == ItemID.HarpyWings
                        || itemToCheck.type == ItemID.CrystalSerpent
                        || itemToCheck.type == ItemID.OrichalcumBar
                        || itemToCheck.type == ItemID.OrichalcumBreastplate
                        || itemToCheck.type == ItemID.OrichalcumChainsaw
                        || itemToCheck.type == ItemID.OrichalcumDrill
                        || itemToCheck.type == ItemID.OrichalcumHalberd
                        || itemToCheck.type == ItemID.OrichalcumHeadgear
                        || itemToCheck.type == ItemID.OrichalcumHelmet
                        || itemToCheck.type == ItemID.OrichalcumLeggings
                        || itemToCheck.type == ItemID.OrichalcumMask
                        || itemToCheck.type == ItemID.OrichalcumOre
                        || itemToCheck.type == ItemID.OrichalcumPickaxe
                        || itemToCheck.type == ItemID.OrichalcumRepeater
                        || itemToCheck.type == ItemID.OrichalcumSword
                        || itemToCheck.type == ItemID.OrichalcumWaraxe
                        || itemToCheck.type == ItemID.MythrilBar
                        || itemToCheck.type == ItemID.MythrilChainmail
                        || itemToCheck.type == ItemID.MythrilChainsaw
                        || itemToCheck.type == ItemID.MythrilDrill
                        || itemToCheck.type == ItemID.MythrilGreaves
                        || itemToCheck.type == ItemID.MythrilHalberd
                        || itemToCheck.type == ItemID.MythrilHat
                        || itemToCheck.type == ItemID.MythrilHelmet
                        || itemToCheck.type == ItemID.MythrilHood
                        || itemToCheck.type == ItemID.MythrilOre
                        || itemToCheck.type == ItemID.MythrilPickaxe
                        || itemToCheck.type == ItemID.MythrilRepeater
                        || itemToCheck.type == ItemID.MythrilSword
                        || itemToCheck.type == ItemID.MythrilWaraxe
                        || itemToCheck.type == ItemID.CobaltBar
                        || itemToCheck.type == ItemID.CobaltBreastplate
                        || itemToCheck.type == ItemID.CobaltChainsaw
                        || itemToCheck.type == ItemID.CobaltDrill
                        || itemToCheck.type == ItemID.CobaltHat
                        || itemToCheck.type == ItemID.CobaltHelmet
                        || itemToCheck.type == ItemID.CobaltLeggings
                        || itemToCheck.type == ItemID.CobaltMask
                        || itemToCheck.type == ItemID.CobaltNaginata
                        || itemToCheck.type == ItemID.CobaltOre
                        || itemToCheck.type == ItemID.CobaltPickaxe
                        || itemToCheck.type == ItemID.CobaltRepeater
                        || itemToCheck.type == ItemID.CobaltSword
                        || itemToCheck.type == ItemID.CobaltWaraxe
                        || itemToCheck.type == ItemID.PalladiumBar
                        || itemToCheck.type == ItemID.PalladiumBreastplate
                        || itemToCheck.type == ItemID.PalladiumChainsaw
                        || itemToCheck.type == ItemID.PalladiumDrill
                        || itemToCheck.type == ItemID.PalladiumHeadgear
                        || itemToCheck.type == ItemID.PalladiumHelmet
                        || itemToCheck.type == ItemID.PalladiumLeggings
                        || itemToCheck.type == ItemID.PalladiumMask
                        || itemToCheck.type == ItemID.PalladiumOre
                        || itemToCheck.type == ItemID.PalladiumPickaxe
                        || itemToCheck.type == ItemID.PalladiumPike
                        || itemToCheck.type == ItemID.PalladiumRepeater
                        || itemToCheck.type == ItemID.PalladiumSword
                        || itemToCheck.type == ItemID.PalladiumWaraxe
                        || itemToCheck.type == ItemID.CursedArrow
                        || itemToCheck.type == ItemID.CursedFlames
                        || itemToCheck.type == ItemID.CursedFlare
                        || itemToCheck.type == ItemID.CursedDart
                        || itemToCheck.type == ItemID.CursedBullet
                        || itemToCheck.type == ItemID.Ichor
                        || itemToCheck.type == ItemID.IchorBullet
                        || itemToCheck.type == ItemID.IchorDart
                        || itemToCheck.type == ItemID.IchorDart
                        || itemToCheck.type == ItemID.FlaskofIchor
                        || itemToCheck.type == ItemID.FlaskofCursedFlames
                        || itemToCheck.type == ItemID.SpiderFang
                        || itemToCheck.type == ItemID.SpiderMask
                        || itemToCheck.type == ItemID.SpiderBreastplate
                        || itemToCheck.type == ItemID.SpiderGreaves
                        || itemToCheck.type == ItemID.SpiderStaff
                        || itemToCheck.type == ItemID.QueenSpiderStaff
                        || itemToCheck.type == ItemID.FireWhip
                        || itemToCheck.type == ItemID.BreakerBlade
                        || itemToCheck.type == ItemID.ClockworkAssaultRifle
                        || itemToCheck.type == ItemID.LaserRifle
                        || itemToCheck.type == ItemID.MoonCharm
                        || itemToCheck.type == ItemID.MedusaHead
                        || itemToCheck.type == ItemID.PoisonStaff
                        || itemToCheck.type == ItemID.ManaCloak
                        || itemToCheck.type == ItemID.RangerEmblem
                        || itemToCheck.type == ItemID.SorcererEmblem
                        || itemToCheck.type == ItemID.SummonerEmblem
                        || itemToCheck.type == ItemID.WarriorEmblem
                        || itemToCheck.type == ItemID.DemonHeart
                        || itemToCheck.type == ItemID.Gatligator
                        || itemToCheck.type == ItemID.Uzi
                        || itemToCheck.type == ItemID.Shotgun
                        || itemToCheck.type == ItemID.Toxikarp
                        || itemToCheck.type == ItemID.Bladetongue
                        || itemToCheck.type == ItemID.MagicQuiver
                        || itemToCheck.type == ItemID.MoltenQuiver
                        || itemToCheck.type == ItemID.StalkersQuiver
                        || itemToCheck.type == ItemID.BluePhasesaber
                        || itemToCheck.type == ItemID.BluePhasesaberOld
                        || itemToCheck.type == ItemID.GreenPhasesaber
                        || itemToCheck.type == ItemID.GreenPhasesaberOld
                        || itemToCheck.type == ItemID.OrangePhasesaber
                        || itemToCheck.type == ItemID.PurplePhasesaber
                        || itemToCheck.type == ItemID.PurplePhasesaberOld
                        || itemToCheck.type == ItemID.RedPhasesaber
                        || itemToCheck.type == ItemID.RedPhasesaberOld
                        || itemToCheck.type == ItemID.WhitePhasesaber
                        || itemToCheck.type == ItemID.WhitePhasesaberOld
                        || itemToCheck.type == ItemID.YellowPhasesaber
                        || itemToCheck.type == ItemID.YellowPhasesaberOld
                        || itemToCheck.type == ItemID.IceSickle
                        || itemToCheck.type == ItemID.HamBat
                        || itemToCheck.type == ItemID.Cutlass
                        || itemToCheck.type == ItemID.FormatC
                        || itemToCheck.type == ItemID.Gradient
                        || itemToCheck.type == ItemID.HelFire
                        || itemToCheck.type == ItemID.Amarok
                        || itemToCheck.type == ItemID.Yelets
                        || itemToCheck.type == ItemID.Code2
                        || itemToCheck.type == ItemID.ObsidianSwordfish
                        || itemToCheck.type == ItemID.BouncingShield
                        || itemToCheck.type == ItemID.Anchor
                        || itemToCheck.type == ItemID.CorruptFishingCrateHard
                        || itemToCheck.type == ItemID.CrimsonFishingCrateHard
                        || itemToCheck.type == ItemID.DungeonFishingCrateHard
                        || itemToCheck.type == ItemID.FloatingIslandFishingCrateHard
                        || itemToCheck.type == ItemID.FrozenCrateHard
                        || itemToCheck.type == ItemID.GoldenCrateHard
                        || itemToCheck.type == ItemID.HallowedFishingCrateHard
                        || itemToCheck.type == ItemID.IronCrateHard
                        || itemToCheck.type == ItemID.JungleFishingCrateHard
                        || itemToCheck.type == ItemID.LavaCrateHard
                        || itemToCheck.type == ItemID.OasisCrateHard
                        || itemToCheck.type == ItemID.OceanCrateHard
                        || itemToCheck.type == ItemID.WoodenCrateHard)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedSkeletron == false)
                {
                    if (itemToCheck.type == ItemID.ShadowKey
                        || itemToCheck.type == ItemID.HellwingBow
                        || itemToCheck.type == ItemID.Sunfury
                        || itemToCheck.type == ItemID.FlowerofFire
                        || itemToCheck.type == ItemID.Flamelash
                        || itemToCheck.type == ItemID.DarkLance
                        || itemToCheck.type == ItemID.TreasureMagnet)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedQB == false)
                {
                    if (itemToCheck.type == ItemID.PygmyNecklace
                        || itemToCheck.type == ItemID.HiveBackpack
                        || itemToCheck.type == ItemID.BeeGun
                        || itemToCheck.type == ItemID.BeeKeeper
                        || itemToCheck.type == ItemID.BeesKnees
                        || itemToCheck.type == ItemID.QueenBeeBossBag
                        || itemToCheck.type == ItemID.HoneyComb
                        || itemToCheck.type == ItemID.BeeWax
                        || itemToCheck.type == ItemID.BeeGreaves
                        || itemToCheck.type == ItemID.BeeCloak
                        || itemToCheck.type == ItemID.BeeBreastplate
                        || itemToCheck.type == ItemID.BeeHeadgear
                        || itemToCheck.type == ItemID.Beenade
                        || itemToCheck.type == ItemID.HiveFive
                        || itemToCheck.type == ItemID.HornetStaff
                        || itemToCheck.type == ItemID.HoneyBalloon
                        || itemToCheck.type == ItemID.StingerNecklace
                        || itemToCheck.type == ItemID.SweetheartNecklace)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedEvil == false)
                {
                    if (itemToCheck.type == ItemID.ShadowScale
                        || itemToCheck.type == ItemID.WormScarf
                        || itemToCheck.type == ItemID.EaterOfWorldsBossBag
                        || itemToCheck.type == ItemID.BrainOfConfusion
                        || itemToCheck.type == ItemID.BrainOfCthulhuBossBag
                        || itemToCheck.type == ItemID.TissueSample
                        || itemToCheck.type == ItemID.Hellstone
                        || itemToCheck.type == ItemID.HellstoneBar
                        || itemToCheck.type == ItemID.FireproofBugNet
                        || itemToCheck.type == ItemID.Flamarang
                        || itemToCheck.type == ItemID.ImpStaff
                        || itemToCheck.type == ItemID.MoltenFury
                        || itemToCheck.type == ItemID.MoltenHamaxe
                        || itemToCheck.type == ItemID.PhoenixBlaster
                        || itemToCheck.type == ItemID.FieryGreatsword
                        || itemToCheck.type == ItemID.MoltenPickaxe
                        || itemToCheck.type == ItemID.MoltenHelmet
                        || itemToCheck.type == ItemID.MoltenGreaves
                        || itemToCheck.type == ItemID.MoltenBreastplate)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedDeer == false)
                {
                    if (itemToCheck.type == ItemID.BoneHelm
                        || itemToCheck.type == ItemID.PewMaticHorn
                        || itemToCheck.type == ItemID.WeatherPain
                        || itemToCheck.type == ItemID.HoundiusShootius
                        || itemToCheck.type == ItemID.LucyTheAxe
                        || itemToCheck.type == ItemID.DeerclopsBossBag)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedEoc == false)
                {
                    if (itemToCheck.type == ItemID.EyeOfCthulhuBossBag
                        || itemToCheck.type == ItemID.EoCShield)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedGoblins == false)
                {
                    if (itemToCheck.type == ItemID.TinkerersWorkshop)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                if (config.downedSlimeKing == false)
                {
                    if (itemToCheck.type == ItemID.KingSlimeBossBag)
                    {
                        TSPlayer.All.SendMessage(@"Player: " + player.Name + " had illegal items. [i/s" + itemToCheck.stack + ":" + itemToCheck.type + "] on slot:" + slot, 255, 0, 0);
                        itemToCheck.SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, slot);
                    }
                }
                #endregion
            };


        }


        private void OnUpdate(EventArgs args)
        {
            UpdateCount++;

            // Check for 999 platinum coins every 15 frames
            if (UpdateCount % 10 == 0)
            {
                foreach (TSPlayer plr in GetLoggedInPlayers())
                {
                    for (int i = 0; i < 260; i++)
                    {
                        CheckSlot(plr, i);
                    }
                }
            }
        }

        private void OnUpdateBoss(EventArgs args)
        {
            BossStatus bossStatus = LoadConfig();


            if (bossStatus.downedEoc == false && NPC.downedBoss1)
            {
                bossStatus.downedEoc = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedSlimeKing == false && NPC.downedSlimeKing)
            {
                bossStatus.downedSlimeKing = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedEvil == false && NPC.downedBoss2)
            {
                bossStatus.downedEvil = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedGoblins == false && NPC.downedGoblins)
            {
                // Update the BossStatus accordingly
                bossStatus.downedGoblins = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedDeer == false && NPC.downedDeerclops)
            {
                // Update the BossStatus accordingly
                bossStatus.downedDeer = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedQB == false && NPC.downedQueenBee)
            {
                // Update the BossStatus accordingly
                bossStatus.downedQB = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedSkeletron == false && NPC.downedBoss3)
            {
                // Update the BossStatus accordingly
                bossStatus.downedSkeletron = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedWOF == false && Main.hardMode)
            {
                // Update the BossStatus accordingly
                bossStatus.downedWOF = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedQS == false && NPC.downedQueenSlime)
            {
                // Update the BossStatus accordingly
                bossStatus.downedQS = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedDestroyer == false && NPC.downedMechBoss1)
            {
                // Update the BossStatus accordingly
                bossStatus.downedDestroyer = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedTwins == false && NPC.downedMechBoss2)
            {
                // Update the BossStatus accordingly
                bossStatus.downedTwins = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedPrime == false && NPC.downedMechBoss3)
            {
                // Update the BossStatus accordingly
                bossStatus.downedPrime = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedOneMech == false && NPC.downedMechBossAny)
            {
                // Update the BossStatus accordingly
                bossStatus.downedOneMech = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedAllMechs == false && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                // Update the BossStatus accordingly
                bossStatus.downedAllMechs = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedDuke == false && NPC.downedFishron)
            {
                // Update the BossStatus accordingly
                bossStatus.downedDuke = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedPlantera == false && NPC.downedPlantBoss)
            {
                // Update the BossStatus accordingly
                bossStatus.downedPlantera = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedEOL == false && NPC.downedEmpressOfLight)
            {
                // Update the BossStatus accordingly
                bossStatus.downedEOL = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedGolem == false && NPC.downedGolemBoss)
            {
                // Update the BossStatus accordingly
                bossStatus.downedGolem = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedLC == false && NPC.downedAncientCultist)
            {
                // Update the BossStatus accordingly
                bossStatus.downedLC = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedNebula == false && NPC.downedTowerNebula)
            {
                // Update the BossStatus accordingly
                bossStatus.downedNebula = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedSolar == false && NPC.downedTowerSolar)
            {
                // Update the BossStatus accordingly
                bossStatus.downedSolar = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedStardust == false && NPC.downedTowerStardust)
            {
                // Update the BossStatus accordingly
                bossStatus.downedStardust = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedVortex == false && NPC.downedTowerVortex)
            {
                // Update the BossStatus accordingly
                bossStatus.downedVortex = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedAllPillars == false && NPC.downedTowerVortex && NPC.downedTowerStardust && NPC.downedTowerSolar && NPC.downedTowerNebula)
            {
                // Update the BossStatus accordingly
                bossStatus.downedAllPillars = true;
                SaveConfig(bossStatus);
            }
            if (bossStatus.downedML == false && NPC.downedMoonlord)
            {
                // Update the BossStatus accordingly
                bossStatus.downedML = true;
                SaveConfig(bossStatus);
            }
        }

        private void resetprogress(CommandArgs args)
        {
            SaveConfig(new BossStatus
            {
                downedSlimeKing = false,
                downedEoc = false,
                downedGoblins = false,
                downedDeer = false,
                downedEvil = false,
                downedQB = false,
                downedSkeletron = false,
                downedWOF = false,
                downedQS = false,
                downedDestroyer = false,
                downedTwins = false,
                downedPrime = false,
                downedAllMechs = false,
                downedOneMech = false,
                downedDuke = false,
                downedPlantera = false,
                downedEOL = false,
                downedGolem = false,
                downedLC = false,
                downedVortex = false,
                downedNebula = false,
                downedStardust = false,
                downedSolar = false,
                downedAllPillars = false,
                downedML = false,
            });
        }

        private BossStatus LoadConfig()
        {
            var configFile = Path.Combine(TShock.SavePath, "BossStatus", "config.json");
            if (File.Exists(configFile))
            {
                return JsonConvert.DeserializeObject<BossStatus>(File.ReadAllText(configFile));
            }
            else
            {
                return new BossStatus
                {

                };
            }
        }
        private void SaveConfig(BossStatus config)
        {
            var configFile = Path.Combine(TShock.SavePath, "BossStatus", "config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(configFile));
            File.WriteAllText(configFile, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
        public class BossStatus
        {
            public static BossStatus Instance = new();

            public bool downedSlimeKing;
            public bool downedEoc;
            public bool downedGoblins;
            public bool downedDeer;
            public bool downedEvil;
            public bool downedQB;
            public bool downedSkeletron;
            public bool downedWOF;
            public bool downedQS;
            public bool downedDestroyer;
            public bool downedTwins;
            public bool downedPrime;
            public bool downedAllMechs;
            public bool downedOneMech;
            public bool downedDuke;
            public bool downedPlantera;
            public bool downedEOL;
            public bool downedGolem;
            public bool downedLC;
            public bool downedVortex;
            public bool downedNebula;
            public bool downedStardust;
            public bool downedSolar;
            public bool downedAllPillars;
            public bool downedML;




            public BossStatus()
            {

                downedSlimeKing = false;
                downedEoc = false;
                downedGoblins = false;
                downedDeer = false;
                downedEvil = false;
                downedQB = false;
                downedSkeletron = false;
                downedWOF = false;
                downedQS = false;
                downedDestroyer = false;
                downedTwins = false;
                downedPrime = false;
                downedAllMechs = false;
                downedOneMech = false;
                downedDuke = false;
                downedPlantera = false;
                downedEOL = false;
                downedGolem = false;
                downedLC = false;
                downedVortex = false;
                downedNebula = false;
                downedStardust = false;
                downedSolar = false;
                downedAllPillars = false;
                downedML = false;


            }
        }
    }
}
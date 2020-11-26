namespace LeagueSharp.Common
{
    /// <summary>
    ///     Adds hacks to the menu.
    /// </summary>
    internal class Hacks
    {
        #region Constants

        private const int WM_KEYDOWN = 0x100;

        private const int WM_KEYUP = 0x101;

        #endregion

        #region Static Fields

        private static Menu menu;

        private static MenuItem MenuAntiAfk;

        private static MenuItem MenuDisableDrawings;

        private static MenuItem MenuDisableSay;

        private static MenuItem MenuTowerRange;

        public static bool AntiAFK;

        public static bool DisableDrawings;

        public static bool DisableSay;

        public static bool TowerRanges;

        #endregion

        #region Public Methods and Operators

        public static void Shutdown()
        {
            Menu.Remove(menu);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    menu = new Menu("Hacks", "Hacks");

                    MenuAntiAfk = menu.AddItem(new MenuItem("AfkHack", "Anti-AFK").SetValue(false));
                    MenuAntiAfk.ValueChanged += (sender, args) => AntiAFK = args.GetNewValue<bool>();

                    MenuDisableDrawings = menu.AddItem(new MenuItem("DrawingHack", "Disable Drawing").SetValue(false));
                    MenuDisableDrawings.ValueChanged += (sender, args) => DisableDrawings = args.GetNewValue<bool>();
                    MenuDisableDrawings.SetValue(DisableDrawings);

                    MenuDisableSay = menu.AddItem(new MenuItem("SayHack", "Disable L# Send Chat").SetValue(false).SetTooltip("Block Game.Say from Assemblies"));
                    MenuDisableSay.ValueChanged += (sender, args) => DisableSay = args.GetNewValue<bool>();

                    MenuTowerRange = menu.AddItem(new MenuItem("TowerHack", "Show Tower Ranges").SetValue(false));
                    MenuTowerRange.ValueChanged += (sender, args) => TowerRanges = args.GetNewValue<bool>();

                    AntiAFK = MenuAntiAfk.GetValue<bool>();
                    DisableDrawings = MenuDisableDrawings.GetValue<bool>();
                    DisableSay = MenuDisableSay.GetValue<bool>();
                    TowerRanges = MenuTowerRange.GetValue<bool>();

                    CommonMenu.Instance.AddSubMenu(menu);

                    Game.OnWndProc += args =>
                        {
                            if (!MenuDisableDrawings.GetValue<bool>())
                            {
                                return;
                            }

                            if ((int)args.WParam != Config.ShowMenuPressKey)
                            {
                                return;
                            }
                        };
                };
        }

        #endregion
    }
}
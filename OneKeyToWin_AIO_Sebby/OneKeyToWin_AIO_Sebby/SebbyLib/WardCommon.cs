using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.SebbyLib
{
    public enum WardType
    {
        VisionWard,
        ControlWard,
        AnyWard
    }

    public class WardCommon
    {
        private static readonly List<string> RegularWardNames = new List<string>
        {
            "BlueTrinket",
            "SightWard",
            "YellowTrinket",
            "VisionWard"
        };

        private static readonly List<string> ControlWardNames = new List<string>
        {
            "JammerDevice"
        };

        private static readonly List<int> VisionWardItems = new List<int>
        {
            2045, 2049, 2050, 2301, 2302, 2303, 3340, 3361, 3362,
            3711, 1408, 1409, 1410, 1411, 2043, 3340, 3859, 3851,
            3853, 3364, 3851, 3855, 3857, 3859, 3860, 3863, 3864,
            3363
        };

        private static readonly List<int> ControlWardItems = new List<int>
        {
            2055
        };
        
        private static IEnumerable<int> GetWardItems(WardType type)
        {
            switch (type)
            {
                case WardType.VisionWard:
                    return VisionWardItems;
                case WardType.ControlWard:
                    return ControlWardItems;
                case WardType.AnyWard:
                    return VisionWardItems.Concat(ControlWardItems).ToList();
            }

            return Enumerable.Empty<int>();
        }
        
        private static int ObjectsPlaced(Func<string, bool> predicate)
        {
            var playerNetworkId = ObjectManager.Player.NetworkId;
            return ObjectManager
                .Get<Obj_AI_Minion>()
                .Where(gameObject => predicate(gameObject.Name))
                .Count(minion => minion.Owner.NetworkId == playerNetworkId);
        }

        public static bool IsWard(string name, WardType wardType = WardType.VisionWard)
        {
            switch (wardType)
            {
                case WardType.VisionWard:
                    return RegularWardNames.Contains(name);
                case WardType.ControlWard:
                    return ControlWardNames.Contains(name);
                case WardType.AnyWard:
                    return RegularWardNames.Contains(name) || ControlWardNames.Contains(name);
            }

            return false;
        }


        public static bool CastWard(Vector3 position, WardType type = WardType.VisionWard)
        {
            if (position.Distance(ObjectManager.Player.ServerPosition) < 600.0f)
            {
                foreach (var wardItem in GetWardItems(type).Where(Items.CanUseItem))
                {
                    Items.UseItem(wardItem);
                    return true;
                }
            }

            return false;
        }
        
        public static bool CanCastWard(WardType wardType = WardType.VisionWard)
        {
            return WardAmmo(wardType) > 0;
        }

        public static int WardsPlaced(WardType wardType = WardType.VisionWard)
        {
            return ObjectsPlaced(name => IsWard(name, wardType));
        }

        public static int WardAmmo(WardType wardType = WardType.VisionWard)
        {
            return GetWardItems(wardType)
                .Where(Items.CanUseItem)
                .Select(Items.GetItemSlot)
                .Where(itemSlot => itemSlot != null)
                .Sum(itemSlot => itemSlot.Charges);
        } 
    }
}
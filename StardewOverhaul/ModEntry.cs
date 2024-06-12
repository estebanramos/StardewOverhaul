using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewOverhaul
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private float totalChestValue;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }


        /*********
        ** Private methods
        *********/
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            IClickableMenu activeClickableMenu = e.NewMenu;
            ItemGrabMenu itemGrabMenu = (ItemGrabMenu)(object)((activeClickableMenu is ItemGrabMenu) ? activeClickableMenu : null);
            if (itemGrabMenu == null)
            {
                return;
            }
            this.totalChestValue = 0f;
            object context = itemGrabMenu.context;
            Chest chest = (Chest)((context is Chest) ? context : null);
            if (chest != null)
            {
                foreach (Item item in chest.Items)
                {
                    float? sellPrice = item.sellToStorePrice(-1L);
                    float itemPrice = sellPrice.Value * (float)item.Stack;
                    this.totalChestValue += ((itemPrice > 0f) ? itemPrice : 0f);
                    this.Monitor.Log($"Item: {item.Name}, Bonus Price: {sellPrice.Value}", LogLevel.Debug);
                }
                this.Monitor.Log($"Total Chest Value: {this.totalChestValue}", LogLevel.Debug);
            }
        }

    }
}

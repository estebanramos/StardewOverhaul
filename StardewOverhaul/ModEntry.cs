using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewOverhaul
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private bool _isDrawing;
        private float totalChestValue;
        private Texture2D backgroundTexture;
        private Texture2D moneyBoxTexture;
        private RenderTarget2D textRenderTarget;
        private SpriteFont font;
        private int MoneyIndicatorX = 100; // Example X position
        private int MoneyIndicatorY = 100;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            font = Game1.dialogueFont;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Display.RenderedActiveMenu += RenderedActiveMenu;
            this.backgroundTexture = helper.ModContent.Load<Texture2D>("assets/money_indicator.png");
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
                    //this.Monitor.Log($"Item: {item.Name}, Bonus Price: {sellPrice.Value}", LogLevel.Debug);
                }
                //this.Monitor.Log($"Total Chest Value: {this.totalChestValue}", LogLevel.Debug);
            }
        }
        private static string FormatNumber(float number)
        {
            return number.ToString("#,##0");
        }
        private void RenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (this._isDrawing)
            {
                return;
            }

            IClickableMenu activeClickableMenu = Game1.activeClickableMenu;
            ItemGrabMenu itemGrabMenu = activeClickableMenu as ItemGrabMenu;
            if (itemGrabMenu == null || (!(itemGrabMenu.context is Chest) && !(itemGrabMenu.context is JunimoHut)))
            {
                return;
            }

            this._isDrawing = true;
            SpriteBatch spriteBatch = e.SpriteBatch;
            if (spriteBatch != null && !spriteBatch.IsDisposed)
            {
                string text = this.totalChestValue.ToString();
                Vector2 textSize = Game1.dialogueFont.MeasureString(text);
                Vector2 minTextSize = Game1.dialogueFont.MeasureString("00000000");
                if (textSize.X < minTextSize.X)
                {
                    textSize = minTextSize;
                }

                Rectangle chestBounds = new Rectangle(itemGrabMenu.xPositionOnScreen, itemGrabMenu.yPositionOnScreen, itemGrabMenu.width, itemGrabMenu.height);
                float leftOffset = itemGrabMenu.width / 2;
                float topOffset = 49f;

                Chest chest = itemGrabMenu.context as Chest;
                if (chest != null && chest.specialChestType.ToString() == "BigChest")
                {
                    topOffset += 15f;
                }

                Vector2 position = new Vector2(chestBounds.X + leftOffset, chestBounds.Y - topOffset * 2.25f);
                
                Rectangle backgroundRectangle = new Rectangle((int)position.X - 75, (int)position.Y - 15, (int)textSize.X + 105, (int)textSize.Y + 15);

                //Draws money indicator asset
                spriteBatch.Draw(this.backgroundTexture, backgroundRectangle, Color.White);

                Color textColor = new Color(128, 0, 0);

                int numberofCells = 8;
                float cellWidth = 19.4f; //4.8f
                float characterSpacing = cellWidth;
                Vector2 startPosition = new Vector2(chestBounds.X + leftOffset + (numberofCells * cellWidth) - 24, chestBounds.Y - topOffset * 2.25f);
                Vector2 currentCharPosition = startPosition;
                for (int i = text.Length - 1; i >= 0; i--)
                    {
                    //spriteBatch.DrawString(Game1.dialogueFont, c.ToString(), currentCharPosition, Color.Maroon);
                    //spriteBatch.Draw(Game1.mouseCursors, currentCharPosition, backgroundRectangle, Color.Maroon);
                    //currentCharPosition.X += Game1.dialogueFont.MeasureString(text[i].ToString()).X + cellWidth;

                    int digit = int.Parse(text[i].ToString());
                    float num2 = i * 1.2f; // Example offset for each digit
                    Rectangle sourceRect = new Rectangle(286, 502 - digit * 8, 6, 8);
                    position = new Vector2(currentCharPosition.X + num2, currentCharPosition.Y + 12);
                    e.SpriteBatch.Draw(Game1.mouseCursors, position, sourceRect, Color.Maroon, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
                    
                    currentCharPosition.X -= (cellWidth + num2);


                }
            }

            this._isDrawing = false;
        }
    }

}

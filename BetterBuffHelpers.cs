using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;


namespace BetterBuffs {
	public static class BetterBuffHelpers {
		public static Rectangle GetWorldFrameOfScreen() {
			int screenWid = (int)((float)Main.screenWidth / Main.GameZoomTarget);
			int screenHi = (int)((float)Main.screenHeight / Main.GameZoomTarget);
			int screenX = (int)Main.screenPosition.X + ((Main.screenWidth - screenWid) / 2);
			int screenY = (int)Main.screenPosition.Y + ((Main.screenHeight - screenHi) / 2);

			return new Rectangle( screenX, screenY, screenWid, screenHi );
		}


		public static IDictionary<int, Rectangle> GetBuffIconRectanglesByPosition( bool applyInterfaceScaling ) {
			var rects = new Dictionary<int, Rectangle>();
			var player = Main.LocalPlayer;
			int dim = 32;
			Vector2 screenOffset = Vector2.Zero;

			if( applyInterfaceScaling ) {
				var worldFrame = BetterBuffHelpers.GetWorldFrameOfScreen();
				screenOffset.X = worldFrame.X - Main.screenPosition.X;
				screenOffset.Y = worldFrame.Y - Main.screenPosition.Y;
			}
			
			//if( scaleType == InterfaceScaleType.UI ) {
			//if( scaleType == InterfaceScaleType.Game ) {
			if( applyInterfaceScaling ) {
				dim = (int)(((float)dim * Main.UIScale) / Main.GameZoomTarget);
			}

			for( int i = 0; i < player.buffType.Length; i++ ) {
				if( player.buffType[i] <= 0 ) { continue; }

				int x = 32 + ((i % 11) * 38);
				int y = 76 + (50 * (i / 11));

				//if( scaleType == InterfaceScaleType.UI ) {
				//if( scaleType == InterfaceScaleType.Game ) {
				if( applyInterfaceScaling ) {
					x = (int)((((float)x * Main.UIScale) / Main.GameZoomTarget) + screenOffset.X);
					y = (int)((((float)y * Main.UIScale) / Main.GameZoomTarget) + screenOffset.Y);
				}

				rects[i] = new Rectangle( x, y, dim, dim );
			}

			return rects;
		}


		public static bool CanRefreshBuffAt( Player player, int pos ) {
			int buffType = player.buffType[pos];
			
			for( int i = 0; i < player.inventory.Length; i++ ) {
				Item item = player.inventory[i];

				if( !item.IsAir && item.stack > 0 && item.consumable ) {
					if( item.buffType == buffType ) {
						return true;
					}
					if( buffType == BuffID.PotionSickness && item.potion ) {
						return true;
					}
				}
			}
			return false;
		}


		public static void RefreshBuffAt( Player player, int pos ) {
			if( player.noItems ) { return; }
			
			int buffType = player.buffType[ pos ];
			
			if( buffType == BuffID.PotionSickness ) {
				player.DelBuff( pos );
				player.potionDelay = 0;
				player.QuickHeal();
				return;
			}

			for( int i=0; i<player.inventory.Length; i++ ) {
				Item item = player.inventory[i];

				if( !item.IsAir && item.stack > 0 && item.consumable ) {
					if( item.buffType == buffType ) {
						BetterBuffHelpers.ConsumeItemForBuff( player, item, pos );
						break;
					}
				}
			}
		}


		private static void ConsumeItemForBuff( Player player, Item item, int pos ) {
			player.buffTime[pos] = item.buffTime;
			Main.PlaySound( item.UseSound, player.position );

			item.stack--;
			if( item.stack == 0 ) {
				item.TurnToAir();
				item.active = false;
			}

			Recipe.FindRecipes();
		}
	}
}

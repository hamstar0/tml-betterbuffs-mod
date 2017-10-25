using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;


namespace BetterBuffs {
	public static class BetterBuffHelpers {
		public static Rectangle GetWorldFrameOfScreen() {
			int screen_wid = (int)((float)Main.screenWidth / Main.GameZoomTarget);
			int screen_hei = (int)((float)Main.screenHeight / Main.GameZoomTarget);
			int screen_x = (int)Main.screenPosition.X + ((Main.screenWidth - screen_wid) / 2);
			int screen_y = (int)Main.screenPosition.Y + ((Main.screenHeight - screen_hei) / 2);

			return new Rectangle( screen_x, screen_y, screen_wid, screen_hei );
		}


		public static IDictionary<int, Rectangle> GetBuffIconRectanglesByPosition( bool apply_interface_scaling ) {
			var rects = new Dictionary<int, Rectangle>();
			var player = Main.LocalPlayer;
			int dim = 32;
			Vector2 screen_offset = Vector2.Zero;

			if( apply_interface_scaling ) {
				var world_frame = BetterBuffHelpers.GetWorldFrameOfScreen();
				screen_offset.X = world_frame.X - Main.screenPosition.X;
				screen_offset.Y = world_frame.Y - Main.screenPosition.Y;
			}
			
			//if( scale_type == InterfaceScaleType.UI ) {
			//if( scale_type == InterfaceScaleType.Game ) {
			if( apply_interface_scaling ) {
				dim = (int)(((float)dim * Main.UIScale) / Main.GameZoomTarget);
			}

			for( int i = 0; i < player.buffType.Length; i++ ) {
				if( player.buffType[i] <= 0 ) { continue; }

				int x = 32 + i * 38;
				int y = 76;
				if( i >= 11 ) {
					x = 32 + (i - 11) * 38;
					y += 50;
				}

				//if( scale_type == InterfaceScaleType.UI ) {
				//if( scale_type == InterfaceScaleType.Game ) {
				if( apply_interface_scaling ) {
					x = (int)((((float)x * Main.UIScale) / Main.GameZoomTarget) + screen_offset.X);
					y = (int)((((float)y * Main.UIScale) / Main.GameZoomTarget) + screen_offset.Y);
				}

				rects[i] = new Rectangle( x, y, dim, dim );
			}

			return rects;
		}


		public static bool CanRefreshBuffAt( Player player, int pos ) {
			int buff_type = player.buffType[pos];
			
			for( int i = 0; i < player.inventory.Length; i++ ) {
				Item item = player.inventory[i];

				if( !item.IsAir && item.stack > 0 && item.consumable ) {
					if( item.buffType == buff_type ) {
						return true;
					}
					if( buff_type == BuffID.PotionSickness && item.potion ) {
						return true;
					}
				}
			}
			return false;
		}


		public static void RefreshBuffAt( Player player, int pos ) {
			if( player.noItems ) { return; }
			
			int buff_type = player.buffType[ pos ];
			
			if( buff_type == BuffID.PotionSickness ) {
				player.DelBuff( pos );
				player.potionDelay = 0;
				player.QuickHeal();
				return;
			}

			for( int i=0; i<player.inventory.Length; i++ ) {
				Item item = player.inventory[i];

				if( !item.IsAir && item.stack > 0 && item.consumable ) {
					if( item.buffType == buff_type ) {
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

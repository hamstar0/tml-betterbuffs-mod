using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace BetterBuffs {
	public static class BetterBuffHelpers {
		public static Rectangle GetWorldFrameOfScreen() {
			int screen_wid = (int)((float)Main.screenWidth / Main.GameZoomTarget);
			int screen_hei = (int)((float)Main.screenHeight / Main.GameZoomTarget);
			int screen_x = (int)Main.screenPosition.X + ((Main.screenWidth - screen_wid) / 2);
			int screen_y = (int)Main.screenPosition.Y + ((Main.screenHeight - screen_hei) / 2);

			return new Rectangle( screen_x, screen_y, screen_wid, screen_hei );
		}


		public static IDictionary<int, Rectangle> GetBuffIconRectangles( InterfaceScaleType scale_type ) {
			var rects = new Dictionary<int, Rectangle>();
			var player = Main.LocalPlayer;
			var world_frame = BetterBuffHelpers.GetWorldFrameOfScreen();
			var screen_offset = new Vector2( world_frame.X - Main.screenPosition.X, world_frame.Y - Main.screenPosition.Y );
			int dim = 32;

			if( scale_type == InterfaceScaleType.Game ) {
				dim = (int)((float)dim / Main.GameZoomTarget);
			}

			for( int i = 0; i < 22; i++ ) {
				if( player.buffType[i] <= 0 ) { continue; }

				int x = 32 + i * 38;
				int y = 76;
				if( i >= 11 ) {
					x = 32 + (i - 11) * 38;
					y += 50;
				}

				if( scale_type == InterfaceScaleType.Game ) {
					x = (int)(((float)x / Main.GameZoomTarget) + screen_offset.X);
					y = (int)(((float)y / Main.GameZoomTarget) + screen_offset.Y);
				}

				rects[i] = new Rectangle( x, y, dim, dim );
			}

			return rects;
		}


		public static void RefreshBuffAt( int pos ) {
			var player = Main.LocalPlayer;
			int buff_type = player.buffType[ pos ];

			for( int i=0; i<player.inventory.Length; i++ ) {
				Item item = player.inventory[i];

				if( !item.IsAir && item.buffType == buff_type && item.stack > 0 ) {
					player.buffTime[ pos ] = item.buffTime;

					Main.PlaySound( item.UseSound, player.position );

					if( item.consumable ) {
						item.stack--;
						if( item.stack == 0 ) {
							item.TurnToAir();
							item.active = false;
						}
					}
					break;
				}
			}
		}
	}
}

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;


namespace BetterBuffs {
	public static class BetterBuffHelpers {
		public static IDictionary<int, Rectangle> GetBuffIconRectangles() {
			var rects = new Dictionary<int, Rectangle>();
			var player = Main.LocalPlayer;
			
			for( int i = 0; i < 22; i++ ) {
				if( player.buffType[i] <= 0 ) { continue; }

				int x = 32 + i * 38;
				int y = 76;
				if( i >= 11 ) {
					x = 32 + (i - 11) * 38;
					y += 50;
				}

				rects[i] = new Rectangle( x, y, 32, 32 );
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

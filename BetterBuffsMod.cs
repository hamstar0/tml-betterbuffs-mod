using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace BetterBuffs {
	public class BetterBuffsMod : Mod {
		public Texture2D ShadowBox = null;



		public override void Load() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			if( !Main.dedServ ) {
				this.ShadowBox = this.GetTexture( "ShadowBox" );
			}
		}


		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			int idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Inventory" ) );
			if( idx != -1 ) {
				var interface_layer = new LegacyGameInterfaceLayer( "BetterBuffDisplays: Buff Overlay",
					delegate {
						Player player = Main.LocalPlayer;
						var modplayer = player.GetModPlayer<BetterBuffsPlayer>();

						modplayer.UpdateBuffTimes();
						
						foreach( var kv in BetterBuffHelpers.GetBuffIconRectangles() ) {
							this.DrawShadow( player, kv.Value, player.buffType[kv.Key], player.buffTime[kv.Key] );
						}

						return true;
					} );
				layers.Insert( idx, interface_layer );
			}
		}


		public void DrawShadow( Player player, Rectangle rect, int buff_type, int buff_time ) {
			var modplayer = player.GetModPlayer<BetterBuffsPlayer>();
			if( !modplayer.MaxBuffTimes.ContainsKey(buff_type) ) { return; }

			float ratio = 1f - ((float)buff_time / ( float)modplayer.MaxBuffTimes[buff_type]);
			int width = rect.Width;
			int height = (int)((float)rect.Height * ratio);
			var src_rect = new Rectangle( 0, 0, width, height );
			var color = new Color( 255, 255, 255, 144 );
			var pos = new Vector2( rect.X, rect.Y );
			
			Main.spriteBatch.Draw( this.ShadowBox, pos, src_rect, color );
		}
	}
}

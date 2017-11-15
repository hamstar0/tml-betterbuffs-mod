using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace BetterBuffs {
	class BetterBuffsMod : Mod {
		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-betterbuffs-mod"; } }


		////////////////

		public Texture2D ShadowBox = null;
		public Texture2D LockBox = null;



		////////////////

		public override void Load() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			var hamhelpmod = ModLoader.GetMod( "HamstarHelpers" );
			var min_ver = new Version( 1, 2, 0 );
			if( hamhelpmod.Version < min_ver ) {
				throw new Exception( "Hamstar Helpers must be version " + min_ver.ToString() + " or greater." );
			}

			if( !Main.dedServ ) {
				this.ShadowBox = this.GetTexture( "ShadowBox" );
				this.LockBox = this.GetTexture( "LockBox" );
			}
		}

		////////////////

		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			int idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Inventory" ) );
			if( idx != -1 ) {
				var interface_layer = new LegacyGameInterfaceLayer( "BetterBuffDisplays: Buff Overlay",
					delegate {
						Player player = Main.LocalPlayer;
						var modplayer = player.GetModPlayer<MyPlayer>();
						
						foreach( var kv in BetterBuffHelpers.GetBuffIconRectanglesByPosition( false ) ) {
							int pos = kv.Key;
							Rectangle rect = kv.Value;
							int buff_type = player.buffType[pos];
							int buff_time = player.buffTime[pos];

							this.DrawShadow( player, rect, buff_type, buff_time );

							if( modplayer.BuffLocks.Contains( buff_type ) ) {
								this.DrawLock( player, rect, buff_type, buff_time );
							}
						}

						return true;
					}, InterfaceScaleType.UI );
				layers.Insert( idx, interface_layer );
			}
		}


		public void DrawShadow( Player player, Rectangle rect, int buff_type, int buff_time ) {
			var modplayer = player.GetModPlayer<MyPlayer>();
			if( !modplayer.MaxBuffTimes.ContainsKey(buff_type) ) { return; }

			Texture2D tex = this.ShadowBox;
			float ratio = 1f - ((float)buff_time / ( float)modplayer.MaxBuffTimes[buff_type]);
			int width = tex.Width;
			int height = (int)((float)tex.Height * ratio);
			var src_rect = new Rectangle( 0, 0, width, height );
			var color = new Color( 255, 255, 255, 144 );
			var pos = new Vector2( rect.X, rect.Y );
			
			Main.spriteBatch.Draw( this.ShadowBox, pos, src_rect, color );
		}


		public void DrawLock( Player player, Rectangle rect, int buff_type, int buff_time ) {
			var modplayer = player.GetModPlayer<MyPlayer>();
			if( !modplayer.MaxBuffTimes.ContainsKey( buff_type ) ) { return; }

			var pos = new Vector2( rect.X - 4, rect.Y - 4 );
			var color = new Color( 255, 255, 255, 128 );

			Main.spriteBatch.Draw( this.LockBox, pos, null, color );
		}
	}
}

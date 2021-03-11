using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace BetterBuffs {
	class BetterBuffsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-betterbuffs-mod";


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

			if( !Main.dedServ ) {
				this.ShadowBox = this.GetTexture( "ShadowBox" );
				this.LockBox = this.GetTexture( "LockBox" );
			}
		}

		////////////////

		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			int idx = layers.FindIndex( layer => layer.Name.Equals( "Vanilla: Inventory" ) );
			if( idx != -1 ) {
				var interfaceLayer = new LegacyGameInterfaceLayer( "BetterBuffDisplays: Buff Overlay",
					delegate {
						if( Main.playerInventory ) { return true; }

						Player player = Main.LocalPlayer;
						var modplayer = player.GetModPlayer<BetterBuffsPlayer>();

						foreach( var kv in BetterBuffHelpers.GetBuffIconRectanglesByPosition( false ) ) {
							int pos = kv.Key;
							Rectangle rect = kv.Value;
							int buffType = player.buffType[pos];
							int buffTime = player.buffTime[pos];

							this.DrawShadow( player, rect, buffType, buffTime );

							if( modplayer.BuffLocks.Contains( buffType ) ) {
								this.DrawLock( player, rect, buffType, buffTime );
							}
						}

						return true;
					}, InterfaceScaleType.UI );
				layers.Insert( idx, interfaceLayer );
			}
		}


		public void DrawShadow( Player player, Rectangle rect, int buffType, int buffTime ) {
			var modplayer = player.GetModPlayer<BetterBuffsPlayer>();
			if( !modplayer.MaxBuffTimes.ContainsKey(buffType) ) { return; }

			Texture2D tex = this.ShadowBox;
			float ratio = 1f - ((float)buffTime / ( float)modplayer.MaxBuffTimes[buffType]);
			int width = tex.Width;
			int height = (int)((float)tex.Height * ratio);
			var srcRect = new Rectangle( 0, 0, width, height );
			var color = new Color( 255, 255, 255, 144 );
			var pos = new Vector2( rect.X, rect.Y );
			
			Main.spriteBatch.Draw( this.ShadowBox, pos, srcRect, color );
		}


		public void DrawLock( Player player, Rectangle rect, int buffType, int buffTime ) {
			var modplayer = player.GetModPlayer<BetterBuffsPlayer>();
			if( !modplayer.MaxBuffTimes.ContainsKey( buffType ) ) { return; }

			var pos = new Vector2( rect.X - 4, rect.Y - 4 );
			var color = new Color( 255, 255, 255, 128 );

			Main.spriteBatch.Draw( this.LockBox, pos, null, color );
		}
	}
}

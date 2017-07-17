using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace BetterBuffs {
	class BetterBuffsPlayer : ModPlayer {
		public IDictionary<int, int> MaxBuffTimes = new Dictionary<int, int>();

		public bool IsLeftClickAndRelease { get; private set; }



		////////////////

		public override void Initialize() {
			this.MaxBuffTimes = new Dictionary<int, int>();
		}

		public override void clientClone( ModPlayer clone ) {
			var myclone = (BetterBuffsPlayer)clone;
			myclone.MaxBuffTimes = this.MaxBuffTimes;
		}

		////////////////

		public override void Load( TagCompound tags ) {
			this.MaxBuffTimes = new Dictionary<int, int>();
			int buff_count = tags.GetInt( "buff_count" );

			for( int i=0; i<buff_count; i++ ) {
				this.MaxBuffTimes[ tags.GetInt("buff_type_"+i) ] = tags.GetInt("buff_time_"+i);
			}
		}

		public override TagCompound Save() {
			var tags = new TagCompound { { "buff_count", this.MaxBuffTimes.Count } };

			int i = 0;
			foreach( var kv in this.MaxBuffTimes ) {
				tags.Set( "buff_type_" + i, kv.Key );
				tags.Set( "buff_time_" + i, kv.Value );
				i++;
			}

			return tags;
		}

		////////////////


		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				if( Main.mouseLeftRelease && Main.mouseLeft ) {
					if( !this.IsLeftClickAndRelease && !Main.playerInventory ) {
						var mouse = new Rectangle( Main.mouseX, Main.mouseY, 1, 1 );

						foreach( var kv in BetterBuffHelpers.GetBuffIconRectangles( InterfaceScaleType.Game ) ) {
							if( kv.Value.Intersects( mouse ) ) {
								BetterBuffHelpers.RefreshBuffAt( kv.Key );
								break;
							}
						}
					}

					this.IsLeftClickAndRelease = true;
				} else {
					this.IsLeftClickAndRelease = false;
				}
			}
		}

		
		////////////////

		public void UpdateBuffTimes() {
			ISet<int> mybuffs = new HashSet<int>();

			for( int i = 0; i < 22; i++ ) {
				if( this.player.buffType[i] <= 0 ) { continue; }
				int buff_type = this.player.buffType[i];
				mybuffs.Add( buff_type );

				if( !this.MaxBuffTimes.ContainsKey( buff_type ) ) {
					this.MaxBuffTimes[buff_type] = this.player.buffTime[i];
				}
			}

			foreach( var kv in this.MaxBuffTimes.ToList() ) {
				if( !mybuffs.Contains( kv.Key ) ) {
					this.MaxBuffTimes.Remove( kv.Key );
					continue;
				}
			}
		}
	}
}

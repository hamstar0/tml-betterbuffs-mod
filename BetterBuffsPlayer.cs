using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace BetterBuffs {
	class BetterBuffsPlayer : ModPlayer {
		public IDictionary<int, int> MaxBuffTimes = new Dictionary<int, int>();
		public ISet<int> BuffLocks = new HashSet<int>();

		public bool IsLeftClickAndRelease { get; private set; }

		
		////////////////

		public override void Initialize() {
			this.MaxBuffTimes = new Dictionary<int, int>();
			this.BuffLocks = new HashSet<int>();
		}

		public override void clientClone( ModPlayer clone ) {
			var myclone = (BetterBuffsPlayer)clone;
			myclone.MaxBuffTimes = this.MaxBuffTimes;
			myclone.BuffLocks = this.BuffLocks;
		}

		////////////////

		public override void Load( TagCompound tags ) {
			this.MaxBuffTimes = new Dictionary<int, int>();
			int buffCount = tags.GetInt( "buff_count" );

			for( int i=0; i<buffCount; i++ ) {
				this.MaxBuffTimes[ tags.GetInt("buff_type_"+i) ] = tags.GetInt( "buff_time_"+i );
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
			this.UpdateBuffTimes();

			if( this.player.whoAmI == Main.myPlayer ) {
				if( Main.mouseLeftRelease && Main.mouseLeft ) {
					if( !this.IsLeftClickAndRelease && !Main.playerInventory ) {
						var mouse = new Rectangle( Main.mouseX, Main.mouseY, 1, 1 );

						foreach( var kv in BetterBuffHelpers.GetBuffIconRectanglesByPosition( true ) ) {
							int pos = kv.Key;

							if( kv.Value.Intersects( mouse ) ) {
								if( !BetterBuffHelpers.CanRefreshBuffAt( this.player, pos ) ) { continue; }

								if( this.player.controlTorch ) {
									this.ToggleBuffLock( pos );
								} else if( this.player.buffType[pos] != BuffID.PotionSickness ) {
									BetterBuffHelpers.RefreshBuffAt( this.player, pos );
								}
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
			ISet<int> activeBuffTypes = new HashSet<int>();
			
			for( int i = 0; i < this.player.buffType.Length; i++ ) {
				int buffType = this.player.buffType[i];
				int buffTime = this.player.buffTime[i];

				if( buffType <= 0 ) { continue; }

				activeBuffTypes.Add( buffType );

				if( !this.MaxBuffTimes.ContainsKey( buffType ) ) {
					this.MaxBuffTimes[ buffType ] = buffTime;
				}
				
				if( buffTime == 1 && this.BuffLocks.Contains(buffType) ) {
					if( BetterBuffHelpers.CanRefreshBuffAt( this.player, i ) ) {
						BetterBuffHelpers.RefreshBuffAt( this.player, i );
					}
				}
			}

			foreach( int buffType in this.MaxBuffTimes.Keys.ToList() ) {
				if( !activeBuffTypes.Contains( buffType ) ) {
					if( this.MaxBuffTimes.ContainsKey( buffType ) ) {
						this.MaxBuffTimes.Remove( buffType );
					}
					if( this.BuffLocks.Contains( buffType ) ) {
						this.BuffLocks.Remove( buffType );
					}

					continue;
				}
			}
		}


		////////////////

		public void ToggleBuffLock( int pos ) {
			int buffType = this.player.buffType[ pos ];
			if( buffType <= 0 ) { return; }
			
			if( this.BuffLocks.Contains(buffType) ) {
				this.BuffLocks.Remove( buffType );
			} else {
				this.BuffLocks.Add( buffType );
			}
		}
	}
}

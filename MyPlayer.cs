using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace BetterBuffs {
	class MyPlayer : ModPlayer {
		public IDictionary<int, int> MaxBuffTimes = new Dictionary<int, int>();
		public ISet<int> BuffLocks = new HashSet<int>();

		public bool IsLeftClickAndRelease { get; private set; }

		
		////////////////

		public override void Initialize() {
			this.MaxBuffTimes = new Dictionary<int, int>();
			this.BuffLocks = new HashSet<int>();
		}

		public override void clientClone( ModPlayer clone ) {
			var myclone = (MyPlayer)clone;
			myclone.MaxBuffTimes = this.MaxBuffTimes;
			myclone.BuffLocks = this.BuffLocks;
		}

		////////////////

		public override void Load( TagCompound tags ) {
			this.MaxBuffTimes = new Dictionary<int, int>();
			int buff_count = tags.GetInt( "buff_count" );

			for( int i=0; i<buff_count; i++ ) {
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
			ISet<int> active_buff_types = new HashSet<int>();
			
			for( int i = 0; i < 22; i++ ) {
				int buff_type = this.player.buffType[i];
				int buff_time = this.player.buffTime[i];

				if( buff_type <= 0 ) { continue; }

				active_buff_types.Add( buff_type );

				if( !this.MaxBuffTimes.ContainsKey( buff_type ) ) {
					this.MaxBuffTimes[ buff_type ] = buff_time;
				}
				
				if( buff_time < 5 && this.BuffLocks.Contains(buff_type) ) {
					if( BetterBuffHelpers.CanRefreshBuffAt( this.player, i ) ) {
						BetterBuffHelpers.RefreshBuffAt( this.player, i );
					}
				}
			}

			foreach( int buff_type in this.MaxBuffTimes.Keys.ToList() ) {
				if( !active_buff_types.Contains( buff_type ) ) {
					if( this.MaxBuffTimes.ContainsKey( buff_type ) ) {
						this.MaxBuffTimes.Remove( buff_type );
					}
					if( this.BuffLocks.Contains( buff_type ) ) {
						this.BuffLocks.Remove( buff_type );
					}

					continue;
				}
			}
		}


		////////////////

		public void ToggleBuffLock( int pos ) {
			int buff_type = this.player.buffType[ pos ];
			if( buff_type <= 0 ) { return; }
			
			if( this.BuffLocks.Contains(buff_type) ) {
				this.BuffLocks.Remove( buff_type );
			} else {
				this.BuffLocks.Add( buff_type );
			}
		}
	}
}

using Terraria;
using Terraria.ModLoader;


namespace BetterBuffs {
	class MyGlobalItem : GlobalItem {
		public override bool ConsumeItem( Item item, Player player ) {
			bool can_consume = base.ConsumeItem( item, player );
			var modplayer = player.GetModPlayer<BetterBuffsPlayer>();

			if( can_consume && item.buffType > 0 ) {
				modplayer.MaxBuffTimes[ item.buffType ] = item.buffTime;
			}

			return can_consume;
		}
	}
}

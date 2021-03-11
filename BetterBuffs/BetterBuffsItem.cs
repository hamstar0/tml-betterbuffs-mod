using Terraria;
using Terraria.ModLoader;


namespace BetterBuffs {
	class MyGlobalItem : GlobalItem {
		public override bool ConsumeItem( Item item, Player player ) {
			bool canConsume = base.ConsumeItem( item, player );
			var modplayer = player.GetModPlayer<BetterBuffsPlayer>();

			if( canConsume && item.buffType > 0 ) {
				modplayer.MaxBuffTimes[ item.buffType ] = item.buffTime;
			}

			return canConsume;
		}
	}
}

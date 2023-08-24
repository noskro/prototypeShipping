using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public partial class CustomTile : HexagonalRuleTile
{
    public EnumTileMovability movability;

    public EnumTileType type;
    public EnumTileSiblingGroup siblingGroup;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is CustomTile 
            && (other as CustomTile).siblingGroup != EnumTileSiblingGroup.None 
            && this.siblingGroup != EnumTileSiblingGroup.None)
        {
            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            switch (neighbor)
            {
                case TilingRule.Neighbor.This:
                    {
                        return other is CustomTile
                            && (other as CustomTile).siblingGroup == this.siblingGroup;
                    }
                case TilingRule.Neighbor.NotThis:
                    {
                        return !(other is CustomTile
                            && (other as CustomTile).siblingGroup == this.siblingGroup);
                    }
            }
        }

        return base.RuleMatch(neighbor, other);
    }
}

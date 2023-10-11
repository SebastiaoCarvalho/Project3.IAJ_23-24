using System.Collections;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Formations
{
    public class TriangleFormation : FormationPattern 
    {
        // This is a very triangle formation, with the anchor being the position of the anchor at index 0.
        private static readonly float offset = 3.0f;
        private static readonly float sideOffset = 2.0f;

        public TriangleFormation()
        {
            this.FreeAnchor = false;
        }

        public override Vector3 GetOrientation(FormationManager formation )
        {
            //In this formation, the orientation is defined by the first character's transform rotation...

            //antigo
            //Quaternion rotation = formation.SlotAssignment[0].transform.rotation;

            //lat year
            //Quaternion rotation = formation.SlotAssignment.Keys.First().transform.rotation;

            //novo
            return formation.SlotAssignment.Keys.First().transform.forward;

            //Vector2 orientation = new Vector2(rotation.x, rotation.z);
            //return orientation;

            //return new Vector3(rotation.x,rotation.y,rotation.z);
        }

        public override Vector3 GetSlotLocation(FormationManager formation, int slotNumber) => slotNumber switch
        {
            0 => formation.AnchorPosition,
            1 => formation.AnchorPosition + -offset * slotNumber * this.GetOrientation(formation),
            2 => formation.AnchorPosition + -2*offset * this.GetOrientation(formation) + -sideOffset * formation.SlotAssignment.Keys.First().transform.right,
            _ => formation.AnchorPosition + -2*offset * this.GetOrientation(formation) + sideOffset * formation.SlotAssignment.Keys.First().transform.right
        };

        public override  bool SupportSlot(int slotCount)
        {
            return (slotCount <= 4); 
        }

        
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentItem : MonoBehaviour
{
    #region Enums
    public enum EquipmentType
    {
        Equipment,
        Gear,
        Weapon
    }

    /// <summary>
    /// Enum for different body parts.
    /// </summary>
    public enum BodyPart
    {
        /// <summary>
        /// Head body part.
        /// </summary>
        Head,

        /// <summary>
        /// Eyes body part.
        /// </summary>
        Eyes,

        /// <summary>
        /// Ears body part.
        /// </summary>
        Ears,

        /// <summary>
        /// Shoulders body part.
        /// </summary>
        Shoulders,

        /// <summary>
        /// Hands body part.
        /// </summary>
        Hands,

        /// <summary>
        /// Chest body part.
        /// </summary>
        Chest,

        /// <summary>
        /// Back body part.
        /// </summary>
        Back,

        /// <summary>
        /// Hips body part.
        /// </summary>
        Hips,

        /// <summary>
        /// Legs body part.
        /// </summary>
        Legs,

        /// <summary>
        /// Knees body part.
        /// </summary>
        Knees,

        /// <summary>
        /// Feet body part.
        /// </summary>
        Feet
    };
    #endregion

    #region Serialization
    /// <summary>
    /// Index of the equipment item.
    /// </summary>
    public int itemIndex;

    /// <summary>
    /// Body part type of the equipment item.
    /// </summary>
    public BodyPart bodyPart;

    public LoadoutManager.WeaponType weaponType;
    public LoadoutManager.WeaponCategory weaponCategory;
    public EquipmentType equipmentType;
    #endregion
}

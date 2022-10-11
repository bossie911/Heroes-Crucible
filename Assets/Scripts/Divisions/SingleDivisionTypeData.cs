using System;
using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions.Upgrades;
using Mirror;
using UnityEditor;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    [Serializable]
    public class SingleDivisionTypeData
    {
        public string TypeName = string.Empty;
        public DivisionType Type = DivisionType.Swordsmen;
        [NonSerialized] private Sprite m_Icon = null;
        [NonSerialized] private Unit m_PrefabUnit = null;

        public Sprite Icon
        {
            get { return m_Icon ??= UnityEngine.Resources.Load<Sprite>(IconPath); }
        }

        public Unit PrefabUnit
        {
            get { return m_PrefabUnit ??= UnityEngine.Resources.Load<Unit>(PrefabPath); }
        }

        public string IconPath;
        public string PrefabPath;
        public DivisionType IsWeakToType = DivisionType.Swordsmen;
        public int MaxUnitCount = 24;
        public float MovementSpeed = 4f;
        public float MovementSpeedRunning = 8f;
        public float DistanceFromEnemiesToStartRunning = 0f;
        public float MoveSpeedUpTime = 10f;
        public float MoveSpeedUpFinishedTime = 15f;
        public float MoveSpeedIncreasePercent = 0.5f;
        public float Damage = 1;
        public float WeaknessHitMultiplier = 2f;
        public float HitChance = 0.5f;
        public float Range = 3f;
        public float AutoAttackRange = 10f;
        public float Cooldown = 2f;
        public float KnockbackDistance = 3f;
        public float KnockbackDuration = 0.5f;
        public float ChargeUpTime = 0.5f;
        public float MaxHealth = 1;
        public float FavorGainedOnHit = 1;

        public SingleDivisionTypeData()
        {
        }


        public SingleDivisionTypeData(DivisionTypeData typeData)
        {
            TypeName = typeData.TypeName;
            Type = typeData.Type;
            IconPath = typeData.IconPath;
            m_Icon = null;
            PrefabPath = typeData.PrefabUnitPath;
            m_PrefabUnit = null;
            IsWeakToType = typeData.IsWeakToType;
            MaxUnitCount = typeData.MaxUnitCount;
            MovementSpeed = typeData.MovementSpeed;
            MovementSpeedRunning = typeData.MovementSpeedRunning;
            DistanceFromEnemiesToStartRunning = typeData.DistanceFromEnemiesToStartRunning;
            MoveSpeedUpTime = typeData.MoveSpeedUpTime;
            MoveSpeedUpFinishedTime = typeData.MoveSpeedUpFinishedTime;
            MoveSpeedIncreasePercent = typeData.MoveSpeedIncreasePercent;
            Damage = typeData.Damage;
            WeaknessHitMultiplier = typeData.WeaknessHitMultiplier;
            HitChance = typeData.HitChance;
            Range = typeData.Range;
            AutoAttackRange = typeData.AutoAttackRange;
            Cooldown = typeData.Cooldown;
            ChargeUpTime = typeData.ChargeUpTime;
            MaxHealth = typeData.MaxHealth;
            FavorGainedOnHit = typeData.FavorGainedOnHit;
        }
    }
}
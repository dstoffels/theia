﻿using System;
using System.Collections.Generic;


namespace Stats
{
    public class Skill : Stat<SkillData>, IStatObserver, IStatSubject
    {
        Dictionary<string, int> attributeValues = new Dictionary<string, int>();
        public int aptitude { get; private set; }
        void SetAptitude(StatEvent statEvent)
        {
            if (!attributeValues.ContainsKey(statEvent.name)) attributeValues.Add(statEvent.name, statEvent.value);
            attributeValues[statEvent.name] = statEvent.value;
            aptitude = 0;
            foreach (var attValue in attributeValues.Values) aptitude += attValue;
            SetLevel();
        }

        public int xp { get; private set; }
        public void AddXP(int amt)
        {
            xp = Math.Max(0, xp + amt);
            SetProficiency();
        }

        static float FIRST_LEVELUP_AT = 1000;
        static float LEVELUP_MULTIPLIER = 1.02f;

        float nextLevelupAt = FIRST_LEVELUP_AT;
        float requiredXp = FIRST_LEVELUP_AT;
        float lastLevelupAt = 0;
        public int proficiency { get; private set; }

        void SetProficiency()
        {
            if (NeedsUpdate())
            {
                proficiency = 0;
                lastLevelupAt = 0;
                nextLevelupAt = FIRST_LEVELUP_AT;
                requiredXp = FIRST_LEVELUP_AT;

                while (NeedsUpdate())
                {
                    requiredXp *= LEVELUP_MULTIPLIER;
                    lastLevelupAt = nextLevelupAt;
                    nextLevelupAt += requiredXp;
                    proficiency++;
                }
                SetLevel();
                NotifyDependents();
            }
        }

        bool NeedsUpdate() => xp >= nextLevelupAt || xp < lastLevelupAt;

        void SetLevel()
        {
            level = aptitude + proficiency;
        }

        public void Update(StatEvent statEvent)
        {
            SetAptitude(statEvent);
        }

        public void Attach(IStatObserver observer) => observers.Add(observer);

        public void Detach(IStatObserver observer) => observers.Remove(observer);

        public void NotifyDependents()
        {
            foreach (var observer in observers)
            {
                int value = observer.name == data.primaryAttribute.name ? level * 2 : level;
                observer.Update(new StatEvent(name, value));
            }
        }

        public Skill(SkillData data) : base(data) { }

    }
}
using RolePlayCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellFormedNames;

namespace EmotionalPlayer
{
    class SuecaEvent
    {
        public string Name;
        public List<Name> Events;
        public List<string> Tags;
        public List<string> Meanings;
        public bool Finished;
        public string[] OtherStringInfos;
        public int[] OtherIntInfos;

        public SuecaEvent(string name)
        {
            Name = name;
            Events = new List<Name>();
            Tags = new List<string>();
            Meanings = new List<string>();
            Finished = false;
        }

        public void AddPropertyChange(string propertyName, string value, string subject)
        {
            Events.Add(EventHelper.PropertyChange(propertyName, value, subject));
        }

        public void AddActionEnd(string subject, string actionName, string target)
        {
            Events.Add(EventHelper.ActionEnd(subject, actionName, target));
        }

        public void ChangeTagsAndMeanings(string[] tags, string[] meanings)
        {
            Tags = new List<string>(tags);
            Meanings = new List<string>(meanings);
        }

        public void AddTagAndMeaning(string tag, string meaning)
        {
            Tags.Add(tag);
            Meanings.Add(meaning);
        }

    }
}

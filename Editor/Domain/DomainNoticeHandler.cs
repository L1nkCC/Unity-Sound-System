using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CC.SoundSystem.Editor
{
    public static class DomainNoticeHandler
    {
        struct Notice
        {
            public HelpBox HelpBox;
            public System.Func<string, bool> Condition;//input will be domainName

            public Notice(HelpBox helpBox, System.Func<string, bool> condition)
            {
                HelpBox = helpBox;
                Condition = condition;
            }
        }

        private static readonly List<Notice> m_notices = new List<Notice>()
        {
            new Notice(new HelpBox("There is no well defined root.",HelpBoxMessageType.Error), (string domainName) => { return !Domain.HasOneRoot(domainName); }),
        };


        public static void UpdateDomainNotices(this VisualElement parent, string domainName)
        {
            foreach(Notice notice in m_notices)
            {
                UpdateDomainNotice(parent, domainName, notice);
            }
        }


        private static void UpdateDomainNotice(VisualElement parent, string domainName, Notice notice)
        {
            if (notice.Condition(domainName))
            {
                if (!parent.Contains(notice.HelpBox))
                {
                    parent.Add(notice.HelpBox);
                }
            }
            else
            {
                if (parent.Contains(notice.HelpBox))
                {
                    parent.Remove(notice.HelpBox);
                }
            }
        }
    }
}

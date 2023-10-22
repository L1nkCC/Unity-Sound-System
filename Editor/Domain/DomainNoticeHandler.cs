using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/21/2023
    /// Last Edited: 10/21/2023
    /// 
    /// <summary>
    /// Support class to help display Domain Information through helpboxes
    /// </summary>
    public static class DomainNoticeHandler
    {
        //format of displays to visualElement
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

        //List of possible notices
        private static readonly List<Notice> m_notices = new List<Notice>()
        {
            new Notice(new HelpBox("There is no well defined root.",HelpBoxMessageType.Error), (string domainName) => { return !Domain.HasOneRoot(domainName); }),
        };


        /// <summary>
        /// Update the display to include helpboxes describing Domain Conidition including errors and warnings
        /// </summary>
        /// <param name="parent">Parent of help box notices</param>
        /// <param name="domainName">Domain to get notices for </param>
        public static void UpdateDomainNotices(this VisualElement parent, string domainName)
        {
            foreach(Notice notice in m_notices)
            {
                UpdateDomainNotice(parent, domainName, notice);
            }
        }

        /// <summary>
        /// Display the notice based on its condition
        /// </summary>
        /// <param name="parent">Parent of helpbox notices</param>
        /// <param name="domainName">Domain to get notices for</param>
        /// <param name="notice">Notice being checked</param>
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

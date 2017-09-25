using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace NeoFalconSkill
{
    [Serializable]
    public class AccessaryQuery
    {
        [Prompt("What kind of {&} Would like to control")]
        [Optional]
        public string Accessary { get; set; }
    }
}
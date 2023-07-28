using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace CyberNet.Core.Dialog
{
    public struct DialogConfig
    {
        public List<Phrase> Phrase;
    }
    
    public struct Phrase
    {
        [JsonProperty("phrase_id")]
        public int Phrase_id;
        [JsonProperty("character")]
        public string Character;
        [JsonProperty("dialog")]
        public string Dialog;
    }
    
    public struct CharacterDialogConfig
    {
        [JsonProperty("image_key")]
        public string Image_key;
        [JsonProperty("loc_name")]
        public string Loc_name;
    }
}
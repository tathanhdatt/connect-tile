using System.Collections.Generic;
using Core;
using UnityEngine;

namespace _Module.Core.SaveGame
{
    public class PlayerPrefSave : ISaveManager
    {
        private readonly GameData gameData;
        public PlayerPrefSave(GameData gameData)
        {
            this.gameData = gameData;
        }

        public void Save()
        {
            PlayerPrefs.SetInt(nameof(gameData.CurrentLevelOrder), gameData.CurrentLevelOrder);
            PlayerPrefs.SetFloat(nameof(gameData.Volume), gameData.Volume);
            PlayerPrefs.SetInt(nameof(gameData.Vibration), gameData.Vibration ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
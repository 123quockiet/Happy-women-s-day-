```csharp
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace VoxelFPS.Core
{
    /// <summary>
    /// Core Game Manager handling state, configurations, and system initialization.
    /// Follows a Singleton pattern for global access in the vertical slice.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }
        public GameConfig ActiveConfig { get; private set; }

        public delegate void GameStateChanged(GameState newState);
        public event GameStateChanged OnGameStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadConfiguration();
            ChangeState(GameState.Lobby);
        }

        private void LoadConfiguration()
        {
            string configPath = Path.Combine(Application.streamingAssetsPath, "GameConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                ActiveConfig = JsonUtility.FromJson<GameConfig>(json);
                Debug.Log("AAA Voxel FPS: Configuration Loaded.");
            }
            else
            {
                Debug.LogWarning("Config missing. Using defaults.");
                ActiveConfig = new GameConfig(); // Fallback
            }
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            // Handle state transitions
            switch (newState)
            {
                case GameState.Lobby:
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }
    }

    public enum GameState { Boot, Lobby, Loading, Playing, Paused, GameOver }

    [System.Serializable]
    public class GameConfig
    {
        public string GameVersion = "1.0.0-HDRP";
        public float GlobalVolume = 1.0f;
        public int TargetFramerate = 144;
        public float BaseMovementSpeed = 8.5f;
        public float Gravity = -19.81f;
        public int ChunkSize = 16;
        public int WorldHeight = 128;
    }
}

```
